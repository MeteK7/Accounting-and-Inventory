using KabaAccounting.CUL;
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
                String sqlQuery = "INSERT INTO tbl_inventory_adjustment_detailed (/*id,*/ inventory_adjustment_id, product_id, product_unit_id, product_amount_in_real, product_amount_in_stock, product_cost_price, product_sale_price) VALUES (/*@id,*/ @inventory_adjustment_id, @product_id, @product_unit_id, @product_amount_in_real, @product_amount_in_stock, @product_cost_price, @product_sale_price)";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                //cmd.Parameters.AddWithValue("@id", inventoryAdjustmentDetailCUL.Id);//The column id in the database is not auto incremental. This is to prevent the number from increasing when the user deletes an existing inventory adjustment page and creates a new one.
                cmd.Parameters.AddWithValue("@inventory_adjustment_id", inventoryAdjustmentDetailCUL.InventoryAdjustmentId);
                cmd.Parameters.AddWithValue("@product_id", inventoryAdjustmentDetailCUL.ProductId);
                cmd.Parameters.AddWithValue("@product_unit_id", inventoryAdjustmentDetailCUL.ProductUnitId);
                cmd.Parameters.AddWithValue("@product_amount_in_real", inventoryAdjustmentDetailCUL.ProductAmountInReal);
                cmd.Parameters.AddWithValue("@product_amount_in_stock", inventoryAdjustmentDetailCUL.ProductAmountInStock);
                cmd.Parameters.AddWithValue("@product_cost_price", inventoryAdjustmentDetailCUL.ProductCostPrice);
                cmd.Parameters.AddWithValue("@product_sale_price", inventoryAdjustmentDetailCUL.ProductSalePrice);

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

        #region GETTING THE ROW INFORMATION OF A TABLE BY USING INVENTORY ADJUSTMENT ID.
        public DataTable Search(int inventoryAdjustmentId)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                DataTable dataTable = new DataTable();

                String sqlQuery = "SELECT * FROM tbl_inventory_adjustment_detailed WHERE inventory_adjustment_id= " + inventoryAdjustmentId + "";//SQL query to get the last id of rows in the table.

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
