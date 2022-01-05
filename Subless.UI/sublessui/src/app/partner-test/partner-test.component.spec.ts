import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerTestComponent } from './partner-test.component';

describe('PartnerTestComponent', () => {
  let component: PartnerTestComponent;
  let fixture: ComponentFixture<PartnerTestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PartnerTestComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerTestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
