using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayerServices {
    public interface IVendorRepository {

        string Get();

        void Update(DataLayer.Entities.Vendor vendor);
    }
}