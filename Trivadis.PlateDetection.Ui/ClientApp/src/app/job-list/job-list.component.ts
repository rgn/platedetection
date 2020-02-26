import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Job } from '../models/job.model';
import { JobState } from '../models/jobstate.model';

@Component({
  selector: 'app-job-list',
  templateUrl: './job-list.component.html'
})
export class JobListComponent {
  // TODO: define model
  public jobs: Job[];
  public JobState = JobState;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<Job[]>(baseUrl + 'api/job').subscribe(result => {
      this.jobs = result;
    }, error => console.error(error));
  }
}
