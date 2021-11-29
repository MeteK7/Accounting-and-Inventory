using CUL;
using DAL;
using CUL;
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

namespace GUI
{
    /// <summary>
    /// Interaction logic for WinPopReport.xaml
    /// </summary>
    public partial class WinPopReport : Window
    {
        UserDAL userDAL = new UserDAL();
        ProductDAL productDAL = new ProductDAL();
        ProductCUL productCUL = new ProductCUL();
        PopReportDetailCUL popReportDetailCUL = new PopReportDetailCUL();
        PointOfPurchaseDAL pointOfPurchaseDAL = new PointOfPurchaseDAL();
        PointOfPurchaseDetailDAL pointOfPurchaseDetailDAL = new PointOfPurchaseDetailDAL();
        string dateFrom, dateTo;
        const int initialIndex = 0;

        public WinPopReport()
        {
            InitializeComponent();
            LoadDefaultDate();
            LoadPayments();
            LoadListView();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            dateFrom = String.Format("{0:yyyy-MM-dd}", dtpPopReportFrom.SelectedDate) + " " + String.Format("{0:HH:mm:ss}", timePickerFrom.Value);
            dateTo = String.Format("{0:yyyy-MM-dd}", dtpPopReportTo.SelectedDate) + " " + String.Format("{0:HH:mm:ss}", timePickerTo.Value);

            LoadPayments();
            LoadListView();
        }

        private void LoadDefaultDate()
        {
            dtpPopReportFrom.SelectedDate = DateTime.Today;
            dtpPopReportTo.SelectedDate = DateTime.Today;
            timePickerFrom.Value = Convert.ToDateTime("00:00:00");
            timePickerTo.Value = Convert.ToDateTime("23:59:59");

            dateFrom = String.Format("{0:yyyy-MM-dd}", dtpPopReportFrom.SelectedDate) + " " + String.Format("{0:HH:mm:ss}", timePickerFrom.Value);
            dateTo = String.Format("{0:yyyy-MM-dd}", dtpPopReportTo.SelectedDate) + " " + String.Format("{0:HH:mm:ss}", timePickerTo.Value);
        }

        private void ClearGroupBoxes()
        {
            lblNumOfPurchasesVar.Content = initialIndex.ToString();
            lblCostVar.Content = initialIndex.ToString();
            lblRevenueVar.Content = initialIndex.ToString();
        }

        private void LoadPayments()
        {
            ClearGroupBoxes();

            #region NUMBER OF SALES
            bool cash = true, credit = false;

            lblCashPurchases.Content = pointOfPurchaseDAL.CountPaymentTypeByToday(dateFrom, dateTo, cash);//Send the variable cash to the parameter of the CountByDay method in the ProductDAL.

            lblCreditPurchases.Content = pointOfPurchaseDAL.CountPaymentTypeByToday(dateFrom, dateTo, credit);//Send the variable credit to the parameter of the CountByDay method in the ProductDAL.

            lblNumOfPurchasesVar.Content = Convert.ToInt32(lblCashPurchases.Content) + Convert.ToInt32(lblCreditPurchases.Content);//Sum cash and credit amount to find the total number of purchases.
            #endregion

            #region PAYMENT RESULTS
            DataTable dtPop = pointOfPurchaseDAL.FetchReportByDate(dateFrom, dateTo);

            for (int rowIndex = 0; rowIndex < dtPop.Rows.Count; rowIndex++)
            {
                lblCostVar.Content = Convert.ToDecimal(lblCostVar.Content) + Convert.ToDecimal(dtPop.Rows[rowIndex]["cost_total"]);
                lblRevenueVar.Content = Convert.ToDecimal(lblRevenueVar.Content) + Convert.ToDecimal(dtPop.Rows[rowIndex]["grand_total"]);
            }
            #endregion

            #region USER SALES
            lvwUserPurchases.Items.Clear();//Clearing the listview before loading new datas in case the user changes the dates.
            DataTable dtUserPurchases = pointOfPurchaseDAL.SumAmountByUserBetweenDates(dateFrom, dateTo);
            int userId;
            decimal userPurchaseAmount;
            string userFullName;

            for (int rowIndex = 0; rowIndex < dtUserPurchases.Rows.Count; rowIndex++)
            {
                userId = Convert.ToInt32(dtUserPurchases.Rows[rowIndex]["added_by"]);
                DataTable dtUsers = userDAL.GetUserInfoById(userId);

                userFullName = dtUsers.Rows[initialIndex]["first_name"].ToString() + " " + dtUsers.Rows[initialIndex]["last_name"].ToString();

                userPurchaseAmount = Convert.ToDecimal(dtUserPurchases.Rows[rowIndex]["grand_total"]);

                lvwUserPurchases.Items.Add(
                    new PopReportDetailCUL()
                    {
                        UserFullName = userFullName,
                        UserPurchaseAmount = userPurchaseAmount
                    });
            }
            #endregion
        }

        private void LoadListView()
        {
            string productId;
            int initialIndex = 0;
            bool addNew = true;
            IEnumerable items;

            lvwUserPurchases.Items.Clear();

            DataTable dtPopJoined = pointOfPurchaseDAL.JoinReportByDate(dateFrom, dateTo);
            DataTable dtProduct;

            for (int rowIndex = 0; rowIndex < dtPopJoined.Rows.Count; rowIndex++)
            {
                addNew = true;
                items = lvwUserPurchases.Items;


                foreach (PopReportDetailCUL product in items)//This loop is for preventing duplications.
                {
                    if (product.ProductId == Convert.ToInt32(dtPopJoined.Rows[rowIndex]["product_id"]))
                    {
                        product.ProductQuantityPurchased += Convert.ToDecimal(dtPopJoined.Rows[rowIndex]["quantity"]);
                        product.ProductTotalCostPrice = String.Format("{0:0.00}",
                            Convert.ToDecimal(product.ProductTotalCostPrice) +
                            (Convert.ToDecimal(dtPopJoined.Rows[rowIndex]["product_purchase_price"]) * Convert.ToDecimal(dtPopJoined.Rows[rowIndex]["quantity"])));
                        addNew = false;//No need to run the loop again since there can be only one entry of a unique product.
                        break;
                    }
                }

                if (addNew == true)
                {
                    productId = dtPopJoined.Rows[rowIndex]["product_id"].ToString();
                    dtProduct = productDAL.SearchById(productId);

                    lvwUserPurchases.Items.Add(
                        new PopReportDetailCUL()
                        {
                            //InvoiceId = Convert.ToInt32(dataTablePopDetailToday.Rows[posDetailIndex]["invoice_no"]),
                            ProductId = Convert.ToInt32(dtPopJoined.Rows[rowIndex]["product_id"]),
                            ProductName = dtProduct.Rows[initialIndex]["name"].ToString(),
                            ProductQuantityPurchased = Convert.ToDecimal(dtPopJoined.Rows[rowIndex]["quantity"]),
                            ProductTotalCostPrice = String.Format("{0:0.00}", (Convert.ToDecimal(dtPopJoined.Rows[rowIndex]["product_purchase_price"]) * Convert.ToDecimal(dtPopJoined.Rows[rowIndex]["quantity"])))
                        });
                }
            }
        }
    }
}
