import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ApiResponse, OrganizerDashboardResponse } from '../models/types';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  getOrganizerDashboard(): Observable<ApiResponse<OrganizerDashboardResponse>> {
    return this.http.get<ApiResponse<OrganizerDashboardResponse>>(`${this.apiUrl}/api/Dashboard/organizer`);
  }
}
