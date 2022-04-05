import { Component, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { AuthorizationService } from '../services/authorization.service';
import { TermsService } from '../services/terms.service';

@Component({
  selector: 'app-terms',
  templateUrl: './terms.component.html',
  styleUrls: ['./terms.component.css']
})
export class TermsComponent implements OnInit, OnDestroy {
  private subs: Subscription[] = [];
  public url: string = "https://www.subless.com/terms";
  protected acceptMethod = this.termsService.acceptTerms;
  constructor(
    protected termsService: TermsService,
    private authService: AuthorizationService) { }

  ngOnInit(): void {
  }
  ngOnDestroy(): void {
    this.subs.forEach((item: Subscription) => { item.unsubscribe(); })
  }
  public accept(): void {
    this.subs.push(this.acceptMethod().subscribe({
      next: () => {
        this.authService.redirect();
      }
    }));
  }
}
