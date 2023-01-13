import { Component, Input, OnInit } from '@angular/core';
// import { ActivatedRoute } from '@angular/router';
// import { DeckTypeService } from '../deck-type.service';
// import { SheetDefinition } from '../sheet-definition';
import { OrderedSheetDefinition } from '../ordered-sheet-definition';

@Component({
  selector: 'app-printable-sheet-list',
  templateUrl: './printable-sheet-list.component.html',
  styleUrls: ['./printable-sheet-list.component.css']
})
export class PrintableSheetListComponent {

  @Input() printableSheets: OrderedSheetDefinition[] = [];

}
