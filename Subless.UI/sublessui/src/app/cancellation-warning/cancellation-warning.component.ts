import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, Subscription } from 'rxjs';
import { SubscriptionStatus } from '../models/SubscriptionStatus';
import { CheckoutService } from '../services/checkout.service';
import { DateFormatter } from '../services/dateformatter.service';

@Component({
  selector: 'app-cancellation-warning',
  templateUrl: './cancellation-warning.component.html',
  styleUrls: ['./cancellation-warning.component.scss']
})

export class CancellationWarningComponent implements OnInit, OnDestroy {
  public shouldWarn: boolean = false;
  private subs: Subscription[] = [];
  private model$: Observable<SubscriptionStatus> | null = null;
  protected model: SubscriptionStatus = new SubscriptionStatus(false, false, new Date(), new Date(), new Date());
  constructor(
    private checkoutService: CheckoutService,
    private changeDetector: ChangeDetectorRef,
    private dateFormatterService: DateFormatter,
    private router: Router) {

  }
  ngOnDestroy(): void {
    this.subs.forEach((item: Subscription) => { item.unsubscribe(); })
  }
  ngOnInit(): void {
    this.model$ = this.checkoutService.getSubscriptionStatus();
    this.subs.push(this.model$.subscribe({
      next: (sub: SubscriptionStatus) => {
        this.model = sub;
        this.shouldWarn = true;
        this.changeDetector.detectChanges()
      }
    }));
  }
  redirectToPlan() {
    this.router.navigate(["change-plan"]);
  }

  cancellationDate() {
    return this.dateFormatterService.formatDatestamp(this.model.cancellationDate);
  }
}
