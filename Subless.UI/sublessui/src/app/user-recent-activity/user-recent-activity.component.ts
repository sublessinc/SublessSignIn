import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { IHitView } from '../models/IHitView';
import { DateFormatter } from '../services/dateformatter.service';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-user-recent-activity',
  templateUrl: './user-recent-activity.component.html',
  styleUrls: ['./user-recent-activity.component.scss']
})
export class UserRecentActivityComponent implements OnInit, OnDestroy {
  public recentHits: IHitView[] = [];
  private subs: Subscription[] = [];

  constructor(private userService: UserService,
    public dateService: DateFormatter,
    private changeDetector: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.subs.push(this.userService.getRecentFeed().subscribe({
      next: (hits: IHitView[]) => {
        this.recentHits = hits;
        this.changeDetector.detectChanges();
      }
    }));
  }

  ngOnDestroy(): void {
    this.subs.forEach((item: Subscription) => { item.unsubscribe(); })
  }
}