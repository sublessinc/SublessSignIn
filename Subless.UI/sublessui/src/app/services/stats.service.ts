import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ICreator } from '../models/ICreator';
import { IStat } from '../models/IStat';

@Injectable({
  providedIn: 'root'
})
export class StatsService {

  constructor(private httpClient: HttpClient) { }

  downloadCreatorFile(): Observable<{ [key: string]: string }> {
    return this.httpClient.get<{ [key: string]: string }>('/api/Creator/statscsv');
  }

  downloadPartnerFile(): any {
    return this.httpClient.get('/api/Partner/statscsv', { responseType: 'blob' });
  }
}
