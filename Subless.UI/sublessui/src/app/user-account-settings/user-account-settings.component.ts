import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
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
export class UserAccountSettingsComponent implements OnInit {

  constructor(
    private checkoutService: CheckoutService,
    private authService: AuthorizationService,
    public dialog: MatDialog
  ) { }

  ngOnInit(): void {
  }

  returnToStripe() {
    this.checkoutService.getUserSession().subscribe({
      next: (sessionId: SessionId) => {
        this.checkoutService.loadCustomerPortal(sessionId.id).subscribe({
          next: (redirect: IStripeRedirect) => {
            window.location.href = redirect.url;
          }
        });
      }
    });
  }

  cancelSubscription() {
    this.checkoutService.cancelSubscription().subscribe({
      next: (completed: boolean) => {
        this.authService.redirect();
      }
    })
  }

  openDialog() {
    const data: IDialogData = {
      title: 'Are you sure?',
      text: "<h3>Your subscription will be cancelled, and you will not be billed next month.</h3><h3>A partial refund for this month's remaining balance will be refunded to your payment method.</h3>",
      proceedText: 'Cancel Subscription',
      cancelText: 'Nevermind!',
    }
    const config = new MatDialogConfig();
    config.data = data;
    const dialogRef = this.dialog.open(WarnDialogComponent, config);

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.cancelSubscription();
      }
    });
  }
}
