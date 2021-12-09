using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUL
{
    public class PosReportDetailCUL
    {
        public int PosId { get; set; }
        public int SaleDateId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }//Filled by ProductDAL.
        public decimal ProductQuantity { get; set; }
        public string ProductTotalSalePrice { get; set; }
        public decimal PosTotalProductQuantity { get; set; }
        public decimal PosGrossAmount { get; set; }
        public decimal PosDiscount { get; set; }
        public decimal PosVAT { get; set; }
        public decimal PosGrandTotal { get; set; }
        public string PosCustomer { get; set; }
        public DateTime PosDate { get; set; }
        public string UserFullName { get; set; }
        public decimal UserSaleAmount { get; set; }
    }
}
