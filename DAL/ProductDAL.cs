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
    public class ProductDAL
    {
        static string connString = ConfigurationManager.ConnectionStrings["KabaAccountingConnString"].ConnectionString;

        #region Select All Data from Database | Search Product by Keyword (LIKE QUERY)
        public DataTable SelectAllOrByKeyword(string searchBy = null, string keyword = null)
        {
            SqlConnection conn = new SqlConnection(connString);//Static method to connect database
            DataTable dataTable = new DataTable();//To hold the data from database
            try
            {
                String sql;

                if (keyword==null && searchBy==null)
                    sql = "SELECT * FROM tbl_products";//SQL query to get data from database 
                else if(keyword!=null && searchBy == null)
                    sql = "SELECT * FROM tbl_products WHERE id LIKE '%" + keyword + "%' OR name LIKE '%" + keyword + "%' OR barcode_retail LIKE '%" + keyword + "%' OR barcode_wholesale LIKE '%" + keyword + "%'";//SQL query to search data from database 
                else if(keyword != null && searchBy != null)
                    sql = "SELECT * FROM tbl_products WHERE (id LIKE '%" + keyword + "%' OR name LIKE '%" + keyword + "%' OR barcode_retail LIKE '%" + keyword + "%' OR barcode_wholesale LIKE '%" + keyword + "%') AND category_id = '" + searchBy + "'";//SQL query to search data from database 
                else
                    sql = "SELECT * FROM tbl_products WHERE category_id = '" + keyword + "";//SQL query to search data from database 

                SqlCommand cmd = new SqlCommand(sql, conn);//For executing the command 
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);//Getting data from database           
                conn.Open();//Opening the database connection
                dataAdapter.Fill(dataTable);//Filling the data in our datatable
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
        public bool Insert(ProductCUL product)
        {
            bool isSuccess = false;
            SqlConnection conn = new SqlConnection(connString);
            try
            {
                String sqlQuery = "INSERT INTO tbl_products (name, category_id, description, rating, barcode_retail, barcode_wholesale, amount_in_unit, amount_in_stock, costprice, saleprice, unit_retail_id, unit_wholesale_id, added_date, added_by) VALUES (@name, @category_id, @description, @rating, @barcode_retail, @barcode_wholesale, @amount_in_unit, @amount_in_stock, @costprice, @saleprice, @unit_retail_id, @unit_wholesale_id, @added_date, @added_by)";
                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("@name", product.Name);
                cmd.Parameters.AddWithValue("@category_id", product.CategoryId);
                cmd.Parameters.AddWithValue("@description", product.Description);
                cmd.Parameters.AddWithValue("@rating", product.Rating);
                cmd.Parameters.AddWithValue("@barcode_retail", product.BarcodeRetail);
                cmd.Parameters.AddWithValue("@barcode_wholesale", product.BarcodeWholesale);
                cmd.Parameters.AddWithValue("@amount_in_unit", product.AmountInUnit);
                cmd.Parameters.AddWithValue("@amount_in_stock", product.AmountInStock);
                cmd.Parameters.AddWithValue("@costprice", product.CostPrice);
                cmd.Parameters.AddWithValue("@saleprice", product.SalePrice);
                cmd.Parameters.AddWithValue("@unit_retail_id", product.UnitRetail);
                cmd.Parameters.AddWithValue("@unit_wholesale_id", product.UnitWholesale);
                cmd.Parameters.AddWithValue("@added_date", product.AddedDate);
                cmd.Parameters.AddWithValue("@added_by", product.AddedBy);

                conn.Open();

                int rows = cmd.ExecuteNonQuery();

                //If the query is executed successfully, then the value of rows will be greater than 0. Otherwise, it will be less than 0.
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
        public bool Update(ProductCUL product)
        {
            bool isSuccess = false;
            SqlConnection conn = new SqlConnection(connString);

            try
            {
                string sql = "UPDATE tbl_products SET name=@name, category_id=@category_id, description=@description, rating=@rating, barcode_retail=@barcode_retail, barcode_wholesale=@barcode_wholesale, amount_in_unit=@amount_in_unit, costprice=@costprice, saleprice=@saleprice, unit_retail_id=@unit_retail_id, unit_wholesale_id=@unit_wholesale_id, added_date=@added_date, added_by=@added_by WHERE id=@id";
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@name", product.Name);
                cmd.Parameters.AddWithValue("@category_id", product.CategoryId);
                cmd.Parameters.AddWithValue("@description", product.Description);
                cmd.Parameters.AddWithValue("@rating", product.Rating);
                cmd.Parameters.AddWithValue("@barcode_retail", product.BarcodeRetail);
                cmd.Parameters.AddWithValue("@barcode_wholesale", product.BarcodeWholesale);
                cmd.Parameters.AddWithValue("@amount_in_unit", product.AmountInUnit);
                cmd.Parameters.AddWithValue("@costprice", product.CostPrice);
                cmd.Parameters.AddWithValue("@saleprice", product.SalePrice);
                cmd.Parameters.AddWithValue("@unit_retail_id", product.UnitRetail);
                cmd.Parameters.AddWithValue("@unit_wholesale_id", product.UnitWholesale);
                cmd.Parameters.AddWithValue("@added_date", product.AddedDate);
                cmd.Parameters.AddWithValue("@added_by", product.AddedBy);
                cmd.Parameters.AddWithValue("@id", product.Id); //Do you REALLY need to update an ID? You have already the ID in the query above.

                conn.Open();

                int rows = cmd.ExecuteNonQuery();

                //If the query is executed successfully, then the value of rows will be greater than 0. Otherwise, it will be less than 0.
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
        public bool Delete(ProductCUL product)
        {
            bool isSuccess = false;
            SqlConnection conn = new SqlConnection(connString);

            try
            {
                string sqlQuery = "DELETE FROM tbl_products WHERE id=@id";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("@id", product.Id);
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

        //YOU MAY WISH TO ERASE THIS CODE BLOCK!!!
        #region SEARCH BY ID METHOD
        public DataTable SearchProductByIdBarcode(string keyword)
        {
            SqlConnection conn = new SqlConnection(connString);//Static method to connect database
            DataTable dataTable = new DataTable();//To hold the data from database
            try
            {
                String sql = "SELECT * FROM tbl_products WHERE barcode_retail='" + keyword + "' OR barcode_wholesale='"+ keyword + "' OR id="+ keyword + "";//SQL query to search data from database 
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

        #region SEARCH DUPLICATIONS
        public DataTable SearchDuplications(string barcode)
        {
            SqlConnection conn = new SqlConnection(connString);//Static method to connect database
            DataTable dataTable = new DataTable();//To hold the data from database
            try
            {
                String sql = "SELECT * FROM tbl_products WHERE barcode_retail='" + barcode + "' OR barcode_wholesale='" + barcode + "'";//SQL query to search data from database 
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

        #region GETTING THE PRODUCT INFORMATIONS BY USING PRODUCT ID.
        public DataTable SearchById(string productId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                DataTable dataTable = new DataTable();

                String sqlQuery = "SELECT * FROM tbl_products WHERE id= " + productId + "";

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

        #region UPDATE AMOUNT IN STOCK METHOD
        public bool UpdateAmountInStock(ProductCUL product)
        {
            bool isSuccessProductAmount = false;
            SqlConnection conn = new SqlConnection(connString);

            try
            {
                string sql = "UPDATE tbl_products SET amount_in_stock=@amount_in_stock WHERE id=@id";
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@amount_in_stock", product.AmountInStock);
                cmd.Parameters.AddWithValue("@id", product.Id); //Do you REALLY need to update an ID? You have already the ID in the query above.

                conn.Open();

                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    isSuccessProductAmount = true;
                }
                else
                {
                    isSuccessProductAmount = false;
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

            return isSuccessProductAmount;
        }
        #endregion
    }
}
