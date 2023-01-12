import { Component, OnDestroy } from '@angular/core';
import { IStat } from '../models/IStat';
import { StatsService } from '../services/stats.service';
import * as fileSaver from 'file-saver';
import { Subscription } from 'rxjs';
import { NGXLogger } from 'ngx-logger';

@Component({
  selector: 'app-creatorstats',
  templateUrl: './creatorstats.component.html',
  styleUrls: ['./creatorstats.component.css']
})
export class CreatorstatsComponent implements OnDestroy {
  stats: IStat[] = []
  dates: Date[] = [];
  private subs: Subscription[] = [];

  constructor(private statsService: StatsService, private logger: NGXLogger) { }

  ngOnDestroy(): void {
    this.subs.forEach((item: Subscription) => { item.unsubscribe(); })
  }
  selectedDateChanged(e: any) {

  }
  download() {
    this.subs.push(this.statsService.downloadCreatorFile().subscribe(
      (response: { [key: string]: string }) => {
        for (let creator in response) {
          let blob: any = new Blob([response[creator]], { type: 'text/json; charset=utf-8' });
          const url = window.URL.createObjectURL(blob);
          fileSaver.saveAs(blob, creator + '-payout-history.csv');
        }
      })),
      (error: any) => this.logger.info('Error downloading the file'),
      () => this.logger.info('File downloaded successfully');
  }
}
