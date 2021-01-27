using KabaAccounting.CUL;
using KabaAccounting.DAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for WinPhyInventory.xaml
    /// </summary>
    public partial class WinPhyInventory : Window
    {
        public WinPhyInventory()
        {
            InitializeComponent();
            LoadLvwPhysicalInventory();
        }

        ProductCUL productCUL = new ProductCUL();
        ProductDAL productDAL = new ProductDAL();

        private void LoadLvwPhysicalInventory()
        {
            DataTable dataTable = productDAL.Select();

            for (int rowIndex = 0; rowIndex < dataTable.Rows.Count; rowIndex++)
            {


                lvwPhyInventory.Items.Add(
                    new ProductCUL()
                    {
                        Id = Convert.ToInt32(dataTable.Rows[rowIndex]["id"]),
                        Name = dataTable.Rows[rowIndex]["name"].ToString(),
                        Category = Convert.ToInt32(dataTable.Rows[rowIndex]["category"]),
                        Rating = Convert.ToDecimal(dataTable.Rows[rowIndex]["rating"]),
                        AmountInStock = Convert.ToInt32(dataTable.Rows[rowIndex]["amount_in_stock"]),
                        CostPrice = Convert.ToDecimal(dataTable.Rows[rowIndex]["costprice"]),
                        SalePrice = Convert.ToDecimal(dataTable.Rows[rowIndex]["saleprice"]),
                        AddedDate = Convert.ToDateTime(dataTable.Rows[rowIndex]["added_date"]),
                        AddedBy = Convert.ToInt32(dataTable.Rows[rowIndex]["added_by"])
                    });
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
