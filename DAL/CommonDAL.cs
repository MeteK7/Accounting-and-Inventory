using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class CommonDAL
    {
        static string connString = ConfigurationManager.ConnectionStrings["KabaAccountingConnString"].ConnectionString;

        #region GETTING ANY OR THE LAST ID AND ROW DATAS OF THE TABLE IN THE DATABASE
        public DataTable GetByIdOrLastId(string calledBy, int id = 0)//Optional parameter
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                DataTable dataTable = new DataTable();
                String sql;

                if (id == 0)//If the id is 0 which means user did not send any argument, then get the last Id using the following query.
                {
                    sql = "SELECT * FROM "+calledBy+ " WHERE id=(SELECT max(id) FROM " + calledBy + ")";
                    //sql = "SELECT * FROM tbl_pos WHERE id=IDENT_CURRENT('tbl_pos')";//SQL query to get the last id of rows in the table.
                }

                else
                {
                    sql = "SELECT * FROM " + calledBy + " WHERE id=" + id + "";
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
    }
}
