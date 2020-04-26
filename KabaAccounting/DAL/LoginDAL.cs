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
    class LoginDAL
    {
        //Static string to connect to the Database

        static string connString = ConfigurationManager.ConnectionStrings["KabaAccountingConnString"].ConnectionString;

        public bool CheckLogin(LoginBLL login)
        {
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                //SQL query to check login
                string sqlQuery = "SELECT * FROM tbl_users WHERE username=@username AND password=@password AND user_type=@user_type";

                //Creating SQL command to pass the value
                SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                cmd.Parameters.AddWithValue("@username", login.Username);
                cmd.Parameters.AddWithValue("@password", login.Password);
                cmd.Parameters.AddWithValue("@user_type", login.UserType);

                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();

                dataAdapter.Fill(dataTable);

                if (dataTable.Rows.Count>0)
                {
                    //Login Successful
                    isSuccess = true;
                }
                else
                {
                    //Login Failed
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
    }
}
