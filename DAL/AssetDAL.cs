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
                string sqlQuery = "UPDATE tbl_assets SET source_balance=@source_balance WHERE id=@id";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

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
        #region SEARCH METHOD BY ID
        public DataTable SearchById(int id)
        {
            SqlConnection conn = new SqlConnection(connString);//Static method to connect database

            DataTable dataTable = new DataTable();//To hold the data from database
            try
            {
                String sql = "SELECT * FROM tbl_assets WHERE id=" + id + "";//SQL query to search data from database 
                SqlCommand cmd = new SqlCommand(sql, conn);//For executing the command 
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);//Getting data from database           
                conn.Open();//Opening the database connection
                dataAdapter.Fill(dataTable);//Passing values from adapter to Data Table
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return dataTable;
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
