import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserTopContentComponent } from './user-top-content.component';

describe('UserTopContentComponent', () => {
  let component: UserTopContentComponent;
  let fixture: ComponentFixture<UserTopContentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UserTopContentComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserTopContentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
