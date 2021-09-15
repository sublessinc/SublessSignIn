import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IPartner } from '../models/IPartner';

@Injectable({
  providedIn: 'root'
})
export class PartnerService {

  constructor(private httpClient: HttpClient) { }

  getPartner(): Observable<IPartner> {
    return this.httpClient.get<IPartner>('/api/Partner/config');
  }

  updatePayPal(payPal: string): Observable<IPartner> {
    return this.httpClient.put<IPartner>('/api/Partner/payPalId?payPalId=' + payPal, null);
  }
}
