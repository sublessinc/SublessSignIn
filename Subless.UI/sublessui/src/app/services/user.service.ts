import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { IAnalytics } from '../models/IAnalytics';
import { IHitView } from '../models/IHitView';
import { IPerCreatorHitCount } from '../models/IPerCreatorHitCount';
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

  getRecentFeed(): Observable<IHitView[]> {
    return this.httpClient.get<IHitView[]>("/api/User/RecentFeed");
  }

  getTopFeed(): Observable<IPerCreatorHitCount[]> {
    return this.httpClient.get<IPerCreatorHitCount[]>("/api/User/TopFeed");
  }

}
