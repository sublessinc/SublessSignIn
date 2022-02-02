import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IntegrationtestComponent } from './integrationtest.component';

describe('IntegrationtestComponent', () => {
  let component: IntegrationtestComponent;
  let fixture: ComponentFixture<IntegrationtestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ IntegrationtestComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(IntegrationtestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
