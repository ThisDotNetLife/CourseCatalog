using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.Entities {
    public class Webcast {
        [Key]
        public int ID { get; set; }

        [MaxLength(200)]
        public string PhysicalPath { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(500, ErrorMessage = "Title cannot be greater than 500 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vendor is required.")]
        [MaxLength(25, ErrorMessage = "Name of vendor cannot be greater than 25 characters.")]
        public string Vendor { get; set; }

        [Required(ErrorMessage = "Name of author is required.")]
        [MaxLength(50, ErrorMessage = "Name of author cannot be greater than 50 characters.")]
        public string Author { get; set; }

        [Required(ErrorMessage = "Date when webcast was acquired or when it was released is required.")]
        public string ReleaseDate { get; set; }

        [Required(ErrorMessage = "Summary that describes the webcast is required.")]
        public string Summary { get; set; }

        [Required(ErrorMessage = "URL to original webcast is required.")]
        [MaxLength(200)]
        public string URL { get; set; }

        [Required(ErrorMessage = "Tag (topic) is required.")]
        [MaxLength(200)]
        public string Tag { get; set; }

        //public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}
