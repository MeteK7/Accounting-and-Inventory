using DAL;
using KabaAccounting.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class CommonBLL
    {
        PointOfSaleDAL pointOfSaleDAL = new PointOfSaleDAL();
        PointOfPurchaseDAL pointOfPurchaseDAL = new PointOfPurchaseDAL();
        ExpenseDAL expenseDAL = new ExpenseDAL();
        public int GetLastRecordById(string calledBy)
        {
            int initialIndex = 0, recordId;

            DataTable dataTable;

            if (calledBy == "WinPOS")

                dataTable = pointOfSaleDAL.GetByIdOrLastId();//Searching the last id number in the tbl_pos which actually stands for the current invoice number to save it to tbl_pos_details as an invoice number for this sale.

            else if (calledBy == "WinPOP")
                dataTable = pointOfPurchaseDAL.GetByIdOrLastId();

            else
                dataTable = expenseDAL.GetByIdOrLastId();

            if (dataTable.Rows.Count != 0)//If there is an invoice number in the database, that means the datatable's first row cannot be null, and the datatable's first index is 0.
            {
                recordId = Convert.ToInt32(dataTable.Rows[initialIndex]["id"]);//We defined this code out of the for loop below because all of the products has the same invoice number in every sale. So, no need to call this method for every products again and again.
            }
            else//If there is no any invoice number, that means it is the first sale. So, assing invoiceNo with 0;
            {
                recordId = initialIndex;
            }
            return recordId;
        }
    }
}
