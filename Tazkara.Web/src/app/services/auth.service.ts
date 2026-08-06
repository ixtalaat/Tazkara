import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { ApiResponse, AuthResponse, User } from '../models/types';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;
  
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor() {
    this.loadUserFromStorage();
  }

  public get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }

  public get isLoggedIn(): boolean {
    return !!this.currentUserValue;
  }

  public get isOrganizer(): boolean {
    return this.currentUserValue?.role === 'Organizer';
  }

  public get isAdmin(): boolean {
    return this.currentUserValue?.role === 'Admin';
  }

  public get isCustomer(): boolean {
    return this.currentUserValue?.role === 'Customer';
  }

  login(credentials: any): Observable<ApiResponse<AuthResponse>> {
    return this.http.post<ApiResponse<AuthResponse>>(`${this.apiUrl}/api/Auth/login`, credentials).pipe(
      tap(response => {
        if (response.success && response.data) {
          const authData = response.data;
          const user: User = {
            id: authData.id,
            firstName: authData.firstName,
            lastName: authData.lastName,
            email: authData.email,
            role: authData.roles?.[0] || authData.role || ''
          };
          localStorage.setItem('tazkara_token', authData.token);
          localStorage.setItem('tazkara_user', JSON.stringify(user));
          this.currentUserSubject.next(user);
        }
      })
    );
  }

  register(userData: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/api/Auth/register`, userData);
  }

  logout(): void {
    localStorage.removeItem('tazkara_token');
    localStorage.removeItem('tazkara_user');
    this.currentUserSubject.next(null);
  }

  private loadUserFromStorage(): void {
    const token = localStorage.getItem('tazkara_token');
    const userJson = localStorage.getItem('tazkara_user');

    if (token && userJson) {
      try {
        const user = JSON.parse(userJson) as User;
        // Older sessions were stored from an AuthResponse that exposed `roles`
        // rather than a single `role`. Recover the role from the JWT so an
        // already logged-in customer does not need to clear browser storage.
        if (!user.role) {
          user.role = this.readRoleFromToken(token) || '';
        }
        this.currentUserSubject.next(user);
      } catch (e) {
        this.logout();
      }
    }
  }

  private readRoleFromToken(token: string): string | null {
    try {
      const payload = token.split('.')[1];
      const claims = JSON.parse(atob(payload.replace(/-/g, '+').replace(/_/g, '/')));
      const roleClaim = claims['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?? claims.role;
      return Array.isArray(roleClaim) ? roleClaim[0] ?? null : roleClaim ?? null;
    } catch {
      return null;
    }
  }
}
