import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { ApiResponse, AuthResponse, User } from '../models/types';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  
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
    return this.http.post<ApiResponse<AuthResponse>>('/api/Auth/login', credentials).pipe(
      tap(response => {
        if (response.success && response.data) {
          const authData = response.data;
          const user: User = {
            id: '', // Empty in login DTO, but role & token are here
            firstName: authData.firstName,
            lastName: authData.lastName,
            email: authData.email,
            role: authData.role
          };
          localStorage.setItem('tazkara_token', authData.token);
          localStorage.setItem('tazkara_user', JSON.stringify(user));
          this.currentUserSubject.next(user);
        }
      })
    );
  }

  register(userData: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>('/api/Auth/register', userData);
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
        this.currentUserSubject.next(user);
      } catch (e) {
        this.logout();
      }
    }
  }
}
