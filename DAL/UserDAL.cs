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
    public class UserDAL
    {
        static string connString = ConfigurationManager.ConnectionStrings["KabaAccountingConnString"].ConnectionString;

        #region Select Data from Database
        public DataTable Select()
        {
            SqlConnection conn = new SqlConnection(connString);//Static method to connect database
            DataTable dataTable = new DataTable();//To hold the data from database
            try
            {  
                String sql = "SELECT * FROM tbl_users";//SQL query to get data from database 
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

        #region Insert Data in Database
        public bool Insert(UserCUL usr)
        {
            bool isSuccess = false;
            SqlConnection conn = new SqlConnection(connString);
            try
            {
                String sqlQuery = "INSERT INTO tbl_users (first_name, last_name, email, username, password, contact, address, gender, user_type, added_date, added_by) VALUES (@first_name, @last_name, @email, @username, @password, @contact, @address, @gender, @user_type, @added_date, @added_by)";
                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("@first_name", usr.FirstName);
                cmd.Parameters.AddWithValue("@last_name", usr.LastName);
                cmd.Parameters.AddWithValue("@email", usr.Email);
                cmd.Parameters.AddWithValue("@username", usr.UserName);
                cmd.Parameters.AddWithValue("@password", usr.Password);
                cmd.Parameters.AddWithValue("@contact", usr.Contact);
                cmd.Parameters.AddWithValue("@address", usr.Address);
                cmd.Parameters.AddWithValue("@gender", usr.Gender);
                cmd.Parameters.AddWithValue("@user_type", usr.UserType);
                cmd.Parameters.AddWithValue("@added_date", usr.AddedDate);
                cmd.Parameters.AddWithValue("@added_by", usr.AddedBy);

                conn.Open();

                int rows = cmd.ExecuteNonQuery();

                //If the query is executed successfully, then the value of rows will be greater than 0. Otherwise, it will be less than 0.
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
                //MessageBox.Show(ex.Message);
            }

            finally
            {
                conn.Close();
            }

            return isSuccess;
        }
        #endregion

        #region Update data in Database
        public bool Update(UserCUL usr)
        {
            bool isSuccess = false;
            SqlConnection conn = new SqlConnection(connString);

            try
            {
                string sql = "UPDATE tbl_users SET first_name=@first_name, last_name=@last_name, email=@email, username=@username, password=@password, contact=@contact, address=@address, gender=@gender, user_type=@user_type, added_date=@added_date, added_by=@added_by WHERE id=@id";
                SqlCommand cmd = new SqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@first_name", usr.FirstName);
                cmd.Parameters.AddWithValue("@last_name", usr.LastName);
                cmd.Parameters.AddWithValue("@email", usr.Email);
                cmd.Parameters.AddWithValue("@username", usr.UserName);
                cmd.Parameters.AddWithValue("@password", usr.Password);
                cmd.Parameters.AddWithValue("@contact", usr.Contact);
                cmd.Parameters.AddWithValue("@address", usr.Address);
                cmd.Parameters.AddWithValue("@gender", usr.Gender);
                cmd.Parameters.AddWithValue("@user_type", usr.UserType);
                cmd.Parameters.AddWithValue("@added_date", usr.AddedDate);
                cmd.Parameters.AddWithValue("@added_by", usr.AddedBy);
                cmd.Parameters.AddWithValue("@id", usr.Id); //Do you REALLY need to update an ID?

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

        #region Delete Data from Database
        public bool Delete(UserCUL usr)
        {
            bool isSuccess = false;
            SqlConnection conn = new SqlConnection(connString);

            try
            {
                string sqlQuery = "DELETE FROM tbl_users WHERE id=@id";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("@id", usr.Id);
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
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return isSuccess;
        }


        #endregion

        #region Search User on Database using Keywords
        public DataTable Search(string keyword)
        {
            SqlConnection conn = new SqlConnection(connString);//Static method to connect database
            DataTable dataTable = new DataTable();//To hold the data from database
            try
            {
                String sql = "SELECT * FROM tbl_users WHERE id LIKE '%"+keyword+ "%' OR first_name LIKE '%" + keyword + "%' OR last_name LIKE '%" + keyword + "%' OR username LIKE '%" + keyword + "%'";//SQL query to search data from database 
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

        #region Getting User ID from Username
        public UserCUL GetIdFromUsername(string username)
        {
            UserCUL usrCUL = new UserCUL();
            SqlConnection conn = new SqlConnection(connString);
            DataTable dataTable = new DataTable();

            try
            {
                string sqlQuery = "SELECT id FROM tbl_users WHERE username='" + username + "'";

                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlQuery, conn);
                conn.Open();

                dataAdapter.Fill(dataTable);
                if (dataTable.Rows.Count>0)
                {
                    usrCUL.Id = int.Parse(dataTable.Rows[0]["id"].ToString());//TRY to understand this line of code!!!!
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
            return usrCUL;
        }
        #endregion

        #region Getting User Infos from User Id
        public DataTable GetUserInfoById(int userId)
        {
            UserCUL usrCUL = new UserCUL();
            SqlConnection conn = new SqlConnection(connString);
            DataTable dataTable = new DataTable();

            try
            {
                string sqlQuery = "SELECT * FROM tbl_users WHERE id='" + userId + "'";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);//For executing the command 
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
