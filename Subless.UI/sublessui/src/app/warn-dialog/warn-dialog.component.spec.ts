import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WarnDialogComponent } from './warn-dialog.component';

describe('WarnDialogComponent', () => {
  let component: WarnDialogComponent;
  let fixture: ComponentFixture<WarnDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ WarnDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(WarnDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
