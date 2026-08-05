import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse, Ticket, ReserveTicketRequest } from '../models/types';

@Injectable({
  providedIn: 'root'
})
export class TicketService {
  private http = inject(HttpClient);

  reserveTicket(eventId: string): Observable<ApiResponse<Ticket>> {
    const request: ReserveTicketRequest = { eventId };
    return this.http.post<ApiResponse<Ticket>>('/api/Tickets/reserve', request);
  }

  cancelReservation(ticketId: string): Observable<ApiResponse<boolean>> {
    return this.http.patch<ApiResponse<boolean>>(`/api/Tickets/${ticketId}/cancel`, {});
  }

  getMyTickets(): Observable<ApiResponse<Ticket[]>> {
    return this.http.get<ApiResponse<Ticket[]>>('/api/Tickets/my-tickets');
  }

  getTicketById(ticketId: string): Observable<ApiResponse<Ticket>> {
    return this.http.get<ApiResponse<Ticket>>(`/api/Tickets/${ticketId}`);
  }
}
