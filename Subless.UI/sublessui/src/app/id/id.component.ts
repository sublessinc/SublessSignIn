import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { AdminService } from '../services/admin.service';

@Component({
  selector: 'app-id',
  templateUrl: './id.component.html',
  styleUrls: ['./id.component.css']
})
export class IdComponent implements OnInit, OnDestroy {
  token: string = "";
  id: string = "";
  enabled: boolean = false;
  private subs: Subscription[] = [];

  constructor(
    private adminService: AdminService,
    private route: ActivatedRoute,
  ) {

  }

  ngOnInit(): void {
    this.subs.push(this.route.fragment.subscribe({
      next: (fragment: string | null) => {
        if (fragment && fragment == 'id') {
          this.enabled = true;
          this.getTokens();
        }
      }
    }));
  }
  ngOnDestroy(): void {
    this.subs.forEach((item: Subscription) => { item.unsubscribe(); })
  }
  getTokens(): void {
    this.subs.push(this.adminService.getId().subscribe({
      next: (user: any) => {
        this.id = user.id;
      }
    }));
  }
}
