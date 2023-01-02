import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-error-page',
  templateUrl: './error-page.component.html',
  styleUrls: ['./error-page.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class ErrorPageComponent implements OnInit {
  public errorTitle: string = "You broke subless!";
  public errorAction: string = "Tell us how by emailing <a href='mailto:contact@subless.com'>contact@subless.com</a>";
  constructor(private route: ActivatedRoute) { }
  ngOnInit() {
    // get URL parameters
    this.route.params.subscribe(params => {
      if (params.code == "expired") {
        this.errorTitle = "Your creator activation code has expired. This could happen if you don't immediately login after being directed to subless.";
        this.errorAction = "Please log out, and start the activation process again.";
      }
    });
  }

}
