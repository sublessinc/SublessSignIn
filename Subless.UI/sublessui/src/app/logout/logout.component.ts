import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthorizationService } from '../services/authorization.service';

@Component({
  selector: 'app-logout',
  templateUrl: './logout.component.html',
  styleUrls: ['./logout.component.css']
})
export class LogoutComponent implements OnInit {

  constructor(
    private router: Router,
    private authService: AuthorizationService
  ) { }

  ngOnInit(): void {
    
  }

  partner(): void {
    this.router.navigateByUrl('/partner-profile');
  }

  creator(): void {
    this.router.navigateByUrl('/creator-profile');
  }

  logout() {
    sessionStorage.removeItem("activation");
    this.authService.redirectToLogout();
  }
}
