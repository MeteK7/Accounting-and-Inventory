using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IPointOfSaleBLL
    {
        public void LoadPastInvoice(int invoiceNo = 0, int invoiceArrow = -1);
    }
}
