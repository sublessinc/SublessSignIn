import { Component, OnDestroy, OnInit } from '@angular/core';
import * as fileSaver from 'file-saver';
import { NGXLogger } from 'ngx-logger';
import { Subscription } from 'rxjs';
import { StatsService } from '../services/stats.service';

@Component({
  selector: 'app-partnerstats',
  templateUrl: './partnerstats.component.html',
  styleUrls: ['./partnerstats.component.css']
})
export class PartnerstatsComponent implements OnDestroy {
  private subs: Subscription[] = [];

  constructor(private statsService: StatsService, private logger: NGXLogger) { }

  ngOnDestroy(): void {
    this.subs.forEach((item: Subscription) => { item.unsubscribe(); })
  }

  download() {
    this.subs.push(this.statsService.downloadPartnerFile().subscribe(
      (response: any) => {
        let blob: any = new Blob([response], { type: 'text/json; charset=utf-8' });
        const url = window.URL.createObjectURL(blob);
        fileSaver.saveAs(blob, 'payout-history.csv');
      })),
      (error: any) => this.logger.warn('Error downloading the file'),
      () => this.logger.info('File downloaded successfully');
  }
}
