using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseCatalog.api.Services {
    public interface IDataRepository {
        string GetConnectionString();
    }
}
