import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserRecentActivityComponent } from './user-recent-activity.component';

describe('UserRecentActivityComponent', () => {
  let component: UserRecentActivityComponent;
  let fixture: ComponentFixture<UserRecentActivityComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UserRecentActivityComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserRecentActivityComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
