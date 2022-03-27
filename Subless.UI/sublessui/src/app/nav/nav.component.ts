import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatDrawer } from '@angular/material/sidenav';
import { NavigationEnd, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';
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
  private isSmallScreen: boolean = window.innerWidth <= 700
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
      this.isSmallScreen = true;
      this.drawer?.toggle();
      if (this.drawer != null) {
        this.drawer!.mode = "over";
      }
    } else {
      this.showHamburger = true;
    }
    this.subs.push(
      // grab all the routing activity
      this.router.events.pipe(
        filter(
          (e) =>
            // we only care about when navigation is complete
            e instanceof NavigationEnd
        )).subscribe((e) => {
          // if it's a small screen, hide the drawer
          if (this.isSmallScreen) {
            this.drawer?.toggle();
          }
        }));
  }
  ngOnDestroy(): void {
    this.subs.forEach((item: Subscription) => { item.unsubscribe(); });
    this.authService.OnDestroy();
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