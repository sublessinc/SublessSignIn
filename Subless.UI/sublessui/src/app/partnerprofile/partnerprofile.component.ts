import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { IPartner } from '../models/IPartner';
import { PartnerService } from '../services/partner.service';

@Component({
  selector: 'app-partnerprofile',
  templateUrl: './partnerprofile.component.html',
  styleUrls: ['./partnerprofile.component.css']
})
export class PartnerprofileComponent implements OnInit {
  private model$: Observable<IPartner> | undefined;
  public model: IPartner =  { PayoneerId:"", Site:"", UserPattern:"" };
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

  }

}
