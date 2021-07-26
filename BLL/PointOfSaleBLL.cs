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

        public DataTable GetLastInvoiceRecord()
        {
            DataTable dataTable = pointOfSaleDAL.GetByIdOrLastId();//A METHOD WHICH HAS AN OPTIONAL PARAMETER

            return dataTable;
        }

        //public string[,] GetDataGridContent(DataTable dgProducts)
        //{
        //    int rowLength = dgProducts.Rows.Count;
        //    int colLength = 8;
        //    string[,] dgProductCells = new string[rowLength, colLength];

        //    for (int rowIndex = 0; rowIndex < rowLength; rowIndex++)
        //    {
        //        DataRow dgRow = dgProducts.Rows[rowIndex];

        //        for (int colNo = 0; colNo < colLength; colNo++)
        //        {
        //            DataColumn contentCell = dgProducts.Columns[colNo];

        //            dgProductCells[rowIndex, colNo] = contentCell.ToString();

        //            //dgOldProductCells[rowNo, colNo] = cells[rowNo, colNo];//Assigning the old products' informations to the global array called "dgOldProductCells" so that we can access to the old products to revert the changes.
        //        }
        //    }

        //    return dgProductCells;
        //}
    }
}
