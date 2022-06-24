import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerstatsComponent } from './partnerstats.component';

describe('PartnerstatsComponent', () => {
  let component: PartnerstatsComponent;
  let fixture: ComponentFixture<PartnerstatsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PartnerstatsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerstatsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
