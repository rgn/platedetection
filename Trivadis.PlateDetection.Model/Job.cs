using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Trivadis.PlateDetection.Model
{
    [Table(name: "jobs")]
    public class Job : ITrackable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid JobId { get; set; }
        public string FileName { get; set; }
        public int ImageHeight { get; set; }
        public int ImageWidth { get; set; }
        public float TotalProcessingTimeInMs { get; set; }
        public string Result { get; set; }
        public JobState State { get; set; }        
        [JsonIgnore]
        public byte[] ImageData { get; set; }
        public ICollection<Rectangle> Rectangles { get; set; }
        public ICollection<DetectionResult> DetectionResults { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public string LastUpdatedBy { get; set; }
    }
}
