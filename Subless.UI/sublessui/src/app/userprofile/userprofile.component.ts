import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { IAnalytics } from '../models/IAnalytics';
import { DateFormatter } from '../services/dateformatter.service';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-userprofile',
  templateUrl: './userprofile.component.html',
  styleUrls: ['./userprofile.component.scss']
})
export class UserprofileComponent implements OnInit, OnDestroy {
  public analytics: IAnalytics = {
    thisMonth: { views: 0, creators: 0, partners: 0, periodStart: new Date(), periodEnd: new Date() },
    lastMonth: { views: 0, creators: 0, partners: 0, periodStart: new Date(), periodEnd: new Date() }
  };
  private subs: Subscription[] = [];

  constructor(
    private userService: UserService,
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
    this.subs.push(this.userService.getAnalytics().subscribe({
      next: (analytics: IAnalytics) => {
        this.analytics = analytics;
        this.changeDetector.detectChanges();
      }
    }));
  }

}
