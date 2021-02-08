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
    /// Interaction logic for WinInventoryAdjustment.xaml
    /// </summary>
    public partial class WinInventoryAdjustment : Window
    {
        public WinInventoryAdjustment()
        {
            InitializeComponent();
            FillStaffInformations();
        }

        UserDAL userDAL = new UserDAL();
        PointOfSaleDAL pointOfSaleDAL = new PointOfSaleDAL();
        PointOfSaleCUL pointOfSaleCUL = new PointOfSaleCUL();
        PointOfSaleDetailDAL pointOfSaleDetailDAL = new PointOfSaleDetailDAL();
        PointOfSaleDetailCUL pointOfSaleDetailCUL = new PointOfSaleDetailCUL();
        InventoryAdjustmentDAL inventoryAdjustmentDAL = new InventoryAdjustmentDAL();
        ProductDAL productDAL = new ProductDAL();
        ProductCUL productCUL = new ProductCUL();
        PaymentDAL paymentDAL = new PaymentDAL();
        PaymentCUL paymentCUL = new PaymentCUL();
        CustomerDAL customerDAL = new CustomerDAL();
        CustomerCUL customerCUL = new CustomerCUL();
        UnitDAL unitDAL = new UnitDAL();
        UnitCUL unitCUL = new UnitCUL();

        int btnNewOrEdit;//0 stands for user clicked the button New, and 1 stands for user clicked the button Edit.

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FillStaffInformations()
        {
            txtStaffName.Text = WinLogin.loggedIn;
            txtStaffPosition.Text = WinLogin.loggedInPosition;
        }

        private void ClearProductEntranceTextBox()
        {
            txtProductId.Text = "";
            txtProductName.Text = "";
            txtProductUnit.Text = "";
            txtProductCostPrice.Text = "";
            txtProductSalePrice.Text = "";
            txtProductAmount.Text = "";
            txtProductTotalPrice.Text = "";
            Keyboard.Focus(txtProductId); // set keyboard focus
            DisableProductEntranceButtons();
        }

        private void ClearBasketTextBox()
        {
            txtBasketAmount.Text = "0";
            txtBasketGrandTotal.Text = "0";
        }

        private void ClearInventoryAdjustmentListView()
        {
            dgProducts.Items.Clear();
        }

        private int GetLastInventoryAdjustmentId()
        {
            int specificRowIndex = 0, inventoryAdjustmentId;

            DataTable dataTable = inventoryAdjustmentDAL.Search();//Searching the last id number in the tbl_inventory_adjustment.

            if (dataTable.Rows.Count != 0)//If there is an inventory adjustment id in the database, that means the database table's first row cannot be null, and the datatable table's first index is 0.
            {
                inventoryAdjustmentId = Convert.ToInt32(dataTable.Rows[specificRowIndex]["id"]);//We defined this code out of the for loop below because all of the products has the same invoice number in every sale. So, no need to call this method for every products again and again.
            }
            else//If there is no any inventory adjustment id in the db, that means it is the first record. So, assing inventoryAdjustmentId with 0;
            {
                inventoryAdjustmentId = 0;
            }
            return inventoryAdjustmentId;
        }

        private void LoadNewInventoryAdjustment()/*INVENTORY ADJUSTMENT ID REFERS TO THE ID NUMBER IN THE DATABASE FOR WinInventoryAdjustment.*/
        {
            ClearBasketTextBox();
            ClearInventoryAdjustmentListView();

            int inventoryAdjustmentId, increment = 1;

            inventoryAdjustmentId = GetLastInventoryAdjustmentId();//Getting the last inventory adjustment record id and assign it to the variable called inventoryAdjustmentId.
            inventoryAdjustmentId += increment;//We are adding one to the last inventory adjustment record id because every new inventory adjustment record id is one greater tham the previous one.
            lblIventoryAdjustmentId.Content = inventoryAdjustmentId;//Assigning inventory adjustment record id to the content of the inventory adjustment record id label.
        }

        private void ModifyTools()//Do NOT repeat yourself! You have used IsEnabled function for these toolbox contents many times!
        {
            btnNew.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnPrint.IsEnabled = true;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            txtProductUnit.IsEnabled = true;
            txtProductId.IsEnabled = true;
            txtProductName.IsEnabled = true;
            txtProductSalePrice.IsEnabled = true;
            txtProductAmount.IsEnabled = true;
            txtProductTotalPrice.IsEnabled = true;
            dgProducts.IsHitTestVisible = true;//Enabling the datagrid clicking.

        }
        private void DisableProductEntranceButtons()
        {
            btnProductAdd.IsEnabled = false; //Disabling the add button if all text boxes are cleared.
            btnProductClear.IsEnabled = false; //Disabling the clear button if all text boxes are cleared.
        }

        private void PopulateBasket()
        {
            decimal amountFromProductEntry = Convert.ToDecimal(txtProductAmount.Text);

            txtBasketAmount.Text = (Convert.ToDecimal(txtBasketAmount.Text) + amountFromProductEntry).ToString();

            txtBasketGrandTotal.Text = (Convert.ToDecimal(txtBasketGrandTotal.Text) + (Convert.ToDecimal(txtProductSalePrice.Text) * amountFromProductEntry)).ToString();
        }

        private void txtProductId_KeyUp(object sender, KeyEventArgs e)
        {
            int number;

            DataTable dataTableProduct = productDAL.SearchProductByIdBarcode(txtProductId.Text);

            if (txtProductId.Text != 0.ToString() && int.TryParse(txtProductId.Text, out number) && dataTableProduct.Rows.Count != 0)//Validating the barcode if it is a number(except zero) or not.
            {
                int productAmountInStock;
                int rowIndex = 0;
                int productId;
                int productUnit;
                string productBarcodeRetail;
                string costPrice, salePrice;

                btnProductAdd.IsEnabled = true; //Enabling the add button if any valid barcode is entered.
                btnProductClear.IsEnabled = true;//Enabling the clear button if any valid barcode is entered.


                productId = Convert.ToInt32(dataTableProduct.Rows[rowIndex]["id"]);
                productBarcodeRetail = dataTableProduct.Rows[rowIndex]["barcode_retail"].ToString();
                txtProductName.Text = dataTableProduct.Rows[rowIndex]["name"].ToString();//Filling the product name textbox from the database

                if (productBarcodeRetail == txtProductId.Text || productId.ToString() == txtProductId.Text)//If the barcode equals the product's barcode_retail or id, then take the product's retail unit id.
                {
                    productUnit = Convert.ToInt32(dataTableProduct.Rows[rowIndex]["unit_retail"]);
                }

                else //If the barcode equals to the barcode_wholesale, then take the product's wholesale unit id.
                {
                    productUnit = Convert.ToInt32(dataTableProduct.Rows[rowIndex]["unit_wholesale"]);
                }

                DataTable dataTableProductUnit = unitDAL.GetUnitInfoById(productUnit);//Datatable for finding the unit name by unit id.

                txtProductUnit.Text=dataTableProductUnit.Rows[rowIndex]["name"].ToString();//Populating the textbox with the related unit name from dataTableUnit.
                productAmountInStock = Convert.ToInt32(dataTableProduct.Rows[rowIndex]["amount_in_stock"]);
                costPrice = dataTableProduct.Rows[rowIndex]["costprice"].ToString();
                salePrice = dataTableProduct.Rows[rowIndex]["saleprice"].ToString();

                txtProductCostPrice.Text = costPrice;
                txtProductSalePrice.Text = salePrice;
                txtProductAmountInStock.Text = productAmountInStock.ToString();
            }


            /*
            If the txtProductId is empty which means user has clicked the backspace button and if the txtProductName is filled once before entering an id, then erase all the text contents.
            I just checked the btnProductAdd to know if there was a product entry before or not.
            If the btnProductAdd is not enabled in the if block above once before, then no need to call the method ClearProductEntranceTextBox.*/
            else if (txtProductId.Text == "" && btnProductAdd.IsEnabled == true)
            {
                ClearProductEntranceTextBox();
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            btnNewOrEdit = 0;//0 stands for the user has entered the btnNew.
            LoadNewInventoryAdjustment();
            ModifyTools();
        }

        private void btnProductAdd_Click(object sender, RoutedEventArgs e)
        {
            bool addNewProductLine = true;
            int barcodeColNo = 0;
            int amountColNo = 5;
            int totalCostColNo = 6;
            int totalPriceColNo = 7;
            int amount = 0;
            decimal totalPrice;
            int rowQuntity = dgProducts.Items.Count;

            for (int i = 0; i < rowQuntity; i++)
            {
                DataGridRow row = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(i);

                TextBlock barcodeCellContent = dgProducts.Columns[barcodeColNo].GetCellContent(row) as TextBlock;    //Try to understand this code!!!  

                if (barcodeCellContent.Text == txtProductId.Text)
                {
                    if (MessageBox.Show("There is already the same item in the list. Would you like to sum them?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        TextBlock tbCellAmountContent = dgProducts.Columns[amountColNo].GetCellContent(row) as TextBlock;    //Try to understand this code!!!                         
                        TextBlock tbCellTotalCostContent = dgProducts.Columns[totalCostColNo].GetCellContent(row) as TextBlock;    //Try to understand this code!!! 
                        TextBlock tbCellTotalPriceContent = dgProducts.Columns[totalPriceColNo].GetCellContent(row) as TextBlock;

                        amount = Convert.ToInt32(tbCellAmountContent.Text);
                        amount += Convert.ToInt32(txtProductAmount.Text);//We are adding the amount entered in the "txtProductAmount" to the previous amount cell's amount.

                        tbCellAmountContent.Text = amount.ToString();//Assignment of the new amount to the related cell.
                        
                        
                        tbCellTotalCostContent.Text = (amount * Convert.ToDecimal(txtProductCostPrice.Text)).ToString();
                        totalPrice = amount * Convert.ToDecimal(txtProductSalePrice.Text);//Calculating the new total price according to the new entry. Then, assigning the result into the total price variable. User may have entered a new price in the entry box.
                        tbCellTotalPriceContent.Text = totalPrice.ToString();//Assignment of the total price to the related cell.
                        addNewProductLine = false;
                        break;//We have to break the loop if the user clicked "yes" because no need to scan the rest of the rows after confirming.
                    }
                }
            }


            if (addNewProductLine == true)//Use ENUMS instead of this!!!!!!!
            {
                decimal totalCost = Convert.ToDecimal(txtProductCostPrice.Text) * Convert.ToDecimal(txtProductAmount.Text);
                //dgProducts.Items.Add(new ProductCUL(){ Id = Convert.ToInt32(txtProductId.Text), Name = txtProductName.Text });// You can also apply this code instead of the code below. Note that you have to change the binding name in the datagrid with the name of the property in ProductCUL if you wish to use this code.
                dgProducts.Items.Add(new { Id = txtProductId.Text, Name = txtProductName.Text, Unit = txtProductUnit.Text, CostPrice = txtProductCostPrice.Text, SalePrice = txtProductSalePrice.Text, Amount = txtProductAmount.Text, AmountInStock=txtProductAmountInStock.Text, AmountDifference=txtProductAmountDifference.Text, TotalCostPrice = totalCost.ToString(), TotalSalePrice = txtProductTotalPrice.Text });
            }

            dgProducts.UpdateLayout();
            rowQuntity = dgProducts.Items.Count;//Renewing the row quantity after adding a new product.

            PopulateBasket();

            ClearProductEntranceTextBox();

            //items[0].BarcodeRetail = "EXAMPLECODE"; This code can change the 0th row's data on the column called BarcodeRetail.
        }

        private void txtProductAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtProductAmount.IsFocused == true)//If the cursor is not focused on this textbox, then no need to check this code.
            {
                if (txtProductAmount.Text != "")
                {
                    decimal number;
                    string textProductAmount = txtProductAmount.Text;

                    char lastCharacter = char.Parse(textProductAmount.Substring(textProductAmount.Length - 1));//Getting the last character to check if the user has entered a missing amount like " 3, "

                    bool result = Char.IsDigit(lastCharacter);//Checking if the last digit of the number is a number or not.

                    if (decimal.TryParse(textProductAmount, out number) && result == true)
                    {
                        DataTable dataTable = productDAL.SearchProductByIdBarcode(txtProductId.Text);

                        string unitKg = "Kilogram", unitLt = "Liter";
                        int rowIndex = 0;
                        decimal productAmount;
                        string productSalePrice = dataTable.Rows[rowIndex]["saleprice"].ToString();

                        if (txtProductUnit.Text != unitKg && txtProductUnit.Text != unitLt)
                        {
                            /*If the user entered any unit except kilogram or liter, there cannot be a decimal quantity. 
                            So, convert the quantity to integer even the user has entered a decimal quantity as a mistake.*/
                            productAmount = Convert.ToInt32(Convert.ToDecimal(txtProductAmount.Text));
                            txtProductAmount.Text = productAmount.ToString();
                        }
                        else//If the user has defined the unit as kilogram or liter, then there can be a decimal amount like "3,5 liter."
                        {
                            productAmount = Convert.ToDecimal(txtProductAmount.Text);
                        }

                        txtProductAmountDifference.Text = (Convert.ToDecimal(txtProductAmountInStock.Text) - productAmount).ToString();//Getting the amount difference by subtracting the amount in stock from the current amount.

                        txtProductTotalPrice.Text = (Convert.ToDecimal(productSalePrice) * productAmount).ToString();
                    }

                    else//Revert the amount to the default value if the text of txtProductAmount is not empty, otherwise no need for correction.
                    {
                        MessageBox.Show("Please enter a valid number");
                        txtProductAmount.Text = "1";//We are reverting the amount of the product to default if the user has pressed a wrong key such as "a-b-c".
                        btnProductAdd.IsEnabled = true;
                    }
                }

                /* If the user left the txtProductAmount as empty, wait for him to enter a new value and block the btnProductAdd. 
                   Note: Because the "TextChanged" function works immediately, we don't revert the value into the default. User may click on the "backspace" to correct it by himself"*/
                else
                {
                    btnProductAdd.IsEnabled = false;
                }
            }
        }
    }
}
