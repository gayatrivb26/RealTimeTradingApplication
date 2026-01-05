import { Injectable } from '@angular/core';
import { ApiService } from '../../core/services/api.services';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})

export class AuthService {
  private readonly ACCESS_TOKEN_KEY = 'access_token';

  constructor(private api: ApiService) { }

  login(email: string, password: string) {
    return this.api.post<any>('/auth/login', { email, password })
      .pipe(
        tap(response => {
          localStorage.setItem(this.ACCESS_TOKEN_KEY, response.accessToken);
        })
      );
  }

  logout() {
    localStorage.removeItem(this.ACCESS_TOKEN_KEY);
    return this.api.post('/auth/logout', {});
  }

  getAccessToken(): string | null {
    return localStorage.getItem(this.ACCESS_TOKEN_KEY);
  }

  refreshToken() {
    return this.api.post<any>('/auth/refresh', {});
  }

  isLoggedIn(): boolean{
    return !!this.getAccessToken();
  }
}
