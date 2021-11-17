import { Injectable } from '@angular/core';
import {
    HttpRequest,
    HttpHandler,
    HttpEvent,
    HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthorizationService } from './authorization.service';

@Injectable()
export class UserDataInterceptor implements HttpInterceptor {

    constructor(private authService: AuthorizationService) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        var email = this.authService.getEmail();
        if (email) {
            const userDataRequest = request.clone({ headers: request.headers.set('email', email) });
            return next.handle(userDataRequest)
        }
        else {
            return next.handle(request);
        }

    }
}