using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KabaAccounting.BLL
{
    class ProductBLL
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Category { get; set; }
        public string Description { get; set; }
        public decimal Rate { get; set; }
        public decimal Quantity { get; set; }
        public DateTime AddedDate { get; set; }
        public int AddedBy { get; set; }
    }
}
