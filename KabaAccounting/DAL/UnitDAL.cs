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
    class UnitDAL
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
                string sqlQuery = "SELECT * FROM tbl_units";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                conn.Open();

                //Adding the value from dataAdapter into the dataTable.
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

        #region Getting Unit Infos By Id
        public DataTable GetUnitInfoById(int unitId)
        {
            SqlConnection conn = new SqlConnection(connString);//Static method to connect database
            DataTable dataTable = new DataTable();//To hold the data from database
            try
            {
                String sql = "SELECT * FROM tbl_units WHERE id=" + unitId + "";//SQL query to search data from database 
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
