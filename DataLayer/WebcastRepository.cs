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

        public string Search(CourseCatalog.DAL.DTO.SearchCriteria searchCriteria) {
            string jsonInput = JsonConvert.SerializeObject(searchCriteria, Formatting.None);
            string jsonOutput = string.Empty;
            string spName = "jsonSearchWebcasts";

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

                string target = "Tags" + (char)34 + ":" + (char)34 + "[";

                if (jsonOutput.IndexOf(target) > -1) {
                    return CleanupStringArray(jsonOutput);
                } else {
                    return jsonOutput;
                }
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine(ex);
                throw;
            }
        }

        /// <summary>
        /// This method will cleanup JSON string arrays returned by SQL Server so they are correctly 
        /// interpreted by JSON parsers such as Postman. For example, jsonInput might contain the
        /// following JSON fragment:  "Tags":"[\"Javascript\",\"JavaScript Classes\"]"}]
        /// After cleanup, this fragment would look like: "Tags": ["Javascript","JavaScript Classes"]
        /// </summary>
        /// <param name="jsonInput"></param>
        /// <returns></returns>
        private string CleanupStringArray(string jsonInput) {
            const char dblQuote = (char)34;
            string str1of3B = "Tags" + dblQuote + ":" + dblQuote + "[";
            string str1of3A = "Tags" + dblQuote + ":" + "[";
            string str2of3B = "]" + dblQuote + "}]";
            string str2of3A = "]}]";
            string str3of3 = @"\" + dblQuote;

            return jsonInput.Replace(str1of3B, str1of3A).Replace(str2of3B, str2of3A).Replace(str3of3, dblQuote.ToString());
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
                string target = "Tags" + (char)34 + ":" + (char)34 + "[";

                if (jsonOutput.IndexOf(target) > -1) {
                    return CleanupStringArray(jsonOutput);
                } else {
                    return jsonOutput;
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
    }
}
