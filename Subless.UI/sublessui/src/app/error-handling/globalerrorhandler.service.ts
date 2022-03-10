import { ErrorHandler, Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class GlobalerrorhandlerService implements ErrorHandler {

  constructor(private router: Router) {
  }

  handleError(error: Error) {
    this.router.navigate(['error']);
  }

}