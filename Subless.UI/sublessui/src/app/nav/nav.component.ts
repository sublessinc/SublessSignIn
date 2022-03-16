import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatDrawer, MatSidenav } from '@angular/material/sidenav';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { IStripeRedirect } from '../models/IStripeRedirect';
import { SessionId } from '../models/SessionId';
import { AuthorizationService } from '../services/authorization.service';
import { CheckoutService } from '../services/checkout.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit, OnDestroy {
  @ViewChild('content') sublessbackground: ElementRef | null = null;
  @ViewChild('drawer') drawer: MatDrawer | null = null;

  public user: boolean = false;
  public creator: boolean = false;
  public partner: boolean = false;
  public showHamburger: boolean = false;
  private subs: Subscription[] = [];

  constructor(
    private authService: AuthorizationService,
    private checkoutService: CheckoutService,
    private router: Router

  ) { }

  ngOnInit(): void {
    this.subs.push(this.authService.getRoutes().subscribe({
      next: (routes: number[]) => {
        this.user = routes.includes(2);
        this.creator = routes.includes(3);
        this.partner = routes.includes(4);
      }
    }));
    if (window.innerWidth <= 700) {
      this.drawer?.toggle();
      if (this.drawer != null) {
        this.drawer!.mode = "over";
      }
    } else {
      this.showHamburger = true;
    }
  }
  ngOnDestroy(): void {
    this.subs.forEach((item: Subscription) => { item.unsubscribe(); });
    this.authService.OnDestroy();
  }
  ngAfterViewInit() {

  }

  hamburgerPress() {
    this.drawer?.toggle();
  }

  showDrawer() {
    return !this.router.url.startsWith("/register-payment") && !this.router.url.startsWith("/payout-setup");
  }

  returnToStripe() {
    this.subs.push(this.checkoutService.getUserSession().subscribe({
      next: (sessionId: SessionId) => {
        this.checkoutService.loadCustomerPortal(sessionId.id).subscribe({
          next: (redirect: IStripeRedirect) => {
            const win = window.open(redirect.url, "_blank", 'noreferrer');
            if (win) {
              win.focus();
            }
          }
        });
      }
    }));
  }

  logout() {
    sessionStorage.removeItem("activation");
    sessionStorage.removeItem("postActivationRedirect");
    this.authService.redirectToLogout();
  }
}