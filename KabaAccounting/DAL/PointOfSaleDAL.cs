using KabaAccounting.BLL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KabaAccounting.DAL
{
    class PointOfSaleDAL
    {
        static string connString = ConfigurationManager.ConnectionStrings["KabaAccountingConnString"].ConnectionString;


        #region SELECT METHOD
        public DataTable Select()
        {
            SqlConnection conn = new SqlConnection(connString);

            DataTable dataTable = new DataTable();

            try
            {
                string sqlQuery = "SELECT * FROM tbl_pos";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                conn.Open();

                dataAdapter.Fill(dataTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return dataTable;
        }
        #endregion

        #region INSERT METHOD
        public bool Insert(PointOfSaleBLL pointOfSaleBLL)
        {
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                string sqlQuery = "INSERT INTO tbl_pos (sale_type, customer_id, sub_total, vat, discount, grand_total, added_date, added_by) VALUES (@sale_type, @customer_id, @sub_total, @vat, @discount, @grand_total, @added_date, @added_by)";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("sale_type", pointOfSaleBLL.SaleType);
                cmd.Parameters.AddWithValue("customer_id", pointOfSaleBLL.CustomerId);
                cmd.Parameters.AddWithValue("sub_total", pointOfSaleBLL.SubTotal);
                cmd.Parameters.AddWithValue("vat",pointOfSaleBLL.Vat);
                cmd.Parameters.AddWithValue("discount",pointOfSaleBLL.Discount);
                cmd.Parameters.AddWithValue("grand_total",pointOfSaleBLL.GrandTotal);
                cmd.Parameters.AddWithValue("added_date",pointOfSaleBLL.AddedDate);
                cmd.Parameters.AddWithValue("added_by",pointOfSaleBLL.AddedBy);

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
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return isSuccess;
        }
        #endregion

        #region UPDATE METHOD
        public bool Update(PointOfSaleBLL pointOfSaleBLL)
        {
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                string sqlQuery = "UPDATE tbl_pos SET sale_type=@sale_type, customer_id=@customer_id, sub_total=@sub_total, vat=@vat, discount=@discount, grand_total=@grand_total, added_date=@added_date, added_by=@added_by WHERE id=@id";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("sale_type", pointOfSaleBLL.SaleType);
                cmd.Parameters.AddWithValue("customer_id", pointOfSaleBLL.CustomerId);
                cmd.Parameters.AddWithValue("sub_total", pointOfSaleBLL.SubTotal);
                cmd.Parameters.AddWithValue("vat", pointOfSaleBLL.Vat);
                cmd.Parameters.AddWithValue("discount", pointOfSaleBLL.Discount);
                cmd.Parameters.AddWithValue("grand_total", pointOfSaleBLL.GrandTotal);
                cmd.Parameters.AddWithValue("added_date", pointOfSaleBLL.AddedDate);
                cmd.Parameters.AddWithValue("added_by", pointOfSaleBLL.AddedBy);
                cmd.Parameters.AddWithValue("id", pointOfSaleBLL.Id);//Do you really need to update the ID?


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
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return isSuccess;
        }
        #endregion

        #region DELETE METHOD
        public bool Delete(PointOfSaleBLL pointOfSaleBLL)
        {
            //Create a Boolean variable and set its value to false.
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                //SQL Query to Delete from the Database
                string sqlQuery = "DELETE FROM tbl_pos WHERE id=@id";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                //Passing the value using cmd
                cmd.Parameters.AddWithValue("@id", pointOfSaleBLL.Id);

                //Opening the SQL connection
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
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return isSuccess;
        }
        #endregion
    }
}
