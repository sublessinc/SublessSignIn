import { Component } from '@angular/core';
import { TermsComponent } from '../terms/terms.component';

@Component({
  selector: 'app-partner-terms',
  templateUrl: './../terms/terms.component.html',
  styleUrls: ['./../terms/terms.component.css']
})
export class PartnerTermsComponent extends TermsComponent {
  public url = "https://www.subless.com/partner-terms";
  public accept(): void {
    this.subs.push(this.termsService.acceptPartnerTerms().subscribe({
      next: () => {
        this.authService.redirect();
      }
    }));
  }
}
