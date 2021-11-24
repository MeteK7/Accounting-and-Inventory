using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUL
{
    public class PopReportDetailCUL
    {
        public int InvoiceId { get; set; }
        public int PurchaseDateId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }//Filled by ProductDAL.
        public decimal ProductQuantityPurchased { get; set; }
        public string ProductTotalCostPrice { get; set; }
        public string UserFullName { get; set; }
        public decimal UserPurchaseAmount { get; set; }
    }
}
