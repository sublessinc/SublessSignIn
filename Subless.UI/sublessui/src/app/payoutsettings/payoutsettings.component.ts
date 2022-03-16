import { AfterViewInit, Component, HostListener, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { Observable, Subscription } from 'rxjs';
import { Creator } from '../models/Creator';
import { ICreator } from '../models/ICreator';
import { IPartner } from '../models/IPartner';
import { Partner } from '../models/Partner';
import { AuthorizationService } from '../services/authorization.service';
import { CreatorService } from '../services/creator.service';
import { PartnerService } from '../services/partner.service';
import { ComponentCanDeactivate } from '../stop-nav.guard';

@Component({
  selector: 'app-payoutsettings',
  templateUrl: './payoutsettings.component.html',
  styleUrls: ['./payoutsettings.component.scss']
})
export class PayoutsettingsComponent implements OnInit, ComponentCanDeactivate, OnDestroy {
  public activationRedirectUrl: string | null = null;
  public email: string = '';
  private creatorModel$: Observable<ICreator> | undefined;
  public creatorModel: ICreator = new Creator("", "", "");
  private partnerModel$: Observable<IPartner> | undefined;
  public partnerModel: IPartner = new Partner("", "", "", "", "", "");
  public backgroundClass: string = "lightBackground";
  public isModal: boolean = false;
  public creator: boolean = false;
  public partner: boolean = false;
  private subs: Subscription[] = [];

  constructor(private router: Router,
    private creatorService: CreatorService,
    private partnerService: PartnerService,
    private authService: AuthorizationService
  ) {
    this.activationRedirectUrl = sessionStorage.getItem('postActivationRedirect');
  }

  ngOnInit(): void {
    this.subs.push(this.authService.getEmail().subscribe({
      next: (email: string | null) => {
        this.email = email ?? '';
      }
    }));
    if (this.router.url.startsWith("/payout-setup")) {
      this.isModal = true;
      this.backgroundClass = "darkBackground";
    }

    this.subs.push(this.authService.getRoutes().subscribe({
      next: (routes: number[]) => {
        this.creator = routes.includes(3);
        this.partner = routes.includes(4);
        if (this.creator) {
          this.creatorModel$ = this.creatorService.getCreator();
          this.subs.push(this.creatorModel$.subscribe({
            next: (creator: ICreator) => {
              this.creatorModel = creator;
            }
          }));
        }
        if (this.partner) {
          this.partnerModel$ = this.partnerService.getPartner();
          this.subs.push(this.partnerModel$.subscribe({
            next: (partner: IPartner) => {
              this.partnerModel = partner;
            }
          }));
        }
      }
    }));
  }
  ngOnDestroy(): void {
    this.subs.forEach((item: Subscription) => { item.unsubscribe(); })
    this.authService.OnDestroy();
  }

  finalize() {
    if (this.activationRedirectUrl) {
      this.creatorService.finalizeViaRedirect(this.activationRedirectUrl, this.email, this.creatorModel.username);
    }
    else {
      this.router.navigate(["payout-settings"]);
    }
  }
  onCreatorSubmit(): void {
    this.creatorModel$ = this.creatorService.updateCreator(this.creatorModel);
    this.subs.push(this.creatorModel$.subscribe({
      next: (creator: ICreator) => {
        this.creatorModel = creator;
        if (this.isModal) {
          this.finalize();
        }
      }
    }));
  }
  onPartnerSubmit(): void {
    this.partnerModel$ = this.partnerService.updatePartner(this.partnerModel);
    this.subs.push(this.partnerModel$.subscribe({
      next: (partner: IPartner) => {
        this.partnerModel = partner;
        if (this.isModal) {
          this.finalize();
        }
      }
    }));
  }


  @HostListener('window:beforeunload')
  canDeactivate(): Observable<boolean> | boolean {
    // insert logic to check if there are pending changes here;
    // returning true will navigate without confirmation
    // returning false will show a confirm dialog before navigating away
    return !((this.creatorModel.payPalId == null || this.creatorModel.payPalId == ""));
  }

}

