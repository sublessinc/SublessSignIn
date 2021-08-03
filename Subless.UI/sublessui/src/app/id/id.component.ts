import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AdminService } from '../services/admin.service';

@Component({
  selector: 'app-id',
  templateUrl: './id.component.html',
  styleUrls: ['./id.component.css']
})
export class IdComponent implements OnInit {
  token: string = "";
  id: string = "";
  enabled: boolean = false;
  constructor(
    private adminService: AdminService, 
    private route: ActivatedRoute,
    ) {
    
  }

  ngOnInit(): void {
    this.route.fragment.subscribe( {
      next: (fragment: string| null) => {
        if (fragment && fragment=='id' ) {
          this.enabled=true;
          this.getTokens();
        }
      }});
  }

  getTokens(): void {
    this.token = this.adminService.getToken();
    this.adminService.getId().subscribe({
      next: (id: string) => {
        this.id = id;
      }});
  }
}
