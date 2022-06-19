import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { IAnalytics } from '../models/IAnalytics';
import { DateFormatter } from './dateformatter.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private httpClient: HttpClient,
    private dateFormatterService: DateFormatter) { }
  deleteUser(): Observable<boolean> {
    return this.httpClient.delete<boolean>("/api/User/");
  }

  getAnalytics(): Observable<IAnalytics> {
    return this.httpClient.get<IAnalytics>("/api/User/analytics").pipe(map(this.dateFormatterService.ParseUserAnalytics));
  }
}
