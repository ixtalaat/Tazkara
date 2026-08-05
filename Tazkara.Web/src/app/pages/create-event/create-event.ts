import { Component, OnInit, inject } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Category, CreateEventRequest, Event, UpdateEventRequest } from '../../models/types';
import { EventService } from '../../services/event.service';

function endAfterStart(control: AbstractControl): ValidationErrors | null {
  const startDate = control.get('startDate')?.value;
  const endDate = control.get('endDate')?.value;
  return startDate && endDate && new Date(endDate) <= new Date(startDate) ? { endBeforeStart: true } : null;
}

function startInFuture(control: AbstractControl): ValidationErrors | null {
  const value = control.value;
  return value && new Date(value) <= new Date() ? { startInPast: true } : null;
}

@Component({
  selector: 'app-create-event',
  imports: [NgFor, NgIf, ReactiveFormsModule, RouterLink],
  templateUrl: './create-event.html',
  styleUrl: './create-event.css'
})
export class CreateEventComponent implements OnInit {
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private eventService = inject(EventService);

  readonly eventId = this.route.snapshot.paramMap.get('id');
  readonly isEditMode = !!this.eventId;
  categories: Category[] = [];
  // Categories load independently so a slow/unavailable category request
  // never prevents the event form itself from rendering.
  loading = !!this.eventId;
  submitting = false;
  errorMessage = '';

  eventForm = this.fb.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    description: ['', [Validators.required]],
    location: ['', [Validators.required, Validators.maxLength(300)]],
    startDate: ['', [Validators.required, startInFuture]],
    endDate: ['', Validators.required],
    capacity: [null as number | null, [Validators.required, Validators.min(1), Validators.max(100000)]],
    price: [null as number | null, [Validators.required, Validators.min(0)]],
    categoryId: ['', Validators.required]
  }, { validators: endAfterStart });

  ngOnInit(): void {
    this.loadCategories();
    if (this.isEditMode && this.eventId) {
      this.loadEvent(this.eventId);
    }
  }

  loadCategories(): void {
    this.eventService.getCategories().subscribe({
      next: (response) => {
        this.categories = response.data ?? [];
        if (!response.success) this.errorMessage = response.message || 'Unable to load categories.';
      },
      error: (error) => {
        this.errorMessage = error.error?.message || 'Unable to load categories.';
      }
    });
  }

  loadEvent(id: string): void {
    this.eventService.getEventById(id).subscribe({
      next: (response) => {
        if (!response.success || !response.data) {
          this.loading = false;
          this.errorMessage = response.message || 'Unable to load this event.';
          return;
        }
        this.loading = false;
        this.populateForm(response.data);
      },
      error: (error) => {
        this.loading = false;
        this.errorMessage = error.error?.message || 'Unable to load this event.';
      }
    });
  }

  onSubmit(): void {
    if (this.eventForm.invalid || this.submitting) {
      this.eventForm.markAllAsTouched();
      this.errorMessage = 'Please fix the highlighted fields before saving.';
      return;
    }

    const value = this.eventForm.getRawValue();
    const request: CreateEventRequest = {
      title: value.title!, description: value.description!, location: value.location!,
      startDate: new Date(value.startDate!).toISOString(), endDate: new Date(value.endDate!).toISOString(),
      capacity: value.capacity!, price: value.price!, categoryId: value.categoryId!
    };

    this.submitting = true;
    this.errorMessage = '';
    const save$ = this.isEditMode
      ? this.eventService.updateEvent(this.eventId!, { ...request, id: this.eventId! } as UpdateEventRequest)
      : this.eventService.createEvent(request);

    save$.subscribe({
      next: (response) => {
        this.submitting = false;
        if (response.success) this.router.navigate(['/dashboard']);
        else this.errorMessage = this.apiError(response.message, response.errors) || 'Unable to save the event.';
      },
      error: (error) => {
        this.submitting = false;
        this.errorMessage = this.apiError(error.error?.message, error.error?.errors) || 'Unable to save the event.';
      }
    });
  }

  private apiError(message?: string, errors?: string[] | null): string {
    return errors?.filter(Boolean).join(' ') || message || '';
  }

  private populateForm(event: Event): void {
    this.eventForm.patchValue({
      title: event.title, description: event.description, location: event.location,
      startDate: this.toLocalDateTime(event.startDate), endDate: this.toLocalDateTime(event.endDate),
      capacity: event.capacity, price: event.price, categoryId: event.categoryId
    });
  }

  private toLocalDateTime(value: string): string {
    const date = new Date(value);
    const offset = date.getTimezoneOffset() * 60_000;
    return new Date(date.getTime() - offset).toISOString().slice(0, 16);
  }
}
