import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { ICreator } from '../models/ICreator';
import { Creator } from '../payPal/payPal.component';
import { AuthorizationService } from '../services/authorization.service';
import { CreatorService } from '../services/creator.service';

@Component({
  selector: 'app-creatorprofile',
  templateUrl: './creatorprofile.component.html',
  styleUrls: ['./creatorprofile.component.css']
})
export class CreatorprofileComponent implements OnInit {
  public activationRedirectUrl: string | null = null;
  public email: any;
  private model$: Observable<ICreator> | undefined;
  public model: ICreator = new Creator("", "");

  constructor(private authService: AuthorizationService,
    private creatorService: CreatorService) {
    this.activationRedirectUrl = sessionStorage.getItem('postActivationRedirect');
  }


  ngOnInit(): void {
    this.email = this.authService.getEmail();
    this.model$ = this.creatorService.getCreator();
    this.model$.subscribe({
      next: (creator: ICreator) => {
        this.model = creator;
      }
    })
  }

  finalize() {
    if (this.activationRedirectUrl) {
      sessionStorage.removeItem('postActivationRedirect');
      let baseURI = new URL(this.activationRedirectUrl);
      baseURI.searchParams.append("sublessId", this.email);
      baseURI.searchParams.append("creatorId", this.model.username);
      window.location.replace(baseURI.href);
    }
  }
}