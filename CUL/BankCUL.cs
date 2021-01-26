using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KabaAccounting.CUL
{
    public class BankCUL
    {
        public int Id { get; set; }//Properties are named by PascalCase.
        public string Name { get; set; }
        public DateTime AddedDate { get; set; }
        public int AddedBy { get; set; }
    }
}
