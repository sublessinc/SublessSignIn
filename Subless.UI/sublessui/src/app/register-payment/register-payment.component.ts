import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { ICheckoutSettings } from '../models/ICheckoutSettings';
import { ISessionResponse } from '../models/ISessionResponse';
import { CheckoutService } from '../services/checkout.service';
declare var Stripe: any;

@Component({
  selector: 'app-register-payment',
  templateUrl: './register-payment.component.html',
  styleUrls: ['./register-payment.component.css']
})
export class RegisterPaymentComponent implements OnInit {
  private stripe: any;
  private settings!: ICheckoutSettings;
  constructor(private httpClient: HttpClient,
    private checkoutService: CheckoutService) { }
  ngOnInit(): void {
    this.getCheckoutSettings();
  }

  getCheckoutSettings() {
    this.checkoutService.getCheckoutSettings().subscribe({
      next: (settings: ICheckoutSettings) => {
        this.stripe = Stripe(settings.publishableKey);
        this.settings = settings;
      }
    });
  }

  redirectToCheckout() {
    this.checkoutService.createCheckoutSession(this.settings.basicPrice).subscribe({
      next: (session: ISessionResponse) => {
        this.stripe.redirectToCheckout({ sessionId: session.sessionId });
      }
    });
  }
}

