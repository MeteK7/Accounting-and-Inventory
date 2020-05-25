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
    class ProductDAL
    {
        static string connString = ConfigurationManager.ConnectionStrings["KabaAccountingConnString"].ConnectionString;

        #region Select Data from Database
        public DataTable Select()
        {
            SqlConnection conn = new SqlConnection(connString);//Static method to connect database
            DataTable dataTable = new DataTable();//To hold the data from database
            try
            {
                String sql = "SELECT * FROM tbl_products";//SQL query to get data from database 
                SqlCommand cmd = new SqlCommand(sql, conn);//For executing the command 
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);//Getting data from database           
                conn.Open();//Opening the database connection
                dataAdapter.Fill(dataTable);//Filling the data in our datatable
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

        #region Insert Data in Database
        public bool Insert(ProductBLL product)
        {
            bool isSuccess = false;
            SqlConnection conn = new SqlConnection(connString);
            try
            {
                String sqlQuery = "INSERT INTO tbl_products (name, category, description, rating, barcode_retail, barcode_wholesale, amount, costprice, saleprice, unit_retail, unit_wholesale, added_date, added_by) VALUES (@name, @category, @description, @rating, @barcode_retail, @barcode_wholesale, @amount, @costprice, @saleprice, @unit_retail, @unit_wholesale, @added_date, @added_by)";
                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("@name", product.Name);
                cmd.Parameters.AddWithValue("@category", product.Category);
                cmd.Parameters.AddWithValue("@description", product.Description);
                cmd.Parameters.AddWithValue("@rating", product.Rating);
                cmd.Parameters.AddWithValue("@barcode_retail", product.BarcodeRetail);
                cmd.Parameters.AddWithValue("@barcode_wholesale", product.BarcodeWholesale);
                cmd.Parameters.AddWithValue("@amount", product.Amount);
                cmd.Parameters.AddWithValue("@costprice", product.CostPrice);
                cmd.Parameters.AddWithValue("@saleprice", product.SalePrice);
                cmd.Parameters.AddWithValue("@unit_retail", product.UnitRetail);
                cmd.Parameters.AddWithValue("@unit_wholesale", product.UnitWholesale);
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
                MessageBox.Show(ex.Message);
            }

            finally
            {
                conn.Close();
            }

            return isSuccess;
        }
        #endregion

        #region Update data in Database
        public bool Update(ProductBLL product)
        {
            bool isSuccess = false;
            SqlConnection conn = new SqlConnection(connString);

            try
            {
                string sql = "UPDATE tbl_products SET name=@name, category=@category, description=@description, rating=@rating, barcode_retail=@barcode_retail, barcode_wholesale=@barcode_wholesale, amount=@amount, costprice=@costprice, saleprice=@saleprice, unit_retail=@unit_retail, unit_wholesale=@unit_wholesale, added_date=@added_date, added_by=@added_by WHERE id=@id";
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@name", product.Name);
                cmd.Parameters.AddWithValue("@category", product.Category);
                cmd.Parameters.AddWithValue("@description", product.Description);
                cmd.Parameters.AddWithValue("@rating", product.Rating);
                cmd.Parameters.AddWithValue("@barcode_retail", product.BarcodeRetail);
                cmd.Parameters.AddWithValue("@barcode_wholesale", product.BarcodeWholesale);
                cmd.Parameters.AddWithValue("@amount", product.Amount);
                cmd.Parameters.AddWithValue("@costprice", product.CostPrice);
                cmd.Parameters.AddWithValue("@saleprice", product.SalePrice);
                cmd.Parameters.AddWithValue("@unit_retail", product.UnitRetail);
                cmd.Parameters.AddWithValue("@unit_wholesale", product.UnitWholesale);
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
                MessageBox.Show(ex.Message);
            }

            finally
            {
                conn.Close();
            }

            return isSuccess;
        }
        #endregion

        #region Delete Data from Database
        public bool Delete(ProductBLL product)
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
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return isSuccess;
        }


        #endregion

        #region Search Product on Database using Keywords
        public DataTable Search(string keyword)
        {
            SqlConnection conn = new SqlConnection(connString);//Static method to connect database
            DataTable dataTable = new DataTable();//To hold the data from database
            try
            {
                String sql = "SELECT * FROM tbl_products WHERE id LIKE '%" + keyword + "%' OR name LIKE '%" + keyword + "%' OR barcode_retail LIKE '%" + keyword + "%' OR barcode_wholesale LIKE '%" + keyword + "%'";//SQL query to search data from database 
                SqlCommand cmd = new SqlCommand(sql, conn);//For executing the command 
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);//Getting data from database           
                conn.Open();//Opening the database connection
                dataAdapter.Fill(dataTable);//Passing values from adapter to Data Table
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

        #region Search Product Id
        public DataTable SearchProductId(string keyword)
        {
            SqlConnection conn = new SqlConnection(connString);//Static method to connect database
            DataTable dataTable = new DataTable();//To hold the data from database
            try
            {
                String sql = "SELECT * FROM tbl_products WHERE barcode_retail='" + keyword + "' OR barcode_wholesale='"+ keyword + "' OR id="+ Convert.ToInt32(keyword) + "";//SQL query to search data from database 
                SqlCommand cmd = new SqlCommand(sql, conn);//For executing the command 
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);//Getting data from database           
                conn.Open();//Opening the database connection
                dataAdapter.Fill(dataTable);//Passing values from adapter to Data Table
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
    }
}
