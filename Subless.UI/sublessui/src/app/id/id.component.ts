import { Component, OnInit } from '@angular/core';
import { AdminService } from '../services/admin.service';

@Component({
  selector: 'app-id',
  templateUrl: './id.component.html',
  styleUrls: ['./id.component.css']
})
export class IdComponent implements OnInit {
  token: string = "";
  id: string = "";
  constructor(private adminService: AdminService) {
    
  }

  ngOnInit(): void {
    this.token = this.adminService.getToken();
    this.adminService.getId().subscribe({
      next: (id: string) => {
        this.id = id;
      }});
  }
}
