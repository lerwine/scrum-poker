import { Component, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Location } from '@angular/common';
import { DeckType } from '../deck-type';
import { DeckTypeService } from '../deck-type.service';

@Component({
  selector: 'app-deck-type-details',
  templateUrl: './deck-type-details.component.html',
  styleUrls: ['./deck-type-details.component.css']
})
export class DeckTypeDetailsComponent {

  @Input() deckType?: DeckType;

  constructor(private route: ActivatedRoute, private deckTypeService: DeckTypeService, private location: Location) {

  }

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.deckType = this.deckTypeService.getDeckType(id);
  }

  goBack(): void {
    this.location.back();
  }
}
