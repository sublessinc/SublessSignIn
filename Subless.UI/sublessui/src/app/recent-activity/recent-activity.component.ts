import { formatDate } from '@angular/common';
import { ChangeDetectorRef, Component, Inject, LOCALE_ID, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { IHitView } from '../models/IHitView';
import { CreatorService } from '../services/creator.service';

@Component({
  selector: 'app-recent-activity',
  templateUrl: './recent-activity.component.html',
  styleUrls: ['./recent-activity.component.scss']
})
export class RecentActivityComponent implements OnInit, OnDestroy {
  public recentHits: IHitView[] = [];
  private subs: Subscription[] = [];

  constructor(private creatorService: CreatorService,
    private changeDetector: ChangeDetectorRef,
    @Inject(LOCALE_ID) public locale: string) { }

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

  public formatDatestamp(timestamp: Date): string {
    if (formatDate(new Date(), 'dd-MM-yyyy', this.locale) === formatDate(timestamp, 'dd-MM-yyyy', this.locale)) {
      return "Today";
    }
    return formatDate(timestamp, 'dd-MM-yyyy', this.locale);
  }
  public formatTimestamp(timestamp: Date): string {
    return formatDate(timestamp, 'HH:MM', this.locale);
  }
}
