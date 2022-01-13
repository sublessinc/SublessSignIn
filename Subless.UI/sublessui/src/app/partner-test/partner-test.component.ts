import { Component, OnInit } from '@angular/core';
import { AuthorizationService } from '../services/authorization.service';
import { CreatorService } from '../services/creator.service';
import { PartnerService } from '../services/partner.service';

@Component({
  selector: 'app-partner-test',
  templateUrl: './partner-test.component.html',
  styleUrls: ['./partner-test.component.css']
})
export class PartnerTestComponent implements OnInit {

  public webhookFired: Boolean = false;
  public webhookFailed: Boolean = false;
  public model = { uri: "", username: "" };

  constructor(private partnerService: PartnerService, private creatorService: CreatorService, private authService: AuthorizationService) { }

  ngOnInit(): void {
  }

  fireWebhook() {
    this.partnerService.testWebhook().subscribe({
      next: (fired: Boolean) => {
        this.webhookFired = fired;
        this.webhookFailed = !fired;
      }
    });
  }

  triggerRedirect() {
    this.authService.getEmail().subscribe({
      next: (email: string | null) => {
        this.creatorService.finalizeViaRedirect(this.model.uri, email ?? "", this.model.username);
      }
    });
  }
}

