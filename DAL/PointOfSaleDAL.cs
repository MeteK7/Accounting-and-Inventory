using KabaAccounting.CUL;
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
    public class PointOfSaleDAL
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
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return dataTable;
        }
        #endregion

        #region INSERT METHOD
        public bool Insert(PointOfSaleCUL pointOfSaleCUL)
        {
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                string sqlQuery = "INSERT INTO tbl_pos (id, payment_type_id, customer_id, account_id, total_product_quantity, cost_total, sub_total, vat, discount, grand_total, added_date, added_by) VALUES (@id, @payment_type_id, @customer_id, @account_id, @total_product_quantity, @cost_total, @sub_total, @vat, @discount, @grand_total, @added_date, @added_by)";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("@id", pointOfSaleCUL.Id);//The column id in the database is not auto incremental. This is to prevent the number from increasing when the user deletes an existing invoice and creates a new invoice.
                cmd.Parameters.AddWithValue("@payment_type_id", pointOfSaleCUL.PaymentTypeId);
                cmd.Parameters.AddWithValue("@customer_id", pointOfSaleCUL.CustomerId);
                cmd.Parameters.AddWithValue("@account_id", pointOfSaleCUL.AccountId);
                cmd.Parameters.AddWithValue("@total_product_quantity", pointOfSaleCUL.TotalProductQuantity);
                cmd.Parameters.AddWithValue("@cost_total", pointOfSaleCUL.CostTotal);
                cmd.Parameters.AddWithValue("@sub_total", pointOfSaleCUL.SubTotal);
                cmd.Parameters.AddWithValue("@vat",pointOfSaleCUL.Vat);
                cmd.Parameters.AddWithValue("@discount",pointOfSaleCUL.Discount);
                cmd.Parameters.AddWithValue("@grand_total",pointOfSaleCUL.GrandTotal);
                cmd.Parameters.AddWithValue("@added_date",pointOfSaleCUL.AddedDate);
                cmd.Parameters.AddWithValue("@added_by",pointOfSaleCUL.AddedBy);

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

        #region UPDATE METHOD
        public bool Update(PointOfSaleCUL pointOfSaleCUL)
        {
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                string sqlQuery = "UPDATE tbl_pos SET payment_type_id=@payment_type_id, customer_id=@customer_id, account_id=@account_id, total_product_quantity=@total_product_quantity, cost_total=@cost_total, sub_total=@sub_total, vat=@vat, discount=@discount, grand_total=@grand_total, added_date=@added_date, added_by=@added_by WHERE id=@id";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("payment_type_id", pointOfSaleCUL.PaymentTypeId);
                cmd.Parameters.AddWithValue("customer_id", pointOfSaleCUL.CustomerId);
                cmd.Parameters.AddWithValue("account_id", pointOfSaleCUL.AccountId);
                cmd.Parameters.AddWithValue("total_product_quantity", pointOfSaleCUL.TotalProductQuantity);
                cmd.Parameters.AddWithValue("cost_total", pointOfSaleCUL.CostTotal);
                cmd.Parameters.AddWithValue("sub_total", pointOfSaleCUL.SubTotal);
                cmd.Parameters.AddWithValue("vat", pointOfSaleCUL.Vat);
                cmd.Parameters.AddWithValue("discount", pointOfSaleCUL.Discount);
                cmd.Parameters.AddWithValue("grand_total", pointOfSaleCUL.GrandTotal);
                cmd.Parameters.AddWithValue("added_date", pointOfSaleCUL.AddedDate);
                cmd.Parameters.AddWithValue("added_by", pointOfSaleCUL.AddedBy);
                cmd.Parameters.AddWithValue("id", pointOfSaleCUL.Id);//Do you really need to update the ID?


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

        #region DELETE METHOD
        public bool Delete(int invoiceId)
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
                cmd.Parameters.AddWithValue("@id", invoiceId);

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
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }

            return isSuccess;
        }
        #endregion

        #region COUNT BY DAY METHOD
        public int CountPaymentTypeByToday(bool cashOrCredit)
        {
            SqlConnection conn = new SqlConnection(connString);

            int counter = 0;
            string sqlQuery;

            try
            {
                if (cashOrCredit == true)//Get the cash sales for today if the cashOrCredit boolean variable is true
                {
                    sqlQuery = "Select COUNT(*) FROM tbl_pos WHERE added_date>Convert(date, getdate()) AND payment_type_id=1";//This query counts the records from the beginning of the day to the rest of the day.

                }
                else//Get the credit sales for today if the cashOrCredit boolean variable is false
                {
                    sqlQuery = "Select COUNT(*) FROM tbl_pos WHERE added_date>Convert(date, getdate()) AND payment_type_id=2";//This query counts the records from the beginning of the day to the rest of the day.
                }

                conn.Open();
                SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                SqlDataReader sqlDataReader = cmd.ExecuteReader();

                if (sqlDataReader.HasRows)
                {
                    sqlDataReader.Read(); // read first row
                    counter = sqlDataReader.GetInt32(0);
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
            return counter;
        }
        #endregion

        #region FETCH BY TODAY METHOD
        public DataTable FetchReportByToday()
        {
            //Creating database connection
            SqlConnection conn = new SqlConnection(connString);

            DataTable dataTable = new DataTable();

            try
            {
                //Writing SQL Query to get all the datas from database
                string sqlQuery = "Select * FROM tbl_pos WHERE added_date>Convert(date, getdate())";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                conn.Open();

                //Adding the value from dataAdapter into the dataTable.
                dataAdapter.Fill(dataTable);
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

        #region GETTING ANY OR THE LAST ID AND ROW DATAS OF THE TABLE IN THE DATABASE
        public DataTable GetByIdOrLastId(int invoiceId=0)//Optional parameter
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                DataTable dataTable = new DataTable();
                String sql;

                if (invoiceId==0)//If the invoice number is 0 which means user did not send any argument, then get the last Id using the following query.
                {
                    sql = "SELECT * FROM tbl_pos WHERE id=(SELECT max(id) FROM tbl_pos)";
                    //sql = "SELECT * FROM tbl_pos WHERE id=IDENT_CURRENT('tbl_pos')";//SQL query to get the last id of rows in the table.
                }

                else
                {
                    sql = "SELECT * FROM tbl_pos WHERE id=" + invoiceId + "";
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
                        //MessageBox.Show(ex.Message);
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

        #region CHECK RECORD EXISTANCE
        public DataTable CheckReportExistance()
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                DataTable dataTable = new DataTable();

                String sqlQuery = "SELECT TOP 1 * FROM tbl_pos ORDER BY id DESC";

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
                        //MessageBox.Show(ex.Message);
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