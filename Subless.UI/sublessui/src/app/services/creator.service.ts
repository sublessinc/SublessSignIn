import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { CreatorMessage } from '../models/CreatorMessage';
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

  getCreators(): Observable<ICreator[]> {
    return this.httpClient.get<ICreator[]>('/api/Creator');
  }

  updateCreator(creator: ICreator): Observable<ICreator[]> {
    return this.httpClient.put<ICreator[]>('/api/Creator', creator);
  }

  unlinkCreator(id: string): Observable<boolean> {
    return this.httpClient.delete<boolean>('/api/Creator/' + id + "/unlink");
  }

  getAnalytics(): Observable<ICreatorAnalytics[]> {
    return this.httpClient.get<ICreatorAnalytics[]>("/api/Creator/Analytics").pipe(map(x => x.map(this.dateFormatterService.ParseCreatorAnalytics)));
  }

  getRecentFeed(): Observable<IHitView[]> {
    return this.httpClient.get<IHitView[]>("/api/Creator/RecentFeed");
  }

  getTopFeed(): Observable<IHitCount[]> {
    return this.httpClient.get<IHitCount[]>("/api/Creator/TopFeed");
  }

  getCreatorMessage(): Observable<CreatorMessage> {
    return this.httpClient.get<CreatorMessage>("/api/Creator/message");
  }

  getMessageUriWhitelist(): Observable<string[]> {
    return this.httpClient.get<string[]>("/api/Creator/message/whitelist");
  }


  setCreatorMessage(message: string, handleError: any): Observable<CreatorMessage> {
    const headers = new HttpHeaders({
      "Content-Type": "text/plain"
    });
    return this.httpClient.post<CreatorMessage>("/api/Creator/message", { message: message }).pipe(
      catchError(handleError));
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
