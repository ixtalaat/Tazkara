import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ApiResponse, PaymentSessionRequest, PaymentSessionResponse, PaymentVerificationRequest, PaymentVerificationResponse } from '../models/types';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  createPaymentSession(ticketId: string, provider: number): Observable<ApiResponse<PaymentSessionResponse>> {
    const request: PaymentSessionRequest = { ticketId, provider };
    return this.http.post<ApiResponse<PaymentSessionResponse>>(`${this.apiUrl}/api/Payments/session`, request);
  }

  verifyPayment(transactionId: string, verificationToken: string): Observable<ApiResponse<PaymentVerificationResponse>> {
    const request: PaymentVerificationRequest = { transactionId, verificationToken };
    return this.http.post<ApiResponse<PaymentVerificationResponse>>(`${this.apiUrl}/api/Payments/verify`, request);
  }
}
