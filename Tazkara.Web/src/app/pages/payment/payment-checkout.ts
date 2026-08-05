import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { NgIf, NgFor, CurrencyPipe, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TicketService } from '../../services/ticket.service';
import { PaymentService } from '../../services/payment.service';
import { Ticket } from '../../models/types';

@Component({
  selector: 'app-payment-checkout',
  imports: [RouterLink, NgIf, NgFor, CurrencyPipe, DatePipe, FormsModule],
  templateUrl: './payment-checkout.html',
  styleUrl: './payment-checkout.css'
})
export class PaymentCheckoutComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private ticketService = inject(TicketService);
  private paymentService = inject(PaymentService);

  // States
  ticket = signal<Ticket | null>(null);
  loading = signal(true);
  processing = signal(false);
  errorMessage = signal('');
  
  // Checkout flow: 'select_provider' | 'simulating_paypal' | 'simulating_vodafone' | 'payment_success'
  flowStep = signal<'select_provider' | 'simulating_paypal' | 'simulating_vodafone' | 'payment_success'>('select_provider');
  
  // Payment Session details
  transactionId = signal('');
  paymentUrl = signal('');
  selectedProvider = signal<number | null>(null); // 0 = PayPal, 1 = Vodafone Cash

  // Mock Form inputs
  paypalEmail = signal('');
  paypalPassword = signal('');
  vodafoneNumber = signal('');
  vodafonePin = signal('');

  ngOnInit(): void {
    const ticketId = this.route.snapshot.queryParamMap.get('ticketId');
    if (ticketId) {
      this.loadTicket(ticketId);
    } else {
      this.errorMessage.set('No Ticket ID provided.');
      this.loading.set(false);
    }
  }

  loadTicket(ticketId: string): void {
    this.loading.set(true);
    this.ticketService.getTicketById(ticketId).subscribe({
      next: (res) => {
        this.loading.set(false);
        if (res.success && res.data) {
          this.ticket.set(res.data);
          // If already paid
          if (res.data.paymentStatus === 'Paid' || res.data.status === 'Confirmed') {
            this.flowStep.set('payment_success');
          }
        } else {
          this.errorMessage.set(res.message || 'Ticket not found.');
        }
      },
      error: (err) => {
        this.loading.set(false);
        this.errorMessage.set(err.error?.message || 'Failed to retrieve ticket details.');
      }
    });
  }

  initiatePayment(provider: number): void {
    const t = this.ticket();
    if (!t) return;

    this.processing.set(true);
    this.errorMessage.set('');
    this.selectedProvider.set(provider);

    this.paymentService.createPaymentSession(t.id, provider).subscribe({
      next: (res) => {
        this.processing.set(false);
        if (res.success && res.data) {
          this.transactionId.set(res.data.transactionId);
          this.paymentUrl.set(res.data.paymentUrl);
          
          if (provider === 0) {
            this.flowStep.set('simulating_paypal');
          } else {
            this.flowStep.set('simulating_vodafone');
          }
        } else {
          this.errorMessage.set(res.message || 'Failed to create payment session.');
        }
      },
      error: (err) => {
        this.processing.set(false);
        this.errorMessage.set(err.error?.message || 'Error initiating payment session.');
      }
    });
  }

  confirmSimulatedPayment(): void {
    this.processing.set(true);
    this.errorMessage.set('');

    const token = this.selectedProvider() === 0 
      ? `PP-MOCK-TOKEN-${Math.floor(Math.random() * 100000)}`
      : `VF-MOCK-TOKEN-${Math.floor(Math.random() * 100000)}`;

    this.paymentService.verifyPayment(this.transactionId(), token).subscribe({
      next: (res) => {
        this.processing.set(false);
        if (res.success && res.data) {
          // Verify succeeded! Update local ticket status
          if (this.ticket()) {
            const updated: Ticket = {
              ...this.ticket()!,
              status: 'Confirmed',
              paymentStatus: 'Paid'
            };
            this.ticket.set(updated);
          }
          this.flowStep.set('payment_success');
        } else {
          this.errorMessage.set(res.message || 'Simulated payment verification failed.');
        }
      },
      error: (err) => {
        this.processing.set(false);
        this.errorMessage.set(err.error?.message || 'Verification failed. Please try again.');
      }
    });
  }

  cancelCheckoutSimulation(): void {
    this.flowStep.set('select_provider');
    this.errorMessage.set('');
  }

  printTicket(): void {
    window.print();
  }
}
