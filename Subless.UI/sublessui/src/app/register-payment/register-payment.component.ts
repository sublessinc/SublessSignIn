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
export class RegisterPaymentComponent implements OnInit, AfterViewInit {
  private stripe: any;
  private settings!: ICheckoutSettings;
  public priceChosen: number | null = null;
  @ViewChild('sublessbackground') sublessbackground: ElementRef | null = null;

  constructor(
    private checkoutService: CheckoutService,
    private elementRef: ElementRef,
    private changeDetector: ChangeDetectorRef
  ) { }
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

  pickPrice() {
    this.changeDetector.detectChanges();
    console.warn(this.priceChosen);
  }
  redirectToCheckout() {
    this.checkoutService.createCheckoutSession(this.settings.basicPrice).subscribe({
      next: (session: ISessionResponse) => {
        this.stripe.redirectToCheckout({ sessionId: session.sessionId });
      }
    });
  }

}

