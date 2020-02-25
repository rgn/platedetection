using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Trivadis.PlateDetection.Model
{
    [Table(name: "rectangles")]
    public class Rectangle
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid RectangleId { get; set; }        
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Guid JobId { get; set; }
        [JsonIgnore]
        public virtual Job DetectionJob { get; set; }
    }
}
