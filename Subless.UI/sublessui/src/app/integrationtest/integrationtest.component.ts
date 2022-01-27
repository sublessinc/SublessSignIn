import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { IPartner } from '../models/IPartner';
import { IPartnerWrite } from '../models/IPartnerWrite';
import { PartnerService } from '../services/partner.service';

@Component({
  selector: 'app-integrationtest',
  templateUrl: './integrationtest.component.html',
  styleUrls: ['./integrationtest.component.scss']
})
export class IntegrationtestComponent implements OnInit {
  private model$: Observable<IPartner> | undefined;
  public model: IPartner = { payPalId: "", site: "", userPattern: "", creatorWebhook: "", id: "" };
  constructor(private partnerService: PartnerService) { }

  ngOnInit(): void {
    this.model$ = this.partnerService.getPartner();
    this.model$.subscribe({
      next: (partner: IPartner) => {
        this.model = partner;
      }
    })
  }

  onSubmit(): void {
    var writeModel: IPartnerWrite = { id: this.model.id, payPalId: this.model.payPalId, creatorWebhook: this.model.creatorWebhook }
    this.model$ = this.partnerService.updatePartner(writeModel);
    this.model$.subscribe({
      next: (partner: IPartner) => {
        this.model = partner;
      }
    })
  }
}
