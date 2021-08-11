import { Injectable, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { ISettings } from '../models/ISettings';
import { ITokenResponse } from '../models/ITokenResponse';
import { IRedirect } from '../models/IRedirect';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { Observable } from 'rxjs';


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
    public oidcSecurityService: OidcSecurityService
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

  checkLogin() {
    this.oidcSecurityService.checkAuth().subscribe(({ isAuthenticated, userData, accessToken, idToken }) => {
      if (isAuthenticated) {
        this.redirect();
      }
      else {
        this.getLoginLink();
      }
    });
  }
  redirect() {
    const activation = sessionStorage.getItem('activation');
    var headers = new HttpHeaders().set("Activation", activation ?? '')
    if (activation) {
      sessionStorage.setItem('activation', '');
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
    this.oidcSecurityService.logoff();
    this.getSettings().subscribe({
      next: (settings) => {
        window.location.replace(
          settings.issuerUrl
          + "/logout?response_type=code&client_id="
          + settings.appClientId
          + "&logout_uri="
          + this.logoutURI
        );
      }
    })
  }

  getEmail(): string {
    return this.oidcSecurityService.getUserData().username;
  }

  public async getLoginLink() {
    this.oidcSecurityService.authorize();
  }
}

