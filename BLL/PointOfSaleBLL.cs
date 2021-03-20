﻿using KabaAccounting.CUL;
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
        ProductDAL productDAL = new ProductDAL();
        ProductCUL productCUL = new ProductCUL();


        public bool InsertPOS(PointOfSaleCUL pointOfSaleCUL)
        {
            return pointOfSaleDAL.Insert(pointOfSaleCUL);
        }

        public bool UpdatePOS(PointOfSaleCUL pointOfSaleCUL)
        {
            return pointOfSaleDAL.Update(pointOfSaleCUL);
        }

        public void RevertOldAmountInStock(string[,] dgOldProductCells, int oldItemsRowCount)
        {
            int initialRowIndex = 0;
            int colProductId = 0;
            int colProductAmount = 5;
            decimal productAmountFromDB;


            DataTable dataTableProduct = new DataTable();

            for (int rowNo = initialRowIndex; rowNo < oldItemsRowCount; rowNo++)
            {
                dataTableProduct = productDAL.SearchProductByIdBarcode(dgOldProductCells[rowNo, colProductId]);

                productAmountFromDB = Convert.ToInt32(dataTableProduct.Rows[initialRowIndex]["amount_in_stock"]);

                productCUL.AmountInStock = productAmountFromDB + Convert.ToDecimal(dgOldProductCells[rowNo, colProductAmount]);//Revert the amount in stock.

                productCUL.Id = Convert.ToInt32(dgOldProductCells[rowNo, colProductId]);//Getting the product id in order to fix the amount of specific product in the db by id.

                productDAL.UpdateAmountInStock(productCUL);
            }
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