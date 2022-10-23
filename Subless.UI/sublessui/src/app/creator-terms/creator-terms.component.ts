import { Component } from '@angular/core';
import { TermsComponent } from '../terms/terms.component';

@Component({
  selector: 'app-creator-terms',
  templateUrl: './../terms/terms.component.html',
  styleUrls: ['./../terms/terms.component.css']
})
export class CreatorTermsComponent extends TermsComponent {
  public url = "https://www.subless.com/creator-terms";
  public accept(): void {
    this.subs.push(this.termsService.acceptCreatorTerms().subscribe({
      next: () => {
        this.authService.redirect();
      }
    }));
  }
}
