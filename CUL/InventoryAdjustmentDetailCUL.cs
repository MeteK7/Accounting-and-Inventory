using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUL
{
    public class InventoryAdjustmentDetailCUL
    {
        public int Id { get; set; }
        public int InventoryAdjustmentId { get; set; }
        public int ProductId { get; set; }
        public int ProductUnitId { get; set; }
        public decimal ProductQuantityInReal { get; set; }
        public decimal ProductQuantityInStock { get; set; }
        public decimal ProductQuantityDifference { get; set; }
        public decimal ProductCostPrice { get; set; }
        public decimal ProductSalePrice { get; set; }
    }
}
