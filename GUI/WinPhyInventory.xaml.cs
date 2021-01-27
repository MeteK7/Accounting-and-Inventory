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

        CategoryDAL categoryDAL = new CategoryDAL();
        ProductCUL productCUL = new ProductCUL();
        ProductDAL productDAL = new ProductDAL();

        private void LoadLvwPhysicalInventory()
        {
            DataTable dataTableProduct = productDAL.Select();
            DataTable dataTableCategory;

            int categoryId;
            int rowFirstIndex = 0;
            string categoryName;

            for (int rowIndex = 0; rowIndex < dataTableProduct.Rows.Count; rowIndex++)
            {
                categoryId = Convert.ToInt32(dataTableProduct.Rows[rowIndex]["category"]);//Getting the category id first to find its name.
                dataTableCategory = categoryDAL.GetCategoryInfoById(categoryId);//Getting all of the category infos by using id.
                categoryName = dataTableCategory.Rows[rowFirstIndex]["name"].ToString();//Fetching the name of the category from dataTableCategory. Index is always zero since we are dealing with a unique category only.

                lvwPhyInventory.Items.Add(
                    new ProductCUL()
                    {
                        Id = Convert.ToInt32(dataTableProduct.Rows[rowIndex]["id"]),
                        Name = dataTableProduct.Rows[rowIndex]["name"].ToString(),
                        CategoryName = categoryName,
                        Rating = Convert.ToDecimal(dataTableProduct.Rows[rowIndex]["rating"]),
                        AmountInStock = Convert.ToInt32(dataTableProduct.Rows[rowIndex]["amount_in_stock"]),
                        CostPrice = Convert.ToDecimal(dataTableProduct.Rows[rowIndex]["costprice"]),
                        SalePrice = Convert.ToDecimal(dataTableProduct.Rows[rowIndex]["saleprice"]),
                        AddedDate = Convert.ToDateTime(dataTableProduct.Rows[rowIndex]["added_date"]),
                        AddedBy = Convert.ToInt32(dataTableProduct.Rows[rowIndex]["added_by"])
                    });
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
