import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ApiResponse, Ticket, ReserveTicketRequest } from '../models/types';

@Injectable({
  providedIn: 'root'
})
export class TicketService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  reserveTicket(eventId: string): Observable<ApiResponse<Ticket>> {
    const request: ReserveTicketRequest = { eventId };
    return this.http.post<ApiResponse<Ticket>>(`${this.apiUrl}/api/Tickets/reserve`, request);
  }

  cancelReservation(ticketId: string): Observable<ApiResponse<boolean>> {
    return this.http.patch<ApiResponse<boolean>>(`${this.apiUrl}/api/Tickets/${ticketId}/cancel`, {});
  }

  getMyTickets(): Observable<ApiResponse<Ticket[]>> {
    return this.http.get<ApiResponse<Ticket[]>>(`${this.apiUrl}/api/Tickets/my-tickets`);
  }

  getTicketById(ticketId: string): Observable<ApiResponse<Ticket>> {
    return this.http.get<ApiResponse<Ticket>>(`${this.apiUrl}/api/Tickets/${ticketId}`);
  }
}
