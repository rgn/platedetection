import { Component, Inject, OnInit, ViewChild, ViewChildren, AfterViewInit, ElementRef, TemplateRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { Job } from '../models/job.model';
import { DetectedPoint } from '../models/detectedpoint.model';

interface Rectangle {
  x: number;
  y: number;
  width: number;
  height: number;
}

@Component({
  selector: 'app-job-detail',
  templateUrl: './job-detail.component.html'
})
export class JobDetailComponent implements OnInit {

  private jobId: string;
  public job: Job;
 
  private context: CanvasRenderingContext2D;
  private plateImageElementRef: ElementRef;
  private plateImage: HTMLImageElement;
  private canvasElementRef: ElementRef;

  @ViewChild('plateimage', { static: false }) set plateimage(content: ElementRef) {
    if (content === undefined || content === null || content.nativeElement === undefined || content.nativeElement === null) return;

    this.plateImageElementRef = content;
    this.plateImage = this.plateImageElementRef.nativeElement as HTMLImageElement;
    var self = this;
    this.plateImage.onload = () => {
      self.context.drawImage(self.plateImage, 0, 0);
      self.context.lineWidth = 3;
      self.context.strokeStyle = '#38f';

      if (self.job !== undefined && self.job !== null) {
        self.job.detectionResults.forEach(detectionResult => {
          if (detectionResult.detectedPoints === undefined || detectionResult.detectedPoints === null) return;                    
          var first = true;
          var convexHull = this.makeHull(detectionResult.detectedPoints);
          var endPoint = convexHull[convexHull.length - 1];          
          self.context.beginPath();
          convexHull.forEach(detectedPoint => {
            if (first) {
              self.context.moveTo(detectedPoint.x, detectedPoint.y);
              first = false;
            } else {
              self.context.lineTo(detectedPoint.x, detectedPoint.y);
            }
          });
          self.context.closePath();
          self.context.stroke();
          self.context.fillRect(endPoint.x, endPoint.y, 100, 100);
        });
      }
    }
  }

  @ViewChild('canvas', { static: false }) set canvas(content: ElementRef) {
    if (content === undefined || content === null || content.nativeElement === undefined || content.nativeElement === null) return;
    this.canvasElementRef = content;
    this.context = this.canvasElementRef.nativeElement.getContext('2d');
  }

  constructor(
    private readonly http: HttpClient,
    private readonly route: ActivatedRoute,
    @Inject('BASE_URL') private readonly baseUrl: string) {
  }

  ngOnInit() {
    var self = this;

    this.route.paramMap.subscribe(params => {
      this.jobId = params.get("id");

      this.http.get<Job>(this.baseUrl + `api/job/${this.jobId}`).subscribe(result => {
        this.job = result;
        console.log('job', result);
      }, error => console.error(error));
    });    
  }  

  makeHullPresorted(points: DetectedPoint[]): Array<DetectedPoint> {
    if (points.length <= 1)
      return points.slice();

    // Andrew's monotone chain algorithm. Positive y coordinates correspond to "up"
    // as per the mathematical convention, instead of "down" as per the computer
    // graphics convention. This doesn't affect the correctness of the result.

    var upperHull = [];
    for (var i = 0; i < points.length; i++) {
      var p = points[i];
      while (upperHull.length >= 2) {
        var q = upperHull[upperHull.length - 1];
        var r = upperHull[upperHull.length - 2];
        if ((q.x - r.x) * (p.y - r.y) >= (q.y - r.y) * (p.x - r.x))
          upperHull.pop();
        else
          break;
      }
      upperHull.push(p);
    }
    upperHull.pop();

    var lowerHull = [];
    for (var i = points.length - 1; i >= 0; i--) {
      var p = points[i];
      while (lowerHull.length >= 2) {
        var q = lowerHull[lowerHull.length - 1];
        var r = lowerHull[lowerHull.length - 2];
        if ((q.x - r.x) * (p.y - r.y) >= (q.y - r.y) * (p.x - r.x))
          lowerHull.pop();
        else
          break;
      }
      lowerHull.push(p);
    }
    lowerHull.pop();

    if (upperHull.length == 1 && lowerHull.length == 1 && upperHull[0].x == lowerHull[0].x && upperHull[0].y == lowerHull[0].y)
      return upperHull;
    else
      return upperHull.concat(lowerHull);
  }

  makeHull(points: DetectedPoint[]) {
    var newPoints = points.slice();
    newPoints.sort(function (a, b) {
      if (a.x < b.x)
        return -1;
      else if (a.x > b.x)
        return +1;
      else if (a.y < b.y)
        return -1;
      else if (a.y > b.y)
        return +1;
      else
        return 0;
    });
    return this.makeHullPresorted(newPoints);
  };
}

