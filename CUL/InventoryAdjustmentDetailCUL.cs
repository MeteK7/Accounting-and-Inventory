using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KabaAccounting.CUL
{
    public class InventoryAdjustmentDetailCUL
    {
        public int Id { get; set; }
        public int InventoryAdjustmentId { get; set; }
        public int ProductId { get; set; }
        public int ProductUnitId { get; set; }
        public int ProductAmount { get; set; }
    }
}
