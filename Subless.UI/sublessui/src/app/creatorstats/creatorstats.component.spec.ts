import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreatorstatsComponent } from './creatorstats.component';

describe('CreatorstatsComponent', () => {
  let component: CreatorstatsComponent;
  let fixture: ComponentFixture<CreatorstatsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CreatorstatsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CreatorstatsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
