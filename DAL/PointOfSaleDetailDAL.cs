using CUL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DAL
{
    public class PointOfSaleDetailDAL
    {
        static string connString = ConfigurationManager.ConnectionStrings["KabaAccountingConnString"].ConnectionString;


        #region SELECT METHOD
        public DataTable Select()
        {
            SqlConnection conn = new SqlConnection(connString);

            DataTable dataTable = new DataTable();

            try
            {
                string sqlQuery = "SELECT * FROM tbl_pos_details";

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
        public bool Insert(PointOfSaleDetailCUL pointOfSaleDetailCUL)
        {
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                String sqlQuery = "INSERT INTO tbl_pos_detailed (id, product_id, product_unit_id, added_by, rate, quantity, product_cost_price, product_sale_price, product_discount, product_vat) VALUES (@id, @product_id, @product_unit_id, @added_by, @rate, @quantity, @product_cost_price, @product_sale_price, @product_discount, @product_vat)";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("@id", pointOfSaleDetailCUL.Id);//No incremental value in the database because there can be multiple goods with the same invoice id.
                cmd.Parameters.AddWithValue("@product_id", pointOfSaleDetailCUL.ProductId);
                cmd.Parameters.AddWithValue("@product_unit_id", pointOfSaleDetailCUL.ProductUnitId);
                cmd.Parameters.AddWithValue("@added_by", pointOfSaleDetailCUL.AddedBy);
                cmd.Parameters.AddWithValue("@rate", pointOfSaleDetailCUL.ProductRate);
                cmd.Parameters.AddWithValue("@quantity", pointOfSaleDetailCUL.ProductQuantity);
                cmd.Parameters.AddWithValue("@product_cost_price", pointOfSaleDetailCUL.ProductCostPrice);
                cmd.Parameters.AddWithValue("@product_sale_price", pointOfSaleDetailCUL.ProductSalePrice);
                cmd.Parameters.AddWithValue("@product_discount", pointOfSaleDetailCUL.ProductDiscount);
                cmd.Parameters.AddWithValue("@product_vat", pointOfSaleDetailCUL.ProductVAT);

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

        #region JOIN REPORT BY DATE METHOD
        public DataTable FetchReportByDate(string dateFrom, string dateTo)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                DataTable dtReport = new DataTable();

                String sqlQuery = "SELECT * FROM tbl_pos_detailed WHERE added_date >= '" + dateFrom + "' AND added_date <= '" + dateTo + "'";

                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    try
                    {
                        conn.Open();//Opening the database connection

                        using (SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd))
                        {
                            dataAdapter.Fill(dtReport);//Passing values from adapter to Data Table
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
                    return dtReport;
                }
            }
        }
        #endregion

        /*#region UPDATE METHOD
        public bool Update(PointOfSaleDetailCUL pointOfSaleDetailCUL)
        {
            bool isSuccess = false;Amount

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                string sqlQuery = "UPDATE tbl_pos_detailed SET sale_type=@sale_type, customer_id=@customer_id, sub_total=@sub_total, vat=@vat, discount=@discount, grand_total=@grand_total, added_date=@added_date, added_by=@added_by WHERE invoice_no=@invoice_no";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("@product_id", pointOfSaleDetailCUL.ProductId);
                cmd.Parameters.AddWithValue("@invoice_no", pointOfSaleDetailCUL.InvoiceNo);//Do you really need to update the invoice no?
                cmd.Parameters.AddWithValue("@added_date", pointOfSaleDetailCUL.AddedDate);
                cmd.Parameters.AddWithValue("@added_by", pointOfSaleDetailCUL.AddedBy);
                cmd.Parameters.AddWithValue("@rate", pointOfSaleDetailCUL.ProductRate);
                cmd.Parameters.AddWithValue("@quantity", pointOfSaleDetailCUL.ProductQuantity);
                cmd.Parameters.AddWithValue("@product_cost_price", pointOfSaleDetailCUL.ProductCostPrice);
                cmd.Parameters.AddWithValue("@product_sale_price", pointOfSaleDetailCUL.ProductSalePrice);
                cmd.Parameters.AddWithValue("@total_price", pointOfSaleDetailCUL.ProductTotalPrice);


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
        #endregion*/

        #region DELETE METHOD
        public bool Delete(int invoiceId)
        {
            //Create a Boolean variable and set its value to false.
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                //SQL Query to Delete from the Database
                string sqlQuery = "DELETE FROM tbl_pos_detailed WHERE id=@id";

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

        #region GETTING THE LAST ID OF THE TABLE IN THE DATABASE
        public int Search()
        {
            int lastInvoiceNo = 0;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                String sql = "SELECT * FROM tbl_pos_detailed WHERE id=IDENT_CURRENT('tbl_pos_details')";//SQL query to get the last id of rows in the table.

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    try
                    {
                        /*cmd.CommandType = CommandType.Text;
                        cmd.Connection = conn;*/
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
        #endregion

        #region GETTING THE ROW INFORMATION OF A TABLE BY USING INVOICE NO.
        public DataTable Search(int invoiceNo)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                DataTable dataTable = new DataTable();

                String sqlQuery = "SELECT * FROM tbl_pos_detailed WHERE id= " + invoiceNo + "";

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
