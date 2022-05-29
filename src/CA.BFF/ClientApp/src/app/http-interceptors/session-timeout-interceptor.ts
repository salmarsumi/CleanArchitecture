import { Injectable } from "@angular/core";
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable()
export class SessionTimeoutInterceptor implements HttpInterceptor {
  constructor(
  ) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

    //if (!req.headers.has('X-XSRF-TOKEN')) {
    //  const xsrf = this.cookieServices.getCookie('XSRF-TOKEN');
    //  req = req.clone({
    //    headers: req.headers.set('X-XSRF-TOKEN', xsrf)
    //  });
    //}
    
    return next.handle(req).pipe(
      tap(event => {
        // do nothing
      },
        error => {
          console.log(error);
          if (error.status === 0) {
            window.location.href = '/account/postlogin';
          }
        }
      )
    );
  }
}
