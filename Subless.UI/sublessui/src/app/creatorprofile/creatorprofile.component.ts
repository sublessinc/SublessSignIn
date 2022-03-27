import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { ICreatorAnalytics } from '../models/ICreatorAnalytics';
import { CreatorService } from '../services/creator.service';

@Component({
  selector: 'app-creatorprofile',
  templateUrl: './creatorprofile.component.html',
  styleUrls: ['./creatorprofile.component.scss']
})
export class CreatorprofileComponent implements OnInit, OnDestroy {

  public analytics: ICreatorAnalytics = {
    thisMonth: { views: 0, visitors: 0, piecesOfContent: 0 },
    lastMonth: { views: 0, visitors: 0, piecesOfContent: 0 }
  };
  private subs: Subscription[] = [];

  constructor(
    private creatorService: CreatorService,
    private changeDetector: ChangeDetectorRef
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
