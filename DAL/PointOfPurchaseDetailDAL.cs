﻿using CUL;
using CUL.Enums;
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
    public class PointOfPurchaseDetailDAL
    {
        static string connString = ConfigurationManager.ConnectionStrings["AccountingConnString"].ConnectionString;

        #region SELECT METHOD
        public DataTable Select()
        {
            SqlConnection conn = new SqlConnection(connString);

            DataTable dataTable = new DataTable();

            try
            {
                string sqlQuery = "SELECT * FROM tbl_pop_details";

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
        public bool Insert(PointOfPurchaseDetailCUL pointOfPurchaseDetailCUL)
        {
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                string sqlQuery = "INSERT INTO tbl_pop_detailed (id_pop, product_id, product_unit_id, added_by, rate, quantity, quantity_left_for_sale, product_gross_cost_price, product_cost_price, product_discount, product_vat) VALUES (@id_pop, @product_id, @product_unit_id, @added_by, @rate, @quantity, @quantity_left_for_sale, @product_gross_cost_price, @product_cost_price, @product_discount, @product_vat)";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("@id_pop", pointOfPurchaseDetailCUL.PopId);//No incremental value in the database because there can be multiple goods with the same invoice id.
                cmd.Parameters.AddWithValue("@product_id", pointOfPurchaseDetailCUL.ProductId);
                cmd.Parameters.AddWithValue("@product_unit_id", pointOfPurchaseDetailCUL.ProductUnitId);
                cmd.Parameters.AddWithValue("@added_by", pointOfPurchaseDetailCUL.AddedBy);
                cmd.Parameters.AddWithValue("@rate", pointOfPurchaseDetailCUL.ProductRate);
                cmd.Parameters.AddWithValue("@quantity", pointOfPurchaseDetailCUL.ProductQuantity);
                cmd.Parameters.AddWithValue("@quantity_left_for_sale", pointOfPurchaseDetailCUL.ProductQuantityLeftForSale);
                cmd.Parameters.AddWithValue("@product_gross_cost_price", pointOfPurchaseDetailCUL.ProductGrossCostPrice);
                cmd.Parameters.AddWithValue("@product_cost_price", pointOfPurchaseDetailCUL.ProductCostPrice);
                cmd.Parameters.AddWithValue("@product_discount", pointOfPurchaseDetailCUL.ProductDiscount);
                cmd.Parameters.AddWithValue("@product_vat", pointOfPurchaseDetailCUL.ProductVAT);

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

        /*#region UPDATE METHOD
        public bool Update(PointOfPurchaseDetailCUL pointOfPurchaseDetailCUL)
        {
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                string sqlQuery = "UPDATE tbl_pop_detailed SET sale_type=@sale_type, customer_id=@customer_id, sub_total=@sub_total, vat=@vat, discount=@discount, grand_total=@grand_total, added_date=@added_date, added_by=@added_by WHERE invoice_no=@invoice_no";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("@product_id", pointOfPurchaseDetailCUL.ProductId);
                cmd.Parameters.AddWithValue("@invoice_no", pointOfPurchaseDetailCUL.InvoiceNo);//Do you really need to update the invoice no?
                cmd.Parameters.AddWithValue("@added_date", pointOfPurchaseDetailCUL.AddedDate);
                cmd.Parameters.AddWithValue("@added_by", pointOfPurchaseDetailCUL.AddedBy);
                cmd.Parameters.AddWithValue("@rate", pointOfPurchaseDetailCUL.ProductRate);
                cmd.Parameters.AddWithValue("@quantity", pointOfPurchaseDetailCUL.ProductQuantity);
                cmd.Parameters.AddWithValue("@product_cost_price", pointOfPurchaseDetailCUL.ProductCostPrice);
                cmd.Parameters.AddWithValue("@product_sale_price", pointOfPurchaseDetailCUL.ProductSalePrice);
                cmd.Parameters.AddWithValue("@total_price", pointOfPurchaseDetailCUL.ProductTotalPrice);


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
                string sqlQuery = "DELETE FROM tbl_pop_detailed WHERE id_pop=@id_pop";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                //Passing the value using cmd
                cmd.Parameters.AddWithValue("@id_pop", invoiceId);

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
                string sql = "SELECT * FROM tbl_pop_detailed WHERE id=IDENT_CURRENT('tbl_pop_details')";//SQL query to get the last id of rows in the table.

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

        #region GETTING THE ROW INFORMATION OF A TABLE BY USING INVOICE ID.
        public DataTable Search(int invoiceId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                DataTable dataTable = new DataTable();

                string sqlQuery = "SELECT * FROM tbl_pop_detailed WHERE id_pop= " + invoiceId + "";//SQL query to get the last id of rows in te table.

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

        public DataTable GetProductLatestValidCostPriceById(int productId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                DataTable dtReport = new DataTable();

                string sqlQuery = "SELECT * FROM tbl_pop_detailed WHERE product_id=" + productId + " AND quantity_left_for_sale > 0 ORDER BY id";//Somehow, the enum variable (int)Numbers.InitialIndex does not work.

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

        public bool UpdateProductQuantityLeftForSaleById(int idPointOfPurchase, decimal productQuantityLeftForSale)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                bool isSuccess = false;

                try
                {
                    string sqlQuery = "UPDATE tbl_pop_detailed SET quantity_left_for_sale=@quantity_left_for_sale WHERE id='" + idPointOfPurchase + "'";//Somehow, the enum variable (int)Numbers.InitialIndex does not work.

                    using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("quantity_left_for_sale", productQuantityLeftForSale);

                        conn.Open();//Opening the database connection

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
        }
    }
}
