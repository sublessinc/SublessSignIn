import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ICreator } from '../models/ICreator';
import { ICreatorAnalytics } from '../models/ICreatorAnalytics';
import { IHitCount } from '../models/IHitCount';
import { IHitView } from '../models/IHitView';
import { DateFormatter } from './dateformatter.service';

@Injectable({
  providedIn: 'root'
})
export class CreatorService {

  constructor(private httpClient: HttpClient,
    private dateFormatterService: DateFormatter) { }

  getCreator(): Observable<ICreator> {
    return this.httpClient.get<ICreator>('/api/Creator');
  }

  updateCreator(creator: ICreator): Observable<ICreator> {
    return this.httpClient.put<ICreator>('/api/Creator', creator);
  }

  unlinkCreator(creator: ICreator): Observable<boolean> {
    return this.httpClient.delete<boolean>('/api/Creator/' + creator.id + "/unlink");
  }

  getAnalytics(): Observable<ICreatorAnalytics> {
    return this.httpClient.get<ICreatorAnalytics>("/api/Creator/Analytics").pipe(map(this.dateFormatterService.ParseCreatorAnalytics));
  }

  getRecentFeed(): Observable<IHitView[]> {
    return this.httpClient.get<IHitView[]>("/api/Creator/RecentFeed");
  }

  getTopFeed(): Observable<IHitCount[]> {
    return this.httpClient.get<IHitCount[]>("/api/Creator/TopFeed");
  }

  finalizeViaRedirect(uri: string, email: string, username: string) {
    sessionStorage.removeItem('postActivationRedirect');
    let baseURI = new URL(uri);
    baseURI.searchParams.append("sublessId", email);
    baseURI.searchParams.append("creatorId", username);
    baseURI.searchParams.append("email", email);
    baseURI.searchParams.append("username", username);
    window.location.replace(baseURI.href);
  }
}
