import { Component, OnInit } from '@angular/core';
import { IStat } from '../models/IStat';
import { StatsService } from '../services/stats.service';
import * as fileSaver from 'file-saver';

@Component({
  selector: 'app-creatorstats',
  templateUrl: './creatorstats.component.html',
  styleUrls: ['./creatorstats.component.css']
})
export class CreatorstatsComponent implements OnInit {
  stats: IStat[] = []
  dates: Date[] = [];
  constructor(private statsService: StatsService ) { }
  ngOnInit(): void {
  }

  selectedDateChanged(e: any){

  }
  download() {
    //this.fileService.downloadFile().subscribe(response => {
		this.statsService.downloadFile().subscribe((response: any) => { //when you use stricter type checking
			let blob:any = new Blob([response], { type: 'text/json; charset=utf-8' });
			const url = window.URL.createObjectURL(blob);
			//window.open(url);
			//window.location.href = response.url;
			fileSaver.saveAs(blob, 'stats.csv');
		//}), error => console.log('Error downloading the file'),
		}), (error: any) => console.log('Error downloading the file'), //when you use stricter type checking
                 () => console.info('File downloaded successfully');
  }
}
