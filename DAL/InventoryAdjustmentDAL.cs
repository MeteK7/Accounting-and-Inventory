using KabaAccounting.CUL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KabaAccounting.DAL
{
    public class InventoryAdjustmentDAL
    {
        static string connString = ConfigurationManager.ConnectionStrings["KabaAccountingConnString"].ConnectionString;

        #region Search
        public DataTable Search(int inventoryAdjustmentId = 0)//Optional parameter
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                DataTable dataTable = new DataTable();
                String sql;

                if (inventoryAdjustmentId == 0)//If the inventory adjustment id is 0 which means user did not send any argument, then get the last Id using the following query.
                {
                    sql = "SELECT * FROM tbl_inventory_adjustment WHERE id=(SELECT max(id) FROM tbl_inventory_adjustment)";
                }

                else
                {
                    sql = "SELECT * FROM tbl_inventory_adjustment WHERE id=" + inventoryAdjustmentId + "";
                }

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    try
                    {
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

        #region INSERT METHOD
        public bool Insert(InventoryAdjustmentCUL inventoryAdjustmentCUL)
        {
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                string sqlQuery = "INSERT INTO tbl_pos (id, payment_type_id, customer_id, total_product_amount, cost_total, sub_total, vat, discount, grand_total, added_date, added_by) VALUES (@id, @payment_type_id, @customer_id, @total_product_amount, @cost_total, @sub_total, @vat, @discount, @grand_total, @added_date, @added_by)";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("@id", inventoryAdjustmentCUL.Id);//The column id in the database is not auto incremental. This is to prevent the number from increasing when the user deletes an existing invoice and creates a new invoice.
                cmd.Parameters.AddWithValue("@total_product_amount", inventoryAdjustmentCUL.TotalProductAmount);
                cmd.Parameters.AddWithValue("@grand_total", inventoryAdjustmentCUL.GrandTotal);
                cmd.Parameters.AddWithValue("@added_date", inventoryAdjustmentCUL.AddedDate);
                cmd.Parameters.AddWithValue("@added_by", inventoryAdjustmentCUL.AddedBy);

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
    }
}
