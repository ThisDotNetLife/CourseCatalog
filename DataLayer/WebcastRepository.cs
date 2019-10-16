using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using DataLayer.Entities;
using DataLayerServices;
using System.Transactions;
using Newtonsoft.Json;

namespace CourseCatalog.api.Services {
    public class WebcastRepository : IWebcastRepository {

        private string conn = string.Empty;

        public WebcastRepository(IConfiguration config) {
            conn = config.GetConnectionString("DefaultConnection");
        }

        public Webcast Find(int ID) {
            throw new NotImplementedException();
        }

        public List<Webcast> GetAll() {
            throw new NotImplementedException();
        }

        public Webcast Delete(int ID) {
            throw new NotImplementedException();
        }

        public string Save(Webcast webcast) {
            string jsonInput = JsonConvert.SerializeObject(webcast, Formatting.None);
            string jsonOutput = string.Empty;
            string spName = "jsonSaveWebcast";

            try {
                using (SqlConnection cn = new SqlConnection(conn)) {
                    var cmd = new SqlCommand(cmdText: spName, connection: cn) {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue(parameterName: "@JsonInput", value: jsonInput);
                    cn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows) {
                        reader.Read();
                        jsonOutput = reader["JsonOutput"].ToString();
                    }
                    reader.Close();
                }
                return jsonOutput;
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine(ex);
                throw;
            }

            //throw new NotImplementedException();

            #region Clutter

            //try {
            //    using (db.
            //}
            //catch (Exception) {

            //    throw;
            //}


            ////using var txScope = new TransactionScope();
            //var parameters = new DynamicParameters();
            //parameters.Add("@ID", value: webcast.ID, dbType: DbType.Int32, direction: ParameterDirection.InputOutput);
            //parameters.Add("@PhysicalPath", webcast.PhysicalPath);
            //parameters.Add("@Title", webcast.Title);
            //parameters.Add("@Vendor", webcast.Vendor);
            //parameters.Add("@Author", webcast.Author);
            //parameters.Add("@ReleaseDate", webcast.ReleaseDate);
            //parameters.Add("@Summary", webcast.Summary);
            //parameters.Add("@URL", webcast.URL);
            //parameters.Add("@Tag", webcast.Tag);

            //this.db.Execute("uspSaveWebcast", parameters, commandType: CommandType.StoredProcedure);
            //webcast.ID = parameters.Get<int>("@ID");

            //return Find(webcast.ID);

            //foreach (var addr in contact.Addresses.Where(a => !a.IsDeleted)) {
            //    addr.ContactId = contact.Id;

            //    var addrParams = new DynamicParameters(new {
            //        ContactId = addr.ContactId,
            //        AddressType = addr.AddressType,
            //        StreetAddress = addr.StreetAddress,
            //        City = addr.City,
            //        StateId = addr.StateId,
            //        PostalCode = addr.PostalCode
            //    });
            //    addrParams.Add("@Id", addr.Id, DbType.Int32, ParameterDirection.InputOutput);
            //    this.db.Execute("SaveAddress", addrParams, commandType: CommandType.StoredProcedure);
            //    addr.Id = addrParams.Get<int>("@Id");
            //}

            //foreach (var addr in contact.Addresses.Where(a => a.IsDeleted)) {
            //    this.db.Execute("DeleteAddress", new { Id = addr.Id }, commandType: CommandType.StoredProcedure);
            //}

            //txScope.Complete();




            //var sql = "INSERT INTO Webcast(PhysicalPath, Title, VendorID, AuthorID, ReleaseDate, Summary, URL) VALUES(@PhysicalPath, @Title, @Vendor, @Author, @ReleaseDate, @Sumary, @URL);" +
            //          "SELECT CAST(SCOPE_IDENTITY() AS INT)";
            //var id = this.db.Query<int>(sql, webcast).Single();
            //return webcast.ID;

            #endregion
        }
    }
}
