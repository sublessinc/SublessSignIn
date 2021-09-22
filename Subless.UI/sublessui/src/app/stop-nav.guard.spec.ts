import { TestBed } from '@angular/core/testing';

import { StopNavGuard } from './stop-nav.guard';

describe('StopNavGuard', () => {
  let guard: StopNavGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(StopNavGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
