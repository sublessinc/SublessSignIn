import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class TermsService {

  constructor(private httpClient: HttpClient) { }

  acceptTerms(): Observable<void> {
    return this.httpClient.put<void>('/api/user/terms', null);
  }
  acceptCreatorTerms(): Observable<void> {
    return this.httpClient.put<void>('/api/creator/terms', null);
  }

  acceptPartnerTerms(): Observable<void> {
    return this.httpClient.put<void>('/api/partner/terms', null);
  }
}
