using CUL;
using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ProductBLL
    {
        ProductDAL productDAL = new ProductDAL();
        ProductCUL productCUL = new ProductCUL();

        public void RevertOldQuantityInStock(string[,] dgOldProductCells, int oldItemsRowCount,int colProductQuantity, string calledBy)
        {
            int productId;
            decimal newQuantity;
            int initialRowIndex = 0;
            int colProductId = 0;
            decimal productQuantityFromDB;
            string colQtyNameInDb = "quantity_in_stock";

            DataTable dataTableProduct = new DataTable();

            for (int rowNo = initialRowIndex; rowNo < oldItemsRowCount; rowNo++)
            {
                dataTableProduct = productDAL.SearchById(dgOldProductCells[rowNo, colProductId]);

                productQuantityFromDB = Convert.ToInt32(dataTableProduct.Rows[initialRowIndex][colQtyNameInDb]);

                if(calledBy== "WinPOS")
                    newQuantity = productQuantityFromDB + Convert.ToDecimal(dgOldProductCells[rowNo, colProductQuantity]);//Add the old amount if it is POS.
                else
                    newQuantity = productQuantityFromDB - Convert.ToDecimal(dgOldProductCells[rowNo, colProductQuantity]);//Subtract the old amount if it is POP.

                productId = Convert.ToInt32(dgOldProductCells[rowNo, colProductId]);//Getting the product id in order to fix the amount of specific product in the db by id.

                productDAL.UpdateSpecificColumn(productId, colQtyNameInDb, newQuantity.ToString());
            }
        }
    }
}
