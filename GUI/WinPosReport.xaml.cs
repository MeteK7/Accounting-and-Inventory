using CUL;
using DAL;
using KabaAccounting.CUL;
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

namespace GUI
{
    /// <summary>
    /// Interaction logic for WinPosReport.xaml
    /// </summary>
    public partial class WinPosReport : Window
    {
        ProductDAL productDAL = new ProductDAL();
        ProductCUL productCUL = new ProductCUL();
        PosReportDAL posReportDAL = new PosReportDAL();
        PosReportDetailDAL posReportDetailDAL = new PosReportDetailDAL();
        PointOfSaleDAL pointOfSaleDAL = new PointOfSaleDAL();
        PointOfSaleDetailDAL pointOfSaleDetailDAL = new PointOfSaleDetailDAL();
        string dateFrom, dateTo;

        public WinPosReport()
        {
            InitializeComponent();
            dtpPosReportFrom.SelectedDate = DateTime.Now;
            dtpPosReportTo.SelectedDate = DateTime.Now;
            LoadRectangles();
            LoadDataGrid();
            
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LoadRectangles()
        {
            bool cash = true, credit = false;
            
            dateFrom = String.Format("{0:yyyy-MM-dd HH:mm:ss}", dtpPosReportFrom.SelectedDate);
            dateTo = String.Format("{0:yyyy-MM-dd HH:mm:ss}", dtpPosReportTo.SelectedDate);

            lblCashSales.Content = pointOfSaleDAL.CountPaymentTypeByToday(dateFrom, dateTo, cash);//Send the variable cash to the parameter of the CountByDay method in the ProductDAL.

            lblCreditSales.Content = pointOfSaleDAL.CountPaymentTypeByToday(dateFrom, dateTo, credit);//Send the variable credit to the parameter of the CountByDay method in the ProductDAL.

            lblNumOfSalesVar.Content = Convert.ToInt32(lblCashSales.Content) + Convert.ToInt32(lblCreditSales.Content);//Sum cash and credit amount to find the total number of sales.

            DataTable dtPos = pointOfSaleDAL.JoinReportByDate(dateFrom, dateTo);

            for (int rowIndex = 0; rowIndex < dtPos.Rows.Count; rowIndex++)
            {
                lblCostVar.Content = Convert.ToDecimal(lblCostVar.Content) + Convert.ToDecimal(dtPos.Rows[rowIndex]["cost_total"]);
                lblRevenueVar.Content = Convert.ToDecimal(lblRevenueVar.Content) + Convert.ToDecimal(dtPos.Rows[rowIndex]["grand_total"]);
            }

            lblProfitVar.Content = Convert.ToDecimal(lblRevenueVar.Content) - Convert.ToDecimal(lblCostVar.Content);
        }

        private void LoadDataGrid()
        {
            string productId;
            int initialIndex = 0;
            bool addNew = true;
            IEnumerable items;
            dateFrom = String.Format("{0:yyyy-MM-dd HH:mm:ss}", dtpPosReportFrom.SelectedDate);
            dateTo = String.Format("{0:yyyy-MM-dd HH:mm:ss}", dtpPosReportTo.SelectedDate);

            lvwTopProducts.Items.Clear();

            DataTable dtPosJoined = pointOfSaleDAL.JoinReportByDate(dateFrom, dateTo);
            DataTable dtProduct;

            for (int posIndex = 0; posIndex < dtPosJoined.Rows.Count; posIndex++)
            {
                addNew = true;
                items = this.lvwTopProducts.Items;


                foreach (PosReportDetailCUL product in items)//This loop is for preventing duplications.
                {
                    if (product.ProductId == Convert.ToInt32(dtPosJoined.Rows[posIndex]["product_id"]))
                    {
                        product.ProductQuantitySold += Convert.ToDecimal(dtPosJoined.Rows[posIndex]["quantity"]);
                        product.ProductTotalSalePrice = String.Format("{0:0.00}",
                            Convert.ToDecimal(product.ProductTotalSalePrice) +
                            (Convert.ToDecimal(dtPosJoined.Rows[posIndex]["product_sale_price"]) * Convert.ToDecimal(dtPosJoined.Rows[posIndex]["quantity"])));
                        addNew = false;//No need to run the loop again since there can be only one entry of a unique product.
                        break;
                    }
                }

                if (addNew == true)
                {
                    productId = dtPosJoined.Rows[posIndex]["product_id"].ToString();
                    dtProduct = productDAL.SearchById(productId);

                    lvwTopProducts.Items.Add(
                        new PosReportDetailCUL()
                        {
                            //InvoiceId = Convert.ToInt32(dataTablePosDetailToday.Rows[posDetailIndex]["invoice_no"]),
                            ProductId = Convert.ToInt32(dtPosJoined.Rows[posIndex]["product_id"]),
                            ProductName = dtProduct.Rows[initialIndex]["name"].ToString(),
                            ProductQuantitySold = Convert.ToDecimal(dtPosJoined.Rows[posIndex]["quantity"]),
                            ProductTotalSalePrice = String.Format("{0:0.00}", (Convert.ToDecimal(dtPosJoined.Rows[posIndex]["product_sale_price"]) * Convert.ToDecimal(dtPosJoined.Rows[posIndex]["quantity"])))
                        });
                }
            }
        }

        private void dtpPosReportFrom_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadRectangles();
            LoadDataGrid();
        }

        private void dtpPosReportTo_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadRectangles();
            LoadDataGrid();
        }
    }
}
