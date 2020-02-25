using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Trivadis.PlateDetection.Model
{
    [Table(name: "plates")]
    public class DetectedPlate
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid DetectedPlateId { get; set; }        
        public bool MatchesTemplate { get; set;  }
        public float OverallConfidence { get; set; }
        public string Characters { get; set; }

        #region Navigation
        public Guid DetectionResultId { get; set; }
        [JsonIgnore]
        public virtual DetectionResult DetectionResult { get; set; }
        #endregion

    }
}