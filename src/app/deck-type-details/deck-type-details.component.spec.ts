import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeckTypeDetailsComponent } from './deck-type-details.component';

describe('DeckTypeDetailsComponent', () => {
  let component: DeckTypeDetailsComponent;
  let fixture: ComponentFixture<DeckTypeDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DeckTypeDetailsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DeckTypeDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
