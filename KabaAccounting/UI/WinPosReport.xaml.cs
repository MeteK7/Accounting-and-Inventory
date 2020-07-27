using KabaAccounting.BLL;
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

namespace KabaAccounting.UI
{
    /// <summary>
    /// Interaction logic for WinPosReport.xaml
    /// </summary>
    public partial class WinPosReport : Window
    {
        ProductDAL productDAL = new ProductDAL();
        ProductBLL productBLL = new ProductBLL();

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
            DataTable dataTableProduct = productDAL.FetchByToday();
            
            lblNumOfSalesVar.Content = productDAL.CountByDay();

            for (int rowIndex = 0; rowIndex < dataTableProduct.Rows.Count; rowIndex++)
            {
                lblCostVar.Content = Convert.ToDecimal(lblCostVar.Content)+ Convert.ToDecimal(dataTableProduct.Rows[rowIndex]["cost_total"]);
            }
        }
    }
}
