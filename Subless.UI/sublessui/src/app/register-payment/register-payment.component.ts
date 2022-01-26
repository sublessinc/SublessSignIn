import { ContentObserver } from '@angular/cdk/observers';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AfterViewInit, ChangeDetectorRef, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ToggleType } from '@angular/material/button-toggle';
import { ActivatedRoute, Router } from '@angular/router';
import { ICheckoutSettings } from '../models/ICheckoutSettings';
import { ISessionResponse } from '../models/ISessionResponse';
import { IStripeRedirect } from '../models/IStripeRedirect';
import { CheckoutService } from '../services/checkout.service';
declare var Stripe: any;

@Component({
  selector: 'app-register-payment',
  templateUrl: './register-payment.component.html',
  styleUrls: ['./register-payment.component.scss']
})
export class RegisterPaymentComponent implements OnInit {
  private stripe: any;
  private settings!: ICheckoutSettings;
  public backgroundClass: string = "lightBackground";
  public priceChosen: number | null = null;

  constructor(
    private checkoutService: CheckoutService,
    private elementRef: ElementRef,
    private changeDetector: ChangeDetectorRef,
    private router: Router
  ) { }
  ngOnInit(): void {
    this.getCheckoutSettings();
    if (this.router.url.startsWith("/register-payment")) {
      this.backgroundClass = "darkBackground";
    }
  }

  getCheckoutSettings() {
    this.checkoutService.getCheckoutSettings().subscribe({
      next: (settings: ICheckoutSettings) => {
        this.stripe = Stripe(settings.publishableKey);
        this.settings = settings;
      }
    });
  }

  pickPrice() {
    this.changeDetector.detectChanges();
    console.warn(this.priceChosen);
  }
  redirectToCheckout() {
    this.checkoutService.createCheckoutSession(this.priceChosen).subscribe({
      next: (session: ISessionResponse) => {
        this.stripe.redirectToCheckout({ sessionId: session.sessionId });
      }
    });
  }

}

