import { AfterViewInit, Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { Creator } from '../models/Creator';
import { ICreator } from '../models/ICreator';
import { CreatorService } from '../services/creator.service';
import { ComponentCanDeactivate } from '../stop-nav.guard';

@Component({
  selector: 'app-payoutsettings',
  templateUrl: './payoutsettings.component.html',
  styleUrls: ['./payoutsettings.component.scss']
})
export class PayoutsettingsComponent implements OnInit, ComponentCanDeactivate {
  public activationRedirectUrl: string | null = null;
  public email: string = '';
  private model$: Observable<ICreator> | undefined;
  private formDirty = false;
  public model: ICreator = new Creator("", "", "");
  public backgroundClass: string = "lightBackground";
  public isModal: boolean = false;

  @ViewChild('creatorForm', { read: NgForm }) payPalForm: any;
  constructor(private router: Router,
    private creatorService: CreatorService
  ) {
    this.activationRedirectUrl = sessionStorage.getItem('postActivationRedirect');
  }

  ngOnInit(): void {
    if (this.router.url.startsWith("/creator-payout-setup")) {
      this.isModal = true;
      this.backgroundClass = "darkBackground";
    }
    this.model$ = this.creatorService.getCreator();
    this.model$.subscribe({
      next: (creator: ICreator) => {
        this.model = creator;
      }
    })

  }


  finalize() {
    if (this.activationRedirectUrl) {
      this.creatorService.finalizeViaRedirect(this.activationRedirectUrl, this.email, this.model.username);
    }
    else {
      this.router.navigate(["creator-payout-settings"]);
    }
  }
  onSubmit(): void {
    this.model$ = this.creatorService.updateCreator(this.model);
    this.model$.subscribe({
      next: (creator: ICreator) => {
        this.model = creator;
        if (this.isModal) {
          this.finalize();
        }
      }
    })
    this.formDirty = false;
  }


  @HostListener('window:beforeunload')
  canDeactivate(): Observable<boolean> | boolean {
    // insert logic to check if there are pending changes here;
    // returning true will navigate without confirmation
    // returning false will show a confirm dialog before navigating away
    return !((this.model.payPalId == null || this.model.payPalId == "") && !this.formDirty);
  }

}

