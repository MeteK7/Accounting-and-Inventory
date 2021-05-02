﻿using CUL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class ExpenseDAL
    {
        static string connString = ConfigurationManager.ConnectionStrings["KabaAccountingConnString"].ConnectionString;

        #region INSERT METHOD
        public bool Insert(ExpenseCUL expenseCUL)
        {
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                string sqlQuery = "INSERT INTO tbl_expense (id, id_from, id_to, amount, added_date, added_by) VALUES (@id, @id_from, @id_to, @amount, @added_date, @added_by)";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("@id", expenseCUL.Id);//The column id in the database is not auto incremental. This is to prevent the number from increasing when the user deletes an existing invoice and creates a new invoice.
                cmd.Parameters.AddWithValue("@id_from", expenseCUL.IdFrom);
                cmd.Parameters.AddWithValue("@id_to", expenseCUL.IdTo);
                cmd.Parameters.AddWithValue("@amount", expenseCUL.Amount);
                cmd.Parameters.AddWithValue("@added_date", expenseCUL.AddedDate);
                cmd.Parameters.AddWithValue("@added_by", expenseCUL.AddedBy);

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
        public bool Update(ExpenseCUL expenseCUL)
        {
            bool isSuccess = false;

            SqlConnection conn = new SqlConnection(connString);

            try
            {
                string sqlQuery = "UPDATE tbl_expense SET id_from=@id_from, id_to=@id_to, amount=@amount, added_date=@added_date, added_by=@added_by WHERE id=@id";

                SqlCommand cmd = new SqlCommand(sqlQuery, conn);

                cmd.Parameters.AddWithValue("id_from", expenseCUL.IdFrom);
                cmd.Parameters.AddWithValue("id_to", expenseCUL.IdTo);
                cmd.Parameters.AddWithValue("amount", expenseCUL.Amount);
                cmd.Parameters.AddWithValue("added_date", expenseCUL.AddedDate);
                cmd.Parameters.AddWithValue("added_by", expenseCUL.AddedBy);
                cmd.Parameters.AddWithValue("id", expenseCUL.Id);//Do you really need to update the ID?


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