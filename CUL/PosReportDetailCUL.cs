using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUL
{
    public class PosReportDetailCUL
    {
        public int InvoiceId { get; set; }
        public int SaleDateId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }//Filled by ProductDAL.
        public decimal ProductAmountSold { get; set; }
    }
}
