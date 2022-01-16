import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { IStripeRedirect } from '../models/IStripeRedirect';
import { SessionId } from '../models/SessionId';
import { AuthorizationService } from '../services/authorization.service';
import { CheckoutService } from '../services/checkout.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit {

  public user: boolean = false;
  public creator: boolean = false;
  public partner: boolean = false;
  constructor(
    private router: Router,
    private authService: AuthorizationService,
    private checkoutService: CheckoutService
  ) { }

  ngOnInit(): void {
    this.authService.getRoutes().subscribe({
      next: (routes: number[]) => {
        this.user = routes.includes(2);
        this.creator = routes.includes(3);
        this.partner = routes.includes(4);
      }
    });
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
  logout() {
    sessionStorage.removeItem("activation");
    sessionStorage.removeItem("postActivationRedirect");
    this.authService.redirectToLogout();
  }
}
