import { Component, Inject, OnInit } from '@angular/core';
import { inject } from '@angular/core/testing';
import { MAT_LEGACY_DIALOG_DATA as MAT_DIALOG_DATA } from '@angular/material/legacy-dialog';

@Component({
  selector: 'app-warn-dialog',
  templateUrl: './warn-dialog.component.html',
  styleUrls: ['./warn-dialog.component.css']
})
export class WarnDialogComponent {
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
}

export interface IDialogData {
  title: string;
  text: string;
  proceedText: string;
  cancelText: string;
}