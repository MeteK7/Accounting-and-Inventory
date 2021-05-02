using CUL;
using DAL;
using System;
using System.Collections.Generic;
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
    }
}
