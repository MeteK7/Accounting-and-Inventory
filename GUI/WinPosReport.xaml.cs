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

            lblCashSales.Content = productDAL.CountPaymentTypeByToday(cash);//Send the variable cash to the parameter of the CountByDay method in the ProductDAL.

            lblCreditSales.Content = productDAL.CountPaymentTypeByToday(credit);//Send the variable credit to the parameter of the CountByDay method in the ProductDAL.

            lblNumOfSalesVar.Content = Convert.ToInt32(lblCashSales.Content) + Convert.ToInt32(lblCreditSales.Content);//Sum cash and credit amount to find the total number of sales.

            DataTable dataTableProduct = productDAL.FetchReportByToday();

            for (int rowIndex = 0; rowIndex < dataTableProduct.Rows.Count; rowIndex++)
            {
                lblCostVar.Content = Convert.ToDecimal(lblCostVar.Content) + Convert.ToDecimal(dataTableProduct.Rows[rowIndex]["cost_total"]);
                lblRevenueVar.Content = Convert.ToDecimal(lblRevenueVar.Content) + Convert.ToDecimal(dataTableProduct.Rows[rowIndex]["grand_total"]);
            }

            lblProfitVar.Content = Convert.ToDecimal(lblRevenueVar.Content) - Convert.ToDecimal(lblCostVar.Content);
        }

        private void LoadDataGrid()
        {
            string productId;
            int rowIndex = 0;

            lvwTopProducts.Items.Clear();

            DataTable dateTimePosReport = posReportDAL.FetchIdByDate(DateTime.Now.ToString("MM/dd/yyyy"));
            DataTable dataTablePosReportDetail = posReportDetailDAL.SearchBySaleDateId(Convert.ToInt32(dateTimePosReport.Rows[rowIndex]["id"]));
            DataTable dataTableProduct;

            for (rowIndex = 0; rowIndex < dataTablePosReportDetail.Rows.Count; rowIndex++)
            {
                productId = dataTablePosReportDetail.Rows[rowIndex]["product_id"].ToString();
                dataTableProduct = productDAL.SearchById(productId);

                lvwTopProducts.Items.Add(
                    new PosReportDetailCUL()
                    {
                        SaleDateId = Convert.ToInt32(dataTablePosReportDetail.Rows[rowIndex]["report_id"]),
                        ProductId = Convert.ToInt32(dataTablePosReportDetail.Rows[rowIndex]["product_id"]),
                        ProductName= dataTableProduct.Rows[rowIndex]["name"].ToString(),
                        ProductAmountSold = Convert.ToDecimal(dataTablePosReportDetail.Rows[rowIndex]["product_amount_sold"]),
                    });
            }
        }
    }
}
