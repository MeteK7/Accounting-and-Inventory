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
        ProductDAL productDAL = new ProductDAL();
        ProductCUL productCUL = new ProductCUL();
        PointOfSaleDAL pointOfSaleDAL = new PointOfSaleDAL();
        PointOfSaleDetailDAL pointOfSaleDetailDAL = new PointOfSaleDetailDAL();
        string dateFrom, dateTo;
        string
            colTxtPosId="id",
            colPosCustomerId="customer_id",
            colTxtProductId = "product_id",
            colTxtProductQty = "quantity",
            colTxtPosTotalProductQuantity="total_product_quantity",
            colTxtProductSalePrice = "product_sale_price",
            colTxtCostTotal = "cost_total",
            colTxtPosGrandTotal = "grand_total",
            colTxtPosGrossAmount="gross_amount",
            colTxtPosDiscount="discount",
            colTxtPosVAT="vat",
            colPosDate="added_date",
            colTxtName = "name",
            colTxtAddedBy = "added_by",
            colTxtFirstName = "first_name",
            colTxtLastName = "last_name";

        public WinPosReport()
        {
            InitializeComponent();
            LoadDefaultDate();
            LoadPayments();
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

            LoadPayments();
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

        private void LoadPayments()
        {
            ClearGroupBoxes();

            #region NUMBER OF SALES
            bool cash = true, credit = false;

            lblCashSales.Content = pointOfSaleDAL.CountPaymentTypeByToday(dateFrom, dateTo, cash);//Send the variable cash to the parameter of the CountByDay method in the ProductDAL.

            lblCreditSales.Content = pointOfSaleDAL.CountPaymentTypeByToday(dateFrom, dateTo, credit);//Send the variable credit to the parameter of the CountByDay method in the ProductDAL.

            lblNumOfSalesVar.Content = Convert.ToInt32(lblCashSales.Content) + Convert.ToInt32(lblCreditSales.Content);//Sum cash and credit amount to find the total number of sales.
            #endregion

            #region PAYMENT RESULTS
            DataTable dtPos = pointOfSaleDAL.FetchReportByDate(dateFrom, dateTo);

            for (int rowIndex = (int)Numbers.InitialIndex; rowIndex < dtPos.Rows.Count; rowIndex++)
            {
                lblCostVar.Content = Convert.ToDecimal(lblCostVar.Content) + Convert.ToDecimal(dtPos.Rows[rowIndex][colTxtCostTotal]);
                lblRevenueVar.Content = Convert.ToDecimal(lblRevenueVar.Content) + Convert.ToDecimal(dtPos.Rows[rowIndex][colTxtPosGrandTotal]);
            }

            lblProfitVar.Content = Convert.ToDecimal(lblRevenueVar.Content) - Convert.ToDecimal(lblCostVar.Content);
            #endregion

            #region USER SALES
            lvwUserSales.Items.Clear();//Clearing the listview before loading new datas in case the user changes the dates.
            DataTable dtUserSales = pointOfSaleDAL.SumAmountByUserBetweenDates(dateFrom, dateTo);
            DataTable dtUsers;
            int userId;
            decimal userSaleAmount;
            string userFullName;

            for (int rowIndex = (int)Numbers.InitialIndex; rowIndex < dtUserSales.Rows.Count; rowIndex++)
            {
                userId = Convert.ToInt32(dtUserSales.Rows[rowIndex][colTxtAddedBy]);
                dtUsers = userDAL.GetUserInfoById(userId);

                userFullName = dtUsers.Rows[(int)Numbers.InitialIndex][colTxtFirstName].ToString() + " " + dtUsers.Rows[(int)Numbers.InitialIndex][colTxtLastName].ToString();

                userSaleAmount = Convert.ToDecimal(dtUserSales.Rows[rowIndex][colTxtPosGrandTotal]);

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

            DataTable dtPosJoined = pointOfSaleDAL.JoinProductReportByDate(dateFrom, dateTo);
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
            DataTable dtPos = pointOfSaleDAL.FetchReportByDate(dateFrom, dateTo);
            DataTable dtUsers;
            int userId;
            string userFullName;

            for (int rowIndex = (int)Numbers.InitialIndex; rowIndex < dtPos.Rows.Count; rowIndex++)
            {
                userId = Convert.ToInt32(dtPos.Rows[rowIndex][colTxtAddedBy]);
                dtUsers = userDAL.GetUserInfoById(userId);

                userFullName = dtUsers.Rows[(int)Numbers.InitialIndex][colTxtFirstName].ToString() + " " + dtUsers.Rows[(int)Numbers.InitialIndex][colTxtLastName].ToString();

                lvwSales.Items.Add(
                    new PosReportDetailCUL()
                    {
                        PosId = Convert.ToInt32(dtPos.Rows[rowIndex][colTxtPosId]),
                        PosTotalProductQuantity=Convert.ToDecimal(dtPos.Rows[rowIndex][colTxtPosTotalProductQuantity]),
                        PosGrossAmount=Convert.ToDecimal(dtPos.Rows[rowIndex][colTxtPosGrossAmount]),
                        PosDiscount=Convert.ToDecimal(dtPos.Rows[rowIndex][colTxtPosDiscount]),
                        PosVAT=Convert.ToDecimal(dtPos.Rows[rowIndex][colTxtPosVAT]),
                        PosGrandTotal=Convert.ToDecimal(dtPos.Rows[rowIndex][colTxtPosGrandTotal]),
                        PosCustomer=(dtPos.Rows[rowIndex][colPosCustomerId]).ToString(),
                        UserFullName = userFullName,
                        PosDate=Convert.ToDateTime(dtPos.Rows[rowIndex][colPosDate])
                    });
            }
        }
    }
}
