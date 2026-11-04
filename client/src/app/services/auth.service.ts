
import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';

export interface User {
  id: number;
  username: string;
  email: string;
}

export interface LoginResponse {
  token: string;
  tokenType: string;
  expiresAt: string;
  user: User;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'http://localhost:5277/api/auth';

  readonly token = signal<string | null>(
    localStorage.getItem('travelonion_token')
  );

  readonly currentUser = signal<User | null>(
    this.loadUser()
  );

  readonly isLoggedIn = () => !!this.token();

  register(username: string, email: string, password: string) {
    return this.http.post<User>(`${this.apiUrl}/register`, {
      username,
      email,
      password
    });
  }

  login(email: string, password: string) {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, {
      email,
      password
    }).pipe(
      tap(response => {
        localStorage.setItem('travelonion_token', response.token);
        localStorage.setItem(
          'travelonion_user',
          JSON.stringify(response.user)
        );

        this.token.set(response.token);
        this.currentUser.set(response.user);
      })
    );
  }

  logout() {
    localStorage.removeItem('travelonion_token');
    localStorage.removeItem('travelonion_user');

    this.token.set(null);
    this.currentUser.set(null);
  }

  private loadUser(): User | null {
    const stored = localStorage.getItem('travelonion_user');

    if (!stored) return null;

    try {
      return JSON.parse(stored) as User;
    } catch {
      return null;
    }
  }
}
