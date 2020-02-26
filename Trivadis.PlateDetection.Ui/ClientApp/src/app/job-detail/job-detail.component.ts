import { Component, Inject, OnInit, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { Job } from '../models/job.model';
import { JobState } from '../models/jobstate.model';
import { DetectedPoint } from '../models/detectedpoint.model';
import { DetectionResult } from '../models/detectionresult.model';

@Component({
  selector: 'app-job-detail',
  templateUrl: './job-detail.component.html'
})
export class JobDetailComponent implements OnInit {

  private jobId: string;
  public job: Job;
  public JobState = JobState;
 
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
      
      var lineWidth: number = 2;
      var fontSize: number = 8;
      var margin: number = 2;         

      self.context.lineWidth = lineWidth;

      if (self.job !== undefined && self.job !== null) {
        self.job.detectionResults.forEach(detectionResult => {
          if (detectionResult.detectedPoints === undefined || detectionResult.detectedPoints === null) return;                    
          var first = true;
          var convexHull = this.makeHull(detectionResult.detectedPoints);
          var startPoint = convexHull[0];
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
          self.context.fillStyle = '#38f';
          self.context.strokeStyle = '#38f';
          self.context.stroke();

          if (detectionResult.detectedPlates !== null && detectionResult.detectedPlates.length > 0) {
            var mostConfidentPlate = this.sortDetectedPlatesBy(detectionResult, 'overallConfidence')[0];
            var textMetrics = self.context.measureText(mostConfidentPlate.characters);
            var boxWidth = textMetrics.width + margin * 2;
            var boxHeight = fontSize + margin * 2;
            self.context.fillRect(startPoint.x - lineWidth / 2, startPoint.y, boxWidth, -(boxHeight));
            self.context.font = `${fontSize}px Arial`;
            self.context.fillStyle = '#fff';
            self.context.fillText(mostConfidentPlate.characters, startPoint.x + margin, startPoint.y - margin);
          }
        });
      }
      this.plateImage.remove();
    }
  }

  @ViewChild('canvas', { static: false }) set canvas(content: ElementRef) {
    if (content === undefined || content === null || content.nativeElement === undefined || content.nativeElement === null) return;
    this.canvasElementRef = content;
    this.context = this.canvasElementRef.nativeElement.getContext('2d');
    var blah = this.canvasElementRef.nativeElement as HTMLCanvasElement;
    this.setHiDPICanvas(this.canvasElementRef, blah.width, blah.height, this.getPixelRatio(this.context));
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

  sortDetectedPlatesBy(result: DetectionResult, prop: string) {
    return result.detectedPlates.sort((a, b) => b[prop] > a[prop] ? 1 : a[prop] === b[prop] ? 0 : -1);
  }

  getPixelRatio(ctx: CanvasRenderingContext2D): number {
    const dpr = window.devicePixelRatio || 1;   
    //const bsr = ctx.webkitBackingStorePixelRatio ||
    //  ctx.mozBackingStorePixelRatio ||
    //  ctx.msBackingStorePixelRatio ||
    //  ctx.oBackingStorePixelRatio ||
    //  ctx.backingStorePixelRatio || 1;
    //return dpr / bsr;
    return dpr;
  }

  setHiDPICanvas = function (canvas: ElementRef, w: number, h: number, pixelRatio: number) {    
    var can = canvas;
    can.nativeElement.width = w * pixelRatio;
    can.nativeElement.height = h * pixelRatio;    
    can.nativeElement.style.width = w + "px";
    can.nativeElement.style.height = h + "px";
    can.nativeElement.getContext("2d").setTransform(pixelRatio, 0, 0, pixelRatio, 0, 0);
  }
}

