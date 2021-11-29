using DAL;
using DAL;
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
        CommonDAL commonDAL = new CommonDAL();
        public int GetLastRecordById(string calledBy)
        {
            int initialIndex = 0, recordId;

            DataTable dataTable;

                dataTable = commonDAL.GetByIdOrLastId(calledBy);

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
