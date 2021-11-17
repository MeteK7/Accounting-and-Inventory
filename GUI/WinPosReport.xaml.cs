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

        public WinPosReport()
        {
            InitializeComponent();
            LoadRectangles();
            LoadDataGrid();
            dtpPosReportTo.SelectedDate = DateTime.Today;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void LoadRectangles()
        {
            bool cash = true, credit = false;

            lblCashSales.Content = pointOfSaleDAL.CountPaymentTypeByToday(cash);//Send the variable cash to the parameter of the CountByDay method in the ProductDAL.

            lblCreditSales.Content = pointOfSaleDAL.CountPaymentTypeByToday(credit);//Send the variable credit to the parameter of the CountByDay method in the ProductDAL.

            lblNumOfSalesVar.Content = Convert.ToInt32(lblCashSales.Content) + Convert.ToInt32(lblCreditSales.Content);//Sum cash and credit amount to find the total number of sales.

            DataTable dtPos = pointOfSaleDAL.FetchReportByDate(Convert.ToDateTime(dtpPosReportFrom.SelectedDate), Convert.ToDateTime(dtpPosReportTo.SelectedDate));

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

            lvwTopProducts.Items.Clear();

            DataTable dtPos = pointOfSaleDAL.FetchReportByDate(Convert.ToDateTime(dtpPosReportFrom.SelectedDate), Convert.ToDateTime(dtpPosReportTo.SelectedDate));
            DataTable dtPosDetail;
            DataTable dtProduct;

            for (int posIndex = 0; posIndex < dtPos.Rows.Count; posIndex++)
            {
                dtPosDetail = pointOfSaleDetailDAL.Search(Convert.ToInt32(dtPos.Rows[posIndex]["id"]));

                for (int posDetailIndex = 0; posDetailIndex < dtPosDetail.Rows.Count; posDetailIndex++)
                {
                    addNew = true;
                    items = this.lvwTopProducts.Items;


                    foreach (PosReportDetailCUL product in items)//This loop is for preventing duplications.
                    {
                        if (product.ProductId==Convert.ToInt32(dtPosDetail.Rows[posDetailIndex]["product_id"]))
                        {
                            product.ProductQuantitySold += Convert.ToDecimal(dtPosDetail.Rows[posDetailIndex]["quantity"]);
                            product.ProductTotalSalePrice = String.Format("{0:0.00}",
                                Convert.ToDecimal(product.ProductTotalSalePrice)+
                                (Convert.ToDecimal(dtPosDetail.Rows[posDetailIndex]["product_sale_price"]) * Convert.ToDecimal(dtPosDetail.Rows[posDetailIndex]["quantity"])));
                            addNew = false;//No need to run the loop again since there can be only one entry of a unique product.
                            break;
                        }
                    }

                    if (addNew == true)
                    {
                        productId = dtPosDetail.Rows[posDetailIndex]["product_id"].ToString();
                        dtProduct = productDAL.SearchById(productId);

                        lvwTopProducts.Items.Add(
                            new PosReportDetailCUL()
                            {
                                //InvoiceId = Convert.ToInt32(dataTablePosDetailToday.Rows[posDetailIndex]["invoice_no"]),
                                ProductId = Convert.ToInt32(dtPosDetail.Rows[posDetailIndex]["product_id"]),
                                ProductName = dtProduct.Rows[initialIndex]["name"].ToString(),
                                ProductQuantitySold = Convert.ToDecimal(dtPosDetail.Rows[posDetailIndex]["quantity"]),
                                ProductTotalSalePrice = String.Format("{0:0.00}", (Convert.ToDecimal(dtPosDetail.Rows[posDetailIndex]["product_sale_price"]) * Convert.ToDecimal(dtPosDetail.Rows[posDetailIndex]["quantity"])))
                            });
                    }
                }
            }
        }

        private void dtpPosReportFrom_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageBox.Show(Convert.ToString(dtpPosReportFrom.SelectedDate));
        }
    }
}
