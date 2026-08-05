import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { NgIf, CurrencyPipe, DatePipe } from '@angular/common';
import { EventService } from '../../services/event.service';
import { TicketService } from '../../services/ticket.service';
import { AuthService } from '../../services/auth.service';
import { Event } from '../../models/types';

@Component({
  selector: 'app-event-details',
  imports: [RouterLink, NgIf, CurrencyPipe, DatePipe],
  templateUrl: './event-details.html',
  styleUrl: './event-details.css'
})
export class EventDetailsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private eventService = inject(EventService);
  private ticketService = inject(TicketService);
  protected authService = inject(AuthService);

  // States
  event = signal<Event | null>(null);
  loading = signal(true);
  reserving = signal(false);
  errorMessage = signal('');
  successMessage = signal('');

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadEvent(id);
    } else {
      this.errorMessage.set('Invalid Event ID.');
      this.loading.set(false);
    }
  }

  loadEvent(id: string): void {
    this.loading.set(true);
    this.eventService.getEventById(id).subscribe({
      next: (res) => {
        this.loading.set(false);
        if (res.success && res.data) {
          this.event.set(res.data);
        } else {
          this.errorMessage.set(res.message || 'Event not found.');
        }
      },
      error: (err) => {
        this.loading.set(false);
        this.errorMessage.set(err.error?.message || 'Failed to load event details.');
      }
    });
  }

  reserveTicket(): void {
    const evt = this.event();
    if (!evt) return;

    // Check login
    if (!this.authService.isLoggedIn) {
      this.router.navigate(['/login'], { queryParams: { returnUrl: `/events/${evt.id}` } });
      return;
    }

    // Only Customers can reserve
    if (!this.authService.isCustomer) {
      this.errorMessage.set('Only customers can purchase or reserve tickets.');
      return;
    }

    this.reserving.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    this.ticketService.reserveTicket(evt.id).subscribe({
      next: (res) => {
        this.reserving.set(false);
        if (res.success && res.data) {
          this.successMessage.set('Ticket reserved successfully! Redirecting to checkout...');
          const ticketId = res.data.id;
          setTimeout(() => {
            this.router.navigate(['/payment/checkout'], { queryParams: { ticketId } });
          }, 1500);
        } else {
          this.errorMessage.set(res.message || 'Failed to reserve ticket.');
        }
      },
      error: (err) => {
        this.reserving.set(false);
        this.errorMessage.set(err.error?.message || 'An error occurred during reservation. Please try again.');
      }
    });
  }
}
