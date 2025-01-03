﻿using CUL;
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
    public class PointOfPurchaseDAL
    {
        static string connString = ConfigurationManager.ConnectionStrings["AccountingConnString"].ConnectionString;


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
                string sqlQuery = "INSERT INTO tbl_pop (id, invoice_no, payment_type_id, supplier_id, total_product_quantity, gross_cost_total, discount, sub_total, vat, grand_total, asset_id, added_date, added_by) VALUES (@id, @invoice_no, @payment_type_id, @supplier_id, @total_product_quantity, @gross_cost_total, @discount, @sub_total, @vat, @grand_total, @asset_id, @added_date, @added_by)";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("@id", pointOfPurchaseCUL.Id);//The column id in the database is not auto incremental. This is to prevent the number from increasing when the user deletes an existing invoice and creates a new invoice.
                cmd.Parameters.AddWithValue("@invoice_no", pointOfPurchaseCUL.InvoiceNo);
                cmd.Parameters.AddWithValue("@payment_type_id", pointOfPurchaseCUL.PaymentTypeId);
                cmd.Parameters.AddWithValue("@supplier_id", pointOfPurchaseCUL.SupplierId);
                cmd.Parameters.AddWithValue("@total_product_quantity", pointOfPurchaseCUL.TotalProductQuantity);
                cmd.Parameters.AddWithValue("@gross_cost_total", pointOfPurchaseCUL.GrossCostTotal);
                cmd.Parameters.AddWithValue("@discount", pointOfPurchaseCUL.Discount);
                cmd.Parameters.AddWithValue("@sub_total", pointOfPurchaseCUL.SubTotal);
                cmd.Parameters.AddWithValue("@vat", pointOfPurchaseCUL.Vat);
                cmd.Parameters.AddWithValue("@grand_total", pointOfPurchaseCUL.GrandTotal);
                cmd.Parameters.AddWithValue("@asset_id", pointOfPurchaseCUL.AssetId);
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
        public bool Update(PointOfPurchaseCUL pointOfPurchaseCUL)
        {
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                string sqlQuery = "UPDATE tbl_pop SET payment_type_id=@payment_type_id, supplier_id=@supplier_id, total_product_quantity=@total_product_quantity, gross_cost_total=@gross_cost_total, discount=@discount, sub_total=@sub_total, vat=@vat, grand_total=@grand_total, asset_id=@asset_id, added_date=@added_date, added_by=@added_by WHERE id=@id";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("payment_type_id", pointOfPurchaseCUL.PaymentTypeId);
                cmd.Parameters.AddWithValue("supplier_id", pointOfPurchaseCUL.SupplierId);
                cmd.Parameters.AddWithValue("total_product_quantity", pointOfPurchaseCUL.TotalProductQuantity);
                cmd.Parameters.AddWithValue("gross_cost_total", pointOfPurchaseCUL.GrossCostTotal);
                cmd.Parameters.AddWithValue("discount", pointOfPurchaseCUL.Discount);
                cmd.Parameters.AddWithValue("sub_total", pointOfPurchaseCUL.SubTotal);
                cmd.Parameters.AddWithValue("vat", pointOfPurchaseCUL.Vat);
                cmd.Parameters.AddWithValue("grand_total", pointOfPurchaseCUL.GrandTotal);
                cmd.Parameters.AddWithValue("asset_id", pointOfPurchaseCUL.AssetId);
                cmd.Parameters.AddWithValue("added_date", pointOfPurchaseCUL.AddedDate);
                cmd.Parameters.AddWithValue("added_by", pointOfPurchaseCUL.AddedBy);
                cmd.Parameters.AddWithValue("invoice_no", pointOfPurchaseCUL.InvoiceNo);
                cmd.Parameters.AddWithValue("id", pointOfPurchaseCUL.Id);//Do you really need to update the ID?


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
                string sqlQuery = "DELETE FROM tbl_pop WHERE id=@id";

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
        
        #region GETTING ANY OR THE LAST ID AND ROW DATAS OF THE TABLE IN THE DATABASE
        public DataTable GetByIdOrLastId(int invoiceId = 0)//Optional parameter
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                DataTable dataTable = new DataTable();
                String sql;

                if (invoiceId == 0)//If the invoice number is 0 which means user did not send any argument, then get the last Id using the following query.
                {
                    sql = "SELECT * FROM tbl_pop WHERE id=(SELECT max(id) FROM tbl_pop)";
                    //sql = "SELECT * FROM tbl_pop WHERE id=IDENT_CURRENT('tbl_pop')";//SQL query to get the last id of rows in the table.
                }

                else
                {
                    sql = "SELECT * FROM tbl_pop WHERE id=" + invoiceId + "";
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

        #region GETTING PURCHASES BY PRODUCT ID
        public static DataTable GetPurchasesByProductId(int productId)
        {
            SqlConnection conn = new SqlConnection(connString);
            DataTable dataTable = new DataTable();

            try
            {
                string sqlQuery = @"
            SELECT pd.*, p.added_date
            FROM tbl_pop_detailed pd
            INNER JOIN tbl_pop p ON pd.id_pop = p.id
            WHERE pd.product_id = @productId
            ORDER BY p.added_date ASC";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                cmd.Parameters.AddWithValue("@productId", productId);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                conn.Open();

                dataAdapter.Fill(dataTable);
            }
            catch (Exception ex)
            {
                // Handle exception
            }
            finally
            {
                conn.Close();
            }

            return dataTable;
        }

        #endregion

        #region COUNT BY DAY METHOD
        public int CountPaymentTypeByToday(string dateFrom, string dateTo, bool cashOrCredit) //YOU HAVE THE SAME FUNCTION IN POSDAL!!!!
        {
            SqlConnection conn = new SqlConnection(connString);

            int counter = 0;
            string sqlQuery;

            try
            {
                if (cashOrCredit == true)//Get the cash purchases for today if the cashOrCredit boolean variable is true
                {
                    sqlQuery = "Select COUNT(*) FROM tbl_pop WHERE payment_type_id=1 AND added_date >= '" + dateFrom + "' AND added_date<= '" + dateTo + "'";//This query counts the records from the beginning of the day to the rest of the day.

                }
                else//Get the credit purchases for today if the cashOrCredit boolean variable is false
                {
                    sqlQuery = "Select COUNT(*) FROM tbl_pop WHERE payment_type_id=2 AND added_date >= '" + dateFrom + "' AND added_date<= '" + dateTo + "'";//This query counts the records from the beginning of the day to the rest of the day.
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

        #region JOIN RPORT BY DATE METHOD
        public DataTable FetchReportByDate(string dateFrom, string dateTo) //YOU HAVE THE SAME FUNCTION IN POSDAL!!!!
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                DataTable dtReport = new DataTable();

                String sqlQuery = "SELECT * FROM tbl_pop WHERE added_date >= '" + dateFrom + "' AND added_date <= '" + dateTo + "'";

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

        #region TOTAL PURCHASES BY USER BETWEEN TWO DATES
        public DataTable SumAmountByUserBetweenDates(string dateFrom, string dateTo) //YOU HAVE THE SAME FUNCTION IN POSDAL!!!!
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                DataTable dataTable = new DataTable();

                String sqlQuery = "Select added_by, SUM(grand_total) AS grand_total FROM tbl_pop WHERE added_date >= '" + dateFrom + "' AND added_date <= '" + dateTo + "' GROUP BY added_by";

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

        #region JOIN REPORT BY DATE METHOD
        public DataTable JoinReportByDate(string dateFrom, string dateTo)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                DataTable dtReport = new DataTable();

                String sqlQuery = "SELECT * FROM tbl_pop FULL OUTER JOIN tbl_pop_detailed ON tbl_pop_detailed.id=tbl_pop.id WHERE added_date >= '" + dateFrom + "' AND added_date <= '" + dateTo + "'";

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
    }
}
