using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KabaAccounting.BLL
{
    class PointOfSaleDetailBLL
    {
        public int InvoiceNo { get; set; }
        public int ProductId { get; set; }
        public double ProductRate { get; set; }
        public int ProductAmount { get; set; }
        public decimal ProductCostPrice { get; set; }
        public decimal ProductSalePrice { get; set; }
        public decimal ProductTotalPrice { get; set; }
        public DateTime AddedDate { get; set; }
        public int AddedBy { get; set; }
    }
}
