import { Component, Input } from '@angular/core';
import { DetectionResult } from '../models/detectionresult.model';

@Component({
  selector: 'app-result-detail',
  templateUrl: './result-detail.component.html'
})
export class ResultDetailComponent {

  @Input() public result: DetectionResult;

  constructor() {
  }

  sortDetectedPlatesBy(prop: string) {
    return this.result.detectedPlates.sort((a, b) => b[prop] > a[prop] ? 1 : a[prop] === b[prop] ? 0 : -1);
  } 
}
