import { Component, OnInit, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { NgIf, NgFor, CurrencyPipe, DatePipe } from '@angular/common';
import { TicketService } from '../../services/ticket.service';
import { Ticket } from '../../models/types';

@Component({
  selector: 'app-my-tickets',
  imports: [RouterLink, NgIf, NgFor, CurrencyPipe, DatePipe],
  templateUrl: './my-tickets.html',
  styleUrl: './my-tickets.css'
})
export class MyTicketsComponent implements OnInit {
  private ticketService = inject(TicketService);
  private router = inject(Router);

  // States
  tickets = signal<Ticket[]>([]);
  loading = signal(true);
  cancellingId = signal<string | null>(null);
  errorMessage = signal('');

  ngOnInit(): void {
    this.loadTickets();
  }

  loadTickets(): void {
    this.loading.set(true);
    this.errorMessage.set('');
    this.ticketService.getMyTickets().subscribe({
      next: (res) => {
        this.loading.set(false);
        if (res.success && res.data) {
          this.tickets.set(res.data);
        } else {
          this.errorMessage.set(res.message || 'Failed to load tickets.');
        }
      },
      error: (err) => {
        this.loading.set(false);
        this.errorMessage.set(err.error?.message || 'Error occurred while loading tickets.');
      }
    });
  }

  payTicket(ticketId: string): void {
    this.router.navigate(['/payment/checkout'], { queryParams: { ticketId } });
  }

  cancelReservation(ticketId: string): void {
    if (!confirm('Are you sure you want to cancel this ticket reservation?')) return;

    this.cancellingId.set(ticketId);
    this.errorMessage.set('');

    this.ticketService.cancelReservation(ticketId).subscribe({
      next: (res) => {
        this.cancellingId.set(null);
        if (res.success) {
          // Refresh list
          this.loadTickets();
        } else {
          this.errorMessage.set(res.message || 'Failed to cancel reservation.');
        }
      },
      error: (err) => {
        this.cancellingId.set(null);
        this.errorMessage.set(err.error?.message || 'Error cancelling reservation.');
      }
    });
  }
}
