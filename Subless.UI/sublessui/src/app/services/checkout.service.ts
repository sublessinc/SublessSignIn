import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ICheckoutSettings } from '../models/ICheckoutSettings';
import { ISessionResponse } from '../models/ISessionResponse';

declare var Stripe: any;
@Injectable({
  providedIn: 'root'
})
export class CheckoutService {

  private stripe: any;

  constructor(private httpClient: HttpClient) { }

  getCheckoutSettings(): Observable<ICheckoutSettings> {
    const authHeader = "Bearer " + sessionStorage.getItem('id_token');
    var headers = new HttpHeaders().set('Authorization', authHeader);
    return this.httpClient.get<ICheckoutSettings>('/api/Checkout/setup', { headers });
  }

  createCheckoutSession(priceId: string): Observable<ISessionResponse> {
    const authHeader = "Bearer " + sessionStorage.getItem('id_token');
    var headers = new HttpHeaders().set('Authorization', authHeader);
    return this.httpClient.post<ISessionResponse>("/api/Checkout/create-checkout-session", {
      priceId: priceId
    }, { headers });
  }
}


