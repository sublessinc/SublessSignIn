import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SessionRenewComponent } from './session-renew.component';

describe('SessionRenewComponent', () => {
  let component: SessionRenewComponent;
  let fixture: ComponentFixture<SessionRenewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SessionRenewComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SessionRenewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
