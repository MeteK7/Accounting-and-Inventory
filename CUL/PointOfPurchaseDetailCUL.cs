using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KabaAccounting.CUL
{
    public class PointOfPurchaseDetailCUL
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ProductUnitId { get; set; }
        public decimal ProductRate { get; set; }
        public decimal ProductQuantity { get; set; }
        public decimal ProductGrossCostPrice { get; set; }
        public decimal ProductDiscount { get; set; }
        public decimal ProductVAT { get; set; }
        public decimal ProductCostPrice { get; set; }
        //public DateTime AddedDate { get; set; }
        public int AddedBy { get; set; }
    }
}
