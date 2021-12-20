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
    public class PosReportDAL
    {
        static string connString = ConfigurationManager.ConnectionStrings["AccountingConnString"].ConnectionString;

        #region INSERT METHOD
        public void Insert(PosReportCUL posReportCUL)
        {
            SqlConnection conn = new SqlConnection(connString);
            try
            {
                String sqlQuery = "INSERT INTO tbl_pos_report (sale_date) VALUES (@sale_date)";
                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("@sale_date", posReportCUL.SaleDate);

                conn.Open();

                int rows = cmd.ExecuteNonQuery();
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
        #endregion

        #region FETCH ID BY DATE METHOD
        public DataTable FetchIdByDate(string saleDate)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                DataTable dataTable = new DataTable();

                String sqlQuery = "Select * FROM tbl_pos_report WHERE sale_date= " + saleDate + "";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    try
                    {
                        conn.Open();//Opening the database connection

                        using (SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd))
                        {
                            dataAdapter.Fill(dataTable);//Passing values from adapter to Data Table
                        }
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        conn.Close();
                        dataTable.Dispose();
                    }
                    return dataTable;
                }
            }
        }
        #endregion
    }
}
