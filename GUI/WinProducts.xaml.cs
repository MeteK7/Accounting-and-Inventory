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
    /// Interaction logic for WinProducts.xaml
    /// </summary>
    public partial class WinProducts : Window
    {
        public WinProducts()
        {
            InitializeComponent();
            RefreshProductDataGrid();
            PopulateCboUnits();
        }

        ProductCUL productCUL = new ProductCUL();
        ProductDAL productDAL = new ProductDAL();
        CategoryDAL categoryDAL = new CategoryDAL();
        UserDAL userDAL = new UserDAL();
        UnitDAL unitDAL = new UnitDAL();
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RefreshProductDataGrid()
        {
            int firstRowIndex = 0, productId, categoryId, unitRetailId, unitWholesaleId, addedById;
            string productName, categoryName, amountInUnitWholesale, amountInStock, costPrice, salePrice, unitRetailName, unitWholesaleName, addedDate, addedByUsername;
            DataTable dataTable = productDAL.SelectAllOrByKeyword();
            DataTable dataTableCategoryInfo;
            DataTable dataTableUnitInfo;
            DataTable dataTableUserInfo;

            //dtgs.ItemsSource = dataTable.DefaultView; Adds everything at once.
            dtgProducts.AutoGenerateColumns = true;
            dtgProducts.CanUserAddRows = false;

            #region LOADING THE PRODUCT DATA GRID

            for (int currentRow = firstRowIndex; currentRow < dataTable.Rows.Count; currentRow++)
            {
                productId = Convert.ToInt32(dataTable.Rows[currentRow]["id"]);
                categoryId = Convert.ToInt32(dataTable.Rows[currentRow]["category_id"]);
                unitRetailId = Convert.ToInt32(dataTable.Rows[currentRow]["unit_retail_id"]);
                unitWholesaleId = Convert.ToInt32(dataTable.Rows[currentRow]["unit_wholesale_id"]);

                dataTableUnitInfo = unitDAL.GetUnitInfoById(unitRetailId);
                unitRetailName = dataTableUnitInfo.Rows[firstRowIndex]["Name"].ToString();

                dataTableUnitInfo = unitDAL.GetUnitInfoById(unitWholesaleId);
                unitWholesaleName = dataTableUnitInfo.Rows[firstRowIndex]["Name"].ToString();

                dataTableCategoryInfo = categoryDAL.GetCategoryInfoById(categoryId);
                categoryName = dataTableCategoryInfo.Rows[firstRowIndex]["Name"].ToString();

                productName = dataTable.Rows[currentRow]["name"].ToString();
                amountInUnitWholesale = dataTable.Rows[currentRow]["amount_in_unit"].ToString();
                amountInStock = dataTable.Rows[currentRow]["amount_in_stock"].ToString();
                costPrice = dataTable.Rows[currentRow]["costprice"].ToString();
                salePrice = dataTable.Rows[currentRow]["saleprice"].ToString();
                addedDate = dataTable.Rows[currentRow]["added_date"].ToString();


                addedById = Convert.ToInt32(dataTable.Rows[currentRow]["added_by"]);
                dataTableUserInfo = userDAL.GetUserInfoById(addedById);
                addedByUsername = dataTableUserInfo.Rows[firstRowIndex]["first_name"].ToString() + " " + dataTableUserInfo.Rows[firstRowIndex]["last_name"].ToString();

                dtgProducts.Items.Add(new { Id = productId, Name = productName, CategoryName = categoryName, AmountInUnitWholesale = amountInUnitWholesale, AmountInStock = amountInStock, CostPrice = costPrice, SalePrice = salePrice, AddedDate = addedDate, AddedBy = addedByUsername });
            }
            #endregion
        }

        private void ClearProductTextBox()
        {
            txtProductId.Text = "";
            cboProductCategory.SelectedIndex = -1;
            txtProductDescription.Text = "";
            cboProductUnitRetail.SelectedIndex = -1;
            txtProductBarcodeRetail.Text = "";
            txtProductName.Text = "";
            txtProductCostPriceRetail.Text = "";
            txtProductSalePriceRetail.Text = "";
            cboProductUnitWholesale.SelectedIndex = -1;
            txtProductBarcodeWholesale.Text = "";
            txtProductAmount.Text = "";
            txtProductCostPriceWholesale.Text = "";
            txtProductSalePriceWholesale.Text = "";
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
            txtProductBarcodeRetail.Text = (drv[5]).ToString();
            txtProductBarcodeWholesale.Text = (drv[6]).ToString();
            txtProductAmount.Text = (drv[7]).ToString();
            txtProductCostPriceRetail.Text = (drv[9]).ToString();
            txtProductSalePriceRetail.Text = (drv[10]).ToString();
            cboProductUnitRetail.SelectedValue = (drv[11]);
            cboProductUnitWholesale.SelectedValue = (drv[12]);
            txtProductCostPriceWholesale.Text = CalculateTotalCostPrice().ToString();
            txtProductSalePriceWholesale.Text = CalculateTotalSalePrice().ToString();
        }

        private double CalculateTotalCostPrice()
        {
            if (txtProductCostPriceRetail.Text != "")
            {
                int amount = Convert.ToInt32(txtProductAmount.Text);
                double costPriceRetail = Convert.ToDouble(txtProductCostPriceRetail.Text);

                return amount * costPriceRetail;
            }
            else
            {
                return 0;
            }

        }

        private double CalculateTotalSalePrice()
        {
            if (txtProductSalePriceRetail.Text != "")
            {
                int amount = Convert.ToInt32(txtProductAmount.Text);
                double salePriceRetail = Convert.ToDouble(txtProductSalePriceRetail.Text);

                return salePriceRetail * amount;
            }

            else
            {
                return 0;
            }

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
                DataTable dataTable = productDAL.SelectAllOrByKeyword(keyword);
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
            //Creating Data Table to hold the products from Database
            DataTable dataTable = categoryDAL.Select();

            //Specifying Items Source for product combobox
            cboProductCategory.ItemsSource = dataTable.DefaultView;

            //Here DisplayMemberPath helps to display Text in the ComboBox.
            cboProductCategory.DisplayMemberPath = "name";

            //SelectedValuePath helps to store values like a hidden field.
            cboProductCategory.SelectedValuePath = "id";
        }

        private void PopulateCboUnits()
        {
            //Creating Data Table to hold the products from Database
            DataTable dataTable = unitDAL.Select();

            //Specifying Items Source for product combobox
            cboProductUnitRetail.ItemsSource = dataTable.DefaultView;
            cboProductUnitWholesale.ItemsSource = dataTable.DefaultView;

            //Here DisplayMemberPath helps to display Text in the ComboBox.
            cboProductUnitRetail.DisplayMemberPath = "name";
            cboProductUnitWholesale.DisplayMemberPath = "name";

            //SelectedValuePath helps to store values like a hidden field.
            cboProductUnitRetail.SelectedValuePath = "id";
            cboProductUnitWholesale.SelectedValuePath = "id";
        }

        private int GetUserId()//You used this method in WinProducts, as well. You can Make an external class just for this!!!.
        {
            //Getting the name of the user from the Login Window and fill it into a string variable;
            string loggedUser = WinLogin.loggedIn;

            //Calling the method named GetIdFromUsername in the userDAL and sending the variable loggedUser as a parameter into it.
            //Then, fill the result into the userCUL;
            UserCUL userCUL = userDAL.GetIdFromUsername(loggedUser);

            int userId = userCUL.Id;

            return userId;
        }
        private void btnProductAdd_Click(object sender, RoutedEventArgs e)
        {
            int initialAmount = 0;

            productCUL.Name = txtProductName.Text;
            productCUL.CategoryId = Convert.ToInt32(cboProductCategory.SelectedValue); //SelectedValue Property helps you to get the hidden value of Combobox selected Item.
            productCUL.Description = txtProductDescription.Text;
            productCUL.Rating = 0;
            productCUL.BarcodeRetail = txtProductBarcodeRetail.Text;
            productCUL.BarcodeWholesale = txtProductBarcodeWholesale.Text;
            productCUL.AmountInUnit = int.Parse(txtProductAmount.Text);//You can also use ===> Convert.ToInt32(txtProductAmount.Text)
            productCUL.AmountInStock = Convert.ToDecimal(initialAmount);//Amount in stock is always 0 while recording a new product.
            productCUL.CostPrice = Convert.ToDecimal(txtProductCostPriceRetail.Text);
            productCUL.SalePrice = Convert.ToDecimal(txtProductSalePriceRetail.Text);
            productCUL.UnitRetail = Convert.ToInt32(cboProductUnitRetail.SelectedValue);
            productCUL.UnitWholesale = Convert.ToInt32(cboProductUnitWholesale.SelectedValue);
            productCUL.AddedDate = DateTime.Now;
            productCUL.AddedBy = GetUserId();

            bool isSuccess = productDAL.Insert(productCUL);

            if (isSuccess == true)
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

            productCUL.Id = Convert.ToInt32(txtProductId.Text);
            productCUL.Name = txtProductName.Text;
            productCUL.CategoryId = Convert.ToInt32(cboProductCategory.SelectedValue); //SelectedValue Property helps you to get the hidden value of Combobox selected Item.
            productCUL.Description = txtProductDescription.Text;
            productCUL.BarcodeRetail = txtProductBarcodeRetail.Text;
            productCUL.BarcodeWholesale = txtProductBarcodeWholesale.Text;
            productCUL.AmountInUnit = int.Parse(txtProductAmount.Text);//You can also use ===> Convert.ToInt32(txtProductAmount.Text)
            productCUL.CostPrice = Convert.ToDecimal(txtProductCostPriceRetail.Text);
            productCUL.SalePrice = Convert.ToDecimal(txtProductSalePriceRetail.Text);
            productCUL.UnitRetail = Convert.ToInt32(cboProductUnitRetail.SelectedValue);
            productCUL.UnitWholesale = Convert.ToInt32(cboProductUnitWholesale.SelectedValue);
            productCUL.AddedDate = DateTime.Now;
            productCUL.AddedBy = GetUserId();

            //Updating Data into the database
            bool isSuccess = productDAL.Update(productCUL);

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
            productCUL.Id = Convert.ToInt32(txtProductId.Text);

            bool isSuccess = productDAL.Delete(productCUL);

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

        private void txtProductAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtProductAmount.Text != "")
            {
                txtProductCostPriceWholesale.Text = CalculateTotalCostPrice().ToString();
                txtProductSalePriceWholesale.Text = CalculateTotalSalePrice().ToString();
            }
            else
            {
                txtProductCostPriceWholesale.Text = "";
                txtProductSalePriceWholesale.Text = "";
            }
        }

        private void txtProductCostPriceRetail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtProductAmount.Text != "")
            {
                txtProductCostPriceWholesale.Text = CalculateTotalCostPrice().ToString();
            }
        }

        private void txtProductSalePriceRetail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtProductAmount.Text != "")
            {
                txtProductSalePriceWholesale.Text = CalculateTotalSalePrice().ToString();
            }
        }
    }
}
