import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { IStripeRedirect } from '../models/IStripeRedirect';
import { SessionId } from '../models/SessionId';
import { AuthorizationService } from '../services/authorization.service';
import { CheckoutService } from '../services/checkout.service';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-userprofile',
  templateUrl: './userprofile.component.html',
  styleUrls: ['./userprofile.component.scss']
})
export class UserprofileComponent implements OnInit {
  constructor(
    private route: ActivatedRoute,
    private checkoutService: CheckoutService,
    private authService: AuthorizationService,
    private userService: UserService
  ) { }

  ngOnInit(): void { }

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

  deleteAccount() {
    this.userService.deleteUser().subscribe({
      next: (completed: boolean) => {
        this.authService.redirectToLogout();
      }
    })
  }


  cancelSubscription() {
    this.checkoutService.cancelSubscription().subscribe({
      next: (completed: boolean) => {
        this.authService.redirect();
      }
    })
  }
}
