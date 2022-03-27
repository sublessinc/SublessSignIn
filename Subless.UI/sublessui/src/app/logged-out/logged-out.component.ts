import { Component, OnDestroy, OnInit } from '@angular/core';
import { AuthorizationService } from '../services/authorization.service';

@Component({
  selector: 'app-logged-out',
  templateUrl: './logged-out.component.html',
  styleUrls: ['./logged-out.component.css']
})
export class LoggedOutComponent implements OnInit, OnDestroy {

  constructor(private authService: AuthorizationService) { }
  ngOnDestroy(): void {
    this.authService.OnDestroy();
  }

  ngOnInit(): void {
    this.authService.redirectToLogout();
  }

}
