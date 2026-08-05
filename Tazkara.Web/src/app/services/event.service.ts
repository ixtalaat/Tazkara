import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse, PaginatedResponse, Event, Category, CreateEventRequest, UpdateEventRequest, EventFilterRequest } from '../models/types';

@Injectable({
  providedIn: 'root'
})
export class EventService {
  private http = inject(HttpClient);

  getCategories(): Observable<ApiResponse<Category[]>> {
    return this.http.get<ApiResponse<Category[]>>('/api/Categories');
  }

  createCategory(name: string): Observable<ApiResponse<Category>> {
    return this.http.post<ApiResponse<Category>>('/api/Categories', { name });
  }

  browseEvents(filter: EventFilterRequest): Observable<ApiResponse<PaginatedResponse<Event>>> {
    let params = new HttpParams()
      .set('pageNumber', filter.pageNumber.toString())
      .set('pageSize', filter.pageSize.toString());

    if (filter.categoryId) {
      params = params.set('categoryId', filter.categoryId);
    }
    if (filter.searchTerm) {
      params = params.set('searchTerm', filter.searchTerm);
    }
    if (filter.minPrice !== undefined && filter.minPrice !== null) {
      params = params.set('minPrice', filter.minPrice.toString());
    }
    if (filter.maxPrice !== undefined && filter.maxPrice !== null) {
      params = params.set('maxPrice', filter.maxPrice.toString());
    }
    if (filter.startDate) {
      params = params.set('startDate', filter.startDate);
    }
    if (filter.endDate) {
      params = params.set('endDate', filter.endDate);
    }
    if (filter.status) {
      params = params.set('status', filter.status);
    }

    return this.http.get<ApiResponse<PaginatedResponse<Event>>>('/api/Events', { params });
  }

  getEventById(id: string): Observable<ApiResponse<Event>> {
    return this.http.get<ApiResponse<Event>>(`/api/Events/${id}`);
  }

  createEvent(request: CreateEventRequest): Observable<ApiResponse<Event>> {
    return this.http.post<ApiResponse<Event>>('/api/Events', request);
  }

  updateEvent(id: string, request: UpdateEventRequest): Observable<ApiResponse<Event>> {
    return this.http.put<ApiResponse<Event>>(`/api/Events/${id}`, request);
  }

  deleteEvent(id: string): Observable<ApiResponse<boolean>> {
    return this.http.delete<ApiResponse<boolean>>(`/api/Events/${id}`);
  }

  publishEvent(id: string): Observable<ApiResponse<boolean>> {
    return this.http.patch<ApiResponse<boolean>>(`/api/Events/${id}/publish`, {});
  }

  cancelEvent(id: string): Observable<ApiResponse<boolean>> {
    return this.http.patch<ApiResponse<boolean>>(`/api/Events/${id}/cancel`, {});
  }

  getOrganizerEvents(): Observable<ApiResponse<Event[]>> {
    return this.http.get<ApiResponse<Event[]>>('/api/Events/my-events');
  }
}
