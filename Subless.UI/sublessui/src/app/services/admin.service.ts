import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { OidcSecurityService } from 'angular-auth-oidc-client';
import { Observable } from 'rxjs';
import { ICreator } from '../models/ICreator';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  constructor(private httpClient: HttpClient,
    private oidcSecurityService: OidcSecurityService
    ) { }

  getId(): Observable<string> {
    return this.httpClient.get<string>('/api/Admin/user');
  }

  getToken(): string {
    return this.oidcSecurityService.getIdToken();
  }
}
