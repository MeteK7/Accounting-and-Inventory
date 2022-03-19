using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUL
{
    public class PointOfSaleDetailCUL
    {
        public int Id { get; set; }
        public int InvoiceNo { get; set; }
        public int IdProduct { get; set; }
        public int IdProductUnit { get; set; }
        public int ProductPopId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductRate { get; set; }
        public decimal ProductQuantity { get; set; }
        public decimal ProductDiscount { get; set; }
        public decimal ProductVAT { get; set; }
        public decimal ProductCostPrice { get; set; }
        public decimal ProductSalePrice { get; set; }
        public IEnumerable ProductCboItemsSource { get; set; }
        public object ProductCboSelectedValue { get; set; }
        public string ProductCboSelectedValuePath { get; set; }
        public string ProductCboDisplayMemberPath { get; set; }
        //public DateTime AddedDate { get; set; }
        public int AddedBy { get; set; }
    }
}