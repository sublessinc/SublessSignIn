import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { Creator } from '../models/Creator';
import { ICreator } from '../models/ICreator';
import { AuthorizationService } from '../services/authorization.service';
import { CreatorService } from '../services/creator.service';
import { IDialogData, WarnDialogComponent } from '../warn-dialog/warn-dialog.component';

@Component({
  selector: 'app-creator-account-settings',
  templateUrl: './creator-account-settings.component.html',
  styleUrls: ['./creator-account-settings.component.scss']
})
export class CreatorAccountSettingsComponent implements OnInit {
  private model$: Observable<ICreator> | undefined;
  private formDirty = false;
  public model: ICreator = new Creator("", "", "");
  constructor(
    private creatorService: CreatorService,
    private authService: AuthorizationService,
    public dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.model$ = this.creatorService.getCreator();
    this.model$.subscribe({
      next: (creator: ICreator) => {
        this.model = creator;
      }
    })
  }
  unlink(): void {
    this.creatorService.unlinkCreator(this.model).subscribe({
      next: (success: boolean) => {
        this.authService.redirectToLogout();
      }
    });
  }
  openDialog() {
    const data: IDialogData = {
      title: 'Are you sure?',
      text: "<h3>Unlinking an account will result in the loss of ALL data related to this creator.</h3><h3>All views received this month will be discarded, you will not receive payments for them.</h3>",
      proceedText: 'Unlink',
      cancelText: 'Nevermind!',
    }
    const config = new MatDialogConfig();
    config.data = data;
    const dialogRef = this.dialog.open(WarnDialogComponent, config);

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.unlink();
      }
    });
  }

}
