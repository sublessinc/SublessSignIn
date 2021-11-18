import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Observable } from 'rxjs';
import { IPartner } from '../models/IPartner';
import { IPartnerWrite } from '../models/IPartnerWrite';
import { PartnerService } from '../services/partner.service';
import { ComponentCanDeactivate } from '../stop-nav.guard';

@Component({
  selector: 'app-partnerprofile',
  templateUrl: './partnerprofile.component.html',
  styleUrls: ['./partnerprofile.component.css']
})
export class PartnerprofileComponent implements OnInit, ComponentCanDeactivate {
  private model$: Observable<IPartner> | undefined;
  public model: IPartner = { payPalId: "", site: "", userPattern: "", creatorWebhook: "", id: "" };
  private formDirty = false;
  @ViewChild('partnerForm', { read: NgForm }) payPalForm: any;

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
    this.formDirty = false;
  }
  ngAfterViewInit(): void {
    this.payPalForm.valueChanges
      .subscribe((value: string) => {
        this.formDirty = this.payPalForm.dirty;
      });
  }

  @HostListener('window:beforeunload')
  canDeactivate(): Observable<boolean> | boolean {
    // insert logic to check if there are pending changes here;
    // returning true will navigate without confirmation
    // returning false will show a confirm dialog before navigating away
    return !(this.model.payPalId == null || this.model.payPalId == "") && !this.formDirty;
  }
}
