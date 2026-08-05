import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse, OrganizerDashboardResponse } from '../models/types';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private http = inject(HttpClient);

  getOrganizerDashboard(): Observable<ApiResponse<OrganizerDashboardResponse>> {
    return this.http.get<ApiResponse<OrganizerDashboardResponse>>('/api/Dashboard/organizer');
  }
}
