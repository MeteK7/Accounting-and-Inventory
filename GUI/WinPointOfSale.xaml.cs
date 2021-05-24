using BLL;
using DAL;
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
    /// Interaction logic for WinPointOfSale.xaml
    /// </summary>
    public partial class WinPointOfSale : Window
    {
        UserDAL userDAL = new UserDAL();
        UserBLL userBLL = new UserBLL();
        PointOfSaleDAL pointOfSaleDAL = new PointOfSaleDAL();
        PointOfSaleCUL pointOfSaleCUL = new PointOfSaleCUL();
        PointOfSaleDetailDAL pointOfSaleDetailDAL = new PointOfSaleDetailDAL();
        PointOfSaleDetailCUL pointOfSaleDetailCUL = new PointOfSaleDetailCUL();
        ProductDAL productDAL = new ProductDAL();
        ProductCUL productCUL = new ProductCUL();
        PaymentDAL paymentDAL = new PaymentDAL();
        PaymentCUL paymentCUL = new PaymentCUL();
        CustomerDAL customerDAL = new CustomerDAL();
        CustomerCUL customerCUL = new CustomerCUL();
        UnitDAL unitDAL = new UnitDAL();
        UnitCUL unitCUL = new UnitCUL();
        PointOfSaleBLL pointOfSaleBLL = new PointOfSaleBLL();

        ProductBLL productBLL = new ProductBLL();
        AccountDAL accountDAL = new AccountDAL();

        public WinPointOfSale()
        {
            InitializeComponent();
            DisableTools();
            LoadUserInformations();
            LoadPastInvoice();
        }

        int invoiceArrow;
        int clickedNewOrEdit, clickedNew = 0, clickedEdit = 1;//0 stands for user clicked the button New, and 1 stands for user clicked the button Edit.
        string[,] dgOldProductCells = new string[,] { };
        string calledBy = "WinPOS", colQtyNameInDb = "quantity_in_stock";
        decimal newQuantity;

        CommonBLL commonBLL = new CommonBLL();
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnProductClear_Click(object sender, RoutedEventArgs e)
        {
            ClearProductEntranceTextBox();

        }

        private void FirstTimeRun()
        {
            MessageBox.Show("Welcome!\n Thank you for choosing Kaba Accounting and Inventory System.");
            btnEditRecord.IsEnabled = false;//There cannot be any editable records for the first run.
            btnDeleteRecord.IsEnabled = false;//There cannot be any deletible records for the first run.
            btnPrev.IsEnabled = false;//Disabling the btnPrev button because there is no any records in the database for the first time.
            btnNext.IsEnabled = false;//Disabling the btnNext button because there is no any records in the database for the first time.
        }

        private void LoadUserInformations()
        {
            txtUsername.Text = WinLogin.loggedInUserName;
            txtUserType.Text = WinLogin.loggedInUserType;
        }

        private void RefreshProductDataGrid()
        {
            //Refreshing Data Grid View
            DataTable dataTable = productDAL.SelectAllOrByKeyword();
            dgProducts.ItemsSource = dataTable.DefaultView;
            dgProducts.AutoGenerateColumns = true;
            dgProducts.CanUserAddRows = false;
        }

        private void DisableProductEntranceButtons()
        {
            btnProductAdd.IsEnabled = false; //Disabling the add button if all text boxes are cleared.
            btnProductClear.IsEnabled = false; //Disabling the clear button if all text boxes are cleared.
        }

        private void EnableButtonsOnClickSaveCancel()
        {
            btnNew.IsEnabled = true;//If the products are saved successfully, enable the new button to be able to add new products.
            btnEditRecord.IsEnabled = true;//If the products are saved successfully, enable the edit button to be able to edit an existing invoice.
            btnDeleteRecord.IsEnabled = true;
            btnPrev.IsEnabled = true;
            btnNext.IsEnabled = true;
        }

        private void DisableTools()
        {
            DisableProductEntranceButtons();
            dgProducts.IsHitTestVisible = false;//Disabling the datagrid clicking.
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;
            btnPrint.IsEnabled = false;
            cboMenuPaymentType.IsEnabled = false;
            cboMenuCustomer.IsEnabled = false;
            cboMenuAccount.IsEnabled = false;
            cboProductUnit.IsEnabled = false;
            txtProductId.IsEnabled = false;
            txtProductName.IsEnabled = false;
            txtProductSalePrice.IsEnabled = false;
            txtProductAmount.IsEnabled = false;
            txtProductTotalPrice.IsEnabled = false;
        }

        private void ModifyToolsOnClickBtnNewOrEdit()//Do NOT repeat yourself! You have used IsEnabled function for these toolbox contents many times!
        {
            btnNew.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnEditRecord.IsEnabled = false;
            btnDeleteRecord.IsEnabled = false;
            btnPrint.IsEnabled = true;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            cboMenuPaymentType.IsEnabled = true;
            cboMenuCustomer.IsEnabled = true;
            cboMenuAccount.IsEnabled = true;
            cboProductUnit.IsEnabled = true;
            txtProductId.IsEnabled = true;
            txtProductName.IsEnabled = true;
            txtProductSalePrice.IsEnabled = true;
            txtProductAmount.IsEnabled = true;
            txtProductTotalPrice.IsEnabled = true;
            dgProducts.IsHitTestVisible = true;//Enabling the datagrid clicking.
        }

        private void ClearProductsDataGrid()
        {
            dgProducts.Items.Clear();
        }

        private void ClearBasketTextBox()
        {
            txtBasketAmount.Text = "0";
            txtBasketCostTotal.Text = "0";
            txtBasketSubTotal.Text = "0";
            txtBasketVat.Text = "0";
            txtBasketDiscount.Text = "0";
            txtBasketGrandTotal.Text = "0";
        }

        private void ClearProductEntranceTextBox()
        {
            txtProductId.Text = "";
            txtProductName.Text = "";
            cboProductUnit.SelectedIndex = -1;
            txtProductCostPrice.Text = "";
            txtProductSalePrice.Text = "";
            txtProductAmount.Text = "";
            txtProductTotalPrice.Text = "";
            Keyboard.Focus(txtProductId); // set keyboard focus
            DisableProductEntranceButtons();
        }

        private void LoadNewInvoice()/*INVOICE NUMBER REFERS TO THE ID NUMBER IN THE DATABASE FOR POINT OF SALE.*/
        {
            ClearBasketTextBox();
            ClearProductsDataGrid();

            int invoiceId, increment = 1;

            invoiceId = commonBLL.GetLastRecordById(calledBy);//Getting the last invoice number and assign it to the variable called invoiceNo.
            invoiceId += increment;//We are adding one to the last invoice number because every new invoice number is one greater tham the previous invoice number.
            lblInvoiceId.Content = invoiceId;//Assigning invoiceNo to the content of the InvoiceNo Label.
        }

        private void LoadPastInvoice(int invoiceId = 0, int invoiceArrow = -1)//Optional parameter
        {
            int initalIndex = 0, productUnitId;
            string productId, productName, productUnitName, productCostPrice, productSalePrice, productAmount, productTotalCostPrice, productTotalSalePrice;

            if (invoiceId == initalIndex)//If the ID is 0 came from the optional parameter, that means user just clicked the WinPOS button to open it.
            {
                invoiceId = commonBLL.GetLastRecordById(calledBy);//Getting the last invoice id and assign it to the variable called invoiceId.
            }

            /*WE CANNOT USE ELSE IF FOR THE CODE BELOW! BOTH IF STATEMENTS ABOVE AND BELOVE MUST WORK.*/
            if (invoiceId != initalIndex)// If the invoice number is still 0 even when we get the last invoice number by using code above, that means this is the first sale and do not run this code block.
            {
                DataTable dataTablePos = pointOfSaleDAL.GetByIdOrLastId(invoiceId);

                if (dataTablePos.Rows.Count != initalIndex)
                {
                    DataTable dataTablePosDetail = pointOfSaleDetailDAL.Search(invoiceId);
                    DataTable dataTableUnitInfo;
                    DataTable dataTableProduct;

                    cboMenuPaymentType.SelectedValue = Convert.ToInt32(dataTablePos.Rows[initalIndex]["payment_type_id"].ToString());//Getting the id of purchase type.
                    cboMenuCustomer.SelectedValue = Convert.ToInt32(dataTablePos.Rows[initalIndex]["customer_id"].ToString());//Getting the id of customer.
                    cboMenuAccount.SelectedValue = Convert.ToInt32(dataTablePos.Rows[initalIndex]["account_id"].ToString());//Getting the id of account.
                    lblInvoiceId.Content = dataTablePos.Rows[initalIndex]["id"].ToString();

                    #region LOADING THE PRODUCT DATA GRID
                    for (int currentRow = initalIndex; currentRow < dataTablePosDetail.Rows.Count; currentRow++)
                    {
                        productId = dataTablePosDetail.Rows[currentRow]["product_id"].ToString();
                        productUnitId = Convert.ToInt32(dataTablePosDetail.Rows[currentRow]["product_unit_id"]);

                        dataTableUnitInfo = unitDAL.GetUnitInfoById(productUnitId);//Getting the unit name by unit id.
                        productUnitName = dataTableUnitInfo.Rows[initalIndex]["name"].ToString();//We use initalIndex value for the index number in every loop because there can be only one unit name of a specific id.

                        productCostPrice = dataTablePosDetail.Rows[currentRow]["product_cost_price"].ToString();
                        productSalePrice = dataTablePosDetail.Rows[currentRow]["product_sale_price"].ToString();
                        productAmount = dataTablePosDetail.Rows[currentRow]["amount"].ToString();
                        productTotalCostPrice = String.Format("{0:0.00}", (Convert.ToDecimal(productCostPrice) * Convert.ToDecimal(productAmount)));//We do NOT store the total cost in the db to reduce the storage. Instead of it, we multiply the unit cost with the amount to find the total cost.
                        productTotalSalePrice = String.Format("{0:0.00}", (Convert.ToDecimal(productSalePrice) * Convert.ToDecimal(productAmount)));//We do NOT store the total price in the db to reduce the storage. Instead of it, we multiply the unit price with the amount to find the total price.

                        dataTableProduct = productDAL.SearchById(productId);

                        productName = dataTableProduct.Rows[initalIndex]["name"].ToString();//We used initalIndex because there can be only one row in the datatable for a specific product.

                        dgProducts.Items.Add(new { Id = productId, Name = productName, Unit = productUnitName, CostPrice = productCostPrice, SalePrice = productSalePrice, Amount = productAmount, TotalCostPrice = productTotalCostPrice, TotalSalePrice = productTotalSalePrice });
                    }
                    #endregion

                    #region FILLING THE PREVIOUS BASKET INFORMATIONS

                    //We used initalIndex below as a row name because there can be only one row in the datatable for a specific Invoice.
                    txtBasketAmount.Text = dataTablePos.Rows[initalIndex]["total_product_amount"].ToString();
                    txtBasketCostTotal.Text = dataTablePos.Rows[initalIndex]["cost_total"].ToString();
                    txtBasketSubTotal.Text = dataTablePos.Rows[initalIndex]["sub_total"].ToString();
                    txtBasketVat.Text = dataTablePos.Rows[initalIndex]["vat"].ToString();
                    txtBasketDiscount.Text = dataTablePos.Rows[initalIndex]["discount"].ToString();
                    txtBasketGrandTotal.Text = dataTablePos.Rows[initalIndex]["grand_total"].ToString();

                    #endregion
                }
                else if (dataTablePos.Rows.Count == initalIndex)//If the pos detail row quantity is 0, that means there is no such row so decrease or increase the invoice number according to user preference.
                {
                    if (invoiceArrow == initalIndex)//If the invoice arrow is 0, that means user clicked the previous button.
                    {
                        invoiceId = invoiceId - 1;
                    }
                    else
                    {
                        invoiceId = invoiceId + 1;
                    }

                    if (invoiceArrow != -1)//If the user has not clicked either previous or next button, then the invoiceArrow will be -1 and no need for recursion.
                    {
                        LoadPastInvoice(invoiceId, invoiceArrow);//Call the method again to get the new past invoice.
                    }

                }
            }
            else
            {
                FirstTimeRun();//This method is called when it is the first time of using this program.
            }
        }

        private string[,] GetDataGridContent()
        {
            int rowLength = dgProducts.Items.Count;
            int colLength = 8;
            string[,] dgProductCells = new string[rowLength, colLength];

            for (int rowIndex = 0; rowIndex < rowLength; rowIndex++)
            {
                DataGridRow dgRow = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(rowIndex);

                for (int colNo = 0; colNo < colLength; colNo++)
                {
                    TextBlock tbCellContent = dgProducts.Columns[colNo].GetCellContent(dgRow) as TextBlock;

                    dgProductCells[rowIndex, colNo] = tbCellContent.Text;

                    //dgOldProductCells[rowNo, colNo] = cells[rowNo, colNo];//Assigning the old products' informations to the global array called "dgOldProductCells" so that we can access to the old products to revert the changes.
                }
            }

            return dgProductCells;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            int emptyIndex = -1;
            string[,] dgNewProductCells = new string[,] { };

            dgNewProductCells = (string[,])(GetDataGridContent().Clone());//Cloning one array into another array.

            #region Comparing two multidimensional arrays
            bool isDgEqual =
            dgOldProductCells.Rank == dgNewProductCells.Rank &&
            Enumerable.Range(0, dgOldProductCells.Rank).All(dimension => dgOldProductCells.GetLength(dimension) == dgNewProductCells.GetLength(dimension)) &&
            dgOldProductCells.Cast<string>().SequenceEqual(dgNewProductCells.Cast<string>());
            #endregion

            //If the old datagrid equals new datagrid, no need for saving because the user did not change anything.(ONLY IN CASE OF CLICKING TO THE EDIT BUTTON!!!)
            //-1 means nothing has been chosen in the combobox. Note: We don't add the --&& lblInvoiceNo.Content.ToString()!= "0"-- into the if statement because the invoice label cannot be 0 due to the restrictions.
            if (isDgEqual == false && cboMenuPaymentType.SelectedIndex != emptyIndex && cboMenuCustomer.SelectedIndex != emptyIndex && cboMenuAccount.SelectedIndex != emptyIndex && int.TryParse((lblInvoiceId.Content).ToString(), out int number))
            {
                int userClickedNewOrEdit;
                int invoiceId = Convert.ToInt32(lblInvoiceId.Content); /*lblInvoiceId stands for the invoice id in the database.*/
                int userId = userBLL.GetUserId(WinLogin.loggedInUserName);
                bool isSuccess = false;

                #region TABLE POS SAVING SECTION
                //Getting the values from the POS Window and fill them into the pointOfSaleCUL.
                pointOfSaleCUL.Id = invoiceId;//The column invoice id in the database is not auto incremental. This is for preventing the number increasing when the user deletes an existing invoice and creates a new invoice.
                pointOfSaleCUL.PaymentTypeId = Convert.ToInt32(cboMenuPaymentType.SelectedValue);
                pointOfSaleCUL.CustomerId = Convert.ToInt32(cboMenuCustomer.SelectedValue);
                pointOfSaleCUL.AccountId = Convert.ToInt32(cboMenuAccount.SelectedValue);
                pointOfSaleCUL.TotalProductQuantity = Convert.ToInt32(txtBasketAmount.Text);
                pointOfSaleCUL.CostTotal = Convert.ToDecimal(txtBasketCostTotal.Text);
                pointOfSaleCUL.SubTotal = Convert.ToDecimal(txtBasketSubTotal.Text);
                pointOfSaleCUL.Vat = Convert.ToDecimal(txtBasketVat.Text);
                pointOfSaleCUL.Discount = Convert.ToDecimal(txtBasketDiscount.Text);
                pointOfSaleCUL.GrandTotal = Convert.ToDecimal(txtBasketGrandTotal.Text);
                pointOfSaleCUL.AddedDate = DateTime.Now;
                pointOfSaleCUL.AddedBy = userId;

                userClickedNewOrEdit = clickedNewOrEdit;

                if (userClickedNewOrEdit == 1)//If the user clicked the btnEdit, then update the specific invoice information in tbl_pos at once.
                {
                    isSuccess = pointOfSaleBLL.UpdatePOS(pointOfSaleCUL);
                }

                else
                {
                    //Creating a Boolean variable to insert data into the database.
                    isSuccess = pointOfSaleBLL.InsertPOS(pointOfSaleCUL);
                }
                #endregion

                #region TABLE POS DETAILS SAVING SECTION
                int cellUnit = 2, cellCostPrice = 3, cellSalePrice = 4, cellProductAmount = 5;
                int productId;
                int unitId;
                decimal productOldQtyInStock;
                int initialRowIndex = 0;
                int cellLength = 7;
                int addedBy = userId;
                string[] cells = new string[cellLength];
                DateTime dateTime = DateTime.Now;
                bool isSuccessDetail = false;
                int productRate = 0;//Modify this code dynamically!!!!!!!!!

                DataTable dataTableProduct = new DataTable();
                DataTable dataTableUnit = new DataTable();

                for (int rowNo = 0; rowNo < dgProducts.Items.Count; rowNo++)
                {
                    if (userClickedNewOrEdit == 1)//If the user clicked the btnEdit, then edit the specific invoice's products in tbl_pos_detailed at once.
                    {
                        productBLL.RevertOldQuantityInStock(dgOldProductCells, dgProducts.Items.Count, calledBy);//Reverting the old products' amount in stock.

                        //We are sending invoiceNo as a parameter to the "Delete" Method. So that we can erase all the products which have the specific invoice number.
                        pointOfSaleDetailDAL.Delete(invoiceId);

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

                    pointOfSaleDetailCUL.Id = invoiceId;
                    pointOfSaleDetailCUL.ProductId = productId;
                    pointOfSaleDetailCUL.AddedBy = addedBy;
                    pointOfSaleDetailCUL.ProductRate = productRate;
                    pointOfSaleDetailCUL.ProductUnitId = unitId;
                    pointOfSaleDetailCUL.ProductCostPrice = Convert.ToDecimal(cells[cellCostPrice]);//cells[3] contains cost price of the product in the list.
                    pointOfSaleDetailCUL.ProductSalePrice = Convert.ToDecimal(cells[cellSalePrice]);//cells[4] contains sale price of the product in the list.
                    pointOfSaleDetailCUL.ProductQuantity = Convert.ToDecimal(cells[cellProductAmount]);

                    isSuccessDetail = pointOfSaleDetailDAL.Insert(pointOfSaleDetailCUL);

                    #region PRODUCT AMOUNT UPDATE
                    productOldQtyInStock = Convert.ToDecimal(dataTableProduct.Rows[initialRowIndex][colQtyNameInDb].ToString());//Getting the old product amount in stock.

                    newQuantity = productOldQtyInStock - Convert.ToDecimal(cells[cellProductAmount]);

                    productDAL.UpdateSpecificColumn(productId, colQtyNameInDb, newQuantity.ToString());
                    #endregion

                }
                #endregion

                //If the data is inserted successfully, then the value of the variable isSuccess will be true; otherwise it will be false.
                if (isSuccess == true && isSuccessDetail == true)//IsSuccessDetail is always CHANGING in every loop above! IMPROVE THIS!!!!
                {
                    //ClearBasketTextBox();
                    //ClearPointOfSaleDataGrid();
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

        private void btnProductAdd_Click(object sender, RoutedEventArgs e)//Try to do this by using listview
        {
            bool addNewProductLine = true;
            int firstIndex = 0;
            //int costColNo = 3; NO NEED TO GET THE COST CONTENT AGAIN SINCE WE HAVE ALREADY GOT IT FROM THE FIRST ENTRY OF THIS PRODUCT.
            //int priceColNo = 4;
            int amountColNo = 5;
            int totalCostColNo = 6;
            int totalPriceColNo = 7;
            int amount;
            decimal totalPrice;
            int rowQuntity = dgProducts.Items.Count;
            DataTable dtProduct = productDAL.SearchProductByIdBarcode(txtProductId.Text);
            int productId = Convert.ToInt32(dtProduct.Rows[firstIndex]["id"]); //We need to get the Id of the product from the db even if the user enters an id because user may also enter a barcode.

            for (int i = 0; i < rowQuntity; i++)
            {
                DataGridRow row = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(i);

                TextBlock barcodeCellContent = dgProducts.Columns[firstIndex].GetCellContent(row) as TextBlock;    //Try to understand this code!!!  

                if (barcodeCellContent.Text == productId.ToString())
                {
                    if (MessageBox.Show("There is already the same item in the list. Would you like to sum them?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        //TextBlock tbCellCostContent = dgProducts.Columns[costColNo].GetCellContent(row) as TextBlock;    NO NEED TO GET THE COST CONTENT AGAIN SINCE WE HAVE ALREADY GOT IT FROM THE FIRST ENTRY OF THIS PRODUCT.
                        //TextBlock tbCellPriceContent = dgProducts.Columns[priceColNo].GetCellContent(row) as TextBlock;    //Try to understand this code!!! 
                        TextBlock tbCellAmountContent = dgProducts.Columns[amountColNo].GetCellContent(row) as TextBlock;    //Try to understand this code!!!                         
                        TextBlock tbCellTotalCostContent = dgProducts.Columns[totalCostColNo].GetCellContent(row) as TextBlock;    //Try to understand this code!!! 
                        TextBlock tbCellTotalPriceContent = dgProducts.Columns[totalPriceColNo].GetCellContent(row) as TextBlock;

                        //MessageBox.Show(cellContent.Text);
                        amount = Convert.ToInt32(tbCellAmountContent.Text);
                        amount += Convert.ToInt32(txtProductAmount.Text);//We are adding the amount entered in the "txtProductAmount" to the previous amount cell's amount.

                        //tbCellCostContent.Text = txtProductCostPrice.Text; NO NEED TO GET THE COST CONTENT AGAIN SINCE WE HAVE ALREADY GOT IT FROM THE FIRST ENTRY OF THIS PRODUCT.
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
                decimal totalCostPrice = Convert.ToDecimal(txtProductCostPrice.Text) * Convert.ToDecimal(txtProductAmount.Text);
                //dgProducts.Items.Add(new ProductCUL(){ Id = Convert.ToInt32(txtProductId.Text), Name = txtProductName.Text });// You can also apply this code instead of the code below. Note that you have to change the binding name in the datagrid with the name of the property in ProductCUL if you wish to use this code.
                dgProducts.Items.Add(new { Id = productId, Name = txtProductName.Text, Unit = cboProductUnit.SelectedItem, CostPrice = txtProductCostPrice.Text, SalePrice = txtProductSalePrice.Text, Amount = txtProductAmount.Text, TotalCostPrice = totalCostPrice.ToString(), TotalSalePrice = txtProductTotalPrice.Text });
            }

            dgProducts.UpdateLayout();
            //rowQuntity = dgProducts.Items.Count;//Renewing the row quantity after adding a new product.

            PopulateBasket();

            ClearProductEntranceTextBox();

            //items[0].BarcodeRetail = "EXAMPLECODE"; This code can change the 0th row's data on the column called BarcodeRetail.
        }

        private void PopulateBasket()
        {
            decimal amountFromTextEntry = Convert.ToDecimal(txtProductAmount.Text);

            txtBasketAmount.Text = (Convert.ToDecimal(txtBasketAmount.Text) + amountFromTextEntry).ToString();

            txtBasketCostTotal.Text = (Convert.ToDecimal(txtBasketCostTotal.Text) + (Convert.ToDecimal(txtProductCostPrice.Text) * amountFromTextEntry)).ToString();

            txtBasketSubTotal.Text = (Convert.ToDecimal(txtBasketSubTotal.Text) + (Convert.ToDecimal(txtProductSalePrice.Text) * amountFromTextEntry)).ToString();

            txtBasketGrandTotal.Text = (Convert.ToDecimal(txtBasketSubTotal.Text) + Convert.ToDecimal(txtBasketVat.Text) - Convert.ToDecimal(txtBasketDiscount.Text)).ToString();
        }

        private void SubstractBasket(int selectedRowIndex)
        {
            DataGridRow dataGridRow;
            TextBlock tbCellTotalCost;
            TextBlock tbCellAmount;
            TextBlock tbCellTotalPrice;
            int colProductTotalCost = 6;
            int colProductAmount = 5;
            int colProductTotalPrice = 7;

            dataGridRow = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(selectedRowIndex);

            tbCellAmount = dgProducts.Columns[colProductAmount].GetCellContent(dataGridRow) as TextBlock;

            tbCellTotalCost = dgProducts.Columns[colProductTotalCost].GetCellContent(dataGridRow) as TextBlock;    //Try to understand this code!!!  

            tbCellTotalPrice = dgProducts.Columns[colProductTotalPrice].GetCellContent(dataGridRow) as TextBlock;    //Try to understand this code!!!  


            txtBasketAmount.Text = (Convert.ToDecimal(txtBasketAmount.Text) - Convert.ToDecimal(tbCellAmount.Text)).ToString();

            txtBasketCostTotal.Text = (Convert.ToDecimal(txtBasketCostTotal.Text) - Convert.ToDecimal(tbCellTotalCost.Text)).ToString();

            txtBasketSubTotal.Text = (Convert.ToDecimal(txtBasketSubTotal.Text) - Convert.ToDecimal(tbCellTotalPrice.Text)).ToString();

            txtBasketGrandTotal.Text = (Convert.ToDecimal(txtBasketSubTotal.Text) + Convert.ToDecimal(txtBasketVat.Text) - Convert.ToDecimal(txtBasketDiscount.Text)).ToString();
        }

        private void cboMenuPaymentType_Loaded(object sender, RoutedEventArgs e)
        {
            //Creating Data Table to hold the products from Database
            DataTable dataTable = paymentDAL.Select();

            //Specifying Items Source for product combobox
            cboMenuPaymentType.ItemsSource = dataTable.DefaultView;

            //Here DisplayMemberPath helps to display Text in the ComboBox.
            cboMenuPaymentType.DisplayMemberPath = "payment_type";

            //SelectedValuePath helps to store values like a hidden field.
            cboMenuPaymentType.SelectedValuePath = "id";
        }

        private void cboMenuCustomer_Loaded(object sender, RoutedEventArgs e)
        {
            //Creating Data Table to hold the products from Database
            DataTable dataTable = customerDAL.Select();

            //Specifying Items Source for product combobox
            cboMenuCustomer.ItemsSource = dataTable.DefaultView;

            //Here DisplayMemberPath helps to display Text in the ComboBox.
            cboMenuCustomer.DisplayMemberPath = "name";

            //SelectedValuePath helps to store values like a hidden field.
            cboMenuCustomer.SelectedValuePath = "id";
        }

        private void cboMenuAccount_Loaded(object sender, RoutedEventArgs e)
        {
            //Creating Data Table to hold the products from Database
            DataTable dataTable = accountDAL.Select();

            //Specifying Items Source for product combobox
            cboMenuAccount.ItemsSource = dataTable.DefaultView;

            //Here DisplayMemberPath helps to display Text in the ComboBox.
            cboMenuAccount.DisplayMemberPath = "name";

            //SelectedValuePath helps to store values like a hidden field.
            cboMenuAccount.SelectedValuePath = "id";
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            clickedNewOrEdit = clickedNew;//0 stands for the user has entered the btnNew.
            LoadNewInvoice();
            ModifyToolsOnClickBtnNewOrEdit();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Would you really like to cancel the invoice?", "Cancel Invoice", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    DisableTools();
                    ClearProductEntranceTextBox();
                    ClearProductsDataGrid();
                    LoadPastInvoice();
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

        private void btnEditRecord_Click(object sender, RoutedEventArgs e)
        {
            clickedNewOrEdit = 1;//1 stands for the user has entered the btnEdit.
            dgOldProductCells = (string[,])(GetDataGridContent().Clone());//Cloning one array into another array.
            ModifyToolsOnClickBtnNewOrEdit();
        }

        private void btnDeleteRecord_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Would you really like to delete the invoice?", "Delete Invoice", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:

                    #region DELETE INVOICE
                    int invoiceId = Convert.ToInt32(lblInvoiceId.Content); //GetLastInvoiceNumber(); You can also call this method and add number 1 to get the current invoice number, but getting the ready value is faster than getting the last invoice number from the database and adding a number to it to get the current invoice number.

                    pointOfSaleDetailDAL.Delete(invoiceId);
                    pointOfSaleDAL.Delete(invoiceId);

                    #endregion

                    #region REVERT THE STOCK
                    dgOldProductCells = (string[,])(GetDataGridContent().Clone());//Cloning one array into another array.
                    productBLL.RevertOldQuantityInStock(dgOldProductCells, dgProducts.Items.Count, calledBy);
                    #endregion

                    #region PREPARE TO THE LAST PAGE
                    DisableTools();
                    ClearProductEntranceTextBox();
                    ClearProductsDataGrid();
                    LoadPastInvoice();
                    EnableButtonsOnClickSaveCancel();
                    #endregion

                    break;
                case MessageBoxResult.No:
                    MessageBox.Show("Enjoy!", "Enjoy");
                    break;
                case MessageBoxResult.Cancel:
                    MessageBox.Show("Nevermind then...", "KABA Accounting");
                    break;
            }
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            int firstInvoiceId = 0, currentInvoiceId = Convert.ToInt32(lblInvoiceId.Content);

            if (currentInvoiceId != firstInvoiceId)
            {
                ClearProductsDataGrid();
                int prevInvoice = currentInvoiceId - 1;
                invoiceArrow = 0;//0 means customer has clicked the previous button.
                LoadPastInvoice(prevInvoice, invoiceArrow);
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            int lastInvoiceId = commonBLL.GetLastRecordById(calledBy), currentInvoiceId;

            currentInvoiceId = Convert.ToInt32(lblInvoiceId.Content);

            if (currentInvoiceId != lastInvoiceId)
            {
                ClearProductsDataGrid();
                int nextInvoice = currentInvoiceId + 1;
                invoiceArrow = 1;//1 means customer has clicked the next button.
                LoadPastInvoice(nextInvoice, invoiceArrow);
            }
        }

        private void btnDeleteDataGridRow_Click(object sender, RoutedEventArgs e)
        {
            var selectedRow = dgProducts.SelectedItem;
            int selectedRowIndex = dgProducts.SelectedIndex;

            if (selectedRow != null)
            {
                SubstractBasket(selectedRowIndex);

                dgProducts.Items.Remove(selectedRow);
            }
        }

        /*----THIS IS NOT AN EFFICIENT CODE----*/
        private void txtProductAmount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtProductAmount.IsFocused == true)//If the cursor is not focused on this textbox, then no need to check this code.
            {
                if (txtProductAmount.Text != "")
                {
                    decimal number;
                    string productAmount = txtProductAmount.Text;

                    char lastCharacter = char.Parse(productAmount.Substring(productAmount.Length - 1));//Getting the last character to check if the user has entered a missing amount like " 3, "

                    bool result = Char.IsDigit(lastCharacter);//Checking if the last digit of the number is a number or not.

                    if (decimal.TryParse(productAmount, out number) && result == true)
                    {
                        DataTable dataTable = productDAL.SearchProductByIdBarcode(txtProductId.Text);

                        string unitKg = "Kilogram", unitLt = "Liter";
                        int rowIndex = 0;
                        string productSalePrice = dataTable.Rows[rowIndex]["saleprice"].ToString();

                        if (cboProductUnit.Text != unitKg && cboProductUnit.Text != unitLt)
                        {
                            /*If the user entered any unit except kilogram or liter, there cannot be a decimal quantity. 
                            So, convert the quantity to integer even the user has entered a decimal quantity as a mistake.*/
                            productAmount = Convert.ToInt32(txtProductAmount.Text).ToString();
                            txtProductAmount.Text = productAmount.ToString();
                        }
                        else//If the user has defined the unit as kilogram or liter, then there can be a decimal amount like "3,5 liter."
                        {
                            productAmount = Convert.ToDecimal(txtProductAmount.Text).ToString();
                        }

                        txtProductTotalPrice.Text = (Convert.ToDecimal(productSalePrice) * Convert.ToDecimal(productAmount)).ToString();

                        btnProductAdd.IsEnabled = true;
                    }

                    else//Reverting the amount to the default value.
                    {
                        MessageBox.Show("Please enter a valid number");
                        this.txtProductAmount.Text = "1";//We are reverting the amount of the product to default if the user has pressed a wrong key such as "a-b-c".
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

        private void txtProductId_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void txtProductId_KeyUp(object sender, KeyEventArgs e)
        {
            string productIdFromUser = txtProductId.Text;
            int firstIndex = 0;
            long number;

            DataTable dataTable = productDAL.SearchProductByIdBarcode(productIdFromUser);

            if (e.Key == Key.Enter)
            {
                if (btnProductAdd.IsEnabled == true)//If either product add or cancel is activated, that means the user has entered a valid id and first If statement above is worked.
                {
                    btnProductAdd_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("You cannot enter because the Id/Barcode is wrong!");
                }
            }

            else if (productIdFromUser != firstIndex.ToString() && long.TryParse(productIdFromUser, out number) && dataTable.Rows.Count != firstIndex)//Validating the barcode if it is a number(except zero) or not.
            {
                int productAmount = 1;
                int rowIndex = firstIndex;
                int productId;
                int productUnit;
                string productBarcodeRetail/*, productBarcodeWholesale*/;
                string costPrice, salePrice;

                btnProductAdd.IsEnabled = true; //Enabling the add button if any valid barcode is entered.
                btnProductClear.IsEnabled = true;//Enabling the clear button if any valid barcode is entered.


                productId = Convert.ToInt32(dataTable.Rows[rowIndex]["id"]);
                productBarcodeRetail = dataTable.Rows[rowIndex]["barcode_retail"].ToString();
                //productBarcodeWholesale = dataTable.Rows[rowIndex]["barcode_wholesale"].ToString();


                if (productBarcodeRetail == productIdFromUser || productId.ToString() == productIdFromUser)//If the barcode equals the product's barcode_retail or id, then take the product's retail unit id.
                {
                    productUnit = Convert.ToInt32(dataTable.Rows[rowIndex]["unit_retail_id"]);
                }

                else //If the barcode equals to the barcode_wholesale, then take the product's wholesale unit id.
                {
                    productUnit = Convert.ToInt32(dataTable.Rows[rowIndex]["unit_wholesale_id"]);
                }

                txtProductName.Text = dataTable.Rows[rowIndex]["name"].ToString();//Filling the product name textbox from the database

                DataTable dataTableUnit = unitDAL.GetUnitInfoById(productUnit);//Datatable for finding the unit name by unit id.

                cboProductUnit.Items.Add(dataTableUnit.Rows[rowIndex]["name"].ToString());//Populating the combobox with related unit names from dataTableUnit.
                cboProductUnit.SelectedIndex = firstIndex;//For selecting the combobox's first element. We selected 0 index because we have just one unit of a retail product.

                costPrice = dataTable.Rows[rowIndex]["costprice"].ToString();
                salePrice = dataTable.Rows[rowIndex]["saleprice"].ToString();

                txtProductCostPrice.Text = costPrice;
                txtProductSalePrice.Text = salePrice;
                txtProductAmount.Text = productAmount.ToString();
                txtProductTotalPrice.Text = (Convert.ToDecimal(salePrice) * productAmount).ToString();
            }

            /*--->If the txtProductId is empty which means user has clicked the backspace button and if the txtProductName is filled once before, then erase all the text contents.
            Note: I just checked the btnProductAdd to know if there was a product entry before or not.
                  If the btnProductAdd is not enabled in the if block above once before, then no need to call the method ClearProductEntranceTextBox.*/
            else if (productIdFromUser == "" && btnProductAdd.IsEnabled == true)
            {
                ClearProductEntranceTextBox();
            }

            else
                DisableProductEntranceButtons();//Disable buttons in case of nothing was valid above in order not to enter something wrong to the datagrid.
        }
    }
}
