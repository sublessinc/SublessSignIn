import { Component, Inject, OnInit } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { AuthorizationService } from '../services/authorization.service';
import { ActivatedRoute } from '@angular/router';



@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(@Inject(DOCUMENT) private document: Document,
    private authService: AuthorizationService,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    this.redirectIfNotloggedIn();
  }

  async redirectIfNotloggedIn(): Promise<void> {
    this.route.queryParams
      .subscribe(async params => {
        const codeParameter = params.code;
        const state = params.state;
        var idToken = sessionStorage.getItem("id_token");
        var authToken = sessionStorage.getItem("access_token");
        if ((!idToken || !authToken) && !(codeParameter)) {
          sessionStorage.setItem("state", '');
          sessionStorage.setItem("id_token", '');
          sessionStorage.setItem("access_token", '');
          await this.authService.getLoginLink();
        }
        if (codeParameter) {
          this.authService.processAuthCode(codeParameter, state);
        }
        if (idToken && authToken) {
          this.authService.redirect();
        }
      }
      );
  }


}
