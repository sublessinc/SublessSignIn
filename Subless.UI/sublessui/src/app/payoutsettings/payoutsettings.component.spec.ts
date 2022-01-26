import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PayoutsettingsComponent } from './payoutsettings.component';

describe('PayoutsettingsComponent', () => {
  let component: PayoutsettingsComponent;
  let fixture: ComponentFixture<PayoutsettingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PayoutsettingsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PayoutsettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
