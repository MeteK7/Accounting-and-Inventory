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
    public class CustomerDAL
    {
        //Static string method for Database Connnection String
        static string connString = ConfigurationManager.ConnectionStrings["KabaAccountingConnString"].ConnectionString;

        #region Select Method
        public DataTable Select()
        {
            //Creating database connection
            SqlConnection conn = new SqlConnection(connString);

            DataTable dataTable = new DataTable();

            try
            {
                //Writing SQL Query to get all the datas from database
                string sqlQuery = "SELECT * FROM tbl_customer";

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

        #region Insert Method
        public bool Insert(CustomerCUL customerCUL)
        {
            //Creating a boolean variable and set its default value to false
            bool isSuccess = false;

            //Connecting to the Database
            SqlConnection conn = new SqlConnection(connString);

            try
            {
                //Writing SQL Query to insert new customer to the database
                string sqlQuery = "INSERT INTO tbl_customer (name, email, contact, address, added_date, added_by) VALUES (@name, @email, @contact, @address, @added_date, @added_by)";

                //Creating SQL Command to pass values in our query.
                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                //Passing Values through parameter
                cmd.Parameters.AddWithValue("@name", customerCUL.Name);
                cmd.Parameters.AddWithValue("@email", customerCUL.Email);
                cmd.Parameters.AddWithValue("@contact", customerCUL.Contact);
                cmd.Parameters.AddWithValue("@address", customerCUL.Address);
                cmd.Parameters.AddWithValue("@added_date", customerCUL.AddedDate);
                cmd.Parameters.AddWithValue("@added_by", customerCUL.AddedBy);

                //Opening database connection
                conn.Open();

                //Creating the int variable to execute query
                int rows = cmd.ExecuteNonQuery();

                //If the query is executed successfully the its value will be greater than 0, else it will be less than 0.
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
                //Closing the database connection
                conn.Close();
            }
            return isSuccess;
        }
        #endregion

        #region Update Method
        public bool Update(CustomerCUL customerCUL)
        {
            //Creating boolean variable and set its default value to false
            bool isSuccess = false;

            //Creating SQL Connection
            SqlConnection conn = new SqlConnection(connString);

            try
            {
                //Query to Update Category
                string sqlQuery = "UPDATE tbl_customer SET name=@name, email=@email, contact=@contact, address=@address, added_date=@added_date, added_by=@added_by WHERE id=@id";

                //SQL Command to pass the value on SQL query
                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                //Passing Values through parameter
                cmd.Parameters.AddWithValue("name", customerCUL.Name);
                cmd.Parameters.AddWithValue("email", customerCUL.Email);
                cmd.Parameters.AddWithValue("contact", customerCUL.Contact);
                cmd.Parameters.AddWithValue("address", customerCUL.Address);
                cmd.Parameters.AddWithValue("added_date", customerCUL.AddedDate);
                cmd.Parameters.AddWithValue("added_by", customerCUL.AddedBy);
                cmd.Parameters.AddWithValue("id", customerCUL.Id);

                //Opening the database Connection
                conn.Open();

                //Creating int variable to execute query
                int rows = cmd.ExecuteNonQuery();

                //if the query is successfully executed then the value will be greater than 0.
                if (rows > 0)
                {
                    isSuccess = true;
                }
                else//DO YOU REALLY NEED THE ELSE STATEMENT ?????
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

        #region Delete Category Method
        public bool Delete(CustomerCUL customerCUL)
        {
            //Create a Boolean variable and set its value to false.
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                //SQL Query to Delete from the Database
                string sqlQuery = "DELETE FROM tbl_customer WHERE id=@id";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                //Passing the value using cmd
                cmd.Parameters.AddWithValue("@id", customerCUL.Id);

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

        #region Method for Search Functionality
        public DataTable Search(string keyword)
        {
            SqlConnection conn = new SqlConnection(connString);//Static method to connect database

            DataTable dataTable = new DataTable();//To hold the data from database
            try
            {
                String sql = "SELECT * FROM tbl_customer WHERE id LIKE '%" + keyword + "%' OR name LIKE '%" + keyword + "%' OR email LIKE '%" + keyword + "%' OR contact LIKE '%" + keyword + "%'";//SQL query to search data from database 
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
    }
}
