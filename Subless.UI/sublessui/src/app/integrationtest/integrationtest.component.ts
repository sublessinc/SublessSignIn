import { Component, OnDestroy, OnInit } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { IPartner } from '../models/IPartner';
import { IPartnerWrite } from '../models/IPartnerWrite';
import { PartnerService } from '../services/partner.service';

@Component({
  selector: 'app-integrationtest',
  templateUrl: './integrationtest.component.html',
  styleUrls: ['./integrationtest.component.scss']
})
export class IntegrationtestComponent implements OnInit, OnDestroy {
  private model$: Observable<IPartner> | undefined;
  public model: IPartner = { payPalId: "", site: "", userPattern: "", creatorWebhook: "", id: "" };
  private subs: Subscription[] = [];

  constructor(private partnerService: PartnerService) { }

  ngOnInit(): void {
    this.model$ = this.partnerService.getPartner();
    this.subs.push(this.model$.subscribe({
      next: (partner: IPartner) => {
        this.model = partner;
      }
    }));
  }
  ngOnDestroy(): void {
    this.subs.forEach((item: Subscription) => { item.unsubscribe(); })
  }
  onSubmit(): void {
    var writeModel: IPartnerWrite = { id: this.model.id, payPalId: this.model.payPalId, creatorWebhook: this.model.creatorWebhook }
    this.model$ = this.partnerService.updatePartner(writeModel);
    this.subs.push(this.model$.subscribe({
      next: (partner: IPartner) => {
        this.model = partner;
      }
    }));
  }
}
