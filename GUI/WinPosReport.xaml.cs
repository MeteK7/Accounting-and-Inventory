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
        public WinPosReport()
        {
            InitializeComponent();
            LoadRectangles();
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
    }
}
