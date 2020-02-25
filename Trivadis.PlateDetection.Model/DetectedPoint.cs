using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Trivadis.PlateDetection.Model
{
    [Table(name: "result_points")]
    public class DetectedPoint
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid DetectedPointId { get; set; }        
        
        public int X { get; set; }
        public int Y { get; set; }

        #region Navigation
        public Guid DetectionResultId { get; set; }

        [JsonIgnore]
        public virtual DetectionResult DetectionResult { get; set; }
        #endregion

    }
}