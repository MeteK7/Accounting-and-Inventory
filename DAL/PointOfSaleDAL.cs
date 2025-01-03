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
    public static class PointOfSaleDAL
    {
        static string connString = ConfigurationManager.ConnectionStrings["AccountingConnString"].ConnectionString;


        #region SELECT METHOD
        public static DataTable Select()
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
        public static bool Insert(PointOfSaleCUL pointOfSaleCUL)
        {
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                string sqlQuery = "INSERT INTO tbl_pos (payment_type_id, customer_id, asset_id, discount, vat, added_date, added_by) VALUES (@payment_type_id, @customer_id, @asset_id, @discount, @vat, @added_date, @added_by)";
                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("@payment_type_id", pointOfSaleCUL.PaymentTypeId);
                cmd.Parameters.AddWithValue("@customer_id", pointOfSaleCUL.CustomerId);
                cmd.Parameters.AddWithValue("@asset_id", pointOfSaleCUL.AssetId);
                cmd.Parameters.AddWithValue("@discount", pointOfSaleCUL.Discount);
                cmd.Parameters.AddWithValue("@vat", pointOfSaleCUL.Vat);
                cmd.Parameters.AddWithValue("@added_date", pointOfSaleCUL.AddedDate);
                cmd.Parameters.AddWithValue("@added_by", pointOfSaleCUL.AddedBy);

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
        public static bool Update(PointOfSaleCUL pointOfSaleCUL)
        {
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                string sqlQuery = "UPDATE tbl_pos SET payment_type_id=@payment_type_id, customer_id=@customer_id, asset_id=@asset_id, discount=@discount, vat=@vat, added_date=@added_date, added_by=@added_by WHERE id=@id";
                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("@payment_type_id", pointOfSaleCUL.PaymentTypeId);
                cmd.Parameters.AddWithValue("@customer_id", pointOfSaleCUL.CustomerId);
                cmd.Parameters.AddWithValue("@asset_id", pointOfSaleCUL.AssetId);
                cmd.Parameters.AddWithValue("@discount", pointOfSaleCUL.Discount);
                cmd.Parameters.AddWithValue("@vat", pointOfSaleCUL.Vat);
                cmd.Parameters.AddWithValue("@added_date", pointOfSaleCUL.AddedDate);
                cmd.Parameters.AddWithValue("@added_by", pointOfSaleCUL.AddedBy);
                cmd.Parameters.AddWithValue("@id", pointOfSaleCUL.Id);//Do you really need to update the ID?


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
        public static bool Delete(int invoiceId)
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

        #region FETCH BY TODAY METHOD
        public static DataTable FetchReportByToday()
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

        #region JOIN PRODUCT REPORT BY DATE METHOD
        public static DataTable JoinProductReportByDate(string dateFrom, string dateTo)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                DataTable dtReport = new DataTable();

                String sqlQuery = "SELECT * FROM tbl_pos FULL OUTER JOIN tbl_pos_detailed ON tbl_pos_detailed.id=tbl_pos.id WHERE added_date >= '" + dateFrom + "' AND added_date <= '" + dateTo + "'";

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

        #region TOTAL SALES BY USER BETWEEN TWO DATES
        public static DataTable SumAmountByUserBetweenDates(string dateFrom, string dateTo)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                DataTable dataTable = new DataTable();

                String sqlQuery = "Select A.added_by, SUM(quantity*product_sale_price) AS grand_total FROM tbl_pos FULL OUTER JOIN tbl_pos_detailed A ON A.id=tbl_pos.id WHERE added_date >= '" + dateFrom + "' AND added_date <= '" + dateTo + "' GROUP BY A.added_by";
                //String sqlQuery = "Select added_by, SUM(grand_total) AS grand_total FROM tbl_pos WHERE added_date >= '" + dateFrom + "' AND added_date <= '" + dateTo + "' GROUP BY added_by";

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

        #region FETCH REPORT BY DATE METHOD
        public static DataTable FetchReportByDate(string dateFrom, string dateTo)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                DataTable dtReport = new DataTable();

                String sqlQuery = "SELECT * FROM tbl_pos WHERE added_date >= '" + dateFrom + "' AND added_date <= '" + dateTo + "'";

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

        #region COUNT BY DAY METHOD
        public static int CountPaymentTypeByToday(string dateFrom, string dateTo, bool cashOrCredit)
        {
            SqlConnection conn = new SqlConnection(connString);

            int counter = 0;
            string sqlQuery;

            try
            {
                if (cashOrCredit == true)//Get the cash sales for today if the cashOrCredit boolean variable is true
                {
                    sqlQuery = "Select COUNT(*) FROM tbl_pos WHERE payment_type_id=1 AND added_date >= '" + dateFrom + "' AND added_date<= '" + dateTo + "'";//This query counts the records from the beginning of the day to the rest of the day.

                }
                else//Get the credit sales for today if the cashOrCredit boolean variable is false
                {
                    sqlQuery = "Select COUNT(*) FROM tbl_pos WHERE payment_type_id=2 AND added_date >= '" + dateFrom + "' AND added_date<= '" + dateTo + "'";//This query counts the records from the beginning of the day to the rest of the day.
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

        #region GETTING ANY OR THE LAST ID AND ROW DATAS OF THE TABLE IN THE DATABASE
        public static DataTable GetByIdOrLastId(int invoiceId=0)//Optional parameter
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

        #region GETTING SALES BY PRODUCT ID BEFORE DATE
        public static DataTable GetSalesByProductIdBeforeDate(int productId, DateTime targetDate)
        {
            SqlConnection conn = new SqlConnection(connString);
            DataTable dataTable = new DataTable();

            try
            {
                string sqlQuery = @"
            SELECT pd.*, p.added_date 
            FROM tbl_pos_detailed pd
            INNER JOIN tbl_pos p ON pd.id_pos = p.id
            WHERE pd.product_id = @productId AND p.added_date < @targetDate
            ORDER BY p.added_date ASC";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                cmd.Parameters.AddWithValue("@productId", productId);
                cmd.Parameters.AddWithValue("@targetDate", targetDate);
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

        #region CHECK RECORD EXISTANCE
        public static DataTable CheckReportExistance()
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