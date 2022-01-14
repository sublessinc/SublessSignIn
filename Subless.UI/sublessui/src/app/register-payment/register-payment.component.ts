import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
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
export class RegisterPaymentComponent implements OnInit, AfterViewInit {
  private stripe: any;
  private settings!: ICheckoutSettings;
  @ViewChild('sublessbackground') sublessbackground: ElementRef | null = null;

  constructor(
    private checkoutService: CheckoutService,
    private elementRef: ElementRef) { }
  ngAfterViewInit(): void {
    this.elementRef.nativeElement.ownerDocument
      .body.style.backgroundColor = getComputedStyle(this.sublessbackground!.nativeElement).backgroundColor;
  }
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

