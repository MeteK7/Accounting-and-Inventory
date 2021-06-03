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
        public decimal QuantityInUnit { get; set; }
        public decimal QuantityInStock { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal TotalSalePrice { get; set; }
        public int UnitRetailId { get; set; }
        public int UnitWholesaleId { get; set; }
        public string UnitNameRetail { get; set; }
        public string UnitNameWholesale { get; set; }
        public DateTime AddedDate { get; set; }
        public int AddedBy { get; set; }
        public string AddedByUsername { get; set; }
    }
}
