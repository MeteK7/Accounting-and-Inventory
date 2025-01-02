using CUL;
using DAL;
using CUL;
using DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CUL.Enums;

namespace GUI
{
    /// <summary>
    /// Interaction logic for WinPosReport.xaml
    /// </summary>
    public partial class WinPosReport : Window
    {
        UserDAL userDAL = new UserDAL();
        CustomerDAL customerDAL = new CustomerDAL();
        ProductDAL productDAL = new ProductDAL();
        ProductCUL productCUL = new ProductCUL();
        PointOfSaleDetailDAL pointOfSaleDetailDAL = new PointOfSaleDetailDAL();
        string dateFrom, dateTo;
        string
            colTxtPosId="id",
            colPosCustomerId="customer_id",
            colTxtProductId = "product_id",
            colTxtProductQty = "quantity",
            //colTxtPosTotalProductQuantity="total_product_quantity",
            colTxtProductCostPrice = "product_cost_price",
            colTxtProductSalePrice = "product_sale_price",
            colTxtProductDiscount="product_discount",
            colTxtProductVAT= "product_vat",
            colTxtCostTotal = "cost_total",
            colTxtPosGrandTotal = "grand_total",
            colTxtPosGrossAmount="gross_amount",
            colTxtPosDiscount ="discount",
            colTxtPosVAT="vat",
            colPosDate="added_date",
            colTxtCustomerId="customer_id",
            colTxtName = "name",
            colTxtAddedBy = "added_by",
            colTxtFirstName = "first_name",
            colTxtLastName = "last_name";

        public WinPosReport()
        {
            InitializeComponent();
            LoadDefaultDate();
            LoadSales();
            LoadProductListView();
            LoadSaleListView();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            dateFrom = String.Format("{0:yyyy-MM-dd}", dtpPosReportFrom.SelectedDate) + " " + String.Format("{0:HH:mm:ss}", timePickerFrom.Value);
            dateTo = String.Format("{0:yyyy-MM-dd}", dtpPosReportTo.SelectedDate) + " " + String.Format("{0:HH:mm:ss}", timePickerTo.Value);

            lvwSales.Items.Clear();
            LoadSales();
            LoadProductListView();
            LoadSaleListView();
        }

        private void LoadDefaultDate()
        {
            dtpPosReportFrom.SelectedDate = DateTime.Today;
            dtpPosReportTo.SelectedDate = DateTime.Today;
            timePickerFrom.Value = Convert.ToDateTime("00:00:00");
            timePickerTo.Value = Convert.ToDateTime("23:59:59");

            dateFrom = String.Format("{0:yyyy-MM-dd}", dtpPosReportFrom.SelectedDate) + " " + String.Format("{0:HH:mm:ss}", timePickerFrom.Value);
            dateTo = String.Format("{0:yyyy-MM-dd}", dtpPosReportTo.SelectedDate) + " " + String.Format("{0:HH:mm:ss}", timePickerTo.Value);
        }

        private void ClearGroupBoxes()
        {
            lblNumOfSalesVar.Content = ((int)Numbers.InitialIndex).ToString();
            lblCostVar.Content = ((int)Numbers.InitialIndex).ToString();
            lblRevenueVar.Content = ((int)Numbers.InitialIndex).ToString();
            lblProfitVar.Content = ((int)Numbers.InitialIndex).ToString();
        }

        private void LoadSales()
        {
            ClearGroupBoxes();

            #region NUMBER OF SALES
            bool cash = true, credit = false;

            lblCashSales.Content = PointOfSaleDAL.CountPaymentTypeByToday(dateFrom, dateTo, cash);//Send the variable cash to the parameter of the CountByDay method in the ProductDAL.

            lblCreditSales.Content = PointOfSaleDAL.CountPaymentTypeByToday(dateFrom, dateTo, credit);//Send the variable credit to the parameter of the CountByDay method in the ProductDAL.

            lblNumOfSalesVar.Content = Convert.ToInt32(lblCashSales.Content) + Convert.ToInt32(lblCreditSales.Content);//Sum cash and credit amount to find the total number of sales.
            #endregion

            #region USER SALES
            lvwUserSales.Items.Clear();//Clearing the listview before loading new datas in case the user changes the dates.
            DataTable dtUserSales = PointOfSaleDAL.SumAmountByUserBetweenDates(dateFrom, dateTo);
            DataTable dtUsers;
            int userId;
            decimal userSaleAmount;
            string userFullName;

            for (int rowIndex = (int)Numbers.InitialIndex; rowIndex < dtUserSales.Rows.Count; rowIndex++)
            {
                userId = Convert.ToInt32(dtUserSales.Rows[rowIndex][colTxtAddedBy]);
                dtUsers = userDAL.GetUserInfoById(userId);

                userFullName = dtUsers.Rows[(int)Numbers.InitialIndex][colTxtFirstName].ToString() + " " + dtUsers.Rows[(int)Numbers.InitialIndex][colTxtLastName].ToString();

                userSaleAmount = Convert.ToDecimal(String.Format("{0:0.00}",dtUserSales.Rows[rowIndex][colTxtPosGrandTotal]));

                lvwUserSales.Items.Add(
                    new PosReportDetailCUL()
                    {
                        UserFullName = userFullName,
                        UserSaleAmount = userSaleAmount
                    });
            }
            #endregion
        }

        private void LoadProductListView()
        {
            string productId;
            bool addNew = true;
            IEnumerable items;

            lvwProducts.Items.Clear();

            DataTable dtPosJoined = PointOfSaleDAL.JoinProductReportByDate(dateFrom, dateTo);
            DataTable dtProduct;

            for (int rowIndex = (int)Numbers.InitialIndex; rowIndex < dtPosJoined.Rows.Count; rowIndex++)
            {
                addNew = true;
                items = lvwProducts.Items;

                foreach (PosReportDetailCUL product in items)//This loop is for preventing duplications.
                {
                    if (product.ProductId == Convert.ToInt32(dtPosJoined.Rows[rowIndex][colTxtProductId]))
                    {
                        product.ProductQuantity += Convert.ToDecimal(dtPosJoined.Rows[rowIndex][colTxtProductQty]);
                        product.ProductTotalSalePrice = String.Format("{0:0.00}",
                            Convert.ToDecimal(product.ProductTotalSalePrice) +
                            (Convert.ToDecimal(dtPosJoined.Rows[rowIndex][colTxtProductSalePrice]) * Convert.ToDecimal(dtPosJoined.Rows[rowIndex][colTxtProductQty])));
                        addNew = false;//No need to run the loop again since there can be only one entry of a unique product.
                        break;
                    }
                }

                if (addNew == true)
                {
                    productId = dtPosJoined.Rows[rowIndex][colTxtProductId].ToString();
                    dtProduct = productDAL.SearchById(productId);

                    lvwProducts.Items.Add(
                        new PosReportDetailCUL()
                        {
                            //InvoiceId = Convert.ToInt32(dataTablePosDetailToday.Rows[posDetailIndex]["invoice_no"]),
                            ProductId = Convert.ToInt32(dtPosJoined.Rows[rowIndex][colTxtProductId]),
                            ProductName = dtProduct.Rows[(int)Numbers.InitialIndex][colTxtName].ToString(),
                            ProductQuantity = Convert.ToDecimal(dtPosJoined.Rows[rowIndex][colTxtProductQty]),
                            ProductTotalSalePrice = String.Format("{0:0.00}", (Convert.ToDecimal(dtPosJoined.Rows[rowIndex][colTxtProductSalePrice]) * Convert.ToDecimal(dtPosJoined.Rows[rowIndex][colTxtProductQty])))
                        });
                }
            }
        }

        private void LoadSaleListView()
        {
            DataTable dtPos = PointOfSaleDAL.FetchReportByDate(dateFrom, dateTo);
            DataTable dtPosDetail;
            DataTable dtUsers,dtCustomers;
            decimal totalCostPrice = (int)Numbers.InitialIndex, totalRevenue = (int)Numbers.InitialIndex, totalProfit = (int)Numbers.InitialIndex;
            int userId,customerId;
            string userFullName,customerFullName;
            decimal productQuantity, 
                    productCostPrice,
                    productSalePrice,
                    productVAT ,
                    productDiscount,
                    basketQuantity,
                    basketGrossTotalCostPrice,
                    basketGrossTotalSalePrice,
                    basketDiscount,
                    basketSubTotal,
                    basketVAT,
                    basketTotalSalePrice,
                    saleProfit=(int)Numbers.InitialIndex;

            for (int rowIndex = (int)Numbers.InitialIndex; rowIndex < dtPos.Rows.Count; rowIndex++)
            {
                #region USER FULL NAME
                userId = Convert.ToInt32(dtPos.Rows[rowIndex][colTxtAddedBy]);
                dtUsers = userDAL.GetUserInfoById(userId);

                userFullName = dtUsers.Rows[(int)Numbers.InitialIndex][colTxtFirstName].ToString() + " " + dtUsers.Rows[(int)Numbers.InitialIndex][colTxtLastName].ToString();
                #endregion

                #region CUSTOMER
                customerId = Convert.ToInt32(dtPos.Rows[rowIndex][colTxtCustomerId]);
                dtCustomers = customerDAL.GetCustomerInfoById(customerId);

                customerFullName = dtCustomers.Rows[(int)Numbers.InitialIndex][colTxtName].ToString();
                #endregion

                #region INVOICE ROW POPULATING
                productSalePrice = (int)Numbers.InitialIndex;
                productQuantity = (int)Numbers.InitialIndex;
                basketQuantity = (int)Numbers.InitialIndex;
                basketGrossTotalCostPrice = (int)Numbers.InitialIndex;
                basketGrossTotalSalePrice = (int)Numbers.InitialIndex;
                basketDiscount = (int)Numbers.InitialIndex;
                basketSubTotal = (int)Numbers.InitialIndex;
                basketVAT = (int)Numbers.InitialIndex;
                basketTotalSalePrice = (int)Numbers.InitialIndex;

                dtPosDetail = pointOfSaleDetailDAL.Search(Convert.ToInt32(dtPos.Rows[rowIndex][colTxtPosId]));

                for (int invoiceIndex = (int)Numbers.InitialIndex; invoiceIndex < dtPosDetail.Rows.Count; invoiceIndex++)
                {
                    productQuantity= Convert.ToDecimal(dtPosDetail.Rows[invoiceIndex][colTxtProductQty]);
                    productCostPrice = Convert.ToDecimal(dtPosDetail.Rows[invoiceIndex][colTxtProductCostPrice]);
                    productSalePrice = Convert.ToDecimal(dtPosDetail.Rows[invoiceIndex][colTxtProductSalePrice]);
                    productDiscount= Convert.ToDecimal(dtPosDetail.Rows[invoiceIndex][colTxtProductDiscount]);
                    productVAT = Convert.ToDecimal(dtPosDetail.Rows[invoiceIndex][colTxtProductVAT]);
                    basketQuantity = basketQuantity+ productQuantity;
                    basketGrossTotalCostPrice =  basketGrossTotalCostPrice + (Convert.ToDecimal(productCostPrice) * Convert.ToDecimal(productQuantity));
                    basketGrossTotalSalePrice = basketGrossTotalSalePrice + (Convert.ToDecimal(productSalePrice) * Convert.ToDecimal(productQuantity));
                    basketDiscount = basketDiscount+ productDiscount;
                    basketSubTotal = basketSubTotal + (productSalePrice * productQuantity) + productDiscount;
                    basketVAT = basketVAT + productVAT;
                    basketTotalSalePrice = basketTotalSalePrice + (productSalePrice * productQuantity) - productVAT + productDiscount;


                    // Calculate profit for the sale
                    saleProfit += CalculateProfit(Convert.ToInt32(dtPosDetail.Rows[invoiceIndex][colTxtProductId]), productQuantity, productSalePrice, Convert.ToDateTime(dtPos.Rows[rowIndex][colPosDate]));

                }

                lvwSales.Items.Add(
                    new PosReportDetailCUL()
                    {
                        PosId = Convert.ToInt32(dtPos.Rows[rowIndex][colTxtPosId]),
                        PosTotalProductQuantity= basketQuantity,
                        PosGrossTotalCostPrice = Convert.ToDecimal(String.Format("{0:0.00}", basketGrossTotalCostPrice)),
                        PosGrossTotalSalePrice = Convert.ToDecimal(String.Format("{0:0.00}", basketGrossTotalSalePrice)),
                        PosDiscount= basketDiscount,
                        PosSubTotal = Convert.ToDecimal(String.Format("{0:0.00}", basketSubTotal)),
                        PosVAT = basketVAT,
                        PosGrandTotal= Convert.ToDecimal(String.Format("{0:0.00}", basketTotalSalePrice)),
                        PosCustomer=customerFullName,
                        UserFullName = userFullName,
                        PosDate=Convert.ToDateTime(dtPos.Rows[rowIndex][colPosDate]),
                        Profit = saleProfit
                    });
                #endregion

                //PAYMENT SUMMARY
                totalCostPrice +=Convert.ToDecimal(basketGrossTotalCostPrice);
                totalRevenue += Convert.ToDecimal(basketTotalSalePrice);
                totalProfit += saleProfit;
            }

            #region PAYMENT RESULTS
            lblCostVar.Content = totalCostPrice.ToString("0.00");
            lblRevenueVar.Content = totalRevenue.ToString("0.00");
            lblProfitVar.Content = totalProfit.ToString("0.00");
            #endregion
        }

        public decimal CalculateProfit(int productId, decimal saleQuantity, decimal salePrice, DateTime targetDate)
        {
            decimal profit = 0m;
            decimal remainingSaleQuantity = saleQuantity;

            // Step 1: Retrieve all purchases for the product in FIFO order
            DataTable purchaseData = PointOfPurchaseDAL.GetPurchasesByProductId(productId);
            purchaseData.DefaultView.Sort = "added_date ASC"; // FIFO order

            // Step 2: Deduct prior sales from purchases
            DataTable priorSales = PointOfSaleDAL.GetSalesByProductIdBeforeDate(productId, targetDate);
            foreach (DataRow sale in priorSales.Rows)
            {
                decimal priorSaleQuantity = Convert.ToDecimal(sale["quantity"]);

                foreach (DataRow purchase in purchaseData.Rows)
                {
                    if (priorSaleQuantity <= 0) break;

                    decimal purchaseQuantity = Convert.ToDecimal(purchase["quantity"]);
                    if (purchaseQuantity > 0)
                    {
                        decimal quantityToDeduct = Math.Min(priorSaleQuantity, purchaseQuantity);
                        purchase["quantity"] = purchaseQuantity - quantityToDeduct;
                        priorSaleQuantity -= quantityToDeduct;
                    }
                }
            }

            // Step 3: Calculate profit for sales on the target date
            foreach (DataRow purchase in purchaseData.Rows)
            {
                if (remainingSaleQuantity <= 0) break;

                decimal purchaseQuantity = Convert.ToDecimal(purchase["quantity"]);
                decimal purchaseCostPrice = Convert.ToDecimal(purchase["product_cost_price"]);

                if (purchaseQuantity > 0)
                {
                    decimal quantityToUse = Math.Min(remainingSaleQuantity, purchaseQuantity);
                    remainingSaleQuantity -= quantityToUse;

                    decimal cost = quantityToUse * purchaseCostPrice;
                    decimal revenue = quantityToUse * salePrice;
                    profit += (revenue - cost);
                }
            }

            return profit;
        }

    }
}
