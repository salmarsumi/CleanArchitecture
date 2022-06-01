import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { HeadersInterceptorInterceptor } from './headers-interceptor.interceptor';
import { SessionTimeoutInterceptor } from './session-timeout-interceptor';

export const httpInterceptorProviders = [
  { provide: HTTP_INTERCEPTORS, useClass: HeadersInterceptorInterceptor, multi: true },
  { provide: HTTP_INTERCEPTORS, useClass: SessionTimeoutInterceptor, multi: true }
];
