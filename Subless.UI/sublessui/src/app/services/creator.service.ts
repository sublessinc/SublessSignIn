import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ICreator } from '../models/ICreator';

@Injectable({
  providedIn: 'root'
})
export class CreatorService {

  constructor(private httpClient: HttpClient) { }

  getCreator(): Observable<ICreator> {
    return this.httpClient.get<ICreator>('/api/Creator');
  }

  updateCreator(creator: ICreator): Observable<ICreator> {
    return this.httpClient.put<ICreator>('/api/Creator', creator);
  }

  unlinkCreator(creator: ICreator): Observable<boolean> {
    return this.httpClient.delete<boolean>('/api/Creator/' + creator.id + "/unlink");
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
