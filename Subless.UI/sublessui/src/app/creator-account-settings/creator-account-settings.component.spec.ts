import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatorAccountSettingsComponent } from './creator-account-settings.component';

describe('CreatorAccountSettingsComponent', () => {
  let component: CreatorAccountSettingsComponent;
  let fixture: ComponentFixture<CreatorAccountSettingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreatorAccountSettingsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreatorAccountSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
