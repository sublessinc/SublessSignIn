import { Component, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { AuthorizationService } from '../services/authorization.service';
import { CreatorService } from '../services/creator.service';
import { PartnerService } from '../services/partner.service';

@Component({
  selector: 'app-partner-test',
  templateUrl: './partner-test.component.html',
  styleUrls: ['./partner-test.component.scss']
})
export class PartnerTestComponent implements OnDestroy {

  public webhookFired: Boolean = false;
  public webhookFailed: Boolean = false;
  public model = { uri: "", username: "" };
  private subs: Subscription[] = [];

  constructor(private partnerService: PartnerService, private creatorService: CreatorService, private authService: AuthorizationService) { }

  ngOnDestroy(): void {
    this.subs.forEach((item: Subscription) => { item.unsubscribe(); })
    this.authService.OnDestroy();
  }
  fireWebhook() {
    this.subs.push(this.partnerService.testWebhook().subscribe({
      next: (fired: Boolean) => {
        this.webhookFired = fired;
        this.webhookFailed = !fired;
      }
    }));
  }

  triggerRedirect() {
    this.subs.push(this.authService.getEmail().subscribe({
      next: (email: string | null) => {
        this.creatorService.finalizeViaRedirect(this.model.uri, email ?? "", this.model.username);
      }
    }));
  }
}

