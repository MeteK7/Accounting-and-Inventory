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

        string searchBy; //Fix the naming!!!
        int initialCboIndex=-1;
        bool canLoadListView = true;

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private string ConvertCategoryIdIntoName(DataTable dataTableProduct, int rowIndex)
        {
            DataTable dataTableCategory;

            int categoryId;
            int rowFirstIndex = 0;
            string categoryName;

            categoryId = Convert.ToInt32(dataTableProduct.Rows[rowIndex]["category_id"]);//Getting the category id first to find its name.
            dataTableCategory = categoryDAL.GetCategoryInfoById(categoryId);//Getting all of the category infos by using id.
            categoryName = dataTableCategory.Rows[rowFirstIndex]["name"].ToString();//Fetching the name of the category from dataTableCategory. Index is always zero since we are dealing with a unique category only.

            return categoryName;
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
                        CategoryName = ConvertCategoryIdIntoName(dataTableProduct, rowIndex),
                        Rating = Convert.ToDecimal(dataTableProduct.Rows[rowIndex]["rating"]),
                        AmountInStock = Convert.ToInt32(dataTableProduct.Rows[rowIndex]["amount_in_stock"]),
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
