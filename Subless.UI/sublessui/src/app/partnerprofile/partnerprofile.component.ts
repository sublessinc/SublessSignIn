import { ChangeDetectorRef, Component, HostListener, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Observable, Subscription } from 'rxjs';
import { IPartner } from '../models/IPartner';
import { IPartnerAnalytics } from '../models/IPartnerAnalytics';
import { IPartnerWrite } from '../models/IPartnerWrite';
import { DateFormatter } from '../services/dateformatter.service';
import { PartnerService } from '../services/partner.service';
import { ComponentCanDeactivate } from '../stop-nav.guard';

@Component({
  selector: 'app-partnerprofile',
  templateUrl: './partnerprofile.component.html',
  styleUrls: ['./partnerprofile.component.scss']
})
export class PartnerprofileComponent implements OnInit, OnDestroy {
  public analytics: IPartnerAnalytics = {
    thisMonth: { views: 0, creators: 0, visitors: 0, periodStart: new Date(), periodEnd: new Date() },
    lastMonth: { views: 0, creators: 0, visitors: 0, periodStart: new Date(), periodEnd: new Date() }
  };
  private subs: Subscription[] = [];

  constructor(private partnerService: PartnerService,
    private changeDetector: ChangeDetectorRef,
    public dateFormatter: DateFormatter) { }

  ngOnInit(): void {
    this.subs.push(this.partnerService.getAnalytics().subscribe
      ({
        next: (analytics: IPartnerAnalytics) => {
          this.analytics = analytics;
          this.changeDetector.detectChanges();
        }
      }));
  }
  ngOnDestroy(): void {
    this.subs.forEach((item: Subscription) => { item.unsubscribe(); })
  }
}

