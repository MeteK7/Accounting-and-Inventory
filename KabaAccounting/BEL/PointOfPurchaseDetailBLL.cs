using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KabaAccounting.BLL
{
    class PointOfPurchaseDetailBLL
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ProductUnitId { get; set; }
        public int InvoiceNo { get; set; }
        public decimal ProductRate { get; set; }
        public decimal ProductAmount { get; set; }
        public decimal ProductCostPrice { get; set; }
        public DateTime AddedDate { get; set; }
        public int AddedBy { get; set; }
    }
}
