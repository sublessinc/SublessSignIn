import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { IAnalytics } from '../models/IAnalytics';
import { UserService } from '../services/user.service';

@Component({
  selector: 'app-userprofile',
  templateUrl: './userprofile.component.html',
  styleUrls: ['./userprofile.component.scss']
})
export class UserprofileComponent implements OnInit {
  public analytics: IAnalytics = {
    thisMonth: { views: 0, creators: 0, partners: 0 },
    lastMonth: { views: 0, creators: 0, partners: 0 }
  };
  constructor(
    private userService: UserService,
    private changeDetector: ChangeDetectorRef
  ) { }

  ngOnInit(): void {
    this.getAnalytics();
  }

  getAnalytics() {
    this.userService.getAnalytics().subscribe({
      next: (analytics: IAnalytics) => {
        this.analytics = analytics;
        this.changeDetector.detectChanges();
      }
    });
  }

}
