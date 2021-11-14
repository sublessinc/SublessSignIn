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
}
