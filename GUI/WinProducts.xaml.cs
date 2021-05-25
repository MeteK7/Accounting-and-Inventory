using BLL;
using KabaAccounting.CUL;
using KabaAccounting.DAL;
using System;
using System.Collections;
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
            LoadProductDataGrid();
            PopulateCboUnits();
        }

        ProductCUL productCUL = new ProductCUL();
        ProductDAL productDAL = new ProductDAL();
        CategoryBLL categoryBLL = new CategoryBLL();
        CategoryDAL categoryDAL = new CategoryDAL();
        UserDAL userDAL = new UserDAL();
        UnitDAL unitDAL = new UnitDAL();
        UserBLL userBLL = new UserBLL();
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LoadProductDataGrid()
        {
            int firstRowIndex = 0, productId, categoryId, unitRetailId, unitWholesaleId, addedById;
            string productName, categoryName, unitNameRetail, unitNameWholesale, description, quantityInUnitWholesale, quantityInStock, costPrice, salePrice, addedDate, addedByUsername, barcodeRetail, barcodeWholesale;
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
                unitNameRetail = dataTableUnitInfo.Rows[firstRowIndex]["Name"].ToString();

                dataTableUnitInfo = unitDAL.GetUnitInfoById(unitWholesaleId);
                unitNameWholesale = dataTableUnitInfo.Rows[firstRowIndex]["Name"].ToString();

                dataTableCategoryInfo = categoryDAL.GetCategoryInfoById(categoryId);
                categoryName = dataTableCategoryInfo.Rows[firstRowIndex]["Name"].ToString();

                barcodeRetail = dataTable.Rows[currentRow]["barcode_retail"].ToString();
                barcodeWholesale = dataTable.Rows[currentRow]["barcode_wholesale"].ToString();
                productName = dataTable.Rows[currentRow]["name"].ToString();
                description = dataTable.Rows[currentRow]["description"].ToString();
                quantityInUnitWholesale = dataTable.Rows[currentRow]["quantity_in_unit"].ToString();
                quantityInStock = dataTable.Rows[currentRow]["quantity_in_stock"].ToString();
                costPrice = dataTable.Rows[currentRow]["costprice"].ToString();
                salePrice = dataTable.Rows[currentRow]["saleprice"].ToString();
                addedDate = dataTable.Rows[currentRow]["added_date"].ToString();

                addedById = Convert.ToInt32(dataTable.Rows[currentRow]["added_by"]);
                dataTableUserInfo = userDAL.GetUserInfoById(addedById);
                addedByUsername = dataTableUserInfo.Rows[firstRowIndex]["first_name"].ToString() + " " + dataTableUserInfo.Rows[firstRowIndex]["last_name"].ToString();

                dtgProducts.Items.Add(new { Id = productId, BarcodeRetail = barcodeRetail, BarcodeWholesale = barcodeWholesale, Name = productName, CategoryName = categoryName, Description = description, QuantityInUnitWholesale = quantityInUnitWholesale, QuantityInStock = quantityInStock, CostPrice = costPrice, SalePrice = salePrice, AddedDate = addedDate, AddedBy = addedByUsername, UnitNameRetail = unitNameRetail, UnitNameWholesale = unitNameWholesale });
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
            txtProductQuantityInUnitWholesale.Text = "";
            txtProductCostPriceWholesale.Text = "";
            txtProductSalePriceWholesale.Text = "";
            txtProductSearch.Text = "";
        }

        private void DtgProductsIndexChanged()//Getting the index of a particular row and fill the text boxes with the related columns of the row.
        {
            object row = dtgProducts.SelectedItem;
            int rowId = 0, rowBarcodeRetail = 1, rowBarcodeWholesale = 2, rowProductName = 3, rowProductCategory = 4, rowProductDescription = 5, rowPrQuantityInUnWhol = 6, rowPrCostPriceRet = 8, rowPrSalePriceRet = 9, rowProductUnitRet = 12, rowProductUnitWhol = 13;

            txtProductId.Text = (dtgProducts.Columns[rowId].GetCellContent(row) as TextBlock).Text;
            txtProductBarcodeRetail.Text = (dtgProducts.Columns[rowBarcodeRetail].GetCellContent(row) as TextBlock).Text;
            txtProductBarcodeWholesale.Text = (dtgProducts.Columns[rowBarcodeWholesale].GetCellContent(row) as TextBlock).Text;
            txtProductName.Text = (dtgProducts.Columns[rowProductName].GetCellContent(row) as TextBlock).Text;
            cboProductCategory.Text = (dtgProducts.Columns[rowProductCategory].GetCellContent(row) as TextBlock).Text;
            txtProductDescription.Text = (dtgProducts.Columns[rowProductDescription].GetCellContent(row) as TextBlock).Text;//ANOTHER METHOD: (dtgProducts.SelectedCells[rowProductDescription].Column.GetCellContent(row) as TextBlock).Text;
            txtProductQuantityInUnitWholesale.Text = (dtgProducts.Columns[rowPrQuantityInUnWhol].GetCellContent(row) as TextBlock).Text;
            txtProductCostPriceRetail.Text = (dtgProducts.Columns[rowPrCostPriceRet].GetCellContent(row) as TextBlock).Text;
            txtProductSalePriceRetail.Text = (dtgProducts.Columns[rowPrSalePriceRet].GetCellContent(row) as TextBlock).Text;
            cboProductUnitRetail.Text = (dtgProducts.Columns[rowProductUnitRet].GetCellContent(row) as TextBlock).Text;
            cboProductUnitWholesale.Text = (dtgProducts.Columns[rowProductUnitWhol].GetCellContent(row) as TextBlock).Text;
            txtProductCostPriceWholesale.Text = CalculateTotalCostPrice().ToString();
            txtProductSalePriceWholesale.Text = CalculateTotalSalePrice().ToString();
        }

        private decimal CalculateTotalCostPrice()
        {
            if (txtProductCostPriceRetail.Text != "")
            {
                decimal quantity = Convert.ToDecimal(txtProductQuantityInUnitWholesale.Text);
                decimal costPriceRetail = Convert.ToDecimal(txtProductCostPriceRetail.Text);

                return quantity * costPriceRetail;
            }
            else
            {
                return 0;
            }
        }

        private decimal CalculateTotalSalePrice()
        {
            if (txtProductSalePriceRetail.Text != "")
            {
                decimal quantity = Convert.ToDecimal(txtProductQuantityInUnitWholesale.Text);
                decimal salePriceRetail = Convert.ToDecimal(txtProductSalePriceRetail.Text);

                return salePriceRetail * quantity;
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
                dtgProducts.Items.Clear();

                //Show product informations based on the keyword
                DataTable dataTableProduct = productDAL.SelectAllOrByKeyword(keyword: keyword);//The first "keyword" is the parameter name, and the second "keyword" is the local variable.

                for (int rowIndex = 0; rowIndex < dataTableProduct.Rows.Count; rowIndex++)
                {
                    dtgProducts.Items.Add(
                        new ProductCUL()
                        {
                            Id = Convert.ToInt32(dataTableProduct.Rows[rowIndex]["id"]),
                            BarcodeRetail = dataTableProduct.Rows[rowIndex]["barcode_retail"].ToString(),
                            BarcodeWholesale = dataTableProduct.Rows[rowIndex]["barcode_wholesale"].ToString(),
                            Name = dataTableProduct.Rows[rowIndex]["name"].ToString(),
                            CategoryName = categoryBLL.GetCategoryName(dataTableProduct, rowIndex),
                            Description = dataTableProduct.Rows[rowIndex].ToString()
                        });
                }
            }
            else
            {
                //Show all products from the database.
                LoadProductDataGrid();
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

        private void RunCRUD(string btnType)
        {
            bool isSuccess = false;//Defaultly, it is false.
            string message;

            // THIS IS NOT AN EFFICIENT CODE BLOCK.
            if (txtProductName.Text != "" && cboProductCategory.SelectedValue != null && cboProductUnitRetail.SelectedValue != null && cboProductUnitWholesale.SelectedValue != null && txtProductQuantityInUnitWholesale.Text != "" && txtProductCostPriceRetail.Text != "" && txtProductSalePriceRetail.Text != "")
            {
                productCUL.Name = txtProductName.Text;
                productCUL.CategoryId = Convert.ToInt32(cboProductCategory.SelectedValue); //SelectedValue Property helps you to get the hidden value of Combobox selected Item.
                productCUL.UnitRetail = Convert.ToInt32(cboProductUnitRetail.SelectedValue);
                productCUL.UnitWholesale = Convert.ToInt32(cboProductUnitWholesale.SelectedValue);
                productCUL.QuantityInUnit = Convert.ToDecimal(txtProductQuantityInUnitWholesale.Text);//You can also use ===> Convert.ToInt32(txtProductQuantityInUnitWholesale.Text)
                productCUL.CostPrice = Convert.ToDecimal(txtProductCostPriceRetail.Text);
                productCUL.SalePrice = Convert.ToDecimal(txtProductSalePriceRetail.Text);
                productCUL.Description = txtProductDescription.Text;
                productCUL.BarcodeRetail = txtProductBarcodeRetail.Text;
                productCUL.BarcodeWholesale = txtProductBarcodeWholesale.Text;
                productCUL.AddedDate = DateTime.Now;
                productCUL.AddedBy = userBLL.GetUserId(WinLogin.loggedInUserName);

                #region BUTTON CHECKING PART
                if (btnType == "Add")
                {
                    int initialQuantity = 0;
                    productCUL.Rating = initialQuantity;
                    productCUL.QuantityInStock = Convert.ToDecimal(initialQuantity);//Quantity in stock is always 0 while recording a new product.

                    isSuccess = productDAL.Insert(productCUL);
                    message = "Product has been added successfully.";
                }
                else
                {
                    productCUL.Id = Convert.ToInt32(txtProductId.Text);

                    if (btnType == "Update")
                    {
                        isSuccess = productDAL.Update(productCUL);
                        message = "Product has been updated successfully.";
                    }

                    else
                    {
                        isSuccess = productDAL.Delete(productCUL);
                        message = "Product has been deleted successfully.";
                    }
                }
                #endregion

                if (isSuccess == true)
                {
                    MessageBox.Show(message);
                    dtgProducts.Items.Clear();
                    ClearProductTextBox();
                    LoadProductDataGrid();
                }
            }

            //If any of the properties is null/unassigned in the ProductCUL class, we cannot insert/update a product.
            //bool isNull = productCUL.GetType().GetProperties()
            //                .All(p => p.GetValue(productCUL) != null); 

            else
            {
                MessageBox.Show("Something went wrong :(");
            }
        }
        private void btnProductAdd_Click(object sender, RoutedEventArgs e)
        {
            RunCRUD("Add");
        }

        private void btnProductUpdate_Click(object sender, RoutedEventArgs e)
        {
            RunCRUD("Update");
        }

        private void btnProductDelete_Click(object sender, RoutedEventArgs e)
        {
            RunCRUD("Delete");
        }

        private void txtProductQuantityInUnitWholesale_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtProductQuantityInUnitWholesale.Text != "")
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
            if (txtProductQuantityInUnitWholesale.Text != "")
            {
                txtProductCostPriceWholesale.Text = CalculateTotalCostPrice().ToString();
            }
        }

        private void txtProductSalePriceRetail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtProductQuantityInUnitWholesale.Text != "")
            {
                txtProductSalePriceWholesale.Text = CalculateTotalSalePrice().ToString();
            }
        }

        private void PromptBarcodeRetail()
        {
            int initialQuantity = 0;

            DataTable dtProduct = productDAL.SearchDuplications(txtProductBarcodeRetail.Text);

            if (dtProduct.Rows.Count!= initialQuantity)
            {
                string message = "There is already such product!\n Id: " + dtProduct.Rows[initialQuantity]["id"] + "\nName: " + dtProduct.Rows[initialQuantity]["name"];

                MessageBox.Show(message);

                txtProductBarcodeRetail.Text = "";
            }
        }

        private void PromptBarcodeWholesale()
        {
            int initialQuantity = 0;

            DataTable dtProduct = productDAL.SearchDuplications(txtProductBarcodeWholesale.Text);

            if (dtProduct.Rows.Count != initialQuantity)
            {
                string message = "There is already such product!\n Id: " + dtProduct.Rows[initialQuantity]["id"] + "\nName: " + dtProduct.Rows[initialQuantity]["name"];

                MessageBox.Show(message);

                txtProductBarcodeWholesale.Text = "";
            }
        }

        private void txtProductBarcodeRetail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PromptBarcodeRetail();
            }
        }

        private void txtProductBarcodeWholesale_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PromptBarcodeWholesale();
            }
        }
    }
}
