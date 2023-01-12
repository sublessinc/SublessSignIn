import { ContentObserver } from '@angular/cdk/observers';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AfterViewInit, ChangeDetectorRef, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatButtonToggle, ToggleType } from '@angular/material/button-toggle';
import { Router } from '@angular/router';
import { ICheckoutSettings } from '../models/ICheckoutSettings';
import { ISessionResponse } from '../models/ISessionResponse';
import { CheckoutService } from '../services/checkout.service';
import { MatLegacySnackBar as MatSnackBar } from '@angular/material/legacy-snack-bar';
import { Subscription } from 'rxjs';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
declare var Stripe: any;

@Component({
  selector: 'app-register-payment',
  templateUrl: './register-payment.component.html',
  styleUrls: ['./register-payment.component.scss']
})
export class RegisterPaymentComponent implements OnInit, OnDestroy {
  private stripe: any;
  private settings!: ICheckoutSettings;
  public backgroundClass: string = "lightBackground";
  public priceChosen: string = "10";
  public currentPlan: string | null = null;
  public budgetDifference: number | null = null;
  private subs: Subscription[] = [];
  @ViewChild('customPrice') customPrice: ElementRef | null = null;
  constructor(
    private checkoutService: CheckoutService,
    private changeDetector: ChangeDetectorRef,
    private router: Router,
    private _snackBar: MatSnackBar,
  ) {
  }
  ngOnInit(): void {
    this.getCheckoutSettings();
    if (this.router.url.startsWith("/register-payment")) {
      this.backgroundClass = "darkBackground";
    }
    this.fetchBudget();
  }

  fetchBudget(): void {
    this.subs.push(this.checkoutService.getCurrentPlan().subscribe({
      next: (plan: number | null) => {
        if (plan != null) {
          this.priceChosen = plan.toString();
          this.currentPlan = plan.toString();
          this.changeDetector.detectChanges();
        }
      }
    }));
  }

  ngOnDestroy(): void {
    this.subs.forEach((item: Subscription) => { item.unsubscribe(); })
  }
  getCheckoutSettings() {
    this.subs.push(this.checkoutService.getCheckoutSettings().subscribe({
      next: (settings: ICheckoutSettings) => {
        this.stripe = Stripe(settings.publishableKey);
        this.settings = settings;
      }
    }));
  }


  public setCustomValue() {
    this.changeDetector.detectChanges();
  }


  redirectToCheckout() {
    if (!this.priceChosen) {
      return;
    }
    const price = Number.parseInt(this.priceChosen);
    this.subs.push(this.checkoutService.createCheckoutSession(price).subscribe({
      next: (session: ISessionResponse) => {
        if (session != null) {
          this.stripe.redirectToCheckout({ sessionId: session.sessionId });
        }
        else {
          this._snackBar.open("Saved", "Ok", {
            duration: 2000,
          });
          this.fetchBudget();
        }
      }
    }));
  }
}

