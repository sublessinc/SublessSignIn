import { ChangeDetectorRef, Component, Inject, LOCALE_ID, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { IPerCreatorHitCount } from '../models/IPerCreatorHitCount';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-user-top-content',
  templateUrl: './user-top-content.component.html',
  styleUrls: ['./user-top-content.component.scss']
})
export class UserTopContentComponent implements OnInit, OnDestroy {
  public topHits: IPerCreatorHitCount[] = [];
  private subs: Subscription[] = [];

  constructor(private userService: UserService,
    private changeDetector: ChangeDetectorRef,
    @Inject(LOCALE_ID) public locale: string) { }

  ngOnInit(): void {
    this.subs.push(this.userService.getTopFeed().subscribe({
      next: (hits: IPerCreatorHitCount[]) => {
        hits = hits.sort((a, b) => a.hits - b.hits)
        this.topHits = hits.reverse();
        this.changeDetector.detectChanges();
      }
    }));
  }

  ngOnDestroy(): void {
    this.subs.forEach((item: Subscription) => { item.unsubscribe(); })
  }
}
