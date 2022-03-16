import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { AuthorizationService } from '../services/authorization.service';
import { ActivatedRoute } from '@angular/router';



@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit, OnDestroy {

  constructor(@Inject(DOCUMENT) private document: Document,
    private authService: AuthorizationService,
    private route: ActivatedRoute
  ) { }
  ngOnDestroy(): void {
    this.authService.OnDestroy();
  }

  ngOnInit(): void {
    this.authService.checkLogin();
  }
}
