import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatorMessageComponent } from './creator-message.component';

describe('CreatorMessageComponent', () => {
  let component: CreatorMessageComponent;
  let fixture: ComponentFixture<CreatorMessageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreatorMessageComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreatorMessageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
