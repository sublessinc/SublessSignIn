import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ICreator } from '../models/ICreator';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  constructor(private httpClient: HttpClient) { }

  getId(): Observable<any> {
    return this.httpClient.get<any>('/api/User');
  }
}
