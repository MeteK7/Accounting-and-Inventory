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
    public class ProductBLL
    {
        ProductDAL productDAL = new ProductDAL();
        ProductCUL productCUL = new ProductCUL();

        public void RevertOldAmountInStock(string[,] dgOldProductCells, int oldItemsRowCount, string calledBy)
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

                if(calledBy=="POS")
                    productCUL.AmountInStock = productAmountFromDB + Convert.ToDecimal(dgOldProductCells[rowNo, colProductAmount]);//Add the old amount if it is POS.
                else
                    productCUL.AmountInStock = productAmountFromDB - Convert.ToDecimal(dgOldProductCells[rowNo, colProductAmount]);//Subtract the old amount if it is POP.

                productCUL.Id = Convert.ToInt32(dgOldProductCells[rowNo, colProductId]);//Getting the product id in order to fix the amount of specific product in the db by id.

                productDAL.UpdateAmountInStock(productCUL);
            }
        }
    }
}
