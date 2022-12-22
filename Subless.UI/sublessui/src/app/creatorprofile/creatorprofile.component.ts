import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { ICreatorAnalytics } from '../models/ICreatorAnalytics';
import { CreatorService } from '../services/creator.service';
import { DateFormatter } from '../services/dateformatter.service';

@Component({
  selector: 'app-creatorprofile',
  templateUrl: './creatorprofile.component.html',
  styleUrls: ['./creatorprofile.component.scss']
})
export class CreatorprofileComponent implements OnInit, OnDestroy {

  public analytics: ICreatorAnalytics = {
    thisMonth: { views: 0, visitors: 0, piecesOfContent: 0, periodStart: new Date(), periodEnd: new Date() },
    lastMonth: { views: 0, visitors: 0, piecesOfContent: 0, periodStart: new Date(), periodEnd: new Date() }
  };
  private subs: Subscription[] = [];

  constructor(
    private creatorService: CreatorService,
    private changeDetector: ChangeDetectorRef,
    public dateFormatter: DateFormatter
  ) { }

  ngOnInit(): void {
    this.getAnalytics();
  }

  ngOnDestroy(): void {
    this.subs.forEach((item: Subscription) => { item.unsubscribe(); })
  }

  getAnalytics() {
    this.subs.push(this.creatorService.getAnalytics().subscribe({
      next: (analytics: ICreatorAnalytics) => {
        this.analytics = analytics;
        this.changeDetector.detectChanges();
      }
    }));
  }



}
