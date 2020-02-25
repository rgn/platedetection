using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Trivadis.PlateDetection.Model
{
    [Table(name: "results")]
    public class DetectionResult
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid DetectionResultId { get; set; }        
        public float ProcessingTimeInMs { get; set; }
        public string Region { get; set; }
        public int RegionConfidence { get; set; }
        public int RequestedTopN { get; set; }        

        #region Navigation
        public Guid JobId { get; set; }
        [JsonIgnore]
        public virtual Job DetectionJob { get; set; }
        public ICollection<DetectedPlate> DetectedPlates { get; set; }
        public ICollection<DetectedPoint> DetectedPoints { get; set; }
        #endregion
    }
}