using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayerServices {
    public interface IVendorRepository {

        DataLayer.Entities.Vendor Find(int ID);

        List<DataLayer.Entities.Vendor> GetAll();

        DataLayer.Entities.Vendor Add(DataLayer.Entities.Webcast webcast);

        DataLayer.Entities.Vendor Update(DataLayer.Entities.Webcast webcast);

        DataLayer.Entities.Vendor Remove(int ID);
    }
}