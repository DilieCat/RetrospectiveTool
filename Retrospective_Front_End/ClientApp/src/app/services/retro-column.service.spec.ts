import { TestBed } from '@angular/core/testing';

import { RetrocolumnService } from './retro-column.service';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('RetroColumnService', () => {
  beforeEach(() => TestBed.configureTestingModule({
    imports: [DragDropModule, FormsModule, ReactiveFormsModule, HttpClientTestingModule]
  }));

  it('should be created', () => {
    const service: RetrocolumnService = TestBed.get(RetrocolumnService);
    expect(service).toBeTruthy();
  });
});
