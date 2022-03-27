import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { IAnalytics } from '../models/IAnalytics';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-userprofile',
  templateUrl: './userprofile.component.html',
  styleUrls: ['./userprofile.component.scss']
})
export class UserprofileComponent implements OnInit, OnDestroy {
  public analytics: IAnalytics = {
    thisMonth: { views: 0, creators: 0, partners: 0 },
    lastMonth: { views: 0, creators: 0, partners: 0 }
  };
  private subs: Subscription[] = [];

  constructor(
    private userService: UserService,
    private changeDetector: ChangeDetectorRef
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
