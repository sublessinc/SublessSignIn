import { ErrorHandler, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { NGXLogger } from 'ngx-logger';

@Injectable({
  providedIn: 'root'
})
export class GlobalerrorhandlerService implements ErrorHandler {

  constructor(private router: Router, private logger: NGXLogger) {
  }

  handleError(error: any) {
    if (error["status"] && error["status"] == 401) {
      this.logger.warn("Unauthorized user or expired token..... logging out");
    }
    else if (error["status"] && error["status"] == 410) {
      this.router.navigate(['error', 'expired']);
    }
    else {
      this.logger.error("Error: " + error.message);
      this.logger.error("Stack: " + error.stack);
      this.router.navigate(['error']);
    }
  }

}