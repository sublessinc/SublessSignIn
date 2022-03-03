import { Component, Inject, OnInit } from '@angular/core';
import { inject } from '@angular/core/testing';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-warn-dialog',
  templateUrl: './warn-dialog.component.html',
  styleUrls: ['./warn-dialog.component.css']
})
export class WarnDialogComponent implements OnInit {
  public data: IDialogData = {
    title: '',
    text: '',
    proceedText: '',
    cancelText: '',
  }

  constructor(@Inject(MAT_DIALOG_DATA) public injectedData: IDialogData) {
    if (injectedData) {
      this.data = injectedData;
    }
  }

  ngOnInit(): void {
  }

}

export interface IDialogData {
  title: string;
  text: string;
  proceedText: string;
  cancelText: string;
}