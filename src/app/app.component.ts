import { Component } from '@angular/core';
import { DeckType, getDeckTypes } from './deck-type';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent {
  title = 'scrum-poker';
  deckTypeList: DeckType[] = getDeckTypes();
  selectedDeckType: DeckType | undefined;
  updateSelectedDeckType(deck: DeckType) {
    this.selectedDeckType = deck;
  }
}
