import { Injectable, OnInit } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { ISettings } from '../models/ISettings';
import { IRedirect } from '../models/IRedirect';
import { Observable, of, Subscription, throwError } from 'rxjs';

import { catchError, map } from 'rxjs/operators';
import { IUser } from '../models/IUser';
import { NGXLogger } from 'ngx-logger';


@Injectable({
  providedIn: 'root'
})
export class AuthorizationService {
  private activation: string = '';
  private postActivationRedirect: string = '';
  private subs: Subscription[] = [];

  constructor(
    private httpClient: HttpClient,
    private router: Router,
    private route: ActivatedRoute,
    private logger: NGXLogger
  ) {
    this.subs.push(this.route.queryParams.subscribe(params => {
      this.activation = params['activation'];
      this.postActivationRedirect = params['postActivationRedirect'];
      if (this.activation) {
        sessionStorage.setItem('activation', this.activation);
      }
      if (this.postActivationRedirect) {
        sessionStorage.setItem('postActivationRedirect', this.postActivationRedirect);
      }
    }));
  }

  OnDestroy(): void {
    this.subs.forEach((item: Subscription) => { item.unsubscribe(); })
  }

  getSettings() {
    return this.httpClient.get<ISettings>('/api/Authorization/settings');
  }

  getRoutes() {
    return this.httpClient.get<number[]>('/api/Authorization/routes');
  }

  getUserData(): Observable<IUser | null> {
    return this.httpClient.get<IUser>('/api/User').pipe(
      catchError((err: HttpErrorResponse) => {
        if (err.status == 401) {
          return of(null);
        }
        return throwError(err);
      }));
  }

  public async checkLogin() {
    this.subs.push(this.getUserData().subscribe({
      next: async data => {
        if (data == null) {
          await this.getLoginLink()
        }
        else {
          this.redirect();
        }
      }
    }));
  }

  redirect() {
    this.logger.debug("Starting login");
    const activation = sessionStorage.getItem('activation');
    var headers = new HttpHeaders();

    if (activation) {
      sessionStorage.setItem('activation', '');
      var headers = new HttpHeaders();
      if (activation) {
        headers = new HttpHeaders(
          {
            "Activation": activation ?? '',
          });
      }
    }
    this.subs.push(this.httpClient.get<IRedirect>('/api/Authorization/redirect', { headers: headers }).subscribe({
      next: (redirectResponse: IRedirect) => {
        this.logger.debug("Login redirect");
        switch (redirectResponse.redirectionPath) {
          case 1:
            this.router.navigate(['register-payment']);
            break;
          case 2:
            this.router.navigate(['user-profile']);
            break;
          case 3:
            this.router.navigate(['creator-profile']);
            break;
          case 4:
            this.router.navigate(['partner-profile']);
            break;
          case 5:
            this.router.navigate(['payout-setup']);
            break;
          case 6:
            this.router.navigate(['terms']);
            break;
          case 7:
            this.router.navigate(['creator-terms']);
            break;
          case 8:
            this.router.navigate(['partner-terms']);
            break;
          default: {
            break;
          }
        }
      }
    }));
  }

  redirectToCreatorSurvey() {
    window.open("https://docs.google.com/forms/d/e/1FAIpQLSe24AZPj1IZ-UAsf_cj5zqLfIci3YTmB7YmLZT1Sr5cTYpM0Q/viewform?usp=pp_url&entry.487372404=Creator", "_blank");
    window.location.href = "/bff/logout";

  }
  redirectToSubscriberSurvey() {
    window.open("https://docs.google.com/forms/d/e/1FAIpQLSe24AZPj1IZ-UAsf_cj5zqLfIci3YTmB7YmLZT1Sr5cTYpM0Q/viewform?usp=pp_url&entry.487372404=Subscriber", "_blank");
    window.location.href = "/bff/logout";

  }
  redirectToLogout() {
    window.location.href = "/bff/logout";
  }

  getEmail(): Observable<string | null> {
    return this.getUserData().pipe(map((user: IUser | null) => {
      if (user) {
        return user.email;
      }
      return null;
    }))
  }

  public getLoginLink() {
    window.location.href = "/bff/login?returnUrl=" + window.location.href;
  }
}

