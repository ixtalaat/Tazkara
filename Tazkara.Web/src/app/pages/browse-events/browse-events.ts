import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NgIf, NgFor, CurrencyPipe, DatePipe, SlicePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EventService } from '../../services/event.service';
import { Event, Category } from '../../models/types';

@Component({
  selector: 'app-browse-events',
  imports: [RouterLink, NgIf, NgFor, CurrencyPipe, DatePipe, SlicePipe, FormsModule],
  templateUrl: './browse-events.html',
  styleUrl: './browse-events.css'
})
export class BrowseEventsComponent implements OnInit {
  private eventService = inject(EventService);

  // States
  events = signal<Event[]>([]);
  categories = signal<Category[]>([]);
  loading = signal(false);
  errorMessage = signal('');

  // Pagination & Filters
  pageNumber = signal(1);
  pageSize = signal(9);
  totalCount = signal(0);
  totalPages = signal(1);

  // Filter bindings
  searchTerm = signal('');
  selectedCategoryId = signal('');
  minPrice = signal<number | undefined>(undefined);
  maxPrice = signal<number | undefined>(undefined);
  startDate = signal('');
  endDate = signal('');

  ngOnInit(): void {
    this.loadCategories();
    this.loadEvents();
  }

  loadCategories(): void {
    this.eventService.getCategories().subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.categories.set(res.data);
        }
      }
    });
  }

  loadEvents(): void {
    this.loading.set(true);
    this.errorMessage.set('');

    const filter = {
      pageNumber: this.pageNumber(),
      pageSize: this.pageSize(),
      categoryId: this.selectedCategoryId() || undefined,
      searchTerm: this.searchTerm() || undefined,
      minPrice: this.minPrice() !== null && this.minPrice() !== undefined ? this.minPrice() : undefined,
      maxPrice: this.maxPrice() !== null && this.maxPrice() !== undefined ? this.maxPrice() : undefined,
      startDate: this.startDate() || undefined,
      endDate: this.endDate() || undefined,
      status: 'Published' // Browsing only shows published events
    };

    this.eventService.browseEvents(filter).subscribe({
      next: (res) => {
        this.loading.set(false);
        if (res.success && res.data) {
          this.events.set(res.data.items);
          this.totalCount.set(res.data.totalCount);
          this.totalPages.set(Math.ceil(res.data.totalCount / this.pageSize()));
        } else {
          this.errorMessage.set(res.message || 'Failed to load events.');
        }
      },
      error: (err) => {
        this.loading.set(false);
        this.errorMessage.set(err.error?.message || 'Something went wrong while fetching events.');
      }
    });
  }

  onSearch(): void {
    this.pageNumber.set(1);
    this.loadEvents();
  }

  selectCategory(categoryId: string): void {
    this.selectedCategoryId.set(categoryId);
    this.pageNumber.set(1);
    this.loadEvents();
  }

  clearFilters(): void {
    this.searchTerm.set('');
    this.selectedCategoryId.set('');
    this.minPrice.set(undefined);
    this.maxPrice.set(undefined);
    this.startDate.set('');
    this.endDate.set('');
    this.pageNumber.set(1);
    this.loadEvents();
  }

  changePage(page: number): void {
    if (page >= 1 && page <= this.totalPages()) {
      this.pageNumber.set(page);
      this.loadEvents();
    }
  }

  getPagesArray(): number[] {
    const arr = [];
    for (let i = 1; i <= this.totalPages(); i++) {
      arr.push(i);
    }
    return arr;
  }
}
