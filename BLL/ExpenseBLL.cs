﻿using CUL;
using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ExpenseBLL
    {
        ExpenseDAL expenseDAL = new ExpenseDAL();

        public bool InsertExpense(ExpenseCUL expenseCUL)
        {
            return expenseDAL.Insert(expenseCUL);
        }

        public bool UpdateExpense(ExpenseCUL expenseCUL)
        {
            return expenseDAL.Update(expenseCUL);
        }

        public int GetLastExpenseNumber()
        {
            int initialIndex = 0, expenseNo;

            DataTable dataTable = expenseDAL.GetByIdOrLastId();//Searching the last id number in the tbl_pos which actually stands for the current invoice number to save it to tbl_pos_details as an invoice number for this sale.

            if (dataTable.Rows.Count != 0)//If there is an invoice number in the database, that means the datatable's first row cannot be null, and the datatable's first index is 0.
            {
                expenseNo = Convert.ToInt32(dataTable.Rows[initialIndex]["id"]);//We defined this code out of the for loop below because all of the products has the same invoice number in every sale. So, no need to call this method for every products again and again.
            }
            else//If there is no any expense number, that means it is the first sale. So, assing expenseNo with 0;
            {
                expenseNo = initialIndex;
            }
            return expenseNo;
        }
    }
}
