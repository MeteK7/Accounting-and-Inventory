using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KabaAccounting.CUL
{
    public class ProductCUL
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public decimal Rating { get; set; }
        public string BarcodeRetail { get; set; }
        public string BarcodeWholesale { get; set; }
        public decimal AmountInUnit { get; set; }
        public decimal AmountInStock { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal TotalSalePrice { get; set; }
        public int UnitRetail { get; set; }
        public int UnitWholesale { get; set; }
        public DateTime AddedDate { get; set; }
        public int AddedBy { get; set; }
    }
}
