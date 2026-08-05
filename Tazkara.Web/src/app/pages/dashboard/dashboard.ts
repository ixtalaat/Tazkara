import { Component, OnInit, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { CurrencyPipe, DatePipe, NgFor, NgIf } from '@angular/common';
import { DashboardService } from '../../services/dashboard.service';
import { EventService } from '../../services/event.service';
import { OrganizerDashboardResponse } from '../../models/types';

@Component({
  selector: 'app-organizer-dashboard',
  imports: [RouterLink, NgIf, NgFor, CurrencyPipe, DatePipe],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class OrganizerDashboardComponent implements OnInit {
  private dashboardService = inject(DashboardService);
  private eventService = inject(EventService);
  private router = inject(Router);

  // States
  dashboardData = signal<OrganizerDashboardResponse | null>(null);
  loading = signal(true);
  actioningId = signal<string | null>(null);
  errorMessage = signal('');

  ngOnInit(): void {
    this.loadDashboardData();
  }

  loadDashboardData(): void {
    this.loading.set(true);
    this.errorMessage.set('');
    this.dashboardService.getOrganizerDashboard().subscribe({
      next: (res) => {
        this.loading.set(false);
        if (res.success && res.data) {
          this.dashboardData.set(res.data);
        } else {
          this.errorMessage.set(res.message || 'Failed to load dashboard metrics.');
        }
      },
      error: (err) => {
        this.loading.set(false);
        this.errorMessage.set(err.error?.message || 'Error loading dashboard metrics.');
      }
    });
  }

  publishEvent(eventId: string): void {
    this.actioningId.set(eventId);
    this.errorMessage.set('');

    this.eventService.publishEvent(eventId).subscribe({
      next: (res) => {
        this.actioningId.set(null);
        if (res.success) {
          this.loadDashboardData();
        } else {
          this.errorMessage.set(res.message || 'Failed to publish event.');
        }
      },
      error: (err) => {
        this.actioningId.set(null);
        this.errorMessage.set(err.error?.message || 'Error occurred while publishing event.');
      }
    });
  }

  cancelEvent(eventId: string): void {
    if (!confirm('Are you sure you want to cancel this event? This action cannot be undone and will void reservations.')) return;

    this.actioningId.set(eventId);
    this.errorMessage.set('');

    this.eventService.cancelEvent(eventId).subscribe({
      next: (res) => {
        this.actioningId.set(null);
        if (res.success) {
          this.loadDashboardData();
        } else {
          this.errorMessage.set(res.message || 'Failed to cancel event.');
        }
      },
      error: (err) => {
        this.actioningId.set(null);
        this.errorMessage.set(err.error?.message || 'Error occurred while cancelling event.');
      }
    });
  }

  deleteEvent(eventId: string): void {
    if (!confirm('Are you sure you want to delete this event? This will remove the event permanently.')) return;

    this.actioningId.set(eventId);
    this.errorMessage.set('');

    this.eventService.deleteEvent(eventId).subscribe({
      next: (res) => {
        this.actioningId.set(null);
        if (res.success) {
          this.loadDashboardData();
        } else {
          this.errorMessage.set(res.message || 'Failed to delete event.');
        }
      },
      error: (err) => {
        this.actioningId.set(null);
        this.errorMessage.set(err.error?.message || 'Error occurred while deleting event.');
      }
    });
  }

  editEvent(eventId: string): void {
    this.router.navigate(['/dashboard/events/edit', eventId]);
  }

  statusClass(status: string): string {
    return `badge-${status.toLowerCase()}`;
  }

  ticketProgress(sold: number, capacity: number): number {
    return capacity > 0 ? Math.min((sold / capacity) * 100, 100) : 0;
  }
}
