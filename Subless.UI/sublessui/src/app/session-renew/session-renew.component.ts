import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-session-renew',
  templateUrl: './session-renew.component.html',
  styleUrls: ['./session-renew.component.css']
})
export class SessionRenewComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
    window.close();
  }

}
