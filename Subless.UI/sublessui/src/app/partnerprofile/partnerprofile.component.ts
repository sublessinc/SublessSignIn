import { ChangeDetectorRef, Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Observable } from 'rxjs';
import { IPartner } from '../models/IPartner';
import { IPartnerAnalytics } from '../models/IPartnerAnalytics';
import { IPartnerWrite } from '../models/IPartnerWrite';
import { PartnerService } from '../services/partner.service';
import { ComponentCanDeactivate } from '../stop-nav.guard';

@Component({
  selector: 'app-partnerprofile',
  templateUrl: './partnerprofile.component.html',
  styleUrls: ['./partnerprofile.component.scss']
})
export class PartnerprofileComponent implements OnInit {
  public analytics: IPartnerAnalytics = {
    thisMonth: { views: 0, creators: 0, visitors: 0 },
    lastMonth: { views: 0, creators: 0, visitors: 0 }
  };

  constructor(private partnerService: PartnerService,
    private changeDetector: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.partnerService.getAnalytics().subscribe
      ({
        next: (analytics: IPartnerAnalytics) => {
          this.analytics = analytics;
          this.changeDetector.detectChanges();
        }
      })
  }
}

