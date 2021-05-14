using BLL;
using CUL;
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
    /// Interaction logic for WinPointOfPurchase.xaml
    /// </summary>
    public partial class WinPointOfPurchase : Window
    {
        public WinPointOfPurchase()
        {
            InitializeComponent();
            DisableTools();
            LoadUserInformations();
            LoadPastInvoice();
        }

        UserDAL userDAL = new UserDAL();
        UserBLL userBLL = new UserBLL();
        BankDAL bankDAL = new BankDAL();
        PointOfPurchaseDAL pointOfPurchaseDAL = new PointOfPurchaseDAL();
        PointOfPurchaseCUL pointOfPurchaseCUL = new PointOfPurchaseCUL();
        PointOfPurchaseDetailDAL pointOfPurchaseDetailDAL = new PointOfPurchaseDetailDAL();
        PointOfPurchaseDetailCUL pointOfPurchaseDetailCUL = new PointOfPurchaseDetailCUL();
        PointOfPurchaseBLL pointOfPurchaseBLL = new PointOfPurchaseBLL();
        ProductDAL productDAL = new ProductDAL();
        ProductCUL productCUL = new ProductCUL();
        PaymentDAL paymentDAL = new PaymentDAL();
        PaymentCUL paymentCUL = new PaymentCUL();
        SupplierDAL supplierDAL = new SupplierDAL();
        SupplierCUL supplierCUL = new SupplierCUL();
        UnitDAL unitDAL = new UnitDAL();
        UnitCUL unitCUL = new UnitCUL();
        ProductBLL productBLL = new ProductBLL();
        AccountDAL accountDAL = new AccountDAL();
        AssetDAL assetDAL = new AssetDAL();
        AssetCUL assetCUL = new AssetCUL();
        CommonBLL commonBLL = new CommonBLL();

        int initialIndex = 0;
        const int colLength =6;
        int clickedNewOrEdit;
        const int clickedNothing=-1, clickedNew = 0, clickedEdit = 1,clickedNull=2;//0 stands for user clicked the button New, and 1 stands for user clicked the button Edit.
        int colProductCostPrice=3, colProductQuantity=4, colProductTotalCostPrice = 5;
        string[] dgCellNames = new string[colLength] { "dgTxtProductId", "dgTxtProductName", "dgTxtProductUnit", "dgTxtProductCostPrice", "dgTxtProductQuantity", "dgTxtProductTotalCostPrice" };
        string[,] oldDgProductCells = new string[,] { };
        string calledBy = "POP";
        int account = 1, bank = 2, supplier = 3;
        int calledByVAT = 1, calledByDiscount = 2;
        int oldItemsRowCount;
        int invoiceArrow;
        int oldIdAsset, oldIdAssetSupplier;
        decimal oldBasketCostTotal,oldBasketGrandTotal, oldBasketQuantity;

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
            chkUpdateProductCosts.IsEnabled = false;
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;
            btnPrint.IsEnabled = false;
            cboMenuPaymentType.IsEnabled = false;
            cboMenuSupplier.IsEnabled = false;
            cboMenuAsset.IsEnabled = false;
            cboProductUnit.IsEnabled = false;
            txtProductId.IsEnabled = false;
            txtProductName.IsEnabled = false;
            txtProductCostPrice.IsEnabled = false;
            txtProductAmount.IsEnabled = false;
            txtProductTotalCostPrice.IsEnabled = false;
            txtInvoiceNo.IsEnabled = false;
        }

        private void ModifyToolsOnClickBtnNewOrEdit(int clickedBtn= clickedNothing)//Do NOT repeat yourself! You have used IsEnabled function for these toolbox contents many times! And in the other pages as well!
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
            cboMenuSupplier.IsEnabled = true;
            cboMenuAsset.IsEnabled = true;
            cboProductUnit.IsEnabled = true;
            txtProductId.IsEnabled = true;
            txtProductName.IsEnabled = true;
            txtProductCostPrice.IsEnabled = true;
            txtProductAmount.IsEnabled = true;
            txtProductTotalCostPrice.IsEnabled = true;
            txtInvoiceNo.IsEnabled = true;
            chkUpdateProductCosts.IsEnabled = true;
            dgProducts.IsHitTestVisible = true;//Enabling the datagrid clicking.
            //cboMenuSupplier.SelectedIndex = -1;//-1 Means nothing is selected.

            if (clickedBtn == clickedNew)
                txtInvoiceNo.Text = "";
        }

        private void ClearProductsDataGrid()
        {
            dgProducts.Items.Clear();
        }

        private void ClearBasketTextBox()
        {
            txtBasketAmount.Text = "0";
            txtBasketCostTotal.Text = "0";
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
            txtProductAmount.Text = "";
            txtProductTotalCostPrice.Text = "";
            Keyboard.Focus(txtProductId); // set keyboard focus
            DisableProductEntranceButtons();
        }

        private void LoadNewInvoice()
        {
            ClearBasketTextBox();
            ClearProductsDataGrid();

            int invoiceId, increment = 1;

            invoiceId = commonBLL.GetLastInvoiceId(calledBy);//Getting the last invoice number and assign it to the variable called invoiceNo.
            invoiceId += increment;//We are adding one to the last invoice number because every new invoice number is one greater tham the previous invoice number.
            lblInvoiceId.Content = invoiceId;//Assigning invoiceNo to the content of the InvoiceNo Label.
        }

        //-1 means user did not clicked either previous or next button which means user just clicked the point of purchase button to open it.
        private void LoadPastInvoice(int invoiceId = 0, int invoiceArrow = -1)//Optional parameter
        {
            int initalIndex = 0, productUnitId;
            string productId, productName, productUnitName, productCostPrice, productAmount, productTotalCostPrice;

            if (invoiceId == initalIndex)//If the ID is 0 came from the optional parameter, that means user just clicked the WinPOP button to open it.
            {
                invoiceId = commonBLL.GetLastInvoiceId(calledBy);//Getting the last invoice id and assign it to the variable called invoiceId.
            }

            /*WE CANNOT USE ELSE IF FOR THE CODE BELOW! BOTH IF STATEMENTS ABOVE AND BELOVE MUST WORK.*/
            if (invoiceId != initalIndex)// If the invoice number is still 0 even when we get the last invoice number by using code above, that means this is the first sale and do not run this code block.
            {
                DataTable dataTablePop = pointOfPurchaseDAL.GetByInvoiceId(invoiceId);

                if (dataTablePop.Rows.Count != initalIndex)
                {
                    DataTable dataTablePopDetail = pointOfPurchaseDetailDAL.Search(invoiceId);
                    DataTable dataTableUnitInfo;
                    DataTable dataTableProduct;
                    
                    #region ASSET INFORMATION FILLING REGION
                    int assetId = Convert.ToInt32(dataTablePop.Rows[initalIndex]["asset_id"].ToString());//Getting the id of account.
                    lblAssetId.Content = assetId;

                    DataTable dtAsset = assetDAL.SearchById(assetId);
                    int sourceType = Convert.ToInt32(dtAsset.Rows[initalIndex]["id_source_type"]);

                    if (sourceType == account)
                        rbAccount.IsChecked = true;
                    else
                        rbBank.IsChecked = true;

                    cboMenuAsset.SelectedValue = dtAsset.Rows[initalIndex]["id_source"].ToString();
                    #endregion

                    LoadCboMenuPaymentType();
                    LoadCboMenuSupplier();

                    cboMenuPaymentType.SelectedValue = Convert.ToInt32(dataTablePop.Rows[initalIndex]["payment_type_id"].ToString());//Getting the id of purchase type.
                    cboMenuSupplier.SelectedValue = Convert.ToInt32(dataTablePop.Rows[initalIndex]["supplier_id"].ToString());//Getting the id of supplier.
                    txtInvoiceNo.Text = dataTablePop.Rows[initalIndex]["invoice_no"].ToString();
                    lblInvoiceId.Content= dataTablePop.Rows[initalIndex]["id"].ToString();

                    #region LOADING THE PRODUCT DATA GRID
                    for (int currentRow = initalIndex; currentRow < dataTablePopDetail.Rows.Count; currentRow++)
                    {

                        productId = dataTablePopDetail.Rows[currentRow]["product_id"].ToString();
                        productUnitId = Convert.ToInt32(dataTablePopDetail.Rows[currentRow]["product_unit_id"]);

                        dataTableUnitInfo = unitDAL.GetUnitInfoById(productUnitId);//Getting the unit name by unit id.
                        productUnitName = dataTableUnitInfo.Rows[initalIndex]["name"].ToString();//We use initalIndex value for the index number in every loop because there can be only one unit name of a specific id.

                        productCostPrice = dataTablePopDetail.Rows[currentRow]["product_cost_price"].ToString();
                        productAmount = dataTablePopDetail.Rows[currentRow]["amount"].ToString();
                        productTotalCostPrice = String.Format("{0:0.00}", (Convert.ToDecimal(productCostPrice) * Convert.ToDecimal(productAmount)));//We do NOT store the total cost in the db to reduce the storage. Instead of it, we multiply the unit cost with the amount to find the total cost.

                        dataTableProduct = productDAL.SearchById(productId);

                        productName = dataTableProduct.Rows[initalIndex]["name"].ToString();//We used initalIndex because there can be only one row in the datatable for a specific product.

                        dgProducts.Items.Add(new { Id = productId, Name = productName, Unit = productUnitName, CostPrice = productCostPrice, Amount = productAmount, TotalCostPrice = productTotalCostPrice });

                    }
                    #endregion

                    #region FILLING THE PREVIOUS BASKET INFORMATIONS

                    //We used initalIndex below as a row name because there can be only one row in the datatable for a specific Invoice.
                    txtBasketAmount.Text = dataTablePop.Rows[initalIndex]["total_product_amount"].ToString();
                    txtBasketCostTotal.Text = dataTablePop.Rows[initalIndex]["cost_total"].ToString();
                    txtBasketVat.Text = dataTablePop.Rows[initalIndex]["vat"].ToString();
                    txtBasketDiscount.Text = dataTablePop.Rows[initalIndex]["discount"].ToString();
                    txtBasketGrandTotal.Text = dataTablePop.Rows[initalIndex]["grand_total"].ToString();

                    #endregion
                }
                else if (dataTablePop.Rows.Count == initalIndex)//If the pop detail row quantity is 0, that means there is no such row so decrease or increase the invoice number according to user preference.
                {
                    if (invoiceArrow == 0)//If the invoice arrow is 0, that means user clicked the previous button.
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

        private string[,] GetDataGridContent()//This method stores the previous list in a global array variable called "cells" when we press the Edit button.
        {
            int rowLength = dgProducts.Items.Count;
            string[,] dgProductCells = new string[rowLength, colLength];

            for (int rowNo = 0; rowNo < rowLength; rowNo++)
            {
                DataGridRow dgRow = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(rowNo);

                for (int colNo = 0; colNo < colLength; colNo++)
                {
                    ContentPresenter cpProduct = dgProducts.Columns[colNo].GetCellContent(dgRow) as ContentPresenter;
                    var tmpProduct = cpProduct.ContentTemplate;
                    TextBox tbCellContent = tmpProduct.FindName(dgCellNames[colNo], cpProduct) as TextBox;

                    dgProductCells[rowNo, colNo] = tbCellContent.Text;

                    //dgOldProductCells[rowNo, colNo] = cells[rowNo, colNo];//Assigning the old products' informations to the global array called "dgOldProductCells" so that we can access to the old products to revert the changes.
                }
            }

            return dgProductCells;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            int emptyCboIndex = -1;
            string[,] dgNewProductCells = new string[,] { };

            dgNewProductCells = (string[,])(GetDataGridContent().Clone());//Cloning one array into another array.

            #region Comparing two multidimensional arrays
            bool isDgEqual =
            oldDgProductCells.Rank == dgNewProductCells.Rank &&
            Enumerable.Range(0, oldDgProductCells.Rank).All(dimension => oldDgProductCells.GetLength(dimension) == dgNewProductCells.GetLength(dimension)) &&
            oldDgProductCells.Cast<string>().SequenceEqual(dgNewProductCells.Cast<string>());
            #endregion

            //If the old datagrid equals new datagrid and the old grand and the old asset id equals new asset id, no need for saving because the user did not change anything.
            //-1 means nothing has been chosen in the combobox. Note: We had to add the --&& txtInvoiceNo.Text.ToString()!= "0"-- into the if statement because the invoice text does not have the restriction so that the user may enter wrongly..
            if (int.TryParse(txtInvoiceNo.Text, out int number) && txtInvoiceNo.Text != initialIndex.ToString() || isDgEqual == false || oldIdAsset != Convert.ToInt32(lblAssetId.Content) || oldIdAssetSupplier != Convert.ToInt32(lblAssetSupplierId.Content) || cboMenuPaymentType.SelectedIndex != emptyCboIndex || cboMenuSupplier.SelectedIndex != emptyCboIndex || cboMenuAsset.SelectedIndex != emptyCboIndex)
            {
                int invoiceNo = Convert.ToInt32(txtInvoiceNo.Text);
                int userId = userBLL.GetUserId(WinLogin.loggedInUserName);
                bool isSuccess = false, isSuccessDetail = false,isSuccessAsset=false;
                int cellUnit = 2, cellCostPrice = 3, cellProductAmount = 4;
                int productId;
                int unitId;
                decimal productOldAmountInStock;
                int cellLength = 6;
                int addedBy = userId;
                string[] cells = new string[cellLength];
                DateTime dateTime = DateTime.Now;
                int productRate = 0;//Modify this code dynamically!!!!!!!!!

                int invoiceId = Convert.ToInt32(lblInvoiceId.Content); /*lblInvoiceId stands for the invoice id in the database.*/
                int firstRowIndex = 0;
                DataTable dataTableLastInvoice = pointOfPurchaseBLL.GetLastInvoiceRecord();//Getting the last invoice.
                DataTable dataTableProduct = new DataTable();
                DataTable dataTableUnit = new DataTable();
                DataTable dtAsset= new DataTable();
                decimal oldSourceBalance;

                if (clickedNewOrEdit == clickedEdit)
                {
                    #region TABLE OLD ASSET REVERTING SECTION
                    //REVERTING THE TABLE ASSET FOR BALANCE OF THE SUPPLIER.

                    dtAsset = assetDAL.SearchById(oldIdAssetSupplier);
                    oldSourceBalance = Convert.ToDecimal(dtAsset.Rows[initialIndex]["source_balance"]);

                    assetCUL.Id = Convert.ToInt32(lblAssetSupplierId.Content);
                    assetCUL.SourceBalance = oldSourceBalance+oldBasketGrandTotal;//We have to add the old grandTotal to the source balance because the new grand total may be different from it.
                    isSuccessAsset = assetDAL.Update(assetCUL);
                    #endregion
                }

                #region TABLE ASSET UPDATING SECTION
                //UPDATING THE TABLE ASSET FOR BALANCE OF THE SUPPLIER.

                dtAsset = assetDAL.SearchById(Convert.ToInt32(lblAssetSupplierId.Content));
                oldSourceBalance = Convert.ToDecimal(dtAsset.Rows[initialIndex]["source_balance"]);
                
                assetCUL.SourceBalance = oldSourceBalance - Convert.ToDecimal(txtBasketGrandTotal.Text);//We owe the supplier X Amount for getting this purchase.
                assetCUL.Id = Convert.ToInt32(lblAssetSupplierId.Content);
                
                isSuccessAsset = assetDAL.Update(assetCUL);
                #endregion

                #region TABLE POP SAVING SECTION
                //Getting the values from the POP Window and fill them into the pointOfPurchaseCUL.
                pointOfPurchaseCUL.Id = invoiceId;//The column invoice id in the database is not auto incremental. This is for preventing the number increasing when the user deletes an existing invoice and creates a new invoice.
                pointOfPurchaseCUL.InvoiceNo = invoiceNo;
                pointOfPurchaseCUL.PaymentTypeId = Convert.ToInt32(cboMenuPaymentType.SelectedValue);//Selected value contains the id of the item so that no need to get it from DB.
                pointOfPurchaseCUL.SupplierId = Convert.ToInt32(cboMenuSupplier.SelectedValue);
                pointOfPurchaseCUL.TotalProductAmount = Convert.ToInt32(txtBasketAmount.Text);
                pointOfPurchaseCUL.CostTotal = Convert.ToDecimal(txtBasketCostTotal.Text);
                pointOfPurchaseCUL.Vat = Convert.ToDecimal(txtBasketVat.Text);
                pointOfPurchaseCUL.Discount = Convert.ToDecimal(txtBasketDiscount.Text);
                pointOfPurchaseCUL.GrandTotal = Convert.ToDecimal(txtBasketGrandTotal.Text);
                pointOfPurchaseCUL.AssetId = Convert.ToInt32(lblAssetId.Content);
                pointOfPurchaseCUL.AddedDate = DateTime.Now;
                pointOfPurchaseCUL.AddedBy = userId;

                if (clickedNewOrEdit ==clickedEdit)//If the user clicked the btnEdit, then update the specific invoice information in tbl_pop at once.
                {
                    isSuccess = pointOfPurchaseDAL.Update(pointOfPurchaseCUL);
                }

                else
                {
                    //Creating a Boolean variable to insert data into the database.
                    isSuccess = pointOfPurchaseDAL.Insert(pointOfPurchaseCUL);
                }
                #endregion

                #region TABLE POP DETAILS SAVING SECTION

                for (int rowNo = 0; rowNo < dgProducts.Items.Count; rowNo++)
                {
                    if (clickedNewOrEdit ==clickedEdit)//If the user clicked the btnEdit, then delete the specific invoice's products in tbl_pos_detailed at once.
                    {
                        productBLL.RevertOldAmountInStock(oldDgProductCells, dgProducts.Items.Count, calledBy);//Reverting the old products' amount in stock.

                        //We are sending pointOfPurchaseDetailCUL as a parameter to the Delete method just to use the Id property in the SQL Query. So that we can erase all the products which have the specific id.
                        pointOfPurchaseDetailDAL.Delete(invoiceId);

                        //2 means null for this code. We used this in order to prevent running the if block again and again. Because, we erase all of the products belong to one invoice number at once.
                        clickedNewOrEdit = clickedNull;
                    }

                    DataGridRow dgRow = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(rowNo);

                    for (int colNo = 0; colNo < cellLength; colNo++)
                    {
                        ContentPresenter cpProduct = dgProducts.Columns[colNo].GetCellContent(dgRow) as ContentPresenter;
                        var tmpProduct = cpProduct.ContentTemplate;
                        TextBox tbCellContent = tmpProduct.FindName(dgCellNames[colNo], cpProduct) as TextBox;

                        cells[colNo] = tbCellContent.Text;
                    }

                    dataTableProduct = productDAL.SearchProductByIdBarcode(cells[initialIndex]);//Cell[0] contains the product barcode.
                    productId = Convert.ToInt32(dataTableProduct.Rows[initialIndex]["id"]);//Row index is always zero for this situation because there can be only one row of a product which has a unique barcode on the table.


                    dataTableUnit = unitDAL.GetUnitInfoByName(cells[cellUnit]);//Cell[0] contains the product barcode.
                    unitId = Convert.ToInt32(dataTableUnit.Rows[initialIndex]["id"]);//Row index is always zero for this situation because there can be only one row of a specific unit.

                    pointOfPurchaseDetailCUL.Id = invoiceId;//No incremental value in the database because there can be multiple goods with the same invoice id.
                    pointOfPurchaseDetailCUL.ProductId = productId;
                    pointOfPurchaseDetailCUL.AddedBy = addedBy;
                    pointOfPurchaseDetailCUL.ProductRate = productRate;
                    pointOfPurchaseDetailCUL.ProductUnitId = unitId;
                    pointOfPurchaseDetailCUL.ProductCostPrice = Convert.ToDecimal(cells[cellCostPrice]);//cells[3] contains cost price of the product in the list. We have to store the current cost price as well because it may be changed in the future.
                    pointOfPurchaseDetailCUL.ProductAmount = Convert.ToDecimal(cells[cellProductAmount]);

                    isSuccessDetail = pointOfPurchaseDetailDAL.Insert(pointOfPurchaseDetailCUL);

                    #region PRODUCT AMOUNT UPDATE
                    productOldAmountInStock = Convert.ToDecimal(dataTableProduct.Rows[initialIndex]["amount_in_stock"].ToString());//Getting the old product amount in stock.

                    productCUL.AmountInStock = productOldAmountInStock + Convert.ToDecimal(cells[cellProductAmount]);

                    if (chkUpdateProductCosts.IsChecked == true)
                        productCUL.CostPrice = Convert.ToDecimal(cells[cellCostPrice]);

                    productCUL.Id = productId;//Assigning the Id in the productCUL to update the product columns in the DB using a specific product.

                    productDAL.UpdateAmountInStock(productCUL);
                    #endregion
                }
                #endregion


                //If the data is inserted successfully, then the value of the variable isSuccess will be true; otherwise it will be false.
                if (isSuccess == true && isSuccessDetail == true && isSuccessAsset==true)//IsSuccessDetail is always CHANGING in every loop above! IMPROVE THIS!!!!
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
            //int costColNo = 3; NO NEED TO GET THE COST CONTENT AGAIN SINCE WE HAVE ALREADY GOT IT FROM THE FIRST ENTRY OF THIS PRODUCT.
            //int priceColNo = 4;
            int amountColNo = 4;
            int totalCostColNo = 5;
            int amount;
            int rowQuntity = dgProducts.Items.Count;
            DataTable dtProduct = productDAL.SearchProductByIdBarcode(txtProductId.Text);
            int productId = Convert.ToInt32(dtProduct.Rows[initialIndex]["id"]); //We need to get the Id of the product from the db even if the user enters an id because user may also enter a barcode.

            for (int i = 0; i < rowQuntity; i++)
            {
                DataGridRow row = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(i);

                ContentPresenter cpProduct = dgProducts.Columns[initialIndex].GetCellContent(row) as ContentPresenter;
                var tmpProduct = cpProduct.ContentTemplate;
                TextBox barcodeCellContent = tmpProduct.FindName(dgCellNames[initialIndex], cpProduct) as TextBox;

                if (barcodeCellContent.Text == productId.ToString())
                {
                    if (MessageBox.Show("There is already the same item in the list. Would you like to sum them?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        //TextBlock tbCellCostContent = dgProducts.Columns[costColNo].GetCellContent(row) as TextBlock;    NO NEED TO GET THE COST CONTENT AGAIN SINCE WE HAVE ALREADY GOT IT FROM THE FIRST ENTRY OF THIS PRODUCT.
                        //TextBlock tbCellPriceContent = dgProducts.Columns[priceColNo].GetCellContent(row) as TextBlock;    //Try to understand this code!!! 
                        TextBlock tbCellAmountContent = dgProducts.Columns[amountColNo].GetCellContent(row) as TextBlock;    //Try to understand this code!!!                         
                        TextBlock tbCellTotalCostPriceContent = dgProducts.Columns[totalCostColNo].GetCellContent(row) as TextBlock;    //Try to understand this code!!! 

                        amount = Convert.ToInt32(tbCellAmountContent.Text);
                        amount += Convert.ToInt32(txtProductAmount.Text);//We are adding the amount entered in the "txtProductAmount" to the previous amount cell's amount.

                        tbCellAmountContent.Text = amount.ToString();//Assignment of the new amount to the related cell.
                        tbCellTotalCostPriceContent.Text = (amount * Convert.ToDecimal(txtProductCostPrice.Text)).ToString();//Calculating the new total cost price according to the new entry. Then, assigning the result into the table total price. User may have entered a new price in the entry box.
                        addNewProductLine = false;
                        break;//We have to break the loop if the user clicked "yes" because no need to scan the rest of the rows after confirming.
                    }
                }
            }


            if (addNewProductLine == true)//Use ENUMS instead of this!!!!!!!
            {
                decimal totalCostPrice = Convert.ToDecimal(txtProductCostPrice.Text) * Convert.ToDecimal(txtProductAmount.Text);

                dgProducts.Items.Add(new { Id = productId, Name = txtProductName.Text, Unit = cboProductUnit.SelectedItem, CostPrice = txtProductCostPrice.Text, Amount = txtProductAmount.Text, TotalCostPrice = totalCostPrice.ToString() });
            }

            dgProducts.UpdateLayout();

            rowQuntity = dgProducts.Items.Count;//Renewing the row quantity after adding a new product.

            PopulateBasket();

            ClearProductEntranceTextBox();

            //items[0].BarcodeRetail = "EXAMPLECODE"; This code can change the 0th row's data on the column called BarcodeRetail.
        }

        private void PopulateBasket()
        {
            decimal amountFromTextEntry = Convert.ToDecimal(txtProductAmount.Text);

            txtBasketAmount.Text = (Convert.ToDecimal(txtBasketAmount.Text) + amountFromTextEntry).ToString();

            txtBasketCostTotal.Text = (Convert.ToDecimal(txtBasketCostTotal.Text) + Convert.ToDecimal(txtProductTotalCostPrice.Text)).ToString();

            txtBasketGrandTotal.Text = (Convert.ToDecimal(txtBasketCostTotal.Text) + Convert.ToDecimal(txtBasketVat.Text) - Convert.ToDecimal(txtBasketDiscount.Text)).ToString();
        }

        private void SubstractBasket(int selectedRowIndex)
        {
            DataGridRow dataGridRow;
            TextBlock tbCellTotalCost;
            TextBlock tbCellAmount;

            int colProductAmount = 4;
            int colProductTotalCost = 5;

            dataGridRow = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(selectedRowIndex);

            tbCellAmount = dgProducts.Columns[colProductAmount].GetCellContent(dataGridRow) as TextBlock;

            tbCellTotalCost = dgProducts.Columns[colProductTotalCost].GetCellContent(dataGridRow) as TextBlock;    //Try to understand this code!!!  


            txtBasketAmount.Text = (Convert.ToDecimal(txtBasketAmount.Text) - Convert.ToDecimal(tbCellAmount.Text)).ToString();

            txtBasketCostTotal.Text = (Convert.ToDecimal(txtBasketCostTotal.Text) - Convert.ToDecimal(tbCellTotalCost.Text)).ToString();

            txtBasketGrandTotal.Text = (Convert.ToDecimal(txtBasketCostTotal.Text) + Convert.ToDecimal(txtBasketVat.Text) - Convert.ToDecimal(txtBasketDiscount.Text)).ToString();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            clickedNewOrEdit = clickedNew;//0 stands for the user has entered the btnNew.
            LoadNewInvoice();
            ModifyToolsOnClickBtnNewOrEdit(clickedNewOrEdit);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Would you really like to cancel the invoice, you piece of shit?", "Cancel Invoice", MessageBoxButton.YesNoCancel);
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
            clickedNewOrEdit = clickedEdit;//1 stands for the user has entered the btnEdit.
            oldItemsRowCount = dgProducts.Items.Count;//When the user clicks Edit, the index of old(previously saved) items row will be assigned to oldItemsRowCount.
            oldDgProductCells = (string[,])(GetDataGridContent().Clone());//Cloning one array into another array.
            oldIdAsset= Convert.ToInt32(lblAssetId.Content);
            oldIdAssetSupplier = Convert.ToInt32(lblAssetSupplierId.Content);
            oldBasketGrandTotal = Convert.ToDecimal(txtBasketGrandTotal.Text);
            ModifyToolsOnClickBtnNewOrEdit();
        }

        private void btnDeleteRecord_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Would you really like to delete the invoice, you piece of shit?", "Delete Invoice", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:

                    #region DELETE INVOICE
                    int invoiceId = Convert.ToInt32(lblInvoiceId.Content); //GetLastInvoiceNumber(); You can also call this method and add number 1 to get the current invoice number, but getting the ready value is faster than getting the last invoice number from the database and adding a number to it to get the current invoice number.

                    pointOfPurchaseDetailDAL.Delete(invoiceId);
                    pointOfPurchaseDAL.Delete(invoiceId);

                    #endregion

                    #region REVERT THE STOCK
                    oldItemsRowCount = dgProducts.Items.Count;//When the user clicks Edit, the index of old(previously saved) items row will be assigned to oldItemsRowCount.
                    oldDgProductCells = (string[,])(GetDataGridContent().Clone());//Cloning one array into another array.
                    productBLL.RevertOldAmountInStock(oldDgProductCells, dgProducts.Items.Count, calledBy);
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
            int firstInvoiceId = 1, currentInvoiceId = Convert.ToInt32(lblInvoiceId.Content); ;

            if (currentInvoiceId != firstInvoiceId)
            {
                ClearProductsDataGrid();
                int prevInvoiceId = currentInvoiceId - 1;
                invoiceArrow = 0;//0 means customer has clicked the previous button.
                LoadPastInvoice(prevInvoiceId, invoiceArrow);
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            int lastInvoiceId = commonBLL.GetLastInvoiceId(calledBy), currentInvoiceId;

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
                        string productCostPrice = txtProductCostPrice.Text;

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

                        txtProductTotalCostPrice.Text = (Convert.ToDecimal(productCostPrice) * Convert.ToDecimal(productAmount)).ToString();

                        btnProductAdd.IsEnabled = true;
                    }

                    else//Reverting the amount to the default value.
                    {
                        MessageBox.Show("Please enter a valid number");
                        txtProductAmount.Text = "1";//We are reverting the amount of the product to default if the user has pressed a wrong key such as "a-b-c".  
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

        /*----THIS IS NOT AN EFFICIENT CODE----*/
        private void TextMenuCostPriceChanged()
        {
            if (txtProductCostPrice.IsFocused == true)//If the cursor is not focused on this textbox, then no need to check this code.
            {
                if (txtProductCostPrice.Text != "")
                {
                    decimal number;
                    string productCostPrice = txtProductCostPrice.Text;
                    char lastCharacter = char.Parse(productCostPrice.Substring(productCostPrice.Length - 1));//Getting the last character to check if the user has entered a missing cost price like " 3, ".
                    bool result = Char.IsDigit(lastCharacter);//Checking if the last digit of the number is a number or not.

                    if (decimal.TryParse(productCostPrice, out number) && result == true)
                    {
                        decimal productAmount = Convert.ToDecimal(txtProductAmount.Text);

                        txtProductTotalCostPrice.Text = (Convert.ToDecimal(productCostPrice) * productAmount).ToString();

                        btnProductAdd.IsEnabled = true;
                    }

                    else//Reverting the amount to the default value.
                    {
                        MessageBox.Show("Please enter a valid number");

                        using (DataTable dataTable = productDAL.SearchProductByIdBarcode(txtProductId.Text))
                        {
                            int rowIndex = 0;
                            txtProductCostPrice.Text = dataTable.Rows[rowIndex]["costprice"].ToString();//We are reverting the cost price of the product to default if the user has pressed a wrong key such as "a-b-c".
                        }
                    }
                }

                /* If the user left the txtProductCostPrice as empty, wait for him to enter a new value and block the btnProductAdd. 
                   Note: Because the "TextChanged" function works immediately, we don't revert the value into the default. User may click on the "backspace" to correct it by himself"*/
                else
                {
                    btnProductAdd.IsEnabled = false;
                }
            }
        }

        /*----THIS IS NOT AN EFFICIENT CODE----*/
        private void DgTextChanged()
        {
            ////GETTING TEXTBOX FROM DATAGRID.
            ContentPresenter cpProductCostPrice = dgProducts.Columns[colProductCostPrice].GetCellContent(dgProducts.SelectedItem) as ContentPresenter;
            var tmpProductCostPrice = cpProductCostPrice.ContentTemplate;
            TextBox productCostPrice = tmpProductCostPrice.FindName(dgCellNames[colProductCostPrice], cpProductCostPrice) as TextBox;

            ////GETTING TEXTBOX FROM DATAGRID.
            ContentPresenter cpProductQuantity = dgProducts.Columns[colProductQuantity].GetCellContent(dgProducts.SelectedItem) as ContentPresenter;
            var tmpProductQuantity = cpProductQuantity.ContentTemplate;
            TextBox productQuantity = tmpProductQuantity.FindName(dgCellNames[colProductQuantity], cpProductQuantity) as TextBox;

            //GETTING TEXTBOX FROM DATAGRID
            ContentPresenter cpProductTotalCostPrice = dgProducts.Columns[colProductTotalCostPrice].GetCellContent(dgProducts.SelectedItem) as ContentPresenter;
            var tmpProductTotalCostPrice = cpProductTotalCostPrice.ContentTemplate;
            TextBox productTotalCostPrice = tmpProductTotalCostPrice.FindName(dgCellNames[colProductTotalCostPrice], cpProductTotalCostPrice) as TextBox;

            productQuantity.Text = productQuantity.Text.ToString();//We need to reassign it otherwise it will not be affected.
            productTotalCostPrice.Text = (Convert.ToDecimal(productCostPrice.Text) * Convert.ToDecimal(productQuantity.Text)).ToString();

            txtBasketAmount.Text = (oldBasketQuantity + Convert.ToDecimal(productQuantity.Text)).ToString();
            txtBasketCostTotal.Text = (oldBasketCostTotal+Convert.ToDecimal(productTotalCostPrice.Text)).ToString();
            txtBasketGrandTotal.Text = (oldBasketGrandTotal + Convert.ToDecimal(productTotalCostPrice.Text)).ToString();

            //if (costPriceFromDg.Text != "" && quantityFromDg.Text != "")
            //{
            //    decimal number;
            //    string productCostPrice = costPriceFromDg.Text, productQuantity= quantityFromDg.Text;
            //    char lastCharacter = char.Parse(productCostPrice.Substring(productCostPrice.Length - 1));//Getting the last character to check if the user has entered a missing cost price like " 3, ".
            //    bool result = Char.IsDigit(lastCharacter);//Checking if the last digit of the number is a number or not.

            //    if (decimal.TryParse(productCostPrice, out number) && decimal.TryParse(productQuantity,out number) && result == true)
            //    {
            //        //txtProductTotalCostPrice.Text = (Convert.ToDecimal(productCostPrice) * Convert.ToDecimal(productQuantity)).ToString();
            //    }

            //    else//Reverting the amount to the default value.
            //    {
            //        MessageBox.Show("Please enter a valid number");

            //        //using (DataTable dataTable = productDAL.SearchProductByIdBarcode(txtProductId.Text))
            //        //{
            //        //    int rowIndex = 0;
            //        //    txtProductCostPrice.Text = dataTable.Rows[rowIndex]["costprice"].ToString();//We are reverting the cost price of the product to default if the user has pressed a wrong key such as "a-b-c".
            //        //}
            //    }
            //}

            ///* If the user left the txtProductCostPrice as empty, wait for him to enter a new value and block the btnProductAdd. 
            //   Note: Because the "TextChanged" function works immediately, we don't revert the value into the default. User may click on the "backspace" to correct it by himself"*/
            //else
            //{
            //    btnProductAdd.IsEnabled = false;
            //}
        }

        private void txtProductCostPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextMenuCostPriceChanged();
        }

        private void dgTxtProductCostPrice_KeyUp(object sender, KeyEventArgs e)
        {
            DgTextChanged();
        }
        private void dgTxtProductQuantity_KeyUp(object sender, KeyEventArgs e)
        {
            DgTextChanged();
        }

        private void txtInvoiceNo_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtInvoiceNo.Text == "0")
            {
                txtInvoiceNo.Text = "";
            }
        }

        private void txtInvoiceNo_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtInvoiceNo.Text == "" || !Int32.TryParse(txtInvoiceNo.Text, out int value))//The code will work if the text is empty or does NOT contain a numeric value.
            {
                txtInvoiceNo.Text = "0";
            }
        }

        private void txtProductId_KeyUp(object sender, KeyEventArgs e)
        {
            string productIdFromUser = txtProductId.Text;
            int firstIndex=0;
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
                string costPrice;

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

                txtProductCostPrice.Text = costPrice;
                txtProductAmount.Text = productAmount.ToString();
                txtProductTotalCostPrice.Text = (Convert.ToDecimal(costPrice) * productAmount).ToString();
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
        private void CalculateGrandTotal(int calledByVatOrDiscount)
        {
            if (decimal.TryParse(txtBasketVat.Text, out decimal number) && txtBasketVat.Text != "" && txtBasketDiscount.Text != "")
            {
                txtBasketGrandTotal.Text = (Convert.ToDecimal(txtBasketCostTotal.Text) + Convert.ToDecimal(txtBasketVat.Text) - Convert.ToDecimal(txtBasketDiscount.Text)).ToString();
            }
            else
            {
                if (calledByVatOrDiscount==calledByVAT)
                {
                    txtBasketVat.Text = initialIndex.ToString();
                    txtBasketGrandTotal.Text = (Convert.ToDecimal(txtBasketCostTotal.Text) - Convert.ToDecimal(txtBasketDiscount.Text)).ToString();
                }
                else
                {
                    txtBasketDiscount.Text = initialIndex.ToString();
                    txtBasketGrandTotal.Text = (Convert.ToDecimal(txtBasketCostTotal.Text) + Convert.ToDecimal(txtBasketVat.Text)).ToString();
                }
            }
        }

        private void dgProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgProducts.SelectedItem!=null)
            {
                ////GETTING TEXTBOX FROM DATAGRID.
                ContentPresenter cpProductCostPrice = dgProducts.Columns[colProductTotalCostPrice].GetCellContent(dgProducts.SelectedItem) as ContentPresenter;
                var tmpProductCostPrice = cpProductCostPrice.ContentTemplate;
                TextBox productTotalCostPrice = tmpProductCostPrice.FindName(dgCellNames[colProductTotalCostPrice], cpProductCostPrice) as TextBox;

                ////GETTING TEXTBOX FROM DATAGRID.
                ContentPresenter cpProductQuantity = dgProducts.Columns[colProductQuantity].GetCellContent(dgProducts.SelectedItem) as ContentPresenter;
                var tmpProductQuantity = cpProductQuantity.ContentTemplate;
                TextBox productQuantity = tmpProductQuantity.FindName(dgCellNames[colProductQuantity], cpProductQuantity) as TextBox;

                oldBasketQuantity = Convert.ToDecimal(txtBasketAmount.Text) - Convert.ToDecimal(productQuantity.Text);
                oldBasketCostTotal = Convert.ToDecimal(txtBasketCostTotal.Text) - Convert.ToDecimal(productTotalCostPrice.Text);//Cost total is without VAT.
                oldBasketGrandTotal = Convert.ToDecimal(txtBasketGrandTotal.Text) - Convert.ToDecimal(productTotalCostPrice.Text);//Grand total is with VAT.
            }
        }

        private void txtBasketVat_KeyUp(object sender, KeyEventArgs e)
        {
            CalculateGrandTotal(calledByVAT);
        }

        private void txtBasketDiscount_KeyUp(object sender, KeyEventArgs e)
        {
            CalculateGrandTotal(calledByDiscount);
        }

        private void LoadCboMenuAsset(int checkStatus)
        {
            DataTable dataTable;
            if (checkStatus == account)
                dataTable = accountDAL.Select();


            else
                dataTable = bankDAL.Select();

            //Specifying Items Source for product combobox
            cboMenuAsset.ItemsSource = dataTable.DefaultView;

            //Here DisplayMemberPath helps to display Text in the ComboBox.
            cboMenuAsset.DisplayMemberPath = "name";

            //SelectedValuePath helps to store values like a hidden field.
            cboMenuAsset.SelectedValuePath = "id";
        }



        private void LoadCboMenuSupplier()
        {
            //Creating Data Table to hold the products from Database
            DataTable dataTable = supplierDAL.Select();

            //Specifying Items Source for product combobox
            cboMenuSupplier.ItemsSource = dataTable.DefaultView;

            //Here DisplayMemberPath helps to display Text in the ComboBox.
            cboMenuSupplier.DisplayMemberPath = "name";

            //SelectedValuePath helps to store values like a hidden field.
            cboMenuSupplier.SelectedValuePath = "id";
        }

        private void LoadCboMenuPaymentType()
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

        private void cboMenuSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int sourceId;
            int sourceType=supplier;

            sourceId = Convert.ToInt32(cboMenuSupplier.SelectedValue);
            lblAssetSupplierId.Content = assetDAL.GetAssetIdBySource(sourceId, sourceType);
        }

        private void cboMenuAsset_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int sourceId;
            int sourceType;

            if (rbAccount.IsChecked == true)//DO NOT REPEAT YOURSELF!!!!! YOU HAVE ALREADY HAVE THESE SECTION ABOVE!
                sourceType = account;
            else
                sourceType = bank;

            sourceId = Convert.ToInt32(cboMenuAsset.SelectedValue);
            lblAssetId.Content = assetDAL.GetAssetIdBySource(sourceId,sourceType);
        }

        private void rbAccount_Checked(object sender, RoutedEventArgs e)
        {
            //cboMenuAsset.ItemsSource = null;
            LoadCboMenuAsset(account);
        }

        private void rbBank_Checked(object sender, RoutedEventArgs e)
        {
            //cboMenuAsset.ItemsSource = null;
            LoadCboMenuAsset(bank);
        }
    }
}