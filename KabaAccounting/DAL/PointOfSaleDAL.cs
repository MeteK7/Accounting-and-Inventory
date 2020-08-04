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
                string sqlQuery = "INSERT INTO tbl_pos (id, sale_type, customer_id, cost_total, sub_total, vat, discount, grand_total, added_date, added_by) VALUES (@id, @sale_type, @customer_id, @cost_total, @sub_total, @vat, @discount, @grand_total, @added_date, @added_by)";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("@id", pointOfSaleBLL.Id);
                cmd.Parameters.AddWithValue("@sale_type", pointOfSaleBLL.SaleType);
                cmd.Parameters.AddWithValue("@customer_id", pointOfSaleBLL.CustomerId);
                cmd.Parameters.AddWithValue("@cost_total", pointOfSaleBLL.CostTotal);
                cmd.Parameters.AddWithValue("@sub_total", pointOfSaleBLL.SubTotal);
                cmd.Parameters.AddWithValue("@vat",pointOfSaleBLL.Vat);
                cmd.Parameters.AddWithValue("@discount",pointOfSaleBLL.Discount);
                cmd.Parameters.AddWithValue("@grand_total",pointOfSaleBLL.GrandTotal);
                cmd.Parameters.AddWithValue("@added_date",pointOfSaleBLL.AddedDate);
                cmd.Parameters.AddWithValue("@added_by",pointOfSaleBLL.AddedBy);

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
                string sqlQuery = "UPDATE tbl_pos SET sale_type=@sale_type, customer_id=@customer_id, cost_total=@cost_total, sub_total=@sub_total, vat=@vat, discount=@discount, grand_total=@grand_total, added_date=@added_date, added_by=@added_by WHERE id=@id";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("sale_type", pointOfSaleBLL.SaleType);
                cmd.Parameters.AddWithValue("customer_id", pointOfSaleBLL.CustomerId);
                cmd.Parameters.AddWithValue("cost_total", pointOfSaleBLL.CostTotal);
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

        #region GETTING THE LAST ID AND ROW DATAS OF THE TABLE IN THE DATABASE
        public DataTable Search(int invoiceNo=0)//Optional parameter
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                DataTable dataTable = new DataTable();
                String sql;

                if (invoiceNo==0)//If the invoice number is 0 which means user did not send any argument, then get the last Id using the following query.
                {
                    sql = "SELECT * FROM tbl_pos WHERE id=(SELECT max(id) FROM tbl_pos)";
                    //sql = "SELECT * FROM tbl_pos WHERE id=IDENT_CURRENT('tbl_pos')";//SQL query to get the last id of rows in te table.
                }

                else
                {
                    sql = "SELECT * FROM tbl_pos WHERE id=" + invoiceNo + "";//SQL query to get the last id of rows in te table.
                }

                using (SqlCommand cmd = new SqlCommand(sql, conn)) 
                {
                    try
                    {
                        /*cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;*/
                        conn.Open();//Opening the database connection

                        using (SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd))
                        {
                            dataAdapter.Fill(dataTable);
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
                    return dataTable;
                }
            }
        }
        #endregion
    }
}

/* DO NOT FORGET THIS CODE BLOCK!!! THIS IS VALID FOR IF YOU WANT TO HAVE SOME OF THE CELLS FROM THE TABLE!
         public int Search()
        {
            int lastInvoiceNo=0;

            using (SqlConnection conn = new SqlConnection(connString)) 
            {
                String sql = "SELECT * FROM tbl_pos WHERE id=IDENT_CURRENT('tbl_pos')";

                using (SqlCommand cmd = new SqlCommand(sql, conn)) 
                {
                    try
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;
                        conn.Open();//Opening the database connection

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            lastInvoiceNo = Convert.ToInt32(reader["id"]);
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
                    return lastInvoiceNo;
                }
            }
        }
 */