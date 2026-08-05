import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AdminOverview, ApiResponse, Category } from '../models/types';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private http = inject(HttpClient);
  overview(): Observable<ApiResponse<AdminOverview>> { return this.http.get<ApiResponse<AdminOverview>>('/api/Admin/overview'); }
  publishEvent(id: string): Observable<ApiResponse<boolean>> { return this.http.patch<ApiResponse<boolean>>(`/api/Admin/events/${id}/publish`, {}); }
  rejectEvent(id: string): Observable<ApiResponse<boolean>> { return this.http.patch<ApiResponse<boolean>>(`/api/Admin/events/${id}/reject`, {}); }
  createCategory(name: string): Observable<ApiResponse<Category>> { return this.http.post<ApiResponse<Category>>('/api/Admin/categories', { name }); }
  deleteCategory(id: string): Observable<ApiResponse<boolean>> { return this.http.delete<ApiResponse<boolean>>(`/api/Admin/categories/${id}`); }
}
