using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.Entities {
    public class Author {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Name of author is required.")]
        [MaxLength(50, ErrorMessage = "Name of author cannot be greater than 50 characters.")]
        public string FullName { get; set; }
    }
}
