import { ErrorHandler, Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class GlobalerrorhandlerService implements ErrorHandler {

  constructor(private router: Router) {
  }

  handleError(error: any) {
    if (error["status"] && error["status"] == 401) {
      console.warn("Unauthorized user or expired token..... logging out");
    }
    else {
      console.warn("Error: " + error.message);
      console.warn("Stack: " + error.stack);
      this.router.navigate(['error']);
    }
  }

}