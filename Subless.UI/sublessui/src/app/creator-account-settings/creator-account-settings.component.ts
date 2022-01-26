import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Creator } from '../models/Creator';
import { ICreator } from '../models/ICreator';
import { AuthorizationService } from '../services/authorization.service';
import { CreatorService } from '../services/creator.service';

@Component({
  selector: 'app-creator-account-settings',
  templateUrl: './creator-account-settings.component.html',
  styleUrls: ['./creator-account-settings.component.scss']
})
export class CreatorAccountSettingsComponent implements OnInit {
  private model$: Observable<ICreator> | undefined;
  private formDirty = false;
  public model: ICreator = new Creator("", "", "");
  constructor(private creatorService: CreatorService,
    private authService: AuthorizationService
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

}
