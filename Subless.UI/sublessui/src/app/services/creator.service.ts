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
    const authHeader = "Bearer " + sessionStorage.getItem('id_token');
    var headers = new HttpHeaders().set('Authorization', authHeader);
    return this.httpClient.get<ICreator>('/api/Creator', { headers });
  }

  updateCreator(creator: ICreator): Observable<ICreator> {
    const authHeader = "Bearer " + sessionStorage.getItem('id_token');
    var headers = new HttpHeaders().set('Authorization', authHeader);
    return this.httpClient.put<ICreator>('/api/Creator', creator, { headers });
  }
}
