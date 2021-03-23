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
    public class PointOfPurchaseDAL
    {
        static string connString = ConfigurationManager.ConnectionStrings["KabaAccountingConnString"].ConnectionString;


        #region SELECT METHOD
        public DataTable Select()
        {
            SqlConnection conn = new SqlConnection(connString);

            DataTable dataTable = new DataTable();

            try
            {
                string sqlQuery = "SELECT * FROM tbl_pop";

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
        public bool Insert(PointOfPurchaseCUL pointOfPurchaseCUL)
        {
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                string sqlQuery = "INSERT INTO tbl_pop (id, invoice_no, payment_type_id, supplier_id, total_product_amount, cost_total, sub_total, vat, discount, grand_total, added_date, added_by) VALUES (@id, @invoice_no, @payment_type_id, @supplier_id, @total_product_amount, @cost_total, @sub_total, @vat, @discount, @grand_total, @added_date, @added_by)";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("@id", pointOfPurchaseCUL.Id);//The column id in the database is not auto incremental. This is to prevent the number from increasing when the user deletes an existing invoice and creates a new invoice.
                cmd.Parameters.AddWithValue("@invoice_no", pointOfPurchaseCUL.InvoiceNo);
                cmd.Parameters.AddWithValue("@payment_type_id", pointOfPurchaseCUL.PaymentTypeId);
                cmd.Parameters.AddWithValue("@supplier_id", pointOfPurchaseCUL.SupplierId);
                cmd.Parameters.AddWithValue("@total_product_amount", pointOfPurchaseCUL.TotalProductAmount);
                cmd.Parameters.AddWithValue("@cost_total", pointOfPurchaseCUL.CostTotal);
                cmd.Parameters.AddWithValue("@sub_total", pointOfPurchaseCUL.SubTotal);
                cmd.Parameters.AddWithValue("@vat", pointOfPurchaseCUL.Vat);
                cmd.Parameters.AddWithValue("@discount", pointOfPurchaseCUL.Discount);
                cmd.Parameters.AddWithValue("@grand_total", pointOfPurchaseCUL.GrandTotal);
                cmd.Parameters.AddWithValue("@added_date", pointOfPurchaseCUL.AddedDate);
                cmd.Parameters.AddWithValue("@added_by", pointOfPurchaseCUL.AddedBy);

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
        public bool Update(PointOfPurchaseCUL PointOfPurchaseCUL)
        {
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                string sqlQuery = "UPDATE tbl_pop SET payment_type_id=@payment_type_id, supplier_id=@supplier_id,total_product_amount=@total_product_amount, cost_total=@cost_total, sub_total=@sub_total, vat=@vat, discount=@discount, grand_total=@grand_total, added_date=@added_date, added_by=@added_by WHERE id=@id";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("payment_type_id", PointOfPurchaseCUL.PaymentTypeId);
                cmd.Parameters.AddWithValue("supplier_id", PointOfPurchaseCUL.SupplierId);
                cmd.Parameters.AddWithValue("total_product_amount", PointOfPurchaseCUL.TotalProductAmount);
                cmd.Parameters.AddWithValue("cost_total", PointOfPurchaseCUL.CostTotal);
                cmd.Parameters.AddWithValue("sub_total", PointOfPurchaseCUL.SubTotal);
                cmd.Parameters.AddWithValue("vat", PointOfPurchaseCUL.Vat);
                cmd.Parameters.AddWithValue("discount", PointOfPurchaseCUL.Discount);
                cmd.Parameters.AddWithValue("grand_total", PointOfPurchaseCUL.GrandTotal);
                cmd.Parameters.AddWithValue("added_date", PointOfPurchaseCUL.AddedDate);
                cmd.Parameters.AddWithValue("added_by", PointOfPurchaseCUL.AddedBy);
                cmd.Parameters.AddWithValue("id", PointOfPurchaseCUL.InvoiceNo);//Do you really need to update the ID?


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
        public bool Delete(PointOfPurchaseCUL PointOfPurchaseCUL)
        {
            //Create a Boolean variable and set its value to false.
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                //SQL Query to Delete from the Database
                string sqlQuery = "DELETE FROM tbl_pop WHERE id=@id";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                //Passing the value using cmd
                cmd.Parameters.AddWithValue("@id", PointOfPurchaseCUL.InvoiceNo);

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

        #region GETTING THE LAST ID AND ROW DATAS OF THE TABLE IN THE DATABASE USING INVOICE NO
        public DataTable GetByInvoiceId(int invoiceId = 0)//Optional parameter
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                DataTable dataTable = new DataTable();
                String sql;

                if (invoiceId == 0)//If the invoice number is 0 which means user did not send any argument, then get the last record using the following query.
                {
                    sql = "SELECT * FROM tbl_pop WHERE id=(SELECT max(id) FROM tbl_pop)";
                    //sql = "SELECT * FROM tbl_pop WHERE id=IDENT_CURRENT('tbl_pop')";//SQL query to get the last id of rows in the table.
                }

                else
                {
                    sql = "SELECT * FROM tbl_pop WHERE id=" + invoiceId + "";//SQL query to get the last id of rows in the table.
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

        #region GETTING THE LAST NO AND ROW DATAS OF THE TABLE IN THE DATABASE USING INVOICE ID
        public DataTable SearchByInvoiceNo(int invoiceNo)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                DataTable dataTable = new DataTable();
                String sql;

                    sql = "SELECT * FROM tbl_pop WHERE invoice_no=" + invoiceNo + "";

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
    }
}
