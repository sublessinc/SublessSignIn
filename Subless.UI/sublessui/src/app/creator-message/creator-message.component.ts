import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { ICreatorMessage } from '../models/ICreatorMessage';
import { CreatorService } from '../services/creator.service';

@Component({
  selector: 'app-creator-message',
  templateUrl: './creator-message.component.html',
  styleUrls: ['./creator-message.component.css']
})
export class CreatorMessageComponent implements OnInit {

  public message: string = "";
  private subs: Subscription[] = [];

  constructor(private creatorService: CreatorService, private changeDetector: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.subs.push(this.creatorService.getCreatorMessage().subscribe({
      next: (message: ICreatorMessage) => {
        if (message) {
          this.message = message.message;
        }
        else {
          this.message = "";
        }
        this.changeDetector.detectChanges();
      }
    }));
  }
  onMessageSubmit(): void {
    this.subs.push(this.creatorService.setCreatorMessage(this.message).subscribe({
      next: (message: ICreatorMessage) => {
        if (message) {
          this.message = message.message;
        }
        else {
          this.message = "";
        }
        this.changeDetector.detectChanges();
      }
    }));
  }
}
