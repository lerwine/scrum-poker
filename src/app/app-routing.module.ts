import { NgModule } from '@angular/core';
// import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { DeckTypeListComponent } from './deck-type-list/deck-type-list.component';
import { DeckTypeDetailsComponent } from './deck-type-details/deck-type-details.component';

const routes: Routes = [
  { path: '', redirectTo: '/deckTypes', pathMatch: 'full' },
  { path: 'deckTypes', component: DeckTypeListComponent },
  { path: 'deckType/:id', component: DeckTypeDetailsComponent }
];

@NgModule({
  declarations: [],
  imports: [
    RouterModule.forRoot(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class AppRoutingModule { }
