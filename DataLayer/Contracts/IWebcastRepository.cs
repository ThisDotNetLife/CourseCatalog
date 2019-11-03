using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Entities;

namespace DataLayerServices {
    public interface IWebcastRepository {

        List<Webcast> Search(int ID);

        String Save(Webcast webcast);

        void Delete(int ID);

        List<String> ValidateFilesOnDisk(string driveLetter);

        string GetWebcastByID(int ID);
    }
}
