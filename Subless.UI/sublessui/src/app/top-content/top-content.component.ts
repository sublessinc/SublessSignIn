import { formatDate } from '@angular/common';
import { ChangeDetectorRef, Component, Inject, LOCALE_ID, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { IHitCount } from '../models/IHitCount';
import { CreatorService } from '../services/creator.service';

@Component({
  selector: 'app-top-content',
  templateUrl: './top-content.component.html',
  styleUrls: ['./top-content.component.scss']
})
export class TopContentComponent implements OnInit, OnDestroy {
  public topHits: IHitCount[] = [];
  private subs: Subscription[] = [];

  constructor(private creatorService: CreatorService,
    private changeDetector: ChangeDetectorRef,
    @Inject(LOCALE_ID) public locale: string) { }

  ngOnInit(): void {
    this.subs.push(this.creatorService.getTopFeed().subscribe({
      next: (hits: IHitCount[]) => {
        this.topHits = hits;
        this.changeDetector.detectChanges();
      }
    }));
  }

  ngOnDestroy(): void {
    this.subs.forEach((item: Subscription) => { item.unsubscribe(); })
  }
}
