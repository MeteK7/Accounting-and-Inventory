using KabaAccounting.CUL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class InventoryAdjustmentDetailDAL
    {
        static string connString = ConfigurationManager.ConnectionStrings["KabaAccountingConnString"].ConnectionString;

        #region INSERT METHOD
        public bool Insert(InventoryAdjustmentDetailCUL inventoryAdjustmentDetailCUL)
        {
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                String sqlQuery = "INSERT INTO tbl_inventory_adjustment_detailed (/*id,*/ inventory_adjustment_id, product_id, product_unit_id, product_amount_in_stock) VALUES (/*@id,*/ @inventory_adjustment_id, @product_id, @product_unit_id, @product_amount_in_stock)";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                //cmd.Parameters.AddWithValue("@id", inventoryAdjustmentDetailCUL.Id);//The column id in the database is not auto incremental. This is to prevent the number from increasing when the user deletes an existing invoice and creates a new invoice.
                cmd.Parameters.AddWithValue("@inventory_adjustment_id", inventoryAdjustmentDetailCUL.InventoryAdjustmentId);
                cmd.Parameters.AddWithValue("@product_id", inventoryAdjustmentDetailCUL.ProductId);
                cmd.Parameters.AddWithValue("@product_unit_id", inventoryAdjustmentDetailCUL.ProductUnitId);
                cmd.Parameters.AddWithValue("@product_amount_in_stock", inventoryAdjustmentDetailCUL.ProductAmount);

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

        #region DELETE METHOD
        public bool Delete(int inventoryAdjustmentId)
        {
            //Create a Boolean variable and set its value to false.
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                //SQL Query to Delete from the Database
                string sqlQuery = "DELETE FROM tbl_inventory_adjustment_detailed WHERE inventory_adjustment_id=@inventory_adjustment_id";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                //Passing the value using cmd
                cmd.Parameters.AddWithValue("@inventory_adjustment_id", inventoryAdjustmentId);

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
    }
}
