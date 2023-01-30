import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CancellationWarningComponent } from './cancellation-warning.component';

describe('CancellationWarningComponent', () => {
  let component: CancellationWarningComponent;
  let fixture: ComponentFixture<CancellationWarningComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CancellationWarningComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CancellationWarningComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
