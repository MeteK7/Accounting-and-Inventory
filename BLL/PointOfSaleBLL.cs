using KabaAccounting.CUL;
using KabaAccounting.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class PointOfSaleBLL
    {
        PointOfSaleDAL pointOfSaleDAL = new PointOfSaleDAL();

        public bool InsertPOS(PointOfSaleCUL pointOfSaleCUL)
        {
            return pointOfSaleDAL.Insert(pointOfSaleCUL);
        }

        public bool UpdatePOS(PointOfSaleCUL pointOfSaleCUL)
        {
            return pointOfSaleDAL.Update(pointOfSaleCUL);
        }
    }
}
