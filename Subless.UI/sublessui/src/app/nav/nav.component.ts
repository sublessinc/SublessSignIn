import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthorizationService } from '../services/authorization.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  public user: boolean = false;
  public creator: boolean = false;
  public partner: boolean = false;
  constructor(
    private router: Router,
    private authService: AuthorizationService
  ) { }

  ngOnInit(): void {
    this.authService.getRoutes().subscribe({
      next: (routes: number[]) => {
        this.user = routes.includes(2);
        this.creator = routes.includes(3);
        this.partner = routes.includes(4);
      }});
  }

  logout() {
    sessionStorage.removeItem("activation");
    sessionStorage.removeItem("postActivationRedirect");
    this.authService.redirectToLogout();
  }
}
