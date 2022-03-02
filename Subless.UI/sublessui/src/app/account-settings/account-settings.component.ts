import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { IStripeRedirect } from '../models/IStripeRedirect';
import { SessionId } from '../models/SessionId';
import { AuthorizationService } from '../services/authorization.service';
import { CheckoutService } from '../services/checkout.service';
import { CreatorService } from '../services/creator.service';
import { UserService } from '../services/user.service';
import { MatDialog } from '@angular/material/dialog';
import { WarnDialogComponent } from '../warn-dialog/warn-dialog.component';


@Component({
  selector: 'app-account-settings',
  templateUrl: './account-settings.component.html',
  styleUrls: ['./account-settings.component.scss']
})
export class AccountSettingsComponent implements OnInit {
  public user: boolean = false;
  public creator: boolean = false;
  public partner: boolean = false;

  constructor(

    private authService: AuthorizationService,
    private userService: UserService,
    public dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.authService.getRoutes().subscribe({
      next: (routes: number[]) => {
        this.user = routes.includes(2);
        this.creator = routes.includes(3);
        this.partner = routes.includes(4);
      }
    });
  }



  deleteAccount() {
    this.userService.deleteUser().subscribe({
      next: (completed: boolean) => {
        this.authService.redirectToLogout();
      }
    })
  }

  openDialog() {
    const dialogRef = this.dialog.open(WarnDialogComponent);

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.deleteAccount();
      }
    });
  }
}