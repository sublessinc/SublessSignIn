import { Injectable, OnInit } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { ISettings } from '../models/ISettings';
import { IRedirect } from '../models/IRedirect';
import { Observable, of, throwError } from 'rxjs';

import { catchError } from 'rxjs/operators';


@Injectable({
  providedIn: 'root'
})
export class AuthorizationService {
  private baseURI: string = '';
  private redirectURI: string = '';
  private logoutURI: string = '';
  private activation: string = '';
  private postActivationRedirect: string = '';
  constructor(
    private httpClient: HttpClient,
    private router: Router,
    private route: ActivatedRoute,
    // public oidcSecurityService: OidcSecurityService
  ) {
    this.baseURI = location.protocol + '//' + window.location.hostname + (location.port ? ':' + location.port : '');
    this.redirectURI = this.baseURI + "/login";
    this.logoutURI = this.baseURI + "/login";
    this.route.queryParams.subscribe(params => {
      this.activation = params['activation'];
      this.postActivationRedirect = params['postActivationRedirect'];
      if (this.activation) {
        sessionStorage.setItem('activation', this.activation);
      }
      if (this.postActivationRedirect) {
        sessionStorage.setItem('postActivationRedirect', this.postActivationRedirect);
      }
    });
  }

  getSettings() {
    return this.httpClient.get<ISettings>('/api/Authorization/settings');
  }

  getRoutes() {
    return this.httpClient.get<number[]>('/api/Authorization/routes');
  }

  getUserData(): Observable<string | null> {
    return this.httpClient.get<string>('/api/User').pipe(
      catchError((err: HttpErrorResponse) => {
        if (err.status == 401) {
          return of(null);
        }
        return throwError(err);
      }));
  }

  public async checkLogin() {
    this.getUserData().subscribe({
      next: async data => {
        if (data == null) {
          await this.getLoginLink()
        }
        else {
          this.redirect();
        }
      }
    });
  }

  redirect() {
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
    this.httpClient.get<IRedirect>('/api/Authorization/redirect', { headers: headers }).subscribe({
      next: (redirectResponse: IRedirect) => {
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
          default: {
            break;
          }
        }
      }
    });
  }

  redirectToLogout() {
    window.location.href = "/bff/logout";
  }

  getEmail(): string {
    // if (this.oidcSecurityService.isAuthenticated()) {
    //   var data = this.oidcSecurityService.getUserData();
    //   if (data && data.email) {
    //     return data.email;
    //   }
    // }
    return "";
  }

  public getLoginLink() {
    window.location.href = "/bff/login?returnUrl=" + window.location.href;
  }
}

