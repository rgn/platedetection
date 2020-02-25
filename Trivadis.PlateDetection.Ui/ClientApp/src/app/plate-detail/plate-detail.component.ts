import { Component, Input } from '@angular/core';
import { DetectedPlate } from '../models/detectedplate.model';

@Component({
  selector: 'app-plate-detail',
  templateUrl: './plate-detail.component.html'
})
export class PlateDetailComponent {

  @Input() public plate: DetectedPlate;

  constructor() {
  }
}
