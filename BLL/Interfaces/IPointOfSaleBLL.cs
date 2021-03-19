using KabaAccounting.CUL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IPointOfSaleBLL
    {
        public bool InsertPOS(PointOfSaleCUL pointOfSaleCUL);
        public bool UpdatePOS(PointOfSaleCUL pointOfSaleCUL);
    }
}
