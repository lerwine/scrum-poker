import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { DeckType } from '../deck-type';
import { DeckTypeService } from '../deck-type.service';

@Component({
  selector: 'app-deck-type-list',
  templateUrl: './deck-type-list.component.html',
  styleUrls: ['./deck-type-list.component.css']
})
export class DeckTypeListComponent implements OnInit {

  @Input() deckTypes: DeckType[] = [];

  @Output() selectedDeckType = new EventEmitter<DeckType>();

  constructor(private deckTypeService: DeckTypeService) {

  }

  ngOnInit(): void {
    this.deckTypes = this.deckTypeService.getDeckTypes();
  }

  selectDeckType(deckType: DeckType) {
    this.selectedDeckType.emit(deckType);
  }
}
