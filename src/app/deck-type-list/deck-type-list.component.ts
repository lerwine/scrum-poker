import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { DeckType } from '../deck-type';

@Component({
  selector: 'app-deck-type-list',
  templateUrl: './deck-type-list.component.html',
  styleUrls: ['./deck-type-list.component.css']
})
export class DeckTypeListComponent implements OnInit {

  @Input() deckTypes: DeckType[] = [];

  @Output() selectedDeckType = new EventEmitter<DeckType>();

  constructor() {

  }

  ngOnInit(): void {

  }

  selectDeckType(deckType: DeckType) {
    this.selectedDeckType.emit(deckType);
  }
}
