using BLL;
using CUL;
using DAL;
using KabaAccounting.CUL;
using DAL;
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
        public WinPointOfSale()
        {
            InitializeComponent();
            DisableTools();
            LoadUserInformations();
            LoadPastInvoice();
        }

        UserDAL userDAL = new UserDAL();
        UserBLL userBLL = new UserBLL();
        BankDAL bankDAL = new BankDAL();
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
        AssetDAL assetDAL = new AssetDAL();
        AssetCUL assetCUL = new AssetCUL();
        CommonBLL commonBLL = new CommonBLL();

        const int initialIndex = 0, unitValue = 1;
        const int colLength =9;
        int clickedNewOrEdit;
        const int clickedNothing = -1, clickedNew = 0, clickedEdit = 1, clickedNull = 2;//0 stands for user clicked the button New, and 1 stands for user clicked the button Edit.
        int colNoProductQuantity = 3,colNoProductSalePrice=4,colNoProductGrossTotalSalePrice=5,colNoProductDiscount=6,colNoProductVAT=7, colNoProductTotalSalePrice=8;
        string[] dgCellNames = new string[colLength] { "dgTxtProductId", "dgTxtProductName", "dgTxtProductUnit", "dgTxtProductQuantity", "dgTxtProductSalePrice", "dgTxtProductGrossTotalSalePrice", "dgTxtProductDiscount", "dgTxtProductVAT", "dgTxtProductTotalSalePrice" };
        string[,] oldDgProductCells = new string[,] { };

        string calledBy = "WinPOS";
        string
            colTxtQtyInUnit = "quantity_in_unit",
            colTxtQtyInStock = "quantity_in_stock",
            colTxtSalePrice = "saleprice",
            colTxtAccountId = "account_id",
            colTxtPaymentType = "payment_type",
            colTxtPaymentTypeId = "payment_type_id",
            colTxtCustomerId = "customer_id",
            colTxtInvoiceNo = "invoice_no",
            colTxtId = "id",
            colTxtName = "name",
            colTxtDateAdded = "added_date",
            colTxtProductQty = "quantity",
            colTxtProductId = "product_id",
            colTxtProductUnitId = "product_unit_id",            
            colTxtProductSalePrice = "product_sale_price",
            colTxtProductDiscount="product_discount",
            colTxtProductVAT="product_vat",
            colTxtBarcodeRetail = "barcode_retail",
            colTxtBarcodeWholesale = "barcode_wholesale",
            colTxtUnitRetailId = "unit_retail_id",
            colTxtUnitWholesaleId = "unit_wholesale_id",
            colTxtSubTotal = "sub_total",
            colTxtGrossAmount = "gross_amount",
            colTxtTotalPQuantity = "total_product_quantity",
            colTxtVat = "vat",
            colTxtDiscount = "discount",
            colTxtGrandTotal = "grand_total",
            colTxtSourceBalance= "source_balance",
            colTxtIdSourceType = "id_source_type",
            colTxtIdSource = "id_source",
            coTxtAssetId = "asset_id";

        int account = 1, bank = 2, supplier = 3, customer=4;
        int calledByVAT = 1, calledByDiscount = 2;
        int clickedArrow, clickedPrev = 0, clickedNext = 1;
        int oldIdAsset, oldIdAssetCustomer, oldDgItemsRowCount;
        int uneditedIdAsset, uneditedIdAssetCustomer;
        decimal oldBasketQuantity, oldBasketGrossAmount, oldBasketDiscount, oldBasketSubTotal, oldBasketVAT, oldBasketGrandTotal;//This variables are used while we are editing the datagrid in an active invoice page.
        decimal uneditedBasketSaleTotal, uneditedBasketSubTotal, uneditedBasketGrossAmount, uneditedBasketGrandTotal, uneditedBasketQuantity;//This variables are used when we click the edit button.

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnState_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else
                WindowState = WindowState.Normal;
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
            //btnProductClear.IsEnabled = false; //Disabling the clear button if all text boxes are cleared.
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
            cboMenuAsset.IsEnabled = false;
            cboProductUnit.IsEnabled = false;
            txtProductId.IsEnabled = false;
            txtProductName.IsEnabled = false;
            txtProductSalePrice.IsEnabled = false;
            txtProductQuantity.IsEnabled = false;
            txtProductDiscount.IsEnabled = false;
            txtProductVAT.IsEnabled = false;
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
            cboMenuAsset.IsEnabled = true;
            cboProductUnit.IsEnabled = true;
            txtProductId.IsEnabled = true;
            txtProductName.IsEnabled = true;
            txtProductSalePrice.IsEnabled = true;
            txtProductQuantity.IsEnabled = true;
            txtProductDiscount.IsEnabled = true;
            txtProductVAT.IsEnabled = true;
            dgProducts.IsHitTestVisible = true;//Enabling the datagrid clicking.
        }

        private void ClearProductsDataGrid()
        {
            dgProducts.Items.Clear();
        }

        private void ClearBasketTextBox()
        {
            txtBasketQuantity.Text = initialIndex.ToString();
            txtBasketSubTotal.Text = initialIndex.ToString();
            txtBasketDiscount.Text = initialIndex.ToString();
            txtBasketGrossAmount.Text = initialIndex.ToString();
            txtBasketVat.Text = initialIndex.ToString();
            txtBasketGrandTotal.Text = initialIndex.ToString();
        }

        private void ClearProductEntranceTextBox()
        {
            txtProductId.Text = "";
            txtProductName.Text = "";
            cboProductUnit.ItemsSource = null;
            txtProductSalePrice.Text = "";
            txtProductQuantity.Text = "";
            txtProductDiscount.Text = "";
            txtProductVAT.Text = "";
            txtProductGrossTotalSalePrice.Text = "";
            txtProductTotalSalePrice.Text = "";
            Keyboard.Focus(txtProductId); // set keyboard focus
            DisableProductEntranceButtons();
        }

        private void LoadNewInvoice()/*INVOICE NUMBER REFERS TO THE ID NUMBER IN THE DATABASE FOR POINT OF SALE.*/
        {
            ClearBasketTextBox();
            ClearProductsDataGrid();

            int invoiceId, increment = unitValue;

            invoiceId = commonBLL.GetLastRecordById(calledBy);//Getting the last invoice number and assign it to the variable called invoiceNo.
            invoiceId += increment;//We are adding one to the last invoice number because every new invoice number is one greater tham the previous invoice number.
            lblInvoiceId.Content = invoiceId;//Assigning invoiceNo to the content of the InvoiceNo Label.
        }

        private void LoadPastInvoice(int invoiceId = 0, int clickedArrow = -1)//Optional parameter
        {
            string productId, productName, productSalePrice, productQuantity, productDiscount,productVAT, productGrossTotalSalePrice, productTotalSalePrice;

            if (invoiceId == initialIndex)//If the ID is 0 came from the optional parameter, that means user just clicked the WinPOS button to open it.
            {
                invoiceId = commonBLL.GetLastRecordById(calledBy);//Getting the last invoice id and assign it to the variable called invoiceId.
            }

            /*WE CANNOT USE ELSE IF FOR THE CODE BELOW! BOTH IF STATEMENTS ABOVE AND BELOVE MUST WORK.*/
            if (invoiceId != initialIndex)// If the invoice number is still 0 even when we get the last invoice number by using code above, that means this is the first sale and do not run this code block.
            {
                DataTable dataTablePos = pointOfSaleDAL.GetByIdOrLastId(invoiceId);

                if (dataTablePos.Rows.Count != initialIndex)
                {
                    DataTable dtPosDetail = pointOfSaleDetailDAL.Search(invoiceId);
                    DataTable dtProduct;

                    #region ASSET INFORMATION FILLING REGION
                    int assetId = Convert.ToInt32(dataTablePos.Rows[initialIndex][coTxtAssetId].ToString());//Getting the id of account.
                    lblAssetId.Content = assetId;

                    DataTable dtAsset = assetDAL.SearchById(assetId);
                    int sourceType = Convert.ToInt32(dtAsset.Rows[initialIndex][colTxtIdSourceType]);

                    if (sourceType == account)
                        rbAccount.IsChecked = true;
                    else
                        rbBank.IsChecked = true;

                    cboMenuAsset.SelectedValue = dtAsset.Rows[initialIndex][colTxtIdSource].ToString();
                    #endregion

                    LoadCboMenuPaymentType();
                    LoadCboMenuCustomer();

                    cboMenuPaymentType.SelectedValue = Convert.ToInt32(dataTablePos.Rows[initialIndex][colTxtPaymentTypeId].ToString());//Getting the id of purchase type.
                    cboMenuCustomer.SelectedValue = Convert.ToInt32(dataTablePos.Rows[initialIndex][colTxtCustomerId].ToString());//Getting the id of customer.
                    lblInvoiceId.Content = dataTablePos.Rows[initialIndex][colTxtId].ToString();
                    lblDateAdded.Content = Convert.ToDateTime(dataTablePos.Rows[initialIndex][colTxtDateAdded]).ToString("MM/dd/yyyy HH:mm:ss");

                    #region LOADING THE PRODUCT DATA GRID
                    int productCurrentUnitId, productRetailUnitId, productWholesaleUnitId;

                    for (int currentRow = initialIndex; currentRow < dtPosDetail.Rows.Count; currentRow++)
                    {
                        productId = dtPosDetail.Rows[currentRow][colTxtProductId].ToString();
                        productCurrentUnitId = Convert.ToInt32(dtPosDetail.Rows[currentRow][colTxtProductUnitId]);
                        productQuantity = dtPosDetail.Rows[currentRow][colTxtProductQty].ToString();
                        productSalePrice = dtPosDetail.Rows[currentRow][colTxtProductSalePrice].ToString();
                        productGrossTotalSalePrice = String.Format("{0:0.00}", (Convert.ToDecimal(productSalePrice) * Convert.ToDecimal(productQuantity)));//We do NOT store the total price in the db to reduce the storage. Instead of it, we multiply the unit price with the quantity to find the total price.
                        productDiscount = dtPosDetail.Rows[currentRow][colTxtProductDiscount].ToString();
                        productVAT = dtPosDetail.Rows[currentRow][colTxtProductVAT].ToString();
                        productTotalSalePrice = String.Format("{0:0.00}", (Convert.ToDecimal(productSalePrice) * Convert.ToDecimal(productQuantity))-Convert.ToDecimal(productDiscount)+Convert.ToDecimal(productVAT));

                        dtProduct = productDAL.SearchById(productId);
                        productName = dtProduct.Rows[initialIndex][colTxtName].ToString();//We used initalIndex because there can be only one row in the datatable for a specific product.
                        productRetailUnitId = Convert.ToInt32(dtProduct.Rows[initialIndex][colTxtUnitRetailId]);
                        productWholesaleUnitId = Convert.ToInt32(dtProduct.Rows[initialIndex][colTxtUnitWholesaleId]);

                        #region CBO UNIT INFO FETCHING SECTION
                        List<int> unitIds = new List<int>();

                        unitIds.Add(productRetailUnitId);
                        unitIds.Add(productWholesaleUnitId);

                        DataTable dtUnitInfo = unitDAL.GetProductUnitId(unitIds);
                        #endregion

                        dgProducts.Items.Add(new 
                        {
                            Id = productId,
                            Name = productName,
                            SalePrice = productSalePrice,
                            Quantity = productQuantity,
                            GrossTotalSalePrice=productGrossTotalSalePrice,
                            Discount=productDiscount,
                            VAT=productVAT,
                            TotalSalePrice = productTotalSalePrice,

                            //BINDING DATAGRID COMBOBOX
                            UnitCboItemsSource = dtUnitInfo.DefaultView,
                            UnitCboSValue = productCurrentUnitId,
                            UnitCboSValuePath = colTxtId,
                            UnitCboDMemberPath = colTxtName,
                        });
                    }
                    #endregion

                    #region FILLING THE PREVIOUS BASKET INFORMATIONS

                    //We used initalIndex below as a row name because there can be only one row in the datatable for a specific Invoice.
                    txtBasketQuantity.Text = dataTablePos.Rows[initialIndex][colTxtTotalPQuantity].ToString();
                    txtBasketSubTotal.Text = dataTablePos.Rows[initialIndex][colTxtSubTotal].ToString();
                    txtBasketDiscount.Text = dataTablePos.Rows[initialIndex][colTxtDiscount].ToString();
                    txtBasketGrossAmount.Text = dataTablePos.Rows[initialIndex][colTxtGrossAmount].ToString();
                    txtBasketVat.Text = dataTablePos.Rows[initialIndex][colTxtVat].ToString();
                    txtBasketGrandTotal.Text = dataTablePos.Rows[initialIndex][colTxtGrandTotal].ToString();

                    #endregion
                }
                else if (dataTablePos.Rows.Count == initialIndex)//If the pos detail row quantity is 0, that means there is no such row so decrease or increase the invoice number according to user preference.
                {
                    if (clickedArrow == initialIndex)//If the invoice arrow is 0, that means user clicked the previous button.
                    {
                        invoiceId = invoiceId - unitValue;
                    }
                    else
                    {
                        invoiceId = invoiceId + unitValue;
                    }

                    if (clickedArrow != -unitValue)//If the user has not clicked either previous or next button, then the clickedArrow will be -1 and no need for recursion.
                    {
                        LoadPastInvoice(invoiceId, clickedArrow);//Call the method again to get the new past invoice.
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
            int rowLength = dgProducts.Items.Count, cellUnit = 2;
            string[,] dgProductCells = new string[rowLength, colLength];

            DataGridRow dgRow;
            ContentPresenter cpProduct;
            TextBox tbCellContent;
            ComboBox tbCellContentCbo;

            for (int rowNo = initialIndex; rowNo < rowLength; rowNo++)
            {
                dgRow = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(rowNo);

                for (int colNo = initialIndex; colNo < colLength; colNo++)
                {
                    cpProduct = dgProducts.Columns[colNo].GetCellContent(dgRow) as ContentPresenter;
                    var tmpProduct = cpProduct.ContentTemplate;

                    if (colNo != cellUnit)//If it is not a cbo value, then fetch the data in the default way.
                    {
                        tbCellContent = tmpProduct.FindName(dgCellNames[colNo], cpProduct) as TextBox;
                        dgProductCells[rowNo, colNo] = tbCellContent.Text;
                    }
                    else//If it is a cbo value, then fetch the selected value.
                    {
                        tbCellContentCbo = tmpProduct.FindName(dgCellNames[colNo], cpProduct) as ComboBox;
                        dgProductCells[rowNo, colNo] = tbCellContentCbo.SelectedValue.ToString();
                    }
                    //dgOldProductCells[rowNo, colNo] = cells[rowNo, colNo];//Assigning the old products' informations to the global array called "dgOldProductCells" so that we can access to the old products to revert the changes.
                }
            }

            return dgProductCells;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            int emptyIndex = -unitValue;
            string[,] dgNewProductCells = new string[,] { };

            dgNewProductCells = (string[,])(GetDataGridContent().Clone());//Cloning one array into another array.

            #region Comparing two multidimensional arrays
            bool isDgEqual =
            oldDgProductCells.Rank == dgNewProductCells.Rank &&
            Enumerable.Range(0, oldDgProductCells.Rank).All(dimension => oldDgProductCells.GetLength(dimension) == dgNewProductCells.GetLength(dimension)) &&
            oldDgProductCells.Cast<string>().SequenceEqual(dgNewProductCells.Cast<string>());
            #endregion

            //If the old datagrid equals new datagrid, no need for saving because the user did not change anything.(ONLY IN CASE OF CLICKING TO THE EDIT BUTTON!!!)
            //-1 means nothing has been chosen in the combobox. Note: We don't add the --&& lblInvoiceNo.Content.ToString()!= "0"-- into the if statement because the invoice label cannot be 0 due to the restrictions.
            if (int.TryParse((lblInvoiceId.Content).ToString(), out int number) && isDgEqual == false && cboMenuPaymentType.SelectedIndex != emptyIndex && cboMenuCustomer.SelectedIndex != emptyIndex && cboMenuAsset.SelectedIndex != emptyIndex)
            {
                int invoiceId = Convert.ToInt32(lblInvoiceId.Content); /*lblInvoiceId stands for the invoice id in the database.*/
                int userId = userBLL.GetUserId(WinLogin.loggedInUserName);
                bool isSuccess = false, isSuccessDetail = false, isSuccessAsset = false;
                int cellProductUnit = 2, cellProductQuantity = 3, cellProductSalePrice=4, cellProductDiscount=6, cellProductVAT=7;
                int productId;
                int unitId;
                decimal productOldQtyInStock, newQuantity, newSalePrice;
                int cellLength = 8;
                int addedBy = userId;
                string[] cells = new string[cellLength];
                DateTime dateTime = DateTime.Now;
                int productRate = initialIndex;//Modify this code dynamically!!!!!!!!!

                DataTable dataTableLastInvoice = pointOfSaleBLL.GetLastInvoiceRecord();//Getting the last invoice.
                DataTable dataTableProduct = new DataTable();
                DataTable dataTableUnit = new DataTable();
                DataTable dtAsset = new DataTable();
                decimal oldSourceBalance;

                if (clickedNewOrEdit == clickedEdit)
                {
                    #region ASSET BALANCE REVERTING SECTION
                    dtAsset = assetDAL.SearchById(uneditedIdAsset);
                    oldSourceBalance = Convert.ToDecimal(dtAsset.Rows[initialIndex][colTxtSourceBalance]);

                    assetCUL.SourceBalance = oldSourceBalance - uneditedBasketGrandTotal;//We have to subtract the old grandTotal from the asset's source balance because the new grand total may be different from it.
                    assetCUL.Id = Convert.ToInt32(uneditedIdAsset);
                    isSuccessAsset = assetDAL.Update(assetCUL);
                    #endregion
                }

                #region ASSET BALANCE UPDATING SECTION
                dtAsset = assetDAL.SearchById(Convert.ToInt32(lblAssetId.Content));
                oldSourceBalance = Convert.ToDecimal(dtAsset.Rows[initialIndex][colTxtSourceBalance]);

                assetCUL.SourceBalance = oldSourceBalance + Convert.ToDecimal(txtBasketGrandTotal.Text);//We are earning money for this purchase and adding the price to the old balance of our asset.
                assetCUL.Id = Convert.ToInt32(lblAssetId.Content);
                isSuccessAsset = assetDAL.Update(assetCUL);
                #endregion

                #region TABLE POS SAVING SECTION
                //Getting the values from the POS Window and fill them into the pointOfSaleCUL.
                pointOfSaleCUL.Id = invoiceId;//The column invoice id in the database is not auto incremental. This is for preventing the number increasing when the user deletes an existing invoice and creates a new invoice.
                pointOfSaleCUL.PaymentTypeId = Convert.ToInt32(cboMenuPaymentType.SelectedValue);
                pointOfSaleCUL.CustomerId = Convert.ToInt32(cboMenuCustomer.SelectedValue);
                pointOfSaleCUL.AssetId = Convert.ToInt32(lblAssetId.Content);
                pointOfSaleCUL.TotalProductQuantity = Convert.ToDecimal(txtBasketQuantity.Text);
                pointOfSaleCUL.SubTotal = Convert.ToDecimal(txtBasketSubTotal.Text);
                pointOfSaleCUL.Discount = Convert.ToDecimal(txtBasketDiscount.Text);
                pointOfSaleCUL.GrossAmount = Convert.ToDecimal(txtBasketGrossAmount.Text);
                pointOfSaleCUL.Vat = Convert.ToDecimal(txtBasketVat.Text);
                pointOfSaleCUL.GrandTotal = Convert.ToDecimal(txtBasketGrandTotal.Text);
                pointOfSaleCUL.AddedDate = DateTime.Now;
                pointOfSaleCUL.AddedBy = userId;

                if (clickedNewOrEdit == clickedEdit)//If the user clicked the btnEdit, then update the specific invoice information in tbl_pos at once.
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

                DataGridRow dgRow;
                ContentPresenter cpProduct;
                TextBox tbCellContent;
                ComboBox tbCellContentCbo;

                for (int rowNo = initialIndex; rowNo < dgProducts.Items.Count; rowNo++)
                {
                    if (clickedNewOrEdit == clickedEdit)//If the user clicked the btnEdit, then edit the specific invoice's products in tbl_pos_detailed at once.
                    {
                        productBLL.RevertOldQuantityInStock(oldDgProductCells, oldDgItemsRowCount, calledBy);//Reverting the old products' quantity in stock.

                        //We are sending invoiceNo as a parameter to the "Delete" Method. So that we can erase all the products which have the specific invoice number.
                        pointOfSaleDetailDAL.Delete(invoiceId);

                        //2 means null for this code. We used this in order to prevent running the if block again and again. Because, we erase all of the products belong to one invoice number at once.
                        clickedNewOrEdit = clickedNull;
                    }

                    dgRow = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(rowNo);

                    for (int colNo = initialIndex; colNo < cellLength; colNo++)
                    {
                        cpProduct = dgProducts.Columns[colNo].GetCellContent(dgRow) as ContentPresenter;
                        var tmpProduct = cpProduct.ContentTemplate;

                        if (colNo != cellProductUnit)
                        {
                            tbCellContent = tmpProduct.FindName(dgCellNames[colNo], cpProduct) as TextBox;
                            cells[colNo] = tbCellContent.Text;
                        }
                        else
                        {
                            tbCellContentCbo = tmpProduct.FindName(dgCellNames[colNo], cpProduct) as ComboBox;
                            cells[colNo] = tbCellContentCbo.SelectedValue.ToString();
                        }
                    }

                    dataTableProduct = productDAL.SearchProductByIdBarcode(cells[initialIndex]);//Cell[0] contains product id.
                    productId = Convert.ToInt32(dataTableProduct.Rows[initialIndex][colTxtId]);//Row index is always zero for this situation because there can be only one row of a product which has a unique barcode on the table.


                    dataTableUnit = unitDAL.GetUnitInfoById(Convert.ToInt32(cells[cellProductUnit]));//Cell[2] contains the unit id in the combobox.
                    unitId = Convert.ToInt32(dataTableUnit.Rows[initialIndex][colTxtId]);//Row index is always zero for this situation because there can be only one row of a specific unit.

                    pointOfSaleDetailCUL.Id = invoiceId;//No incremental value in the database because there can be multiple goods with the same invoice id.
                    pointOfSaleDetailCUL.ProductId = productId;
                    pointOfSaleDetailCUL.AddedBy = addedBy;
                    pointOfSaleDetailCUL.ProductRate = productRate;
                    pointOfSaleDetailCUL.ProductUnitId = unitId;
                    pointOfSaleDetailCUL.ProductSalePrice = Convert.ToDecimal(cells[cellProductSalePrice]);//cells[4] contains sale price of the product in the list. We have to store the current sale price as well because it may be changed in the future.
                    pointOfSaleDetailCUL.ProductQuantity = Convert.ToDecimal(cells[cellProductQuantity]);
                    pointOfSaleDetailCUL.ProductDiscount = Convert.ToDecimal(cells[cellProductDiscount]);
                    pointOfSaleDetailCUL.ProductVAT = Convert.ToDecimal(cells[cellProductVAT]);

                    isSuccessDetail = pointOfSaleDetailDAL.Insert(pointOfSaleDetailCUL);

                    #region PRODUCT QUANTITY UPDATE
                    productOldQtyInStock = Convert.ToDecimal(dataTableProduct.Rows[initialIndex][colTxtQtyInStock].ToString());//Getting the old product quantity in stock.

                    newQuantity = productOldQtyInStock - Convert.ToDecimal(cells[cellProductQuantity]);

                    productDAL.UpdateSpecificColumn(productId, colTxtQtyInStock, newQuantity.ToString());
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

        private void cboProductUnit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (txtProductQuantity.Text != "" && cboProductUnit.ItemsSource != null)
            {
                decimal productQuantity;
                int productRetailUnitId;
                string productIdFromUser = txtProductId.Text;
                DataTable dtProduct = productDAL.SearchProductByIdBarcode(productIdFromUser);
                int productId = Convert.ToInt32(dtProduct.Rows[initialIndex][colTxtId]); //We need to get the Id of the product from the db even if the user enters an id because user may also enter a barcode.

                productRetailUnitId = Convert.ToInt32(dtProduct.Rows[initialIndex][colTxtUnitRetailId]);
                //productWholesaleUnitId = Convert.ToInt32(dtProduct.Rows[initialIndex][colTxtUnitWholesaleId]);

                if (Convert.ToInt32(cboProductUnit.SelectedValue) == productRetailUnitId)
                {
                    productQuantity = unitValue;//If it is a unit retail id, the assign one asa default value.
                }

                else
                {
                    productQuantity = Convert.ToDecimal(dtProduct.Rows[initialIndex][colTxtQtyInUnit]);
                }

                txtProductQuantity.Text = productQuantity.ToString();
                txtProductGrossTotalSalePrice.Text = (productQuantity * Convert.ToDecimal(txtProductSalePrice.Text)).ToString();
                txtProductTotalSalePrice.Text = (
                    (productQuantity * Convert.ToDecimal(txtProductSalePrice.Text))-
                    Convert.ToDecimal(txtProductDiscount.Text)+
                    Convert.ToDecimal(txtProductVAT.Text)
                    ).ToString();
            }
        }

        private void btnProductAdd_Click(object sender, RoutedEventArgs e)//Try to do this by using listview
        {
            bool addNewProductLine = true;
            int productQuantity;
            int rowQuntity = dgProducts.Items.Count;
            DataTable dtProduct = productDAL.SearchProductByIdBarcode(txtProductId.Text);
            int productId = Convert.ToInt32(dtProduct.Rows[initialIndex][colTxtId]); //We need to get the Id of the product from the db even if the user enters an id because user may also enter a barcode.

            for (int i = 0; i < rowQuntity; i++)
            {
                DataGridRow row = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(i);

                ContentPresenter cpProduct = dgProducts.Columns[initialIndex].GetCellContent(row) as ContentPresenter;
                var tmpProduct = cpProduct.ContentTemplate;
                TextBox tbBarcodeCellContent = tmpProduct.FindName(dgCellNames[initialIndex], cpProduct) as TextBox;

                if (tbBarcodeCellContent.Text == productId.ToString())
                {
                    if (MessageBox.Show("There is already the same item in the list. Would you like to sum them?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        //GETTING THE CELL CONTENT OF THE PRODUCT QUANTITY
                        ContentPresenter cpProductQty = dgProducts.Columns[colNoProductQuantity].GetCellContent(row) as ContentPresenter;
                        var tmpProductQty = cpProductQty.ContentTemplate;
                        TextBox txtDgProductQty = tmpProductQty.FindName(dgCellNames[colNoProductQuantity], cpProductQty) as TextBox;

                        //GETTING THE CELL CONTENT OF THE PRODUCT GROSS TOTAL SALE PRICE
                        ContentPresenter cpProductGrossTotalSalePrice = dgProducts.Columns[colNoProductGrossTotalSalePrice].GetCellContent(row) as ContentPresenter;
                        var tmpProductGrossTotalSalePrice = cpProductGrossTotalSalePrice.ContentTemplate;
                        TextBox txtDgProductGrossTotalSalePrice = tmpProductGrossTotalSalePrice.FindName(dgCellNames[colNoProductGrossTotalSalePrice], cpProductGrossTotalSalePrice) as TextBox;

                        //GETTING THE CELL CONTENT OF THE PRODUCT TOTAL SALE PRICE
                        ContentPresenter cpProductTotalSalePrice = dgProducts.Columns[colNoProductTotalSalePrice].GetCellContent(row) as ContentPresenter;
                        var tmpProductTotalSalePrice = cpProductTotalSalePrice.ContentTemplate;
                        TextBox txtDgProductTotalSalePrice = tmpProductTotalSalePrice.FindName(dgCellNames[colNoProductTotalSalePrice], cpProductTotalSalePrice) as TextBox;

                        //CALCULATING NEW PRODUCT QUANTITY IN DATAGRID
                        productQuantity = Convert.ToInt32(txtDgProductQty.Text);
                        productQuantity += Convert.ToInt32(txtProductQuantity.Text);//We are adding the quantity entered in the "txtProductQuantity" to the previous quantity cell's quantity.

                        //ASSIGNING NEW VALUES TO THE RELATED DATA GRID CELLS.
                        txtDgProductQty.Text = productQuantity.ToString();
                        txtDgProductGrossTotalSalePrice.Text = (productQuantity * Convert.ToDecimal(txtProductSalePrice.Text)).ToString();
                        txtDgProductTotalSalePrice.Text = (
                            (productQuantity * Convert.ToDecimal(txtProductSalePrice.Text)) -
                            Convert.ToDecimal(txtProductDiscount.Text) +
                            Convert.ToDecimal(txtProductVAT.Text)
                            ).ToString();

                        addNewProductLine = false;
                        break;//We have to break the loop if the user clicked "yes" because no need to scan the rest of the rows after confirming.
                    }
                }
            }

            if (addNewProductLine == true)//Use ENUMS instead of this!!!!!!!
            {                
                //dgProducts.Items.Add(new ProductCUL(){ Id = Convert.ToInt32(txtProductId.Text), Name = txtProductName.Text });// You can also apply this code instead of the code below. Note that you have to change the binding name in the datagrid with the name of the property in ProductCUL if you wish to use this code.
                dgProducts.Items.Add(new
                {
                    Id = productId,
                    Name = txtProductName.Text,
                    SalePrice = txtProductSalePrice.Text,
                    Quantity = txtProductQuantity.Text,
                    GrossTotalSalePrice = txtProductGrossTotalSalePrice.Text,
                    Discount=txtProductDiscount.Text,
                    VAT=txtProductVAT.Text,
                    TotalSalePrice = txtProductTotalSalePrice.Text,

                    //BINDING DATAGRID COMBOBOX
                    UnitCboItemsSource = cboProductUnit.ItemsSource,
                    UnitCboSValue = cboProductUnit.SelectedValue,
                    UnitCboSValuePath = cboProductUnit.SelectedValuePath,
                    UnitCboDMemberPath = cboProductUnit.DisplayMemberPath,
                });
            }

            dgProducts.UpdateLayout();
            PopulateBasket();
            ClearProductEntranceTextBox();
        }

        private void PopulateBasket()
        {
            decimal quantityFromTextEntry = Convert.ToDecimal(txtProductQuantity.Text);

            txtBasketQuantity.Text = (Convert.ToDecimal(txtBasketQuantity.Text) + quantityFromTextEntry).ToString();

            txtBasketGrossAmount.Text = (Convert.ToDecimal(txtBasketGrossAmount.Text) + (Convert.ToDecimal(txtProductSalePrice.Text) * quantityFromTextEntry)).ToString();

            txtBasketDiscount.Text = (Convert.ToDecimal(txtBasketDiscount.Text)+Convert.ToDecimal(txtProductDiscount.Text)).ToString();

            txtBasketSubTotal.Text = (Convert.ToDecimal(txtBasketSubTotal.Text) + ((Convert.ToDecimal(txtProductSalePrice.Text) * quantityFromTextEntry)- Convert.ToDecimal(txtProductDiscount.Text))).ToString();//The previous sub total has already basket discount in it so we only need to subtract the new product's discount.

            txtBasketVat.Text = (Convert.ToDecimal(txtBasketVat.Text) + Convert.ToDecimal(txtProductVAT.Text)).ToString();

            txtBasketGrandTotal.Text = (Convert.ToDecimal(txtBasketGrandTotal.Text) + (Convert.ToDecimal(txtProductSalePrice.Text) * quantityFromTextEntry) - Convert.ToDecimal(txtProductDiscount.Text) + Convert.ToDecimal(txtProductVAT.Text)).ToString();

        }

        private void SubstractBasket(int selectedRowIndex)
        {
            DataGridRow dataGridRow;

            dataGridRow = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(selectedRowIndex);

            //GETTING THE CELL CONTENT OF THE PRODUCT QUANTITY
            ContentPresenter cpProductQuantity = dgProducts.Columns[colNoProductQuantity].GetCellContent(dataGridRow) as ContentPresenter;
            var tmpProductQuantity = cpProductQuantity.ContentTemplate;
            TextBox txtDgProductQty = tmpProductQuantity.FindName(dgCellNames[colNoProductQuantity], cpProductQuantity) as TextBox;

            //GETTING THE CELL CONTENT OF THE PRODUCT GROSS TOTAL SALE PRICE
            ContentPresenter cpProductGrossTotalSalePrice = dgProducts.Columns[colNoProductGrossTotalSalePrice].GetCellContent(dataGridRow) as ContentPresenter;
            var tmpProductGrossTotalSalePrice = cpProductGrossTotalSalePrice.ContentTemplate;
            TextBox txtDgProductGrossTotalSalePrice = tmpProductGrossTotalSalePrice.FindName(dgCellNames[colNoProductGrossTotalSalePrice], cpProductGrossTotalSalePrice) as TextBox;

            //GETTING THE CELL CONTENT OF THE PRODUCT DISCOUNT
            ContentPresenter cpProductDiscount = dgProducts.Columns[colNoProductDiscount].GetCellContent(dataGridRow) as ContentPresenter;
            var tmpProductDiscount = cpProductDiscount.ContentTemplate;
            TextBox txtDgProductDiscount = tmpProductDiscount.FindName(dgCellNames[colNoProductDiscount], cpProductDiscount) as TextBox;

            //GETTING THE CELL CONTENT OF THE PRODUCT VAT
            ContentPresenter cpProductVAT = dgProducts.Columns[colNoProductVAT].GetCellContent(dataGridRow) as ContentPresenter;
            var tmpProductVAT = cpProductVAT.ContentTemplate;
            TextBox txtDgProductVAT = tmpProductVAT.FindName(dgCellNames[colNoProductVAT], cpProductVAT) as TextBox;

            //GETTING THE CELL CONTENT OF THE PRODUCT TOTAL SALE PRICE
            ContentPresenter cpProductTotalSalePrice = dgProducts.Columns[colNoProductTotalSalePrice].GetCellContent(dataGridRow) as ContentPresenter;
            var tmpProductTotalSalePrice = cpProductTotalSalePrice.ContentTemplate;
            TextBox txtDgProductTotalSalePrice = tmpProductTotalSalePrice.FindName(dgCellNames[colNoProductTotalSalePrice], cpProductTotalSalePrice) as TextBox;


            //ASSIGNING NEW VALUES TO THE BASKET'S RELATED TEXT BOXES.
            txtBasketQuantity.Text = (Convert.ToDecimal(txtBasketQuantity.Text) - Convert.ToDecimal(txtDgProductQty.Text)).ToString();

            txtBasketGrossAmount.Text = (Convert.ToDecimal(txtBasketGrossAmount.Text) - Convert.ToDecimal(txtDgProductGrossTotalSalePrice.Text)).ToString();
            
            txtBasketSubTotal.Text = (Convert.ToDecimal(txtBasketSubTotal.Text) - (Convert.ToDecimal(txtDgProductGrossTotalSalePrice.Text) - Convert.ToDecimal(txtDgProductDiscount.Text))).ToString();

            txtBasketDiscount.Text= (Convert.ToDecimal(txtBasketDiscount.Text) - Convert.ToDecimal(txtDgProductDiscount.Text)).ToString();

            txtBasketVat.Text= (Convert.ToDecimal(txtBasketVat.Text) - Convert.ToDecimal(txtDgProductVAT.Text)).ToString();

            txtBasketGrandTotal.Text = (Convert.ToDecimal(txtBasketGrandTotal.Text) - Convert.ToDecimal(txtDgProductTotalSalePrice.Text)).ToString();
        }

        //private void cboMenuAsset_Loaded(object sender, RoutedEventArgs e)
        //{
        //    //Creating Data Table to hold the products from Database
        //    DataTable dataTable = accountDAL.Select();

        //    //Specifying Items Source for product combobox
        //    cboMenuAsset.ItemsSource = dataTable.DefaultView;

        //    //Here DisplayMemberPath helps to display Text in the ComboBox.
        //    cboMenuAsset.DisplayMemberPath = "name";

        //    //SelectedValuePath helps to store values like a hidden field.
        //    cboMenuAsset.SelectedValuePath = "id";
        //}

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
            clickedNewOrEdit = clickedEdit;//1 stands for the user has entered the btnEdit.
            oldDgItemsRowCount = dgProducts.Items.Count;//When the user clicks Edit, the index of old(previously saved) items row will be assigned to oldItemsRowCount.
            oldDgProductCells = (string[,])(GetDataGridContent().Clone());//Cloning one array into another array.

            //YOU MUST ASSIGN THE OLD VALUES TO THE VARIABLES WITH THE PREFIX "unedited" JUST LIKE BELOW instead of "old" BECAUSE THOSE VARIABLES WITH THE PREFIX "old" ARE FOR DATA GRID CHANGE!!!
            uneditedIdAsset = Convert.ToInt32(lblAssetId.Content);
            uneditedIdAssetCustomer = Convert.ToInt32(lblAssetCustomerId.Content);
            uneditedBasketGrandTotal = Convert.ToDecimal(txtBasketGrandTotal.Text);

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
                    oldDgProductCells = (string[,])GetDataGridContent().Clone();//Cloning one array into another array.
                    productBLL.RevertOldQuantityInStock(oldDgProductCells, dgProducts.Items.Count, calledBy);
                    #endregion

                    #region REVERT THE ASSET
                    int assetCompanyId = Convert.ToInt32(lblAssetId.Content);
                    decimal oldCompanyBalance;

                    //CODE DUPLICATION!!!! SIMILAR EXISTS IN SAVE SECTION

                    DataTable dtAssetCompany = assetDAL.SearchById(assetCompanyId);
                    oldCompanyBalance = Convert.ToDecimal(dtAssetCompany.Rows[initialIndex]["source_balance"]);

                    assetCUL.SourceBalance = oldCompanyBalance - Convert.ToDecimal(txtBasketGrandTotal.Text);//We need to give the price back to the customer for reverting this purchase.
                    assetCUL.Id = Convert.ToInt32(lblAssetId.Content);

                    assetDAL.Update(assetCUL);
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
            int firstInvoiceId = unitValue, currentInvoiceId = Convert.ToInt32(lblInvoiceId.Content);

            if (currentInvoiceId != firstInvoiceId)
            {
                ClearProductsDataGrid();
                int prevInvoice = currentInvoiceId - unitValue;
                clickedArrow = clickedPrev;//0 means customer has clicked the previous button.
                LoadPastInvoice(prevInvoice, clickedArrow);
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            int lastInvoiceId = commonBLL.GetLastRecordById(calledBy), currentInvoiceId;

            currentInvoiceId = Convert.ToInt32(lblInvoiceId.Content);

            if (currentInvoiceId != lastInvoiceId)
            {
                ClearProductsDataGrid();
                int nextInvoice = currentInvoiceId + unitValue;
                clickedArrow = clickedNext;//1 means customer has clicked the next button.
                LoadPastInvoice(nextInvoice, clickedArrow);
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
        private void txtProductQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtProductQuantity.IsFocused == true)//If the cursor is not focused on this textbox, then no need to check this code.
            {
                if (txtProductQuantity.Text != "")
                {
                    decimal number;
                    string productQuantity = txtProductQuantity.Text;

                    char lastCharacter = char.Parse(productQuantity.Substring(productQuantity.Length - 1));//Getting the last character to check if the user has entered a missing quantity like " 3, "

                    bool result = Char.IsDigit(lastCharacter);//Checking if the last digit of the number is a number or not.

                    if (decimal.TryParse(productQuantity, out number) && result == true)
                    {
                        string unitKg = "Kilogram", unitLt = "Liter";
                        string productSalePrice = txtProductSalePrice.Text;

                        if (cboProductUnit.Text != unitKg && cboProductUnit.Text != unitLt)
                        {
                            /*If the user entered any unit except kilogram or liter, there cannot be a decimal quantity. 
                            So, convert the quantity to integer even the user has entered a decimal quantity as a mistake.*/
                            productQuantity = Convert.ToInt32(txtProductQuantity.Text).ToString();
                            txtProductQuantity.Text = productQuantity.ToString();
                        }
                        else//If the user has defined the unit as kilogram or liter, then there can be a decimal quantity like "3,5 liter."
                        {
                            productQuantity = Convert.ToDecimal(txtProductQuantity.Text).ToString();
                        }

                        txtProductGrossTotalSalePrice.Text = (Convert.ToDecimal(productSalePrice) * Convert.ToDecimal(productQuantity)).ToString();
                        txtProductTotalSalePrice.Text = (
                            (Convert.ToDecimal(productSalePrice) * Convert.ToDecimal(productQuantity)) -
                            Convert.ToDecimal(txtProductDiscount.Text) +
                            Convert.ToDecimal(txtProductVAT.Text)
                            ).ToString();
                        btnProductAdd.IsEnabled = true;
                    }

                    else//Reverting the quantity to the default value.
                    {
                        MessageBox.Show("Please enter a valid number");
                        txtProductQuantity.Text = unitValue.ToString();//We are reverting the quantity of the product to default if the user has pressed a wrong key such as "a-b-c".
                    }
                }

                /* If the user left the txtProductQuantity as empty, wait for him to enter a new value and block the btnProductAdd. 
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
            //GETTING TEXTBOX FROM DATAGRID.
            ContentPresenter cpProductSalePrice = dgProducts.Columns[colNoProductSalePrice].GetCellContent(dgProducts.SelectedItem) as ContentPresenter;
            var tmpProductSalePrice = cpProductSalePrice.ContentTemplate;
            TextBox txtDgProductSalePrice = tmpProductSalePrice.FindName(dgCellNames[colNoProductSalePrice], cpProductSalePrice) as TextBox;

            //GETTING TEXTBOX FROM DATAGRID.
            ContentPresenter cpProductQuantity = dgProducts.Columns[colNoProductQuantity].GetCellContent(dgProducts.SelectedItem) as ContentPresenter;
            var tmpProductQuantity = cpProductQuantity.ContentTemplate;
            TextBox txtDgProductQuantity = tmpProductQuantity.FindName(dgCellNames[colNoProductQuantity], cpProductQuantity) as TextBox;

            //GETTING THE CELL CONTENT OF THE PRODUCT GROSS TOTAL SALE PRICE
            ContentPresenter cpProductGrossTotalSalePrice = dgProducts.Columns[colNoProductGrossTotalSalePrice].GetCellContent(dgProducts.SelectedItem) as ContentPresenter;
            var tmpProductGrossTotalSalePrice = cpProductGrossTotalSalePrice.ContentTemplate;
            TextBox txtDgProductGrossTotalSalePrice = tmpProductGrossTotalSalePrice.FindName(dgCellNames[colNoProductGrossTotalSalePrice], cpProductGrossTotalSalePrice) as TextBox;

            //GETTING TEXTBOX FROM DATAGRID.
            ContentPresenter cpProductDiscount = dgProducts.Columns[colNoProductDiscount].GetCellContent(dgProducts.SelectedItem) as ContentPresenter;
            var tmpProductDiscount = cpProductDiscount.ContentTemplate;
            TextBox txtDgProductDiscount = tmpProductDiscount.FindName(dgCellNames[colNoProductDiscount], cpProductDiscount) as TextBox;

            //GETTING TEXTBOX FROM DATAGRID.
            ContentPresenter cpProductVAT = dgProducts.Columns[colNoProductVAT].GetCellContent(dgProducts.SelectedItem) as ContentPresenter;
            var tmpProductVAT = cpProductVAT.ContentTemplate;
            TextBox txtDgProductVAT = tmpProductVAT.FindName(dgCellNames[colNoProductVAT], cpProductVAT) as TextBox;

            //GETTING TEXTBOX FROM DATAGRID
            ContentPresenter cpProductTotalSalePrice = dgProducts.Columns[colNoProductTotalSalePrice].GetCellContent(dgProducts.SelectedItem) as ContentPresenter;
            var tmpProductTotalSalePrice = cpProductTotalSalePrice.ContentTemplate;
            TextBox txtDgProductTotalSalePrice = tmpProductTotalSalePrice.FindName(dgCellNames[colNoProductTotalSalePrice], cpProductTotalSalePrice) as TextBox;

            if (txtDgProductQuantity.Text != "" && txtDgProductSalePrice.Text != "")
            {
                txtDgProductQuantity.Text = txtDgProductQuantity.Text.ToString();//We need to reassign it otherwise it will not be affected.
                txtDgProductGrossTotalSalePrice.Text = (Convert.ToDecimal(txtDgProductSalePrice.Text) * Convert.ToDecimal(txtDgProductQuantity.Text)).ToString();
                txtDgProductTotalSalePrice.Text = ((Convert.ToDecimal(txtDgProductSalePrice.Text) * Convert.ToDecimal(txtDgProductQuantity.Text))- Convert.ToDecimal(txtDgProductDiscount.Text)+ Convert.ToDecimal(txtDgProductVAT.Text)).ToString();

                txtBasketQuantity.Text = (oldBasketQuantity + Convert.ToDecimal(txtDgProductQuantity.Text)).ToString();
                txtBasketGrossAmount.Text = (oldBasketGrossAmount + Convert.ToDecimal(txtDgProductGrossTotalSalePrice.Text)).ToString();
                txtBasketSubTotal.Text = (oldBasketSubTotal + Convert.ToDecimal(txtDgProductGrossTotalSalePrice.Text) - Convert.ToDecimal(txtDgProductDiscount.Text)).ToString();
                txtBasketDiscount.Text = (oldBasketDiscount + Convert.ToDecimal(txtDgProductDiscount.Text)).ToString();
                txtBasketVat.Text = (oldBasketVAT + Convert.ToDecimal(txtDgProductVAT.Text)).ToString();
                txtBasketGrandTotal.Text = (oldBasketGrandTotal + Convert.ToDecimal(txtDgProductTotalSalePrice.Text)).ToString();//VAT and Discount are already in the old grand total so no need to calculate them.
            }
        }

        private void dgTxtProductSalePrice_KeyUp(object sender, KeyEventArgs e)
        {
            DgTextChanged();
        }
        private void dgTxtProductQuantity_KeyUp(object sender, KeyEventArgs e)
        {
            DgTextChanged();
        }

        private void txtProductId_KeyUp(object sender, KeyEventArgs e)
        {
            string productIdFromUser = txtProductId.Text;
            long number;

            DataTable dtProduct = productDAL.SearchProductByIdBarcode(productIdFromUser);

            if (e.Key == Key.Enter)
            {
                if (btnProductAdd.IsEnabled == true)//If either product add or cancel is activated, that means the user has entered a valid id and first If statement above is worked.
                {
                    btnProductAdd_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("You cannot enter because the Id/Barcode is wrong!");
                    Keyboard.ClearFocus();//It is necessary to clear focus from the txtId because this method works recursively once we click the enter button after getting the error message.
                    //txtProductId.Focus();
                    //txtProductId.SelectAll();//Selecting all of the text to correct it easily at once.
                }
            }

            else if (productIdFromUser != initialIndex.ToString() && long.TryParse(productIdFromUser, out number) && dtProduct.Rows.Count != initialIndex)//Validating the barcode if it is a number(except zero) or not.
            {
                decimal productQuantity,productDiscount=initialIndex,productVAT=initialIndex;
                int productId;
                int productCurrentUnitId, productRetailUnitId, productWholesaleUnitId;
                string productBarcodeRetail/*, productBarcodeWholesale*/;
                string salePrice;

                btnProductAdd.IsEnabled = true; //Enabling the add button if any valid barcode is entered.

                productId = Convert.ToInt32(dtProduct.Rows[initialIndex][colTxtId]);
                productBarcodeRetail = dtProduct.Rows[initialIndex][colTxtBarcodeRetail].ToString();
                //productBarcodeWholesale = dataTable.Rows[rowIndex]["barcode_wholesale"].ToString();
                txtProductName.Text = dtProduct.Rows[initialIndex][colTxtName].ToString();//Filling the product name textbox from the database
                productRetailUnitId = Convert.ToInt32(dtProduct.Rows[initialIndex][colTxtUnitRetailId]);
                productWholesaleUnitId = Convert.ToInt32(dtProduct.Rows[initialIndex][colTxtUnitWholesaleId]);

                if (productBarcodeRetail == productIdFromUser || productId.ToString() == productIdFromUser)//If the barcode equals the product's barcode_retail or id, then take the product's retail unit id.
                {
                    productCurrentUnitId = productRetailUnitId;
                    productQuantity = unitValue;//If it is a unit retail id, the assign one as a default value.
                }

                else //If the barcode equals to the barcode_wholesale, then take the product's wholesale unit id.
                {
                    productCurrentUnitId = productWholesaleUnitId;
                    productQuantity = Convert.ToDecimal(dtProduct.Rows[initialIndex][colTxtQtyInUnit]);
                }

                #region CBO UNIT POPULATING SECTION

                List<int> unitIds = new List<int>();

                unitIds.Add(productRetailUnitId);
                unitIds.Add(productWholesaleUnitId);

                //var listUnitInfo = unitDAL.GetProductUnitId(primeNumbers).Select(i => i.ToString()).ToList();

                DataTable dtUnitInfo = unitDAL.GetProductUnitId(unitIds);

                //Specifying Items Source for product combobox
                cboProductUnit.ItemsSource = dtUnitInfo.DefaultView;

                //Here DisplayMemberPath helps to display Text in the ComboBox.
                cboProductUnit.DisplayMemberPath = colTxtName;

                //SelectedValuePath helps to store values like a hidden field.
                cboProductUnit.SelectedValuePath = colTxtId;

                cboProductUnit.SelectedValue = productCurrentUnitId;
                #endregion

                salePrice = dtProduct.Rows[initialIndex][colTxtSalePrice].ToString();

                txtProductSalePrice.Text = salePrice;
                txtProductQuantity.Text = productQuantity.ToString();
                txtProductGrossTotalSalePrice.Text = (Convert.ToDecimal(salePrice) * productQuantity).ToString();
                txtProductDiscount.Text = productDiscount.ToString();
                txtProductVAT.Text = productVAT.ToString();
                txtProductTotalSalePrice.Text = (Convert.ToDecimal(salePrice) * productQuantity-productDiscount+productVAT).ToString();
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

        private void txtProductDiscount_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtProductDiscount.Text == "")
                txtProductDiscount.Text = initialIndex.ToString();
        }
        private void txtProductVAT_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtProductVAT.Text == "")
                txtProductVAT.Text = initialIndex.ToString();
        }

        private void txtProductDiscount_KeyUp(object sender, KeyEventArgs e)
        {
            CalculateProductTotalSalePrice();
        }

        private void txtProductVAT_KeyUp(object sender, KeyEventArgs e)
        {
            CalculateProductTotalSalePrice();
        }

        private void CalculateProductTotalSalePrice()
        {
            decimal number;
            if (decimal.TryParse(txtProductDiscount.Text, out number) && decimal.TryParse(txtProductVAT.Text, out number) && txtProductDiscount.Text != "" && txtProductVAT.Text != "")
                txtProductTotalSalePrice.Text = Convert.ToDecimal(Convert.ToDecimal(txtProductGrossTotalSalePrice.Text) - Convert.ToDecimal(txtProductDiscount.Text) + Convert.ToDecimal(txtProductVAT.Text)).ToString();
            
            else if(txtProductDiscount.Text == "")            
                txtProductTotalSalePrice.Text = Convert.ToDecimal(Convert.ToDecimal(txtProductGrossTotalSalePrice.Text) + Convert.ToDecimal(txtProductVAT.Text)).ToString();
            
            else if(txtProductVAT.Text == "")
                txtProductTotalSalePrice.Text = Convert.ToDecimal(Convert.ToDecimal(txtProductGrossTotalSalePrice.Text) - Convert.ToDecimal(txtProductDiscount.Text)).ToString();
        }
        private void CalculateBasketGrandTotal(int calledByVatOrDiscount)
        {
            if (decimal.TryParse(txtBasketVat.Text, out decimal number) && txtBasketVat.Text != "" && txtBasketDiscount.Text != "")//TRY TO SEPERATE VAT AND DISCOUNT SECTION IN THE FOLLOWING IF STATEMENT!
            {
                txtBasketSubTotal.Text = (Convert.ToDecimal(txtBasketGrossAmount.Text) - Convert.ToDecimal(txtBasketDiscount.Text)).ToString();
                txtBasketGrandTotal.Text = (Convert.ToDecimal(txtBasketGrossAmount.Text) - Convert.ToDecimal(txtBasketDiscount.Text) + Convert.ToDecimal(txtBasketVat.Text)).ToString();
            }
            else
            {
                if (calledByVatOrDiscount == calledByVAT)
                {
                    txtBasketVat.Text = initialIndex.ToString();
                    txtBasketGrandTotal.Text = (Convert.ToDecimal(txtBasketGrossAmount.Text) - Convert.ToDecimal(txtBasketDiscount.Text)).ToString();//Since the VAT is zero, we only need to calculate the Grand Total without VAT.
                }
                else
                {
                    txtBasketDiscount.Text = initialIndex.ToString();
                    txtBasketGrandTotal.Text = (Convert.ToDecimal(txtBasketGrossAmount.Text) + Convert.ToDecimal(txtBasketVat.Text)).ToString();//Since the Discount is zero, we only need to calculate the Grand Total without Discount.
                }
            }
        }

        private void txtBasketVat_KeyUp(object sender, KeyEventArgs e)
        {
            CalculateBasketGrandTotal(calledByVAT);
        }

        private void txtBasketDiscount_KeyUp(object sender, KeyEventArgs e)
        {
            CalculateBasketGrandTotal(calledByDiscount);
        }

        private void dgProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgProducts.SelectedItem != null)
            {
                //GETTING TEXTBOX FROM DATAGRID.
                ContentPresenter cpProductSalePrice = dgProducts.Columns[colNoProductTotalSalePrice].GetCellContent(dgProducts.SelectedItem) as ContentPresenter;
                var tmpProductSalePrice = cpProductSalePrice.ContentTemplate;
                TextBox productTotalSalePrice = tmpProductSalePrice.FindName(dgCellNames[colNoProductTotalSalePrice], cpProductSalePrice) as TextBox;

                //GETTING TEXTBOX FROM DATAGRID.
                ContentPresenter cpProductQuantity = dgProducts.Columns[colNoProductQuantity].GetCellContent(dgProducts.SelectedItem) as ContentPresenter;
                var tmpProductQuantity = cpProductQuantity.ContentTemplate;
                TextBox productQuantity = tmpProductQuantity.FindName(dgCellNames[colNoProductQuantity], cpProductQuantity) as TextBox;

                oldBasketQuantity = Convert.ToDecimal(txtBasketQuantity.Text) - Convert.ToDecimal(productQuantity.Text);
                oldBasketGrossAmount = Convert.ToDecimal(txtBasketGrossAmount.Text) - Convert.ToDecimal(productTotalSalePrice.Text);
                oldBasketSubTotal = Convert.ToDecimal(txtBasketSubTotal.Text) - Convert.ToDecimal(productTotalSalePrice.Text);
                oldBasketGrandTotal = Convert.ToDecimal(txtBasketGrandTotal.Text) - Convert.ToDecimal(productTotalSalePrice.Text);//Grand total is with VAT.
            }
        }

        private void LoadCboMenuAsset(int checkStatus)
        {
            DataTable dtAccount;
            if (checkStatus == account)
                dtAccount = accountDAL.Select();

            else
                dtAccount = bankDAL.Select();

            //Specifying Items Source for product combobox
            cboMenuAsset.ItemsSource = dtAccount.DefaultView;

            //Here DisplayMemberPath helps to display Text in the ComboBox.
            cboMenuAsset.DisplayMemberPath = colTxtName;

            //SelectedValuePath helps to store values like a hidden field.
            cboMenuAsset.SelectedValuePath = colTxtId;
        }

        private void LoadCboMenuPaymentType()
        {
            //Creating Data Table to hold the products from Database
            DataTable dtPayment = paymentDAL.Select();

            //Specifying Items Source for product combobox
            cboMenuPaymentType.ItemsSource = dtPayment.DefaultView;

            //Here DisplayMemberPath helps to display Text in the ComboBox.
            cboMenuPaymentType.DisplayMemberPath = colTxtPaymentType;

            //SelectedValuePath helps to store values like a hidden field.
            cboMenuPaymentType.SelectedValuePath = colTxtId;
        }

        private void LoadCboMenuCustomer()
        {
            //Creating Data Table to hold the products from Database
            DataTable dtCustomer = customerDAL.Select();

            //Specifying Items Source for product combobox
            cboMenuCustomer.ItemsSource = dtCustomer.DefaultView;

            //Here DisplayMemberPath helps to display Text in the ComboBox.
            cboMenuCustomer.DisplayMemberPath = colTxtName;

            //SelectedValuePath helps to store values like a hidden field.
            cboMenuCustomer.SelectedValuePath = colTxtId;
        }

        private void cboMenuCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int sourceId;
            int sourceType = customer;

            sourceId = Convert.ToInt32(cboMenuCustomer.SelectedValue);
            lblAssetCustomerId.Content = assetDAL.GetAssetIdBySource(sourceId, sourceType);
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
            lblAssetId.Content = assetDAL.GetAssetIdBySource(sourceId, sourceType);
        }

        private void rbAccount_Checked(object sender, RoutedEventArgs e)
        {
            LoadCboMenuAsset(account);
        }

        private void rbBank_Checked(object sender, RoutedEventArgs e)
        {
            LoadCboMenuAsset(bank);
        }
    }
}
