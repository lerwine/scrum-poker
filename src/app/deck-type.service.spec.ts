import { TestBed } from '@angular/core/testing';

import { DeckTypeService } from './deck-type.service';

describe('DeckTypeService', () => {
  let service: DeckTypeService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DeckTypeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
