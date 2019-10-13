using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace CourseCatalog.api.Services {
    public class DataRepository : IDataRepository {

        private IDbConnection db;

        public DataRepository(IConfiguration config) {
            this.db = new SqlConnection(config.GetConnectionString("DefaultConnection"));
        }

        public string GetConnectionString() {
            return db.ConnectionString;
        }
    }
}
