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

        public DataTable GetLastInvoiceInfo()
        {
            //int specificRowIndex = 0, invoiceNo;

            DataTable dataTable = pointOfSaleDAL.GetByIdOrLastId();//A METHOD WHICH HAS AN OPTIONAL PARAMETER

            return dataTable;
        }

        public int GetLastInvoiceNumber()
        {
            int initialIndex = 0, invoiceNo;

            DataTable dataTable = pointOfSaleDAL.GetByIdOrLastId();//Searching the last id number in the tbl_pos which actually stands for the current invoice number to save it to tbl_pos_details as an invoice number for this sale.

            if (dataTable.Rows.Count != 0)//If there is an invoice number in the database, that means the datatable's first row cannot be null, and the datatable's first index is 0.
            {
                invoiceNo = Convert.ToInt32(dataTable.Rows[initialIndex]["id"]);//We defined this code out of the for loop below because all of the products has the same invoice number in every sale. So, no need to call this method for every products again and again.
            }
            else//If there is no any invoice number, that means it is the first sale. So, assing invoiceNo with 0;
            {
                invoiceNo = initialIndex;
            }
            return invoiceNo;
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
