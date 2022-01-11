import { AfterViewInit, Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Observable } from 'rxjs';
import { ICreator } from '../models/ICreator';
import { AuthorizationService } from '../services/authorization.service';
import { CreatorService } from '../services/creator.service';
import { ComponentCanDeactivate } from '../stop-nav.guard';

@Component({
  selector: 'app-creatorprofile',
  templateUrl: './creatorprofile.component.html',
  styleUrls: ['./creatorprofile.component.css']
})
export class CreatorprofileComponent implements OnInit, ComponentCanDeactivate, AfterViewInit {
  public activationRedirectUrl: string | null = null;
  public email: string = '';
  private model$: Observable<ICreator> | undefined;
  private formDirty = false;
  public model: ICreator = new Creator("", "", "");
  @ViewChild('creatorForm', { read: NgForm }) payPalForm: any;

  constructor(private authService: AuthorizationService,
    private creatorService: CreatorService) {
    this.activationRedirectUrl = sessionStorage.getItem('postActivationRedirect');

  }

  ngOnInit(): void {
    this.authService.getEmail().subscribe({
      next: (email: string | null) => {
        if (email) {
          this.email = email.toString();
        }
      }
    });
    this.model$ = this.creatorService.getCreator();
    this.model$.subscribe({
      next: (creator: ICreator) => {
        this.model = creator;
      }
    })

  }

  ngAfterViewInit(): void {
    this.payPalForm.valueChanges
      .subscribe((value: string) => {
        this.formDirty = this.payPalForm.dirty;
      });
  }

  finalize() {
    if (this.activationRedirectUrl) {
      this.creatorService.finalizeViaRedirect(this.activationRedirectUrl, this.email, this.model.username);
    }
  }
  onSubmit(): void {
    this.model$ = this.creatorService.updateCreator(this.model);
    this.model$.subscribe({
      next: (creator: ICreator) => {
        this.model = creator;
      }
    })
    this.formDirty = false;
  }

  unlink(): void {
    this.creatorService.unlinkCreator(this.model).subscribe({
      next: (success: boolean) => {
        this.authService.redirectToLogout();
      }
    });
  }



  @HostListener('window:beforeunload')
  canDeactivate(): Observable<boolean> | boolean {
    // insert logic to check if there are pending changes here;
    // returning true will navigate without confirmation
    // returning false will show a confirm dialog before navigating away
    return !((this.model.payPalId == null || this.model.payPalId == "") && !this.formDirty);
  }

}

class Creator implements ICreator {
  constructor(
    public username: string,
    public payPalId: string,
    public id: string
  ) { }
}