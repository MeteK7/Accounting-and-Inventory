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
    class BankDAL
    {
        static string connString = ConfigurationManager.ConnectionStrings["KabaAccountingConnString"].ConnectionString;


        #region SELECT METHOD
        public DataTable Select()
        {
            SqlConnection conn = new SqlConnection(connString);

            DataTable dataTable = new DataTable();

            try
            {
                string sqlQuery = "SELECT * FROM tbl_banks";

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
        public bool Insert(BankBLL bankBLL)
        {
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                string sqlQuery = "INSERT INTO tbl_banks (name, added_date, added_by) VALUES (@name, @added_date, @added_by)";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("name", bankBLL.Name);
                cmd.Parameters.AddWithValue("added_date", bankBLL.AddedDate);
                cmd.Parameters.AddWithValue("added_by", bankBLL.AddedBy);

                conn.Open();

                int rows = cmd.ExecuteNonQuery();

                if (rows>0)
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
        public bool Update(BankBLL bankBLL)
        {
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                string sqlQuery = "UPDATE tbl_banks SET name=@name, added_date=@added_date, added_by=@added_by WHERE id=@id";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("name", bankBLL.Name);
                cmd.Parameters.AddWithValue("added_date", bankBLL.AddedDate);
                cmd.Parameters.AddWithValue("added_by", bankBLL.AddedBy);

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
        public bool Delete(BankBLL bankBLL)
        {
            //Create a Boolean variable and set its value to false.
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                //SQL Query to Delete from the Database
                string sqlQuery = "DELETE FROM tbl_banks WHERE id=@id";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                //Passing the value using cmd
                cmd.Parameters.AddWithValue("@id", bankBLL.Id);

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

        #region SEARCH METHOD
        public DataTable Search(string keyword)
        {
            SqlConnection conn = new SqlConnection(connString);//Static method to connect database

            DataTable dataTable = new DataTable();//To hold the data from database
            try
            {
                String sql = "SELECT * FROM tbl_banks WHERE id LIKE '%" + keyword + "%' OR name LIKE '%" + keyword + "%'";//SQL query to search data from database 
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
