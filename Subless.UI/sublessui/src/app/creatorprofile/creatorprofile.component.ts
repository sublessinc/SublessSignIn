import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { ICreator } from '../models/ICreator';
import { CreatorService } from '../services/creator.service';

@Component({
  selector: 'app-creatorprofile',
  templateUrl: './creatorprofile.component.html',
  styleUrls: ['./creatorprofile.component.css']
})
export class CreatorprofileComponent implements OnInit {
  private model$: Observable<ICreator> | undefined;
  model: ICreator = new Creator("", "");
  submitted = false;

  constructor(private creatorService: CreatorService) { }

  ngOnInit(): void {
    this.model$ = this.creatorService.getCreator();
    this.model$.subscribe({
      next: (creator: ICreator) => {
        this.model = creator;
      }
    })
  }
  onSubmit(): void {
    this.submitted = true;
    this.model$ = this.creatorService.updateCreator(this.model);
    this.model$.subscribe({
      next: (creator: ICreator) => {
        this.model = creator;
      }
    })
  }
}

export class Creator implements ICreator {
  constructor(
    public username: string,
    public payoneerId: string,
  ) { }
}