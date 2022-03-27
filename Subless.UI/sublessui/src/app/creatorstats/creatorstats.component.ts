import { Component, OnDestroy, OnInit } from '@angular/core';
import { IStat } from '../models/IStat';
import { StatsService } from '../services/stats.service';
import * as fileSaver from 'file-saver';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-creatorstats',
  templateUrl: './creatorstats.component.html',
  styleUrls: ['./creatorstats.component.css']
})
export class CreatorstatsComponent implements OnInit, OnDestroy {
  stats: IStat[] = []
  dates: Date[] = [];
  private subs: Subscription[] = [];

  constructor(private statsService: StatsService) { }
  ngOnInit(): void {
  }
  ngOnDestroy(): void {
    this.subs.forEach((item: Subscription) => { item.unsubscribe(); })
  }
  selectedDateChanged(e: any) {

  }
  download() {
    this.subs.push(this.statsService.downloadFile().subscribe(
      (response: any) => {
        let blob: any = new Blob([response], { type: 'text/json; charset=utf-8' });
        const url = window.URL.createObjectURL(blob);
        fileSaver.saveAs(blob, 'stats.csv');
      })),
      (error: any) => console.log('Error downloading the file'),
      () => console.info('File downloaded successfully');
  }
}
