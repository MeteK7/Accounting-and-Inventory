using BLL;
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
        CategoryBLL categoryBLL = new CategoryBLL();
        CategoryDAL categoryDAL = new CategoryDAL();
        ProductDAL productDAL = new ProductDAL();

        string searchBy=null; //Fix the naming!!!
        int initialCboIndex=-1;
        bool canLoadListView = true;

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LoadLvwPhysicalInventory(string searchBy=null, string keyword=null)
        {
            lvwPhyInventory.Items.Clear();

            #region AssignSearchBy

            string selectedIndex = cboSearchByCategory.SelectedIndex.ToString();

            if (selectedIndex != initialCboIndex.ToString())
                searchBy = cboSearchByCategory.SelectedValue.ToString();

            #endregion


            DataTable dataTableProduct = productDAL.SelectAllOrByKeyword(searchBy, keyword);//If keyword is null, then fetch all data.

            for (int rowIndex = 0; rowIndex < dataTableProduct.Rows.Count; rowIndex++)
            {
                lvwPhyInventory.Items.Add(
                    new ProductCUL()
                    {
                        Id = Convert.ToInt32(dataTableProduct.Rows[rowIndex]["id"]),
                        Name = dataTableProduct.Rows[rowIndex]["name"].ToString(),
                        CategoryName = categoryBLL.GetCategoryName(dataTableProduct, rowIndex),
                        Rating = Convert.ToDecimal(dataTableProduct.Rows[rowIndex]["rating"]),
                        QuantityInStock = Convert.ToInt32(dataTableProduct.Rows[rowIndex]["quantity_in_stock"]),
                        CostPrice = Convert.ToDecimal(dataTableProduct.Rows[rowIndex]["costprice"]),
                        SalePrice = Convert.ToDecimal(dataTableProduct.Rows[rowIndex]["saleprice"]),
                        AddedDate = Convert.ToDateTime(dataTableProduct.Rows[rowIndex]["added_date"]),
                        AddedBy = Convert.ToInt32(dataTableProduct.Rows[rowIndex]["added_by"])
                    });
            }
        }
        private void txtSearchByKeyword_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (canLoadListView == true)
                LoadLvwPhysicalInventory(searchBy, txtSearchByKeyword.Text);
        }

        private void cboSearchByCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (canLoadListView == true)
                LoadLvwPhysicalInventory(searchBy, txtSearchByKeyword.Text);
        }

        private void cboSearchByCategory_Loaded(object sender, RoutedEventArgs e)
        {
            //Creating Data Table to hold the products from Database
            DataTable dataTable = categoryDAL.Select();

            //Specifying Items Source for product combobox
            cboSearchByCategory.ItemsSource = dataTable.DefaultView;

            //Here DisplayMemberPath helps to display Text in the ComboBox.
            cboSearchByCategory.DisplayMemberPath = "name";

            //SelectedValuePath helps to store values like a hidden field.
            cboSearchByCategory.SelectedValuePath = "id";
        }

        private void btnResetFilters_Click(object sender, RoutedEventArgs e)
        {
            canLoadListView = false; //Do not load the listview in order to block the txt and cbo change triggers.
            txtSearchByKeyword.Text = null;
            cboSearchByCategory.SelectedIndex = initialCboIndex;
            LoadLvwPhysicalInventory();
            canLoadListView = true;//You can now let the listview to be populated in case of any changes in the txt and cbo part.
        }
    }
}
