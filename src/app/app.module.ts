import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { DeckTypeListComponent } from './deck-type-list/deck-type-list.component';

@NgModule({
  declarations: [
    AppComponent,
    DeckTypeListComponent
  ],
  imports: [
    BrowserModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
