using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUL
{
    public class DepositDetailCUL
    {
        public int Id { get; set; }
        public string BankId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }
}
