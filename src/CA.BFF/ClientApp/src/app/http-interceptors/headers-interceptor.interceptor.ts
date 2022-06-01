import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { CookieService } from '../services/CookieService';

@Injectable()
export class HeadersInterceptorInterceptor implements HttpInterceptor {

  constructor(private cookieService: CookieService) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {

    if (!request.headers.has('X-CSRF')) {
      const xsrf = this.cookieService.getCookie('__Host-CSRF-Token');
      request = request.clone({
        headers: request.headers.set('X-CSRF', xsrf)
      });
    }

    return next.handle(request);
  }
}
