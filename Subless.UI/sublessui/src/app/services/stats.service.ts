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

  getStat(): Observable<IStat[]> {
    return this.httpClient.get<IStat[]>('/api/Creator/stats');
  }

  downloadFile(): any {
		return this.httpClient.get('/api/Creator/statscsv', {responseType: 'blob'});
  }
}
