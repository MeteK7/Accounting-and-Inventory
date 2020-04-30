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
    /// Interaction logic for WinProducts.xaml
    /// </summary>
    public partial class WinProducts : Window
    {
        public WinProducts()
        {
            InitializeComponent();
            RefreshProductDataGrid();
        }

        ProductBLL productBLL = new ProductBLL();
        ProductDAL productDAL = new ProductDAL();
        CategoryDAL categoryDAL = new CategoryDAL();
        UserDAL userDAL = new UserDAL();

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RefreshProductDataGrid()
        {
            //Refreshing Data Grid View
            DataTable dataTable = productDAL.Select();
            dtgProducts.ItemsSource = dataTable.DefaultView;
            dtgProducts.AutoGenerateColumns = true;
            dtgProducts.CanUserAddRows = false;
        }

        private void ClearProductTextBox()
        {
            txtProductId.Text = "";
            txtProductName.Text = "";
            cboProductCategory.SelectedIndex = -1;
            txtProductRate.Text = "";
            txtProductDescription.Text = "";
            txtProductSearch.Text = "";    
        }

        private void DtgProductsIndexChanged()
        {
            //Getting the index of a particular row and fill the text boxes with the related columns of the row.

            //int rowIndex = dtgCategories.SelectedIndex;

            DataRowView drv = (DataRowView)dtgProducts.SelectedItem;

            txtProductId.Text = (drv[0]).ToString();//Selecting the specific row
            txtProductName.Text = (drv["name"]).ToString();//You could also define the column name from your table.
            cboProductCategory.SelectedValue = (drv[2]).ToString();
            txtProductDescription.Text = (drv[3]).ToString();
            txtProductRate.Text = (drv[4]).ToString();
        }

        private void dtgProducts_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DtgProductsIndexChanged();
        }

        private void dtgProducts_KeyUp(object sender, KeyEventArgs e)
        {
            DtgProductsIndexChanged();
        }

        private void dtgProducts_KeyDown(object sender, KeyEventArgs e)
        {
            DtgProductsIndexChanged();
        }

        private void txtProductSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Get Keyword from Text box
            string keyword = txtProductSearch.Text;

            //Check if the keyword has value or not

            if (keyword != null) /*Do NOT Repeat yourself!!! Improve if statement block!!! You have similar codes in the RefreshCategoryDataGrid method!!! */
            {
                //Show category informations based on the keyword
                DataTable dataTable = productDAL.Search(keyword);
                dtgProducts.ItemsSource = dataTable.DefaultView;
                dtgProducts.AutoGenerateColumns = true;
                dtgProducts.CanUserAddRows = false;
            }
            else
            {
                //Show all products from the database
                RefreshProductDataGrid();
            }
        }

        private void cboProductCategory_Loaded(object sender, RoutedEventArgs e)
        {
            //Creating Data Table to hold the products from Databaase
            DataTable dataTable = categoryDAL.Select();

            //Specifying Items Source for product combobox
            cboProductCategory.ItemsSource = dataTable.DefaultView;

            //Here DisplayMemberPath helps to display Text in the ComboBox.
            cboProductCategory.DisplayMemberPath = "title";

            //SelectedValuePath helps to store values like a hidden field.
            cboProductCategory.SelectedValuePath = "id";
        }
        private int GetUserId()//You used this method in WinProducts, as well. You can Make an external class just for this!!!.
        {
            //Getting the name of the user from the Login Window and fill it into a string variable;
            string loggedUser = WinLogin.loggedIn;

            //Calling the method named GetIdFromUsername in the userDAL and sending the variable loggedUser as a parameter into it.
            //Then, fill the result into the userBLL;
            UserBLL userBLL = userDAL.GetIdFromUsername(loggedUser);

            int userId = userBLL.Id;

            return userId;
        }
        private void btnProductAdd_Click(object sender, RoutedEventArgs e)
        {
            productBLL.Name = txtProductName.Text;
            productBLL.Category = Convert.ToInt32(cboProductCategory.SelectedValue); //SelectedValue Property helps you to get the hidden value of Combobox selected Item.
            productBLL.Description = txtProductDescription.Text;
            productBLL.Rate = decimal.Parse(txtProductRate.Text); //You can also use ===> Convert.ToDecimal(txtProductRate.Text)
            productBLL.Quantity = 0;
            productBLL.AddedDate = DateTime.Now;
            productBLL.AddedBy = GetUserId();

            bool isSuccess = productDAL.Insert(productBLL);

            if (isSuccess==true)
            {
                MessageBox.Show("Product has been added successfully.");
                ClearProductTextBox();
                RefreshProductDataGrid();
            }

            else
            {
                MessageBox.Show("Something went wrong :(");
            }
        }

        private void btnProductUpdate_Click(object sender, RoutedEventArgs e)
        {
            //Getting values from the WinProducts

            productBLL.Id = Convert.ToInt32(txtProductId.Text);
            productBLL.Name = txtProductName.Text;
            productBLL.Category = Convert.ToInt32(cboProductCategory.SelectedValue);
            productBLL.Rate = Decimal.Parse(txtProductRate.Text);
            productBLL.Description = txtProductDescription.Text;
            productBLL.AddedDate = DateTime.Now;
            productBLL.AddedBy = GetUserId();

            //Updating Data into the database
            bool isSuccess = productDAL.Update(productBLL);

            if (isSuccess == true)
            {
                MessageBox.Show("Product successfully updated");
                ClearProductTextBox();
                RefreshProductDataGrid();
            }
            else
            {
                MessageBox.Show("Failed to update product");
            }
        }

        private void btnProductDelete_Click(object sender, RoutedEventArgs e)
        {
            productBLL.Id = Convert.ToInt32(txtProductId.Text);

            bool isSuccess = productDAL.Delete(productBLL);

            if (isSuccess == true)
            {
                MessageBox.Show("Product has been deleted successfully.");
                ClearProductTextBox();
                RefreshProductDataGrid();
            }
            else
            {
                MessageBox.Show("Something went wrong:/");
            }
        }
    }
}
