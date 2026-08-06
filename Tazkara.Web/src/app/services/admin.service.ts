import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { AdminOverview, ApiResponse, Category } from '../models/types';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;
  overview(): Observable<ApiResponse<AdminOverview>> { return this.http.get<ApiResponse<AdminOverview>>(`${this.apiUrl}/api/Admin/overview`); }
  publishEvent(id: string): Observable<ApiResponse<boolean>> { return this.http.patch<ApiResponse<boolean>>(`${this.apiUrl}/api/Admin/events/${id}/publish`, {}); }
  rejectEvent(id: string): Observable<ApiResponse<boolean>> { return this.http.patch<ApiResponse<boolean>>(`${this.apiUrl}/api/Admin/events/${id}/reject`, {}); }
  createCategory(name: string): Observable<ApiResponse<Category>> { return this.http.post<ApiResponse<Category>>(`${this.apiUrl}/api/Admin/categories`, { name }); }
  deleteCategory(id: string): Observable<ApiResponse<boolean>> { return this.http.delete<ApiResponse<boolean>>(`${this.apiUrl}/api/Admin/categories/${id}`); }
}
