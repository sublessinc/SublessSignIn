import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Observable } from 'rxjs';
import { Creator } from '../models/Creator';
import { ICreator } from '../models/ICreator';
import { ICreatorAnalytics } from '../models/ICreatorAnalytics';
import { ICreatorStats } from '../models/ICreatorStats';
import { AuthorizationService } from '../services/authorization.service';
import { CreatorService } from '../services/creator.service';

@Component({
  selector: 'app-creatorprofile',
  templateUrl: './creatorprofile.component.html',
  styleUrls: ['./creatorprofile.component.scss']
})
export class CreatorprofileComponent implements OnInit {

  public analytics: ICreatorAnalytics = {
    thisMonth: { views: 0, visitors: 0, piecesOfContent: 0 },
    lastMonth: { views: 0, visitors: 0, piecesOfContent: 0 }
  };
  constructor(
    private creatorService: CreatorService,
    private changeDetector: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.getAnalytics();
  }

  getAnalytics() {
    this.creatorService.getAnalytics().subscribe({
      next: (analytics: ICreatorAnalytics) => {
        this.analytics = analytics;
        this.changeDetector.detectChanges();
      }
    });
  }
}
