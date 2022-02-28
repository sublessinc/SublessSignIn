import { ContentObserver } from '@angular/cdk/observers';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AfterViewInit, ChangeDetectorRef, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { ToggleType } from '@angular/material/button-toggle';
import { ActivatedRoute, Router } from '@angular/router';
import { ICheckoutSettings } from '../models/ICheckoutSettings';
import { ISessionResponse } from '../models/ISessionResponse';
import { IStripeRedirect } from '../models/IStripeRedirect';
import { CheckoutService } from '../services/checkout.service';
import { MatSnackBar } from '@angular/material/snack-bar';
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
  public priceChosen: string | null = null;

  constructor(
    private checkoutService: CheckoutService,
    private elementRef: ElementRef,
    private changeDetector: ChangeDetectorRef,
    private router: Router,
    private _snackBar: MatSnackBar
  ) { }
  ngOnInit(): void {
    this.getCheckoutSettings();
    if (this.router.url.startsWith("/register-payment")) {
      this.backgroundClass = "darkBackground";
    }
    this.checkoutService.getCurrentPlan().subscribe({
      next: (plan: number | null) => {
        if (plan != null) {
          this.priceChosen = plan.toString();
          this.changeDetector.detectChanges();
        }
      }
    });
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
    if (!this.priceChosen) {
      return;
    }
    const price = Number.parseInt(this.priceChosen);
    this.checkoutService.createCheckoutSession(price).subscribe({
      next: (session: ISessionResponse) => {
        if (session != null) {
          this.stripe.redirectToCheckout({ sessionId: session.sessionId });
        }
        else {
          this._snackBar.open("Saved", "Ok", {
            duration: 2000,
          });
        }
      }
    });
  }
}

