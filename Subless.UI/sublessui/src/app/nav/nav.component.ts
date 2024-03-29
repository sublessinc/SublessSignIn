import { Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatDrawer } from '@angular/material/sidenav';
import { MatLegacySnackBar as MatSnackBar } from '@angular/material/legacy-snack-bar';
import { NavigationEnd, Router } from '@angular/router';
import { CookieService } from 'ngx-cookie';
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
  public cancelledSub: boolean = false;
  private isSmallScreen: boolean = window.innerWidth <= 700
  private subs: Subscription[] = [];

  constructor(
    private authService: AuthorizationService,
    private checkoutService: CheckoutService,
    protected router: Router,
    private cookieService: CookieService,
    private _snackBar: MatSnackBar,


  ) { }

  ngOnInit(): void {
    this.subs.push(this.authService.getRoutes().subscribe({
      next: (routes: number[]) => {
        this.user = routes.includes(2);
        this.creator = routes.includes(3);
        this.partner = routes.includes(4);
        this.cancelledSub = routes.includes(9);
        this.promptRedirectIfCookiePresent();
        this.hideLoader();
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
    return !this.router.url.startsWith("/register-payment")
      && !this.router.url.startsWith("/payout-setup")
      && !this.router.url.startsWith("/terms")
      && !this.router.url.startsWith("/creator-terms")
      && !this.router.url.startsWith("/partner-terms")
      && !this.router.url.startsWith("/renew");
  }

  hideLoader() {
    const loader = document.getElementById("loadingContainer");
    if (loader && loader?.style.display !== "none") {
      loader.style.display = "none";
    }
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

  promptRedirectIfCookiePresent() {
    const redirect = this.cookieService.get("returnUri");
    this.cookieService.remove("returnUri");
    if (redirect && this.user) {

      const snackBarRef = this._snackBar.open(`Your account setup is complete, would you like to return to ${redirect}?`, "Take me back", {
        duration: 5000,
      })

      snackBarRef.onAction().subscribe(() => {
        window.location.href = redirect;
      });

    }
  }

  showCancelWarning() {
    return (this.cancelledSub
      && !this.router.url.startsWith('/register-payment')
      && !this.router.url.startsWith('/change-plan'));
  }

  logout() {
    sessionStorage.removeItem("activation");
    sessionStorage.removeItem("postActivationRedirect");
    this.authService.redirectToLogout();
  }
}