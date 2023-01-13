import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeckTypeListComponent } from './deck-type-list.component';

describe('DeckTypeListComponent', () => {
  let component: DeckTypeListComponent;
  let fixture: ComponentFixture<DeckTypeListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DeckTypeListComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DeckTypeListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
