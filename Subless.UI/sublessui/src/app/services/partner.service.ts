import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IPartner } from '../models/IPartner';
import { IPartnerAnalytics } from '../models/IPartnerAnalytics';
import { IPartnerWrite } from '../models/IPartnerWrite';
import { map } from 'rxjs/operators';
import { DateFormatter } from './dateformatter.service';

@Injectable({
  providedIn: 'root'
})
export class PartnerService {

  constructor(private httpClient: HttpClient,
    private dateFormatterService: DateFormatter) { }

  getPartner(): Observable<IPartner> {
    return this.httpClient.get<IPartner>('/api/Partner/config');
  }

  updatePartner(partnerWrite: IPartnerWrite): Observable<IPartner> {
    return this.httpClient.put<IPartner>('/api/Partner/' + partnerWrite.id, partnerWrite);
  }

  testWebhook(): Observable<Boolean> {
    return this.httpClient.post<Boolean>('/api/Partner/WebhookTest', null);
  }

  getAnalytics(): Observable<IPartnerAnalytics> {
    return this.httpClient.get<IPartnerAnalytics>("/api/Partner/Analytics").pipe(map(this.dateFormatterService.ParsePartnerAnalytics));
  }
}
