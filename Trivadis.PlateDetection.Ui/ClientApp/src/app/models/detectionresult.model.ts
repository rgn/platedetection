import { DetectedPlate } from "./detectedplate.model";
import { DetectedPoint } from "./detectedpoint.model";

export interface DetectionResult {
  detectionResultId: string;
  region: string;
  regionConfidence: number;
  requestedTopN: number;
  processingTimeInMs: number;
  detectedPlates: DetectedPlate[];
  detectedPoints: DetectedPoint[];
}
