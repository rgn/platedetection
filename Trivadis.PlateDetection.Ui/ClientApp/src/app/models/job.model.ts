import { JobState } from './jobstate.model';
import { Rectangle } from './rectangle.model';
import { DetectionResult } from './detectionresult.model';

export interface Job {
  jobId: string;
  fileName: string;
  imageHeight: number;
  imageWidth: number;
  totalProcessingTimeInMs: number;
  result: string;
  state: JobState;
  rectangles: Rectangle[];
  detectionResults?: DetectionResult[];
}
