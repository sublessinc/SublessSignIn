import { formatDate } from '@angular/common';
import { ChangeDetectorRef, Component, Inject, LOCALE_ID, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { IHitView } from '../models/IHitView';
import { CreatorService } from '../services/creator.service';
import { DateFormatter } from '../services/dateformatter.service';

@Component({
  selector: 'app-recent-activity',
  templateUrl: './recent-activity.component.html',
  styleUrls: ['./recent-activity.component.scss']
})
export class RecentActivityComponent implements OnInit, OnDestroy {
  public recentHits: IHitView[] = [];
  private subs: Subscription[] = [];

  constructor(private creatorService: CreatorService,
    public dateService: DateFormatter,
    private changeDetector: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.subs.push(this.creatorService.getRecentFeed().subscribe({
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
