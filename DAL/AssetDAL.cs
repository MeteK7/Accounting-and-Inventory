using CUL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class AssetDAL
    {
        static string connString = ConfigurationManager.ConnectionStrings["KabaAccountingConnString"].ConnectionString;

        #region UPDATE METHOD
        public bool Update(AssetCUL assetCUL)
        {
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                string sqlQuery = "UPDATE tbl_assets SET source_id=@source_id, source_type=@source_type, source_balance=@source_balance WHERE id=@id";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("source_id", assetCUL.SourceId);
                cmd.Parameters.AddWithValue("source_type", assetCUL.SourceType);
                cmd.Parameters.AddWithValue("source_balance", assetCUL.SourceBalance);
                cmd.Parameters.AddWithValue("id", assetCUL.Id);

                conn.Open();

                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    isSuccess = true;
                }
                else
                {
                    isSuccess = false;
                }

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return isSuccess;
        }
        #endregion

        public int GetAssetIdBySource(int sourceId, string sourceType)
        {
            int id=0;

            SqlConnection conn = new SqlConnection(connString);//Static method to connect database

            using (conn)
            {
                try
                {
                    String sql = "SELECT id FROM tbl_assets WHERE source_id=" + sourceId + " AND source_type='" + sourceType + "'";//SQL query to search data from database 
                    conn.Open();//Opening the database connection
                    SqlCommand cmd = new SqlCommand(sql, conn);//For executing the command 
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        id= reader.GetInt32("id");
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
            return id;
        }
    }
}
