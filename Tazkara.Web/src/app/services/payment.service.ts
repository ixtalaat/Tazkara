import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse, PaymentSessionRequest, PaymentSessionResponse, PaymentVerificationRequest, PaymentVerificationResponse } from '../models/types';

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  private http = inject(HttpClient);

  createPaymentSession(ticketId: string, provider: number): Observable<ApiResponse<PaymentSessionResponse>> {
    const request: PaymentSessionRequest = { ticketId, provider };
    return this.http.post<ApiResponse<PaymentSessionResponse>>('/api/Payments/session', request);
  }

  verifyPayment(transactionId: string, verificationToken: string): Observable<ApiResponse<PaymentVerificationResponse>> {
    const request: PaymentVerificationRequest = { transactionId, verificationToken };
    return this.http.post<ApiResponse<PaymentVerificationResponse>>('/api/Payments/verify', request);
  }
}
