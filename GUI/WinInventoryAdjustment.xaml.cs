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
            DisableTools();
            LoadPastInventoryAdjustmentPage();
        }

        UserDAL userDAL = new UserDAL();
        PointOfSaleDAL pointOfSaleDAL = new PointOfSaleDAL();
        PointOfSaleCUL pointOfSaleCUL = new PointOfSaleCUL();
        PointOfSaleDetailDAL pointOfSaleDetailDAL = new PointOfSaleDetailDAL();
        PointOfSaleDetailCUL pointOfSaleDetailCUL = new PointOfSaleDetailCUL();
        InventoryAdjustmentDAL inventoryAdjustmentDAL = new InventoryAdjustmentDAL();
        InventoryAdjustmentCUL inventoryAdjustmentCUL = new InventoryAdjustmentCUL();
        //InventoryAdjustmentDetailDAL inventoryAdjustmentDetailDAL = new InventoryAdjustmentDetailDAL();
        InventoryAdjustmentDetailCUL inventoryAdjustmentDetailCUL = new InventoryAdjustmentDetailCUL();
        ProductDAL productDAL = new ProductDAL();
        ProductCUL productCUL = new ProductCUL();
        PaymentDAL paymentDAL = new PaymentDAL();
        PaymentCUL paymentCUL = new PaymentCUL();
        CustomerDAL customerDAL = new CustomerDAL();
        CustomerCUL customerCUL = new CustomerCUL();
        UnitDAL unitDAL = new UnitDAL();
        UnitCUL unitCUL = new UnitCUL();

        int btnNewOrEdit;//0 stands for user clicked the button New, and 1 stands for user clicked the button Edit.
        string[,] dgOldProductCells = new string[,] { };
        int oldItemsRowCount;

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FillStaffInformations()
        {
            txtStaffName.Text = WinLogin.loggedIn;
            txtStaffPosition.Text = WinLogin.loggedInPosition;
        }

        private void LoadPastInventoryAdjustmentPage(int invoiceNo = 0, int invoiceArrow = -1)//Optional parameter
        {
            int firstRowIndex = 0, productUnitId;
            string productId, productName, productUnitName, productCostPrice, productSalePrice, productAmount, productTotalCostPrice, productTotalSalePrice;

            if (invoiceNo == 0)
            {
                invoiceNo = GetLastInventoryAdjustmentId();//Getting the last invoice number and assign it to the variable called invoiceNo.
            }

            /*WE CANNOT USE ELSE IF FOR THE CODE BELOW! BOTH IF STATEMENTS ABOVE AND BELOVE MUST WORK.*/
            if (invoiceNo != 0)// If the invoice number is still 0 even when we get the last invoice number by using code above, that means this is the first sale and do not run this code block.
            {
                DataTable dataTablePos = pointOfSaleDAL.Search(invoiceNo);
                DataTable dataTablePosDetail = pointOfSaleDetailDAL.Search(invoiceNo);
                DataTable dataTableUnitInfo;
                DataTable dataTableProduct;

                if (dataTablePosDetail.Rows.Count != 0)
                {
                    #region LOADING THE PRODUCT DATA GRID

                    for (int currentRow = firstRowIndex; currentRow < dataTablePosDetail.Rows.Count; currentRow++)
                    {
                        lblIventoryAdjustmentId.Content = dataTablePos.Rows[firstRowIndex]["id"].ToString();

                        productId = dataTablePosDetail.Rows[currentRow]["product_id"].ToString();
                        productUnitId = Convert.ToInt32(dataTablePosDetail.Rows[currentRow]["product_unit_id"]);

                        dataTableUnitInfo = unitDAL.GetUnitInfoById(productUnitId);//Getting the unit name by unit id.
                        productUnitName = dataTableUnitInfo.Rows[firstRowIndex]["name"].ToString();//We use firstRowIndex value for the index number in every loop because there can be only one unit name of a specific id.

                        productCostPrice = dataTablePosDetail.Rows[currentRow]["product_cost_price"].ToString();
                        productSalePrice = dataTablePosDetail.Rows[currentRow]["product_sale_price"].ToString();
                        productAmount = dataTablePosDetail.Rows[currentRow]["amount"].ToString();
                        productTotalCostPrice = (Convert.ToDecimal(productCostPrice) * Convert.ToDecimal(productAmount)).ToString();//We do NOT store the total cost in the db to reduce the storage. Instead of it, we multiply the unit cost with the amount to find the total cost.
                        productTotalSalePrice = (Convert.ToDecimal(productSalePrice) * Convert.ToDecimal(productAmount)).ToString();//We do NOT store the total price in the db to reduce the storage. Instead of it, we multiply the unit price with the amount to find the total price.

                        dataTableProduct = productDAL.SearchById(productId);

                        productName = dataTableProduct.Rows[firstRowIndex]["name"].ToString();//We used firstRowIndex because there can be only one row in the datatable for a specific product.

                        dgProducts.Items.Add(new { Id = productId, Name = productName, Unit = productUnitName, CostPrice = productCostPrice, SalePrice = productSalePrice, Amount = productAmount, TotalCostPrice = productTotalCostPrice, TotalSalePrice = productTotalSalePrice });
                    }
                    #endregion

                    #region FILLING THE PREVIOUS BASKET INFORMATIONS

                    //We used firstRowIndex below as a row name because there can be only one row in the datatable for a specific Invoice.
                    txtBasketAmount.Text = dataTablePos.Rows[firstRowIndex]["total_product_amount"].ToString();
                    txtBasketGrandTotal.Text = dataTablePos.Rows[firstRowIndex]["grand_total"].ToString();

                    #endregion
                }
                else if (dataTablePosDetail.Rows.Count == 0)//If the pos detail row quantity is 0, that means there is no such row so decrease or increase the invoice number according to user preference.
                {
                    if (invoiceArrow == 0)//If the invoice arrow is 0, that means user clicked the previous button.
                    {
                        invoiceNo = invoiceNo - 1;
                    }
                    else
                    {
                        invoiceNo = invoiceNo + 1;
                    }

                    if (invoiceArrow != -1)//If the user has not clicked either previous or next button, then the invoiceArrow will be -1 and no need for recursion.
                    {
                        LoadPastInventoryAdjustmentPage(invoiceNo, invoiceArrow);//Call the method again to get the new past invoice.
                    }

                }
            }

        }

        private void ClearProductEntranceTextBox()
        {
            txtProductId.Text = "";
            txtProductName.Text = "";
            txtProductUnit.Text = "";
            txtProductCostPrice.Text = "";
            txtProductSalePrice.Text = "";
            txtProductAmount.Text = "";
            txtProductAmountInStock.Text = "";
            txtProductAmountDifference.Text = "";
            txtProductTotalSalePrice.Text = "";
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

        private void ModifyToolsOnClickBtnNewOrEdit()//Do NOT repeat yourself! You have used IsEnabled function for these toolbox contents many times!
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
            txtProductCostPrice.IsEnabled = true;
            txtProductSalePrice.IsEnabled = true;
            txtProductAmount.IsEnabled = true;
            txtProductAmountInStock.IsEnabled = true;
            txtProductAmountDifference.IsEnabled = true;
            txtProductTotalSalePrice.IsEnabled = true;
            dgProducts.IsHitTestVisible = true;//Enabling the datagrid clicking.
        }

        private void DisableTools()
        {
            DisableProductEntranceButtons();
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;
            btnPrint.IsEnabled = false;
            txtProductId.IsEnabled = false;
            txtProductName.IsEnabled = false;
            txtProductAmount.IsEnabled = false;
            txtProductUnit.IsEnabled = false;
            txtProductCostPrice.IsEnabled = false;
            txtProductSalePrice.IsEnabled = false;
            txtProductAmount.IsEnabled = false;
            txtProductAmountInStock.IsEnabled = false;
            txtProductAmountDifference.IsEnabled = false;
            txtProductTotalSalePrice.IsEnabled = false;
            dgProducts.IsHitTestVisible = false;//Disabling the datagrid clicking.
        }

        private void EnableButtonsOnClickSaveCancel()
        {
            btnNew.IsEnabled = true;//If the products are saved successfully, enable the new button to be able to add new products.
            btnEdit.IsEnabled = true;//If the products are saved successfully, enable the edit button to be able to edit an existing invoice.
            btnDelete.IsEnabled = true;
            btnPrev.IsEnabled = true;
            btnNext.IsEnabled = true;
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
                int productUnitId;
                string productId, productBarcodeRetail, productName, productUnitName;
                string costPrice, salePrice;

                productId = dataTableProduct.Rows[rowIndex]["id"].ToString();
                productBarcodeRetail = dataTableProduct.Rows[rowIndex]["barcode_retail"].ToString();
                productName = dataTableProduct.Rows[rowIndex]["name"].ToString();//Filling the product name textbox from the database.
                costPrice = dataTableProduct.Rows[rowIndex]["costprice"].ToString();
                salePrice = dataTableProduct.Rows[rowIndex]["saleprice"].ToString();
                productAmountInStock = Convert.ToInt32(dataTableProduct.Rows[rowIndex]["amount_in_stock"]);


                if (productBarcodeRetail == txtProductId.Text || productId.ToString() == txtProductId.Text)//If the barcode equals the product's barcode_retail or id, then take the product's retail unit id.
                {
                    productUnitId = Convert.ToInt32(dataTableProduct.Rows[rowIndex]["unit_retail"]);
                }

                else //If the barcode equals to the barcode_wholesale, then take the product's wholesale unit id.
                {
                    productUnitId = Convert.ToInt32(dataTableProduct.Rows[rowIndex]["unit_wholesale"]);
                }

                DataTable dataTableProductUnit = unitDAL.GetUnitInfoById(productUnitId);//Datatable for finding the unit name by unit id.
                productUnitName = dataTableProductUnit.Rows[rowIndex]["name"].ToString();//Populating the textbox with the related unit name from dataTableUnit.

                txtProductName.Text = productName;
                txtProductUnit.Text = productUnitName;
                txtProductCostPrice.Text = costPrice;
                txtProductSalePrice.Text = salePrice;
                txtProductAmountInStock.Text = productAmountInStock.ToString();


                btnProductAdd.IsEnabled = true; //Enabling the add button if any valid barcode is entered.
                btnProductClear.IsEnabled = true;//Enabling the clear button if any valid barcode is entered.
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
            ModifyToolsOnClickBtnNewOrEdit();
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
                dgProducts.Items.Add(new { Id = txtProductId.Text, Name = txtProductName.Text, Unit = txtProductUnit.Text, CostPrice = txtProductCostPrice.Text, SalePrice = txtProductSalePrice.Text, Amount = txtProductAmount.Text, AmountInStock=txtProductAmountInStock.Text, AmountDifference=txtProductAmountDifference.Text, TotalCostPrice = totalCost.ToString(), TotalSalePrice = txtProductTotalSalePrice.Text });
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
                    char lastCharacter = char.Parse(textProductAmount.Substring(textProductAmount.Length - 1));//Getting the last character to check if the user has entered a missing amount like " 3, ".
                    bool result = Char.IsDigit(lastCharacter);//Checking if the last digit of the number is a number or not.

                    if (decimal.TryParse(textProductAmount, out number) && result == true)
                    {
                        DataTable dataTable = productDAL.SearchProductByIdBarcode(txtProductId.Text);

                        string unitKg = "Kilogram", unitLt = "Liter";
                        int numberZero = 0;
                        decimal productAmount, productAmountInStock= Convert.ToDecimal(txtProductAmountInStock.Text);
                        string productSalePrice = dataTable.Rows[numberZero]["saleprice"].ToString();

                        #region Checking the unit type
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
                        #endregion

                        #region Checking the sign of the amount in stock.
                        if (productAmountInStock < numberZero) //If it is a negative amount, convert it into positive.
                            productAmountInStock = Math.Abs(productAmountInStock);
                        #endregion

                        txtProductAmountDifference.Text = (productAmount - productAmountInStock).ToString();//Getting the amount difference by subtracting the amount in stock from the current amount.
                        txtProductTotalSalePrice.Text = (Convert.ToDecimal(productSalePrice) * productAmount).ToString();
                    }

                    else//Reverting the amount to the default value.
                    {
                        MessageBox.Show("Please enter a valid number");
                        txtProductAmount.Text = "1";//We are reverting the amount of the product to default if the user has pressed a wrong key such as "a-b-c".
                        btnProductAdd.IsEnabled = true;//Once the mistake has been corrected, enable btnProductAdd.
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

        private void btnProductClear_Click(object sender, RoutedEventArgs e)
        {
            ClearProductEntranceTextBox();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Would you really like to cancel the inventory adjustment page?", "Cancel Invoice", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    DisableTools();
                    ClearProductEntranceTextBox();
                    ClearInventoryAdjustmentListView();
                    LoadNewInventoryAdjustment();
                    EnableButtonsOnClickSaveCancel();
                    break;
                case MessageBoxResult.No:
                    MessageBox.Show("Enjoy!", "Enjoy");
                    break;
                case MessageBoxResult.Cancel:
                    MessageBox.Show("Nevermind then...", "KABA Accounting");
                    break;
            }
        }

        int inventoryAdjustmentArrow;
        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            int firstInventoryAdjustmentId = 0, currentInvoiceNo = Convert.ToInt32(lblIventoryAdjustmentId.Content);

            if (currentInvoiceNo != firstInventoryAdjustmentId)
            {
                ClearInventoryAdjustmentListView();
                int prevInventoryAdjustment = Convert.ToInt32(lblIventoryAdjustmentId.Content) - 1;
                inventoryAdjustmentArrow = 0;//0 means customer has clicked the previous button.
                LoadPastInventoryAdjustmentPage(prevInventoryAdjustment, inventoryAdjustmentArrow);
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            int lastInventoryAdjustmentId = GetLastInventoryAdjustmentId(), currentInvoiceNo = Convert.ToInt32(lblIventoryAdjustmentId.Content);

            if (currentInvoiceNo != lastInventoryAdjustmentId)
            {
                ClearInventoryAdjustmentListView();
                int nextInvoice = Convert.ToInt32(lblIventoryAdjustmentId.Content) + 1;
                inventoryAdjustmentArrow = 1;//1 means customer has clicked the next button.
                LoadPastInventoryAdjustmentPage(nextInvoice, inventoryAdjustmentArrow);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string[,] dgNewProductCells = new string[,] { };

            dgNewProductCells = (string[,])(GetDataGridContent().Clone());//Cloning one array into another array.

            #region Comparing two multidimensional arrays
            bool isDgEqual =
            dgOldProductCells.Rank == dgNewProductCells.Rank &&
            Enumerable.Range(0, dgOldProductCells.Rank).All(dimension => dgOldProductCells.GetLength(dimension) == dgNewProductCells.GetLength(dimension)) &&
            dgOldProductCells.Cast<string>().SequenceEqual(dgNewProductCells.Cast<string>());
            #endregion

            //If the old datagrid equals new datagrid, no need for saving because the user did not change anything.
            //-1 means nothing has been chosen in the combobox. Note: We don't add the --&& lblInvoiceNo.Content.ToString()!= "0"-- into the if statement because the invoice label cannot be 0 due to the restrictions.
            if (isDgEqual == false)
            {

                int inventoryAdjustmentId = Convert.ToInt32(lblIventoryAdjustmentId.Content);
                int userId = GetUserId();

                DataTable dataTableLastInvoice = GetLastInvoice();//Getting the last invoice number and assign it to the variable called invoiceId.
                DataTable dataTableProduct = new DataTable();
                DataTable dataTableUnit = new DataTable();

                //Getting the values from the POS Window and fill them into the pointOfSaleCUL.
                inventoryAdjustmentCUL.Id = inventoryAdjustmentId;//The column invoice id in the database is not auto incremental. This is to prevent the number from increasing when the user deletes an existing invoice and creates a new invoice.
                inventoryAdjustmentCUL.TotalProductAmount = Convert.ToInt32(txtBasketAmount.Text);
                inventoryAdjustmentCUL.GrandTotal = Convert.ToDecimal(txtBasketGrandTotal.Text);
                inventoryAdjustmentCUL.AddedDate = DateTime.Now;
                inventoryAdjustmentCUL.AddedBy = userId;

                #region TABLE POS DETAILS SAVING SECTION

                int userClickedNewOrEdit = btnNewOrEdit;
                int cellUnit = 2, cellCostPrice = 3, cellSalePrice = 4, cellProductAmount = 5;
                int productId;
                int unitId;
                decimal productOldAmountInStock;
                int initialRowIndex = 0;
                int cellLength = 7;
                int addedBy = userId;
                string[] cells = new string[cellLength];
                DateTime dateTime = DateTime.Now;
                bool isSuccessDetail = false;
                bool isSuccess = false;
                int productRate = 0;//Modify this code dynamically!!!!!!!!!

                for (int rowNo = 0; rowNo < dgProducts.Items.Count; rowNo++)
                {
                    if (userClickedNewOrEdit == 1)//If the user clicked the btnEdit, then edit the specific invoice's products in tbl_pos_detailed at once.
                    {
                        RevertOldAmountInStock();//Reverting the old products' amount in stock.

                        //We are sending invoiceNo as a parameter to the "Delete" Method. So that we can erase all the products which have the specific invoice number.
                        pointOfSaleDetailDAL.Delete(inventoryAdjustmentId);

                        //2 means null for this code. We used this in order to prevent running the if block again and again. Because, we erase all of the products belong to one invoice number at once.
                        userClickedNewOrEdit = 2;
                    }

                    DataGridRow row = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(rowNo);

                    for (int colNo = 0; colNo < cellLength; colNo++)
                    {
                        TextBlock cellContent = dgProducts.Columns[colNo].GetCellContent(row) as TextBlock;

                        cells[colNo] = cellContent.Text;
                    }

                    dataTableProduct = productDAL.SearchProductByIdBarcode(cells[initialRowIndex]);//Cell[0] may contain the product id or barcode_retail or barcode_wholesale.
                    productId = Convert.ToInt32(dataTableProduct.Rows[initialRowIndex]["id"]);//Row index is always zero for this situation because there can be only one row of a product which has a unique barcode on the table.


                    dataTableUnit = unitDAL.GetUnitInfoByName(cells[cellUnit]);//Cell[2] contains the unit name.
                    unitId = Convert.ToInt32(dataTableUnit.Rows[initialRowIndex]["id"]);//Row index is always zero for this situation because there can be only one row of a specific unit.

                    inventoryAdjustmentDetailCUL.ProductId = productId;
                    inventoryAdjustmentDetailCUL.InventoryAdjustmentId = inventoryAdjustmentId;
                    inventoryAdjustmentDetailCUL.AddedDate = dateTime;
                    inventoryAdjustmentDetailCUL.AddedBy = addedBy;
                    inventoryAdjustmentDetailCUL.ProductUnitId = unitId;
                    inventoryAdjustmentDetailCUL.ProductAmount = Convert.ToDecimal(cells[cellProductAmount]);


                    productCUL.Id = productId;//Assigning the Id in the productCUL to update the stock in the DB of a specific product.

                    productOldAmountInStock = Convert.ToDecimal(dataTableProduct.Rows[initialRowIndex]["amount_in_stock"].ToString());//Getting the old product amount in stock.

                    productCUL.AmountInStock = productOldAmountInStock - Convert.ToDecimal(cells[cellProductAmount]);

                    productDAL.UpdateAmountInStock(productCUL);

                    isSuccessDetail = pointOfSaleDetailDAL.Insert(pointOfSaleDetailCUL);
                }
                #endregion

                userClickedNewOrEdit = btnNewOrEdit;// We are reassigning the btnNewOrEdit value into userClickedNewOrEdit.

                if (userClickedNewOrEdit == 1)//If the user clicked the btnEdit, then update the specific invoice information in tbl_pos at once.
                {
                    isSuccess = pointOfSaleDAL.Update(pointOfSaleCUL);
                }

                else
                {
                    //Creating a Boolean variable to insert data into the database.
                    isSuccess = pointOfSaleDAL.Insert(pointOfSaleCUL);
                }


                //If the data is inserted successfully, then the value of the variable isSuccess will be true; otherwise it will be false.
                if (isSuccess == true && isSuccessDetail == true)//IsSuccessDetail is always CHANGING in every loop above! IMPROVE THIS!!!!
                {
                    //ClearBasketTextBox();
                    //ClearPointOfSaleListView();
                    ClearProductEntranceTextBox();
                    DisableTools();
                    EnableButtonsOnClickSaveCancel();
                }
                else
                {
                    MessageBox.Show("Something went wrong :(");
                }
            }

            else
            {
                MessageBox.Show("You have a missing part or you are trying to save the same things!");
            }
        }
    }
}
