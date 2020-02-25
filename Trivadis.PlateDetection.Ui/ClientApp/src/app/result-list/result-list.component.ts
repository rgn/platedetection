import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DetectionResult } from '../models/detectionresult.model';

@Component({
  selector: 'app-result-list',
  templateUrl: './result-list.component.html'
})
export class ResultListComponent {
  // TODO: define model
  public results: DetectionResult[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<DetectionResult[]>(baseUrl + 'api/results').subscribe(result => {
      this.results = result;
    }, error => console.error(error));
  }
}
