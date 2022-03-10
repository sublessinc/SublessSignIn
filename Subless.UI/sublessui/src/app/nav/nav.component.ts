import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { MatDrawer, MatSidenav } from '@angular/material/sidenav';
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
  @ViewChild('content') sublessbackground: ElementRef | null = null;
  @ViewChild('drawer') drawer: MatDrawer | null = null;

  public user: boolean = false;
  public creator: boolean = false;
  public partner: boolean = false;
  public showHamburger: boolean = false;
  constructor(
    private authService: AuthorizationService,
    private checkoutService: CheckoutService,
    private router: Router

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

  ngAfterViewInit() {
    if (window.innerWidth <= 700) {
      this.drawer?.toggle();
      if (this.drawer != null) {
        this.drawer!.mode = "over";
      }
    } else {
      this.showHamburger = true;
    }
  }

  hamburgerPress() {
    this.drawer?.toggle();
  }

  showDrawer() {
    return !this.router.url.startsWith("/register-payment") && !this.router.url.startsWith("/payout-setup");
  }

  returnToStripe() {
    this.checkoutService.getUserSession().subscribe({
      next: (sessionId: SessionId) => {
        this.checkoutService.loadCustomerPortal(sessionId.id).subscribe({
          next: (redirect: IStripeRedirect) => {
            window.open(redirect.url, "_blank", 'noreferrer')!.focus();
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