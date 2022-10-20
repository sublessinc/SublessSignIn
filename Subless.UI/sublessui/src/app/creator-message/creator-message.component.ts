import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn } from '@angular/forms';
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
  messageControl = new FormControl('');
  form = new FormGroup({
    messageControl: this.messageControl
  });
  options: Object = {
    charCounterMax: 70,
    toolbarButtons: ['undo', 'redo', 'bold', 'insertLink']
  }

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
    if (this.checkMessage()) {
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
  re = new RegExp("href=[\"']([^'\"]*)");
  public validLinks = ["https://www.patreon.com",
    "https://www.paypal.com",
    "https://www.subscribestar.com",
    "https://ko-fi.com",
    "https://twitter.com",
    "https://www.hentai-foundry.com",
    "https://linktr.ee"];

  public bannedCharacters = [';', '[', ']', '%', "(", ")", "\\"];

  checkMessage(): boolean {
    if (this.bannedCharacters.some(character => this.message.includes(character))) {
      this.form.controls["messageControl"].setErrors({ 'BannedCharacter': true });
      return false;
    }
    if (!this.message.includes("</a>")) {
      return true;
    }

    const links = this.message.match(this.re)?.filter(link => !link.startsWith("href="));
    if (links) {
      for (const link of links) {
        const linkValid = this.validLinks.some(item => link.startsWith(item));
        if (!linkValid) {
          this.form.controls["messageControl"].setErrors({ 'NoWhitelist': true });
        }
      }
    }
    return true;
  }
}
