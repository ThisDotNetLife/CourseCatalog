using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.Entities {
    public class Vendor {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Name of the company that created the webcast is required.")]
        [MaxLength(50, ErrorMessage = "Name of the vendor cannot be greater than 50 characters.")]
        public string CompanyName { get; set; }
    }
}
