using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KabaAccounting.BLL
{
    class PointOfSaleBLL
    {
        public int Id { get; set; }
        public string SaleType { get; set; }
        public int CustomerId { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Vat { get; set; }
        public decimal Discount { get; set; }
        public decimal GrandTotal { get; set; }
        public DateTime AddedDate { get; set; }
        public int AddedBy { get; set; }
    }
}
