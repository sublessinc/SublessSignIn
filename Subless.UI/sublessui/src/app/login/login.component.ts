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
    this.authService.checkLogin();
  }
}
