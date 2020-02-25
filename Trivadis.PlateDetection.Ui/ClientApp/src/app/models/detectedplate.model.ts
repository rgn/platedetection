export interface DetectedPlate {
  detectedPlateId: string;
  matchesTemplate: boolean;
  overallConfidence: number;
  characters: string;
}
