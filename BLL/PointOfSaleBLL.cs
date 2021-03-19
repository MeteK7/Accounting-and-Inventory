using BLL.Interfaces;
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
    public class PointOfSaleBLL: IPointOfSaleBLL
    {
        UnitDAL unitDAL = new UnitDAL();
        UnitCUL unitCUL = new UnitCUL();
        ProductDAL productDAL = new ProductDAL();
        ProductCUL productCUL = new ProductCUL();
        PointOfSaleDAL pointOfSaleDAL = new PointOfSaleDAL();
        PointOfSaleCUL pointOfSaleCUL = new PointOfSaleCUL();
        PointOfSaleDetailDAL pointOfSaleDetailDAL = new PointOfSaleDetailDAL();
        PointOfSaleDetailCUL pointOfSaleDetailCUL = new PointOfSaleDetailCUL();

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
