import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-session-renew',
  templateUrl: './session-renew.component.html',
  styleUrls: ['./session-renew.component.css']
})
export class SessionRenewComponent implements OnInit {

  constructor(private route: ActivatedRoute) { }

  ngOnInit(): void {
    const path = this.route.snapshot.queryParams['return_uri']
    window.location.href = path;
  }

}
