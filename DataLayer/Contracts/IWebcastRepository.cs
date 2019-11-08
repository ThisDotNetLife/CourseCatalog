using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Entities;

namespace DataLayerServices {
    public interface IWebcastRepository {

        String Search(CourseCatalog.DAL.DTO.SearchCriteria searchCriteria);

        String Save(Webcast webcast);

        void Delete(int ID);

        List<String> ValidateFilesOnDisk(string driveLetter);
    }
}
