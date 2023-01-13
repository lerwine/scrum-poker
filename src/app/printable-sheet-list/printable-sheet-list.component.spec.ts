import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PrintableSheetListComponent } from './printable-sheet-list.component';

describe('PrintableSheetListComponent', () => {
  let component: PrintableSheetListComponent;
  let fixture: ComponentFixture<PrintableSheetListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PrintableSheetListComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PrintableSheetListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
