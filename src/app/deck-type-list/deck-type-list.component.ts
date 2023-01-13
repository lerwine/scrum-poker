import { Component, OnInit } from '@angular/core';
import { DeckType } from '../deck-type';
import { DeckTypeService } from '../deck-type.service';

@Component({
  selector: 'app-deck-type-list',
  templateUrl: './deck-type-list.component.html',
  styleUrls: ['./deck-type-list.component.css']
})
export class DeckTypeListComponent implements OnInit {

  deckTypes: DeckType[] = [];

  constructor(private deckTypeService: DeckTypeService) {

  }

  ngOnInit(): void {
    this.deckTypes = this.deckTypeService.getDeckTypes();
  }
}
