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


        public void LoadPastInvoice(int invoiceNo = 0, int invoiceArrow = -1)//Optional parameter
        {
            int firstRowIndex = 0, productUnitId;
            string productId, productName, productUnitName, productCostPrice, productSalePrice, productAmount, productTotalCostPrice, productTotalSalePrice;

            if (invoiceNo == 0)
            {
                invoiceNo = GetLastInvoiceNumber();//Getting the last invoice number and assign it to the variable called invoiceNo.
            }

            /*WE CANNOT USE ELSE IF FOR THE CODE BELOW! BOTH IF STATEMENTS ABOVE AND BELOVE MUST WORK.*/
            if (invoiceNo != 0)// If the invoice number is still 0 even when we get the last invoice number by using code above, that means this is the first sale and do not run this code block.
            {
                DataTable dataTablePos = pointOfSaleDAL.GetByIdOrLastId(invoiceNo);
                DataTable dataTablePosDetail = pointOfSaleDetailDAL.Search(invoiceNo);
                DataTable dataTableUnitInfo;
                DataTable dataTableProduct;

                if (dataTablePosDetail.Rows.Count != 0)
                {
                    #region LOADING THE PRODUCT DATA GRID

                    for (int currentRow = firstRowIndex; currentRow < dataTablePosDetail.Rows.Count; currentRow++)
                    {
                        cboPaymentType.SelectedValue = Convert.ToInt32(dataTablePos.Rows[firstRowIndex]["payment_type_id"].ToString());//Getting the id of purchase type.
                        cboCustomer.SelectedValue = Convert.ToInt32(dataTablePos.Rows[firstRowIndex]["customer_id"].ToString());//Getting the id of supplier.
                        lblInvoiceNo.Content = dataTablePos.Rows[firstRowIndex]["id"].ToString();

                        productId = dataTablePosDetail.Rows[currentRow]["product_id"].ToString();
                        productUnitId = Convert.ToInt32(dataTablePosDetail.Rows[currentRow]["product_unit_id"]);

                        dataTableUnitInfo = unitDAL.GetUnitInfoById(productUnitId);//Getting the unit name by unit id.
                        productUnitName = dataTableUnitInfo.Rows[firstRowIndex]["name"].ToString();//We use firstRowIndex value for the index number in every loop because there can be only one unit name of a specific id.

                        productCostPrice = dataTablePosDetail.Rows[currentRow]["product_cost_price"].ToString();
                        productSalePrice = dataTablePosDetail.Rows[currentRow]["product_sale_price"].ToString();
                        productAmount = dataTablePosDetail.Rows[currentRow]["amount"].ToString();
                        productTotalCostPrice = (Convert.ToDecimal(productCostPrice) * Convert.ToDecimal(productAmount)).ToString();//We do NOT store the total cost in the db to reduce the storage. Instead of it, we multiply the unit cost with the amount to find the total cost.
                        productTotalSalePrice = (Convert.ToDecimal(productSalePrice) * Convert.ToDecimal(productAmount)).ToString();//We do NOT store the total price in the db to reduce the storage. Instead of it, we multiply the unit price with the amount to find the total price.

                        dataTableProduct = productDAL.SearchById(productId);

                        productName = dataTableProduct.Rows[firstRowIndex]["name"].ToString();//We used firstRowIndex because there can be only one row in the datatable for a specific product.

                        dgProducts.Items.Add(new { Id = productId, Name = productName, Unit = productUnitName, CostPrice = productCostPrice, SalePrice = productSalePrice, Amount = productAmount, TotalCostPrice = productTotalCostPrice, TotalSalePrice = productTotalSalePrice });
                    }
                    #endregion

                    #region FILLING THE PREVIOUS BASKET INFORMATIONS

                    //We used firstRowIndex below as a row name because there can be only one row in the datatable for a specific Invoice.
                    txtBasketAmount.Text = dataTablePos.Rows[firstRowIndex]["total_product_amount"].ToString();
                    txtBasketCostTotal.Text = dataTablePos.Rows[firstRowIndex]["cost_total"].ToString();
                    txtBasketSubTotal.Text = dataTablePos.Rows[firstRowIndex]["sub_total"].ToString();
                    txtBasketVat.Text = dataTablePos.Rows[firstRowIndex]["vat"].ToString();
                    txtBasketDiscount.Text = dataTablePos.Rows[firstRowIndex]["discount"].ToString();
                    txtBasketGrandTotal.Text = dataTablePos.Rows[firstRowIndex]["grand_total"].ToString();

                    #endregion
                }
                else if (dataTablePosDetail.Rows.Count == 0)//If the pos detail row quantity is 0, that means there is no such row so decrease or increase the invoice number according to user preference.
                {
                    if (invoiceArrow == 0)//If the invoice arrow is 0, that means user clicked the previous button.
                    {
                        invoiceNo = invoiceNo - 1;
                    }
                    else
                    {
                        invoiceNo = invoiceNo + 1;
                    }

                    if (invoiceArrow != -1)//If the user has not clicked either previous or next button, then the invoiceArrow will be -1 and no need for recursion.
                    {
                        LoadPastInvoice(invoiceNo, invoiceArrow);//Call the method again to get the new past invoice.
                    }

                }
            }
            else
            {
                FirstTimeRun();//This method is called when it is the first time of using this program.
            }

        }
    }
}
