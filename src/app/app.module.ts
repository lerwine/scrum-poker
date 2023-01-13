import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { DeckTypeListComponent } from './deck-type-list/deck-type-list.component';
import { DeckTypeDetailsComponent } from './deck-type-details/deck-type-details.component';
import { AppRoutingModule } from './app-routing.module';
import { PrintableSheetListComponent } from './printable-sheet-list/printable-sheet-list.component';

@NgModule({
  declarations: [
    AppComponent,
    DeckTypeListComponent,
    DeckTypeDetailsComponent,
    PrintableSheetListComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
