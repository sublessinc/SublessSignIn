import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ICheckoutSettings } from '../models/ICheckoutSettings';
import { ISessionResponse } from '../models/ISessionResponse';
import { IStripeRedirect } from '../models/IStripeRedirect';
import { SessionId } from '../models/SessionId';

declare var Stripe: any;
@Injectable({
  providedIn: 'root'
})
export class CheckoutService {

  private stripe: any;

  constructor(private httpClient: HttpClient) { }

  getCheckoutSettings(): Observable<ICheckoutSettings> {
    return this.httpClient.get<ICheckoutSettings>('/api/Checkout/setup');
  }

  createCheckoutSession(price: number | null): Observable<ISessionResponse> {
    return this.httpClient.post<ISessionResponse>("/api/Checkout/create-checkout-session", {
      priceId: price
    });
  }

  getCheckoutSession(sessionId: string): Observable<ISessionResponse> {
    return this.httpClient.get<ISessionResponse>("/api/Checkout/checkout-session?sessionId=" + sessionId);
  }

  loadCustomerPortal(sessionId: string): Observable<IStripeRedirect> {
    return this.httpClient.post<IStripeRedirect>("/api/Checkout/customer-portal", {
      sessionId: sessionId
    });
  }

  getUserSession(): Observable<SessionId> {
    return this.httpClient.get<SessionId>('/api/Checkout/existing-session');
  }

  cancelSubscription(): Observable<boolean> {
    return this.httpClient.delete<boolean>("/api/Checkout/");
  }

  getCurrentPlan(): Observable<number | null> {
    return this.httpClient.get<number | null>("/api/Checkout/plan");
  }
}

