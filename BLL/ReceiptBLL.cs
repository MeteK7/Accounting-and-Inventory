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
    public class ReceiptBLL
    {
        ReceiptDAL receiptDAL = new ReceiptDAL();

        public bool InsertReceipt(ReceiptCUL receiptCUL)
        {
            return receiptDAL.Insert(receiptCUL);
        }

        public bool UpdateReceipt(ReceiptCUL receiptCUL)
        {
            return receiptDAL.Update(receiptCUL);
        }

        public int GetLastReceiptNumber()
        {
            int initialIndex = 0, receiptNo;

            DataTable dataTable = receiptDAL.GetByIdOrLastId();//Searching the last id number in the tbl_pos which actually stands for the current invoice number to save it to tbl_pos_details as an invoice number for this sale.

            if (dataTable.Rows.Count != 0)//If there is an invoice number in the database, that means the datatable's first row cannot be null, and the datatable's first index is 0.
            {
                receiptNo = Convert.ToInt32(dataTable.Rows[initialIndex]["id"]);//We defined this code out of the for loop below because all of the products has the same invoice number in every sale. So, no need to call this method for every products again and again.
            }
            else//If there is no any receipt number, that means it is the first sale. So, assing receiptNo with 0;
            {
                receiptNo = initialIndex;
            }
            return receiptNo;
        }
    }
}
