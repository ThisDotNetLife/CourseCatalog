using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CourseCatalog.api.Entities {
    public class Tag {

        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "One or more tag names are required.")]
        [MaxLength(50, ErrorMessage = "Tag names cannot be greater than 50 characters.")]
        public string Descr { get; set; }
    }
}
