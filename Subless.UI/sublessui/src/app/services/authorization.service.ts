import { Injectable, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { ISettings } from '../models/ISettings';
import { ITokenResponse } from '../models/ITokenResponse';
import { IRedirect } from '../models/IRedirect';
import { OidcSecurityService } from 'angular-auth-oidc-client';


@Injectable({
  providedIn: 'root'
})
export class AuthorizationService implements OnInit {
  private baseURI: string = '';
  private redirectURI: string = '';
  private logoutURI: string = '';
  private code_verifier: string = '';
  constructor(
    private httpClient: HttpClient,
    private router: Router,
    public oidcSecurityService: OidcSecurityService
  ) {
    this.baseURI = location.protocol + '//' + window.location.hostname + (location.port ? ':' + location.port : '');
    this.redirectURI = this.baseURI + "/login";
    this.logoutURI = this.baseURI + "/login";
  }

  ngOnInit() {

  }
  getSettings() {
    return this.httpClient.get<ISettings>('/api/Authorization/settings');
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
    var headers = new HttpHeaders().set("Activation", activation ?? '');
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

  public async getLoginLink() {
    this.oidcSecurityService.authorize();
  }
}

