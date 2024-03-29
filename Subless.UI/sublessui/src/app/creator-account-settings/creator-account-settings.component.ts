import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatLegacyDialog as MatDialog, MatLegacyDialogConfig as MatDialogConfig } from '@angular/material/legacy-dialog';
import { Observable, Subscription } from 'rxjs';
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
export class CreatorAccountSettingsComponent implements OnInit, OnDestroy {
  private model$: Observable<ICreator[]> | undefined;
  private formDirty = false;
  public models: ICreator[] = [new Creator("", "", "")];
  private subs: Subscription[] = [];
  constructor(
    private creatorService: CreatorService,
    private authService: AuthorizationService,
    public dialog: MatDialog
  ) { }

  ngOnDestroy(): void {
    this.subs.forEach((item: Subscription) => { item.unsubscribe(); })
    this.authService.OnDestroy();
  }

  ngOnInit(): void {
    this.model$ = this.creatorService.getCreators();
    this.subs.push(this.model$.subscribe({
      next: (creators: ICreator[]) => {
        this.models = creators;
      }
    }));
  }
  unlink(id: string): void {
    this.subs.push(this.creatorService.unlinkCreator(id).subscribe({
      next: (success: boolean) => {
        this.authService.redirectToCreatorSurvey();
      }
    }));
  }
  openDialog(id: string) {
    const data: IDialogData = {
      title: 'Are you sure?',
      text: "<h3>Unlinking an account will result in the loss of ALL data related to this creator.</h3><h3>All views received this month will be discarded, you will not receive payments for them.</h3>",
      proceedText: 'Unlink',
      cancelText: 'Nevermind!',
    }
    const config = new MatDialogConfig();
    config.data = data;
    const dialogRef = this.dialog.open(WarnDialogComponent, config);

    this.subs.push(dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.unlink(id);
      }
    }));
  }

}
