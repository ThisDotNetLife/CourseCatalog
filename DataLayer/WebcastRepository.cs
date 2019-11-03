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
using System.IO;

namespace CourseCatalog.api.Services {
    public class WebcastRepository : IWebcastRepository {

        private readonly IConfiguration configuration;
        private readonly string conn = string.Empty;
        private string defaultDriveLetter = string.Empty;

        public WebcastRepository(IConfiguration config) {
            configuration = config;
            conn = config.GetConnectionString("DefaultConnection");
            defaultDriveLetter = config.GetSection("Features:FileValidation:DefaultDriveLetter").Value;
        }

        public List<Webcast> Search(int ID) {
            throw new NotImplementedException();
        }

        public void Delete(int ID) {
            string spName = "DeleteWebcast";
            try {
                using (SqlConnection cn = new SqlConnection(conn)) {
                    var cmd = new SqlCommand(cmdText: spName, connection: cn) {
                        CommandType = CommandType.StoredProcedure
                    };

                    cmd.Parameters.AddWithValue(parameterName: "@ID", value: ID);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                }
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine(ex);
                throw;
            }
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
        }

        public List<string> ValidateFilesOnDisk(string driveLetter = "") {
            var results = new List<string>();
            int webcastsNotFound = 0;

            #region Throw exception if requested drive letter not found.

            if (driveLetter.Length == 0) {
                driveLetter = defaultDriveLetter;
            }

            if (!Directory.Exists(driveLetter + @":\")) {
                string errorMsg = String.Format("Drive letter ({0}:) does not exist.", driveLetter);
                throw new System.ArgumentException(errorMsg);
            }

            #endregion
            
            DataTable dt = new DataTable();                // Create new DataTable object for filling.

            string sql = @"SELECT ID, PhysicalPath, Title FROM dbo.Webcast WHERE LEN(PhysicalPath) > 0";

            try {        
                using (SqlConnection cnn = new SqlConnection(conn)) {
                    using (SqlCommand cmd = new SqlCommand(sql, cnn)) {
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd)) {
                            cmd.CommandType = CommandType.Text;
                            da.Fill(dt);                   // Fill DataTable using DataAdapter
                        }
                    }
                }

                #region Throw exception if no webcasts were found in collection.

                if (dt.Rows.Count == 0) {
                    throw new System.ArgumentException("No webcasts were found in database. Recommend checking the connection string.");
                }
                #endregion

                foreach (DataRow row in dt.Rows) {
                    string physicalPath   = row["PhysicalPath"].ToString();
                    string title          = row["Title"].ToString();
                    string folderLocation = string.Format("{0}:\\{1}\\{2}", driveLetter, physicalPath, title);

                    bool isFoundOnDisk = Directory.Exists(folderLocation) ? true : false;
                    if (!isFoundOnDisk) {
                        if (File.Exists(folderLocation + ".mp4")) {
                            isFoundOnDisk = true;
                        }
                    }
                    if (!isFoundOnDisk) {
                        webcastsNotFound++;
                        results.Add(string.Format("Not found on disk: {0}", folderLocation));
                    }
                }
                if (webcastsNotFound == 0) {
                    results.Add("All webcasts stored in the database were found on disk.");
                }
                return results;
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        public string GetWebcastByID(int ID) {
            string jsonOutput = string.Empty;
            string spName = "jsonGetWebcastByID";

            try {
                using (SqlConnection cn = new SqlConnection(conn)) {
                    var cmd = new SqlCommand(cmdText: spName, connection: cn) {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue(parameterName: "@ID", value: ID);

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
        }
    }
}
