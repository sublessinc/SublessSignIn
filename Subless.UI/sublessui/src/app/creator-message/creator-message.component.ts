import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn } from '@angular/forms';
import { Observable, of, Subscription } from 'rxjs';
import { ICreatorMessage } from '../models/ICreatorMessage';
import { CreatorService } from '../services/creator.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { HttpErrorResponse } from '@angular/common/http';


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
  config: Object = {
    removePlugins: "a11yhelp,about,basicstyles,bidi,blockquote,notification,clipboard,panelbutton,panel,floatpanel,colorbutton,colordialog,copyformatting,menu,contextmenu,dialogadvtab,div,editorplaceholder,elementspath,enterkey,entities,exportpdf,popup,filetools,filebrowser,find,floatingspace,listblock,richcombo,font,format,forms,horizontalrule,htmlwriter,iframe,image,indent,indentblock,indentlist,justify,menubutton,language,list,liststyle,magicline,maximize,newpage,pagebreak,xml,ajax,pastetools,pastefromgdocs,pastefromlibreoffice,pastefromword,pastetext,preview,print,removeformat,resize,save,scayt,selectall,showblocks,showborders,sourcearea,stylescombo,tab,table,tabletools,tableselection,templates,undo,lineutils,widgetselection,widget,notificationaggregator,uploadwidget,uploadimage",
    removeButtons: "Anchor",
    linkDefaultProtocol: "https://",
    linkShowTargetTab: false,
    linkShowAdvancedTab: false,
  };




  constructor(private creatorService: CreatorService,
    private _snackBar: MatSnackBar,
    private changeDetector: ChangeDetectorRef) { }

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

  public onChange(event: any) {
    this.message = event.replace("&nbsp;", " ");
    this.checkMessage();
  }

  onMessageSubmit(): void {
    if (this.checkMessage()) {
      this.subs.push(this.creatorService.setCreatorMessage(this.message, () => of(null)).subscribe({
        next: (message: ICreatorMessage) => {
          if (message) {
            this.message = message.message;
            this._snackBar.open("Saved", "Ok", {
              duration: 10000,
            });
          }
          else {
            this.message = "";
            this._snackBar.open("An issue was encountered with your message, please try again", "Ok", {
              duration: 2000,
            });
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

  public bannedCharacters = ['&', '[', ']', '%', "(", ")", "\\"];

  extractContent(htmlContent: string): string {
    var span = document.createElement('span');
    span.innerHTML = htmlContent;
    return span.textContent || span.innerText;
  };

  checkMessage(): boolean {
    const plaintext = this.extractContent(this.message);
    if (this.bannedCharacters.some(character => plaintext.includes(character))) {
      this.form.controls["messageControl"].setErrors({ 'BannedCharacter': true });
      return false;
    }
    if (plaintext.length > 70) {
      this.form.controls["messageControl"].setErrors({ 'TooLong': true });
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
          return false;
        }
      }
    }
    return true;
  }
}
