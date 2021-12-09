using CUL;
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
    public class ReceiptDAL
    {
        static string connString = ConfigurationManager.ConnectionStrings["KabaAccountingConnString"].ConnectionString;

        #region INSERT METHOD
        public bool Insert(ReceiptCUL receiptCUL)
        {
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                string sqlQuery = "INSERT INTO tbl_receipts (id, id_payment_type, id_from, id_to, id_asset_from, id_asset_to, amount, details, added_date, added_by) VALUES (@id,@id_payment_type, @id_from, @id_to, @id_asset_from, @id_asset_to, @amount, @details, @added_date, @added_by)";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("@id", receiptCUL.Id);//The column id in the database is not auto incremental. This is to prevent the number from increasing when the user deletes an existing invoice and creates a new invoice.
                cmd.Parameters.AddWithValue("@id_payment_type", receiptCUL.IdPaymentType);
                cmd.Parameters.AddWithValue("@id_from", receiptCUL.IdFrom);
                cmd.Parameters.AddWithValue("@id_to", receiptCUL.IdTo);
                cmd.Parameters.AddWithValue("@id_asset_from", receiptCUL.IdAssetFrom);
                cmd.Parameters.AddWithValue("@id_asset_to", receiptCUL.IdAssetTo);
                cmd.Parameters.AddWithValue("@amount", receiptCUL.Amount);
                cmd.Parameters.AddWithValue("@details", receiptCUL.Details);
                cmd.Parameters.AddWithValue("@added_date", receiptCUL.AddedDate);
                cmd.Parameters.AddWithValue("@added_by", receiptCUL.AddedBy);

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

        #region UPDATE METHOD
        public bool Update(ReceiptCUL receiptCUL)
        {
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                string sqlQuery = "UPDATE tbl_receipts SET id_payment_type=@id_payment_type, id_from=@id_from, id_to=@id_to, id_asset_from=@id_asset_from, id_asset_to=@id_asset_to, amount=@amount, details=@details, added_date=@added_date, added_by=@added_by WHERE id=@id";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("id_payment_type", receiptCUL.IdPaymentType);
                cmd.Parameters.AddWithValue("id_from", receiptCUL.IdFrom);
                cmd.Parameters.AddWithValue("id_to", receiptCUL.IdTo);
                cmd.Parameters.AddWithValue("id_asset_from", receiptCUL.IdAssetFrom);
                cmd.Parameters.AddWithValue("id_asset_to", receiptCUL.IdAssetTo);
                cmd.Parameters.AddWithValue("amount", receiptCUL.Amount);
                cmd.Parameters.AddWithValue("details", receiptCUL.Details);
                cmd.Parameters.AddWithValue("added_date", receiptCUL.AddedDate);
                cmd.Parameters.AddWithValue("added_by", receiptCUL.AddedBy);
                cmd.Parameters.AddWithValue("id", receiptCUL.Id);//Do you really need to update the ID?


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
        public bool Delete(int receiptId)
        {
            //Create a Boolean variable and set its value to false.
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                //SQL Query to Delete from the Database
                string sqlQuery = "DELETE FROM tbl_receipts WHERE id=@id";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                //Passing the value using cmd
                cmd.Parameters.AddWithValue("@id", receiptId);

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

        #region GETTING ANY OR THE LAST ID AND ROW DATAS OF THE TABLE IN THE DATABASE
        public DataTable GetByIdOrLastId(int receiptNo = 0)//Optional parameter
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                DataTable dataTable = new DataTable();
                String sql;

                if (receiptNo == 0)//If the invoice number is 0 which means user did not send any argument, then get the last Id using the following query.
                {
                    sql = "SELECT * FROM tbl_receipts WHERE id=(SELECT max(id) FROM tbl_receipts)";
                }

                else
                {
                    sql = "SELECT * FROM tbl_receipts WHERE id=" + receiptNo + "";
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

        #region GETTING THE LAST ID AND ROW DATAS OF THE TABLE IN THE DATABASE USING INVOICE NO
        public DataTable GetByReceiptId(int receiptId = 0)//Optional parameter
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                DataTable dataTable = new DataTable();
                String sql;

                if (receiptId == 0)//If the invoice number is 0 which means user did not send any argument, then get the last record using the following query.
                {
                    sql = "SELECT * FROM tbl_receipts WHERE id=(SELECT max(id) FROM tbl_receipts)";
                    //sql = "SELECT * FROM tbl_pop WHERE id=IDENT_CURRENT('tbl_pop')";//SQL query to get the last id of rows in the table.
                }

                else
                {
                    sql = "SELECT * FROM tbl_receipts WHERE id=" + receiptId + "";//SQL query to get the last id of rows in the table.
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
