import { Injectable, OnDestroy } from '@angular/core';
import {
    HttpRequest,
    HttpHandler,
    HttpEvent,
    HttpInterceptor,
    HttpErrorResponse
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { AuthorizationService } from './authorization.service';

@Injectable()
export class UnauthorizedInterceptor implements HttpInterceptor, OnDestroy {

    constructor(private authService: AuthorizationService) { }

    ngOnDestroy(): void {
        this.authService.OnDestroy();
    }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

        return next.handle(request)
            .pipe(tap(() => { },
                (err: any) => {
                    if (err instanceof HttpErrorResponse) {
                        if (err.status !== 401) {
                            return;
                        }
                        this.authService.redirectToLogout();
                    }
                }));
    }
}