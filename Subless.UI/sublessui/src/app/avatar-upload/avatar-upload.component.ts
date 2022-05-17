import { Component, OnInit } from '@angular/core';
import { CreatorService } from '../services/creator.service';

@Component({
  selector: 'app-avatar-upload',
  templateUrl: './avatar-upload.component.html',
  styleUrls: ['./avatar-upload.component.scss']
})
export class AvatarUploadComponent implements OnInit {
  fileToUpload: File | null = null;

  constructor(private creatorService: CreatorService) { }

  ngOnInit(): void {
  }

  handleFileInput(files: FileList) {
    this.fileUploadService.postFile(files.item(0)).subscribe(data => {
      // do something, if upload success
    }, error => {
      console.log(error);
    });
  }

  uploadFileToActivity() {
    this.fileUploadService.postFile(this.fileToUpload).subscribe(data => {
      // do something, if upload success
    }, error => {
      console.log(error);
    });
  }
}
