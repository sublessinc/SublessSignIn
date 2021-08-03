import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PayoneerComponent } from './payoneer.component';

describe('PayoneerComponent', () => {
  let component: PayoneerComponent;
  let fixture: ComponentFixture<PayoneerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PayoneerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PayoneerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
