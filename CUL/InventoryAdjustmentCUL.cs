using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUL
{
    public class InventoryAdjustmentCUL
    {
        public int Id { get; set; }
        public decimal TotalProductQuantity { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTime AddedDate { get; set; }
        public int AddedBy { get; set; }
    }
}
