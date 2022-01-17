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
        static string connString = ConfigurationManager.ConnectionStrings["AccountingConnString"].ConnectionString;

        public string BackupDatabase(BackupCUL backupCUL)
        {
            string isSuccess = "";
            var backupPath = Path.Combine(backupCUL.DatabasePath, backupCUL.DatabaseName);
            // Create the backup in the temp directory (the server should have access there)
            var sqlQuery = "BACKUP DATABASE Accounting TO DISK = @backupPath";

            using (var conn = new SqlConnection(connString))
            { 
                using (var cmd = new SqlCommand(sqlQuery, conn))
                {
                    try
                    {
                        conn.Open();

                        cmd.Parameters.AddWithValue("@databaseName", backupCUL.DatabaseName);
                        cmd.Parameters.AddWithValue("@backupPath", backupPath);
                        int rows = cmd.ExecuteNonQuery();
                        isSuccess = "Backed up!";
                    }
                    catch (Exception)
                    {
                        isSuccess = "Something went wrong :/";
                    }
                    finally
                    {
                        File.Copy(backupPath, backupCUL.DatabaseName); // Copy file to final location
                        conn.Close();
                    }  
                }
            }
            return isSuccess;
        }

        public string RestoreDatabase(BackupCUL backupCUL)
        {
            string isSuccess = "";
            var restorePath = Path.Combine(backupCUL.DatabasePath, backupCUL.DatabaseName);
            // Create the backup in the temp directory (the server should have access there)
            var sqlQuery = "RESTORE DATABASE Sample FROM  DISK = @backupPath WITH  NOUNLOAD,  REPLACE,  STATS = 10";

            using (var conn = new SqlConnection(connString))
            {
                using (var cmd = new SqlCommand(sqlQuery, conn))
                {
                    try
                    {
                        conn.Open();

                        //cmd.Parameters.AddWithValue("@databaseName", backupCUL.DatabaseName);
                        cmd.Parameters.AddWithValue("@backupPath", restorePath);
                        int rows = cmd.ExecuteNonQuery();
                        isSuccess = "Restored!";
                    }
                    catch (Exception)
                    {
                        isSuccess = "Something went wrong :/";
                    }
                    finally
                    {
                        //File.Copy(restorePath, backupCUL.DatabaseName); // Copy file to final location
                        conn.Close();
                    }
                }
            }
            return isSuccess;
        }
    }
}
