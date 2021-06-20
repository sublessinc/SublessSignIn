import { Component, OnInit } from '@angular/core';
import { AuthorizationService } from '../services/authorization.service';

@Component({
  selector: 'app-logout',
  templateUrl: './logout.component.html',
  styleUrls: ['./logout.component.css']
})
export class LogoutComponent implements OnInit {

  constructor(
    private authService: AuthorizationService
  ) { }

  ngOnInit(): void {
  }

  logout() {
    sessionStorage.setItem("state", '');
    sessionStorage.setItem("id_token", '');
    sessionStorage.setItem("access_token", '');
    sessionStorage.setItem("activation", '');
    this.authService.redirectToLogout();
  }
}
