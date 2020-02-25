import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DetectedPlate } from '../models/detectedplate.model';

@Component({
  selector: 'app-plate-list',
  templateUrl: './plate-list.component.html'
})
export class PlateListComponent {
  // TODO: define model
  public plates: DetectedPlate[];

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<DetectedPlate[]>(baseUrl + 'api/plates').subscribe(result => {
      this.plates = result;
    }, error => console.error(error));
  }
}
