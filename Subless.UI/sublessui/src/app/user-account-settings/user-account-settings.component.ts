import { Component, OnDestroy } from '@angular/core';
import { MatLegacyDialog as MatDialog, MatLegacyDialogConfig as MatDialogConfig } from '@angular/material/legacy-dialog';
import { Subscription } from 'rxjs';
import { IStripeRedirect } from '../models/IStripeRedirect';
import { SessionId } from '../models/SessionId';
import { AuthorizationService } from '../services/authorization.service';
import { CheckoutService } from '../services/checkout.service';
import { IDialogData, WarnDialogComponent } from '../warn-dialog/warn-dialog.component';

@Component({
  selector: 'app-user-account-settings',
  templateUrl: './user-account-settings.component.html',
  styleUrls: ['./user-account-settings.component.scss']
})
export class UserAccountSettingsComponent implements OnDestroy {

  private subs: Subscription[] = [];

  constructor(
    private checkoutService: CheckoutService,
    private authService: AuthorizationService,
    public dialog: MatDialog
  ) { }

  ngOnDestroy(): void {
    this.subs.forEach((item: Subscription) => { item.unsubscribe(); })
  }
  returnToStripe() {
    this.subs.push(this.checkoutService.getUserSession().subscribe({
      next: (sessionId: SessionId) => {
        this.checkoutService.loadCustomerPortal(sessionId.id).subscribe({
          next: (redirect: IStripeRedirect) => {
            window.location.href = redirect.url;
          }
        });
      }
    }));
  }

  cancelSubscription() {
    this.subs.push(this.checkoutService.cancelSubscription().subscribe({
      next: (result: boolean) => {
        this.authService.redirectToSubscriberSurvey();
      }
    }));
  }

  openDialog() {
    const data: IDialogData = {
      title: 'Are you sure?',
      text: "<h3>Your subscription will be cancelled, and you will not be billed next month.</h3><h3>Your subscription will remain valid till the end of your billing period.</h3>",
      proceedText: 'Cancel Subscription',
      cancelText: 'Nevermind!',
    }
    const config = new MatDialogConfig();
    config.data = data;
    const dialogRef = this.dialog.open(WarnDialogComponent, config);

    this.subs.push(dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.cancelSubscription();
      }
    }));
  }
}
