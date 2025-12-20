import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { API_ENDPOINTS } from '../constants/api.constants';
import {
  LoginRequest,
  RegisterRequest,
  AuthResponse,
  UserInfo,
  ForgotPasswordRequest,
  ResetPasswordRequest,
  UpdateUserRequest
} from '../models/auth.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private readonly TOKEN_KEY = 'auth_token';
  private readonly USER_KEY = 'user_data';

  // Login
  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(API_ENDPOINTS.AUTH.LOGIN, credentials)
      .pipe(
        tap(response => this.saveAuthData(response))
      );
  }

  // Register
  register(userData: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(API_ENDPOINTS.AUTH.REGISTER, userData)
      .pipe(
        tap(response => this.saveAuthData(response))
      );
  }

  // Forgot Password
  forgotPassword(request: ForgotPasswordRequest): Observable<void> {
    return this.http.post<void>(API_ENDPOINTS.AUTH.FORGOT_PASSWORD, request);
  }

  // Reset Password
  resetPassword(request: ResetPasswordRequest): Observable<void> {
    return this.http.post<void>(API_ENDPOINTS.AUTH.RESET_PASSWORD, request);
  }

  // Update User
  updateUser(request: UpdateUserRequest): Observable<UserInfo> {
    return this.http.put<UserInfo>(API_ENDPOINTS.AUTH.UPDATE_USER, request)
      .pipe(
        tap(user => this.updateUserData(user))
      );
  }

getCurrentUserValue(): UserInfo | null {
  const userData = localStorage.getItem('user_data');
  if (userData) {
    try {
      const parsed: AuthResponse = JSON.parse(userData);
      return {
        userId: parsed.userId,
        fullName: parsed.fullName,
        email: parsed.email
      };
    } catch {
      return null;
    }
  }
  return null;
}

  // Get Current User from API
  getCurrentUserFromApi(): Observable<UserInfo> {
    return this.http.get<UserInfo>(API_ENDPOINTS.AUTH.ME)
      .pipe(
        tap(user => {
          const currentData = this.getUserData();
          if (currentData) {
            const updatedData: AuthResponse = {
              ...currentData,
              fullName: user.fullName,
              email: user.email
            };
            localStorage.setItem(this.USER_KEY, JSON.stringify(updatedData));
          }
        })
      );
  }

  getCurrentUser(): Observable<UserInfo | null> {
    return new Observable(observer => {
      observer.next(this.getCurrentUserValue());

      window.addEventListener('storage', () => {
        observer.next(this.getCurrentUserValue());
      });
    });
  }

  // Token Management
  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) return false;
    
    // Check if token is expired
    const userData = this.getUserData();
    if (userData && userData.expiresAt) {
      return new Date(userData.expiresAt) > new Date();
    }
    return true;
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
  }

  getUserData(): AuthResponse | null {
    const userData = localStorage.getItem(this.USER_KEY);
    return userData ? JSON.parse(userData) : null;
  }

  private saveAuthData(response: AuthResponse): void {
    localStorage.setItem(this.TOKEN_KEY, response.token);
    localStorage.setItem(this.USER_KEY, JSON.stringify(response));
  }

  private updateUserData(user: UserInfo): void {
    const currentData = this.getUserData();
    if (currentData) {
      const updatedData: AuthResponse = {
        ...currentData,
        fullName: user.fullName,
        email: user.email
      };
      localStorage.setItem(this.USER_KEY, JSON.stringify(updatedData));
    }
  }
}