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

  downloadCreatorFile(): any {
    return this.httpClient.get('/api/Creator/statscsv', { responseType: 'blob' });
  }

  downloadPartnerFile(): any {
    return this.httpClient.get('/api/Partner/statscsv', { responseType: 'blob' });
  }
}
