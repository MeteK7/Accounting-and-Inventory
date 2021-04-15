using CUL;
using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class DepositBLL
    {
        DepositDAL depositDAL = new DepositDAL();

        public DataTable GetLastDepositInfo()
        {
            DataTable dataTable = depositDAL.GetByIdOrLastId();//A METHOD WHICH HAS AN OPTIONAL PARAMETER

            return dataTable;
        }

        public bool InsertDeposit(DepositCUL depositCUL)
        {
            return depositDAL.Insert(depositCUL);
        }

        public bool UpdateDeposit(DepositCUL depositCUL)
        {
            return depositDAL.Update(depositCUL);
        }

        public int GetLastDepositId()
        {
            int initialIndex = 0, invoiceNo;

            DataTable dataTable = depositDAL.GetByIdOrLastId();//Searching the last id number in the tbl_pos which actually stands for the current invoice number to save it to tbl_pos_details as an invoice number for this sale.

            if (dataTable.Rows.Count != 0)//If there is an invoice number in the database, that means the datatable's first row cannot be null, and the datatable's first index is 0.
            {
                invoiceNo = Convert.ToInt32(dataTable.Rows[initialIndex]["id"]);//We defined this code out of the for loop below because all of the products has the same invoice number in every sale. So, no need to call this method for every products again and again.
            }
            else//If there is no any invoice number, that means it is the first sale. So, assing invoiceNo with 0;
            {
                invoiceNo = initialIndex;
            }
            return invoiceNo;
        }
    }
}
