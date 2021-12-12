using CUL;
using CUL.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class BackupDAL
    {
        static string connString = ConfigurationManager.ConnectionStrings["KabaAccountingConnString"].ConnectionString;

        public bool BackupDatabase(BackupCUL backupCUL)
        {
            bool isSuccess = false;
            var backupCombined = Path.Combine(backupCUL.DatabasePath, backupCUL.DatabaseName);
            // Create the backup in the temp directory (the server should have access there)
            var sqlQuery = "BACKUP DATABASE KabaAccounting TO DISK = @backup";

            using (var conn = new SqlConnection(connString))
            { 
                using (var cmd = new SqlCommand(sqlQuery, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@databaseName", backupCUL.DatabaseName);
                    cmd.Parameters.AddWithValue("@backup", backupCombined);
                    int rows=cmd.ExecuteNonQuery();

                    conn.Close();

                    if (rows > (int)Numbers.InitialIndex)
                        isSuccess = true;
                    else
                        isSuccess = false;
                }
            }

            File.Copy(backupCombined, backupCUL.DatabaseName); // Copy file to final location

            return isSuccess;
        }
    }
}
