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
    public class PointOfPurchaseBLL
    {
        PointOfPurchaseDAL pointOfPurchaseDAL = new PointOfPurchaseDAL();

        public bool InsertPOP(PointOfPurchaseCUL pointOfPurchaseCUL)
        {
            return pointOfPurchaseDAL.Insert(pointOfPurchaseCUL);
        }

        public bool UpdatePOP(PointOfPurchaseCUL pointOfPurchaseCUL)
        {
            return pointOfPurchaseDAL.Update(pointOfPurchaseCUL);
        }

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

        //This method is getting the product cost price from the oldest pop where we have purchased the related product once before.
        //Note: Cost prices change with almost every purchase, so you should get the new cost price on the next purchase when the quantity for the product from the previous purchase has run out.
        public decimal GetProductLatestValidCostPrice(int productId)
        {
            decimal costPrice = pointOfPurchaseDAL.GetProductLatestValidCostPriceById(productId);

            return costPrice;
        }
    }
}
