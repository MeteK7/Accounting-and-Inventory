using KabaAccounting.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class PointOfPurchaseBLL
    {
        PointOfPurchaseDAL pointOfPurchaseDAL = new PointOfPurchaseDAL();

        //public int GetInvoiceIdByNo(string invoiceNo)
        //{
        //    int voidInvoiceId = 0, currentInvoiceNo = Convert.ToInt32(invoiceNo);

        //    DataTable dataTableCurrentInvoice = pointOfPurchaseDAL.SearchByInvoiceNo(currentInvoiceNo);
        //    int currentInvoiceId = Convert.ToInt32(dataTableCurrentInvoice.Rows[voidInvoiceId]["id"]);//Getting the current invoice id.
        //    return currentInvoiceId;
        //}

        public DataTable GetLastInvoiceRecord()
        {
            DataTable dataTable = pointOfPurchaseDAL.GetByInvoiceId();//If no argument is sent to the method "GetByInvoiceId", then get the last invoice record from db.

            return dataTable;
        }
    }
}
