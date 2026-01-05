import { Injectable } from '@angular/core';
import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../../auth/services/auth.service';
import { catchError, switchMap } from 'rxjs/operators';
import { throwError } from 'rxjs';

@Injectable()

export class AuthInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService) { }

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const accessToken = this.authService.getAccessToken();

    if (accessToken) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${accessToken}`
        }
      });
    }

    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          return this.handleRefreshToken(request, next);
        }

        return throwError(() => error);
      })
    );
  }

  private handleRefreshToken(
    request: HttpRequest<any>,
    next: HttpHandler
  ) {
    return this.authService.refreshToken().pipe(
      switchMap((response: any) => {

        localStorage.setItem('access_token', response.accessToken);

        request = request.clone({
          setHeaders: {
            Authorization: `Bearer ${response.accessToken}`
          }
        });

        return next.handle(request);
      }),
      catchError(() => {
        this.authService.logout();
        return throwError(() => new Error('Session expired'));
      })
    );
  }
}
