import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { SessionTimeoutInterceptor } from './session-timeout-interceptor';

export const httpInterceptorProviders = [
  { provide: HTTP_INTERCEPTORS, useClass: SessionTimeoutInterceptor, multi: true }
];
