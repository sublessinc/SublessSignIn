import { Component } from '@angular/core';
import { TermsComponent } from '../terms/terms.component';

@Component({
  selector: 'partner-terms',
  templateUrl: './../terms/terms.component.html',
  styleUrls: ['./../terms/terms.component.css']
})
export class PartnerTermsComponent extends TermsComponent {
  public url = "https://www.subless.com/partner-terms";
  acceptMethod = this.termsService.acceptPartnerTerms;

}
