using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUL
{
    public class DepositCUL
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime AddedDate { get; set; }
        public int AddedBy { get; set; }
    }
}
