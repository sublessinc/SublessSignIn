import { Component } from '@angular/core';
import { TermsComponent } from '../terms/terms.component';

@Component({
  selector: 'creator-terms',
  templateUrl: './../terms/terms.component.html',
  styleUrls: ['./../terms/terms.component.css']
})
export class CreatorTermsComponent extends TermsComponent {
  public url = "https://www.subless.com/creator-terms";
  acceptMethod = this.termsService.acceptCreatorTerms;

}
