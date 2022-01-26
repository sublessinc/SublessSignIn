import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IAnalytics } from '../models/IAnalytics';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(private httpClient: HttpClient) { }
  deleteUser(): Observable<boolean> {
    return this.httpClient.delete<boolean>("/api/User/");
  }
  getAnalytics(): Observable<IAnalytics> {
    return this.httpClient.get<IAnalytics>("/api/User/analytics");
  }
}
