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
    public class UnitDAL
    {
        UnitCUL unitCUL = new UnitCUL();
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
                //MessageBox.Show(ex.Message);
            }
            finally
            {

                conn.Close();
            }
            return dataTable;
        }
        #endregion

        #region PRODUCT UNIT INFO FETCHING SECTION
        public DataTable GetUnitInfoById(int unitId)
        {
            SqlConnection conn = new SqlConnection(connString);//Static method to connect database
            DataTable dtUnitInfo = new DataTable();//To hold the data from database
            try
            {
                String sql = "SELECT * FROM tbl_units WHERE id=" + unitId + "";//SQL query to search data from database 
                SqlCommand cmd = new SqlCommand(sql, conn);//For executing the command 
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);//Getting data from database           
                conn.Open();//Opening the database connection
                dataAdapter.Fill(dtUnitInfo);//Passing values from adapter to Data Table
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return dtUnitInfo;
        }
        #endregion

        #region PRODUCT UNIT IDS FETCHING SECTION
        public DataTable GetProductUnitId(List<int> listOfIds)
        {
            SqlConnection conn = new SqlConnection(connString);//Static method to connect database

            using (conn)
            {
                var ids = string.Join(",", listOfIds);
                List<UnitCUL> listUnit = new List<UnitCUL>();
                DataTable dtUnitInfo = new DataTable();//To hold the data from database
                dtUnitInfo.Columns.Add("id", typeof(int));
                dtUnitInfo.Columns.Add("name", typeof(string));

                conn.Open();//Opening the database connection
                String sql = "SELECT * FROM tbl_units WHERE id IN (" + ids + ")";//SQL query to search data from database 
                SqlCommand cmd = new SqlCommand(sql, conn);//For executing the command 

                using (SqlDataReader dataReader = cmd.ExecuteReader())
                {
                    if(dataReader==null)
                        throw new NullReferenceException("No Unit Available.");

                    DataRow row = dtUnitInfo.NewRow();
                    UnitCUL unit = new UnitCUL();

                    while (dataReader.Read())
                    {
                        unit.Id = Convert.ToInt32(dataReader["id"]);
                        unit.Name = dataReader["name"].ToString();
                        
                        listUnit.Add(unit);
                    }

                    row[0] = listUnit[0];
                    row[1] = listUnit[1];
                    dtUnitInfo.Rows.Add(row);
                }
                conn.Close();

                return dtUnitInfo;
            }
        }
        #endregion

        #region GETTING THE UNIT INFORMATIONS BY USING UNIT NAME.
        public DataTable GetUnitInfoByName(string unitName)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                DataTable dataTable = new DataTable();

                String sqlQuery = "SELECT * FROM tbl_units WHERE name= '" + unitName + "'";

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
