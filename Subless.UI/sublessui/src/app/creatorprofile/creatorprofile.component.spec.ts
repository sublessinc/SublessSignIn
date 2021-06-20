import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatorprofileComponent } from './creatorprofile.component';

describe('CreatorprofileComponent', () => {
  let component: CreatorprofileComponent;
  let fixture: ComponentFixture<CreatorprofileComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreatorprofileComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreatorprofileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
