import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';
import { NGXLogger } from 'ngx-logger';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private logger: NGXLogger) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request)
      .pipe(tap(() => { },
        (err: any) => {
          if (err instanceof HttpErrorResponse) {
            if (err.status !== 500 && err.status !== 102 && err.status !== 504) {
              return;
            }
            this.logger.warn("Error: " + err.status);
            this.logger.warn("Message: " + err.message);
            this.router.navigate(['error']);
          }
        }));
  }
}
