﻿using System;
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
        public decimal Rating { get; set; }
        public string BarcodeRetail { get; set; }
        public string BarcodeWholesale { get; set; }
        public int Amount { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal TotalSalePrice { get; set; }
        public int UnitRetail { get; set; }
        public int UnitWholesale { get; set; }
        public DateTime AddedDate { get; set; }
        public int AddedBy { get; set; }
    }
}
