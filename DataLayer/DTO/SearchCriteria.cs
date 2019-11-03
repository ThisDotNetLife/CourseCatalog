using System;
using System.Collections.Generic;
using System.Text;

namespace CourseCatalog.DAL.DTO {
    public class SearchCriteria {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Vendor { get; set; }
        public List<string> Tags { get; set; }
        public int YearOfRelease { get; set; }
        public bool OrderByReleaseDate { get; set; }
        public bool OrderByTitle { get; set; }

        public SearchCriteria() {
            Tags = new List<string>();
        }
    }
}
