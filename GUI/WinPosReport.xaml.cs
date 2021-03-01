using CUL;
using DAL;
using KabaAccounting.CUL;
using KabaAccounting.DAL;
using System;
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

            DataTable dataTablePosToday = pointOfSaleDAL.FetchReportByToday();

            for (int rowIndex = 0; rowIndex < dataTablePosToday.Rows.Count; rowIndex++)
            {
                lblCostVar.Content = Convert.ToDecimal(lblCostVar.Content) + Convert.ToDecimal(dataTablePosToday.Rows[rowIndex]["cost_total"]);
                lblRevenueVar.Content = Convert.ToDecimal(lblRevenueVar.Content) + Convert.ToDecimal(dataTablePosToday.Rows[rowIndex]["grand_total"]);
            }

            lblProfitVar.Content = Convert.ToDecimal(lblRevenueVar.Content) - Convert.ToDecimal(lblCostVar.Content);
        }

        private void LoadDataGrid()
        {
            string productId;
            int initialIndex = 0;

            lvwTopProducts.Items.Clear();

            DataTable dataTablePosToday = pointOfSaleDAL.FetchReportByToday();
            DataTable dataTablePosDetailToday;
            DataTable dataTableProduct;

            for (int posIndex = 0; posIndex < dataTablePosToday.Rows.Count; posIndex++)
            {
                dataTablePosDetailToday = pointOfSaleDetailDAL.Search(Convert.ToInt32(dataTablePosToday.Rows[posIndex]["id"]));

                for (int posDetailIndex = 0; posDetailIndex < dataTablePosDetailToday.Rows.Count; posDetailIndex++)
                {
                    productId = dataTablePosDetailToday.Rows[posDetailIndex]["product_id"].ToString();
                    dataTableProduct = productDAL.SearchById(productId);

                    lvwTopProducts.Items.Add(
                        new PosReportDetailCUL()
                        {
                            InvoiceId = Convert.ToInt32(dataTablePosDetailToday.Rows[posDetailIndex]["invoice_no"]),
                            //ProductId = Convert.ToInt32(dataTablePosDetailToday.Rows[posDetailIndex]["product_id"]),
                            ProductName = dataTableProduct.Rows[initialIndex]["name"].ToString(),
                            ProductAmountSold = Convert.ToDecimal(dataTablePosDetailToday.Rows[posDetailIndex]["amount"]),
                        });
                }
            }
        }
    }
}
