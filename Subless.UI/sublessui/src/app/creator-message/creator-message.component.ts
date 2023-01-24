import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { AbstractControl, FormControl, FormGroup, ValidationErrors, ValidatorFn } from '@angular/forms';
import { Observable, of, Subscription } from 'rxjs';
import { ICreatorMessage } from '../models/ICreatorMessage';
import { CreatorService } from '../services/creator.service';
import { MatLegacySnackBar as MatSnackBar } from '@angular/material/legacy-snack-bar';
import { HttpErrorResponse } from '@angular/common/http';


@Component({
  selector: 'app-creator-message',
  templateUrl: './creator-message.component.html',
  styleUrls: ['./creator-message.component.css']
})
export class CreatorMessageComponent implements OnInit {

  public message: string = "";
  private subs: Subscription[] = [];
  public validLinks: string[] = [];
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
    this.subs.push(this.creatorService.getMessageUriWhitelist().subscribe({
      next: (whitelist: string[]) => {
        this.validLinks = whitelist;
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
    if (plaintext.length > 280) {
      this.form.controls["messageControl"].setErrors({ 'TooLong': true });
      return false;
    }
    if (!this.message.includes("</a>")) {
      return true;
    }
    let linkGroup;
    let trimmedMessage = this.message;
    while ((linkGroup = this.re.exec(trimmedMessage)) !== null) {
      for (const link of linkGroup) {
        if (!link.startsWith("href=") && !this.checkLinkDomain(link)) {
          this.form.controls["messageControl"].setErrors({ 'NoWhitelist': true });
          return false;
        }
        trimmedMessage = trimmedMessage.replace(link, "");
      }
    }
    return true;
  }

  checkLinkDomain(uri: string): boolean {
    const linkValid = this.validLinks.some(item => uri.startsWith(item));
    if (linkValid) { return true; }
    for (const link in this.validLinks) {
      let regexedLink = this.validLinks[link].replace("*", "[A-Za-z0-9\\.-]*").replace("https://", "");
      const cleanedUri = uri.replace("https://", "");
      regexedLink += ".*";
      const matches = cleanedUri.match(regexedLink);
      if (matches?.some(x => x == cleanedUri)) {
        return true;
      }
    }
    return false;
  }
}
