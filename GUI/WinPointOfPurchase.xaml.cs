using BLL;
using CUL;
using DAL;
using CUL;
using DAL;
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
using CUL.Enums;

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

        const int colLength = 10;
        int clickedNewOrEdit;
        const int clickedNothing=-1, clickedNew = 0, clickedEdit = 1,clickedNull=2;//0 stands for user clicked the button New, and 1 stands for user clicked the button Edit.
        string[] dgCellNames = new string[colLength] { "dgTxtProductId", "dgTxtProductName", "dgTxtProductUnit", "dgTxtProductQuantity", "dgTxtProductGrossCostPrice", "dgTxtProductGrossTotalCostPrice", "dgTxtProductDiscount", "dgTxtProductVAT", "dgTxtProductCostPrice", "dgTxtProductTotalCostPrice" };
        string[,] oldDgProductCells = new string[,] { };
        string calledBy = "tbl_pop";

        string
            colTxtQtyInUnit = "quantity_in_unit",
            colTxtQtyInStock = "quantity_in_stock",
            colTxtCostPrice = "costprice",
            colTxtPaymentType = "payment_type",
            colTxtPaymentTypeId = "payment_type_id",
            colTxtSupplierId = "supplier_id",
            colTxtInvoiceNo = "invoice_no",
            colTxtId = "id",
            colTxtProductQtyPurchased = "quantity",
            colTxtProductId = "product_id",
            colTxtProductUnitId = "product_unit_id",
            colTxtName = "name",
            colTxtProductGrossCostPrice = "product_gross_cost_price",
            colTxtProductCostPrice = "product_cost_price",
            colTxtProductDiscount = "product_discount",
            colTxtProductVAT = "product_vat",
            colTxtBarcodeRetail = "barcode_retail",
            colTxtBarcodeWholesale = "barcode_wholesale",
            colTxtUnitRetailId = "unit_retail_id",
            colTxtUnitWholesaleId = "unit_wholesale_id",
            colTxtTotalPQuantity = "total_product_quantity",
            colTxtGrossCostTotal = "gross_cost_total",
            colTxtVat = "vat",
            colTxtDiscount = "discount",
            colTxtSubTotal="sub_total",
            colTxtGrandTotal = "grand_total",
            colTxtSourceBalance = "source_balance",
            colTxtIdSourceType= "id_source_type",
            colTxtIdSource="id_source",
            coTxtAssetId="asset_id";

        int calledByVAT = 1, calledByDiscount = 2;
        int clickedArrow,clickedPrev=0,clickedNext=1;
        int oldIdAsset, oldIdAssetSupplier,oldDgItemsRowCount;
        int uneditedIdAsset, uneditedIdAssetSupplier;
        decimal oldBasketQuantity, oldBasketGrossCostTotal, oldBasketDiscount, oldBasketSubTotal, oldBasketVAT, oldBasketGrandTotal;//This variables are used while we are editing the datagrid in an active invoice page.
        decimal uneditedBasketGrandTotal;//This variables are used when we click the edit button.

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
            txtProductQuantity.IsEnabled = false;
            txtProductGrossCostPrice.IsEnabled = false;
            txtProductDiscount.IsEnabled = false;
            txtProductVAT.IsEnabled = false;
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
            txtProductQuantity.IsEnabled = true;
            txtProductGrossCostPrice.IsEnabled = true;
            txtProductDiscount.IsEnabled = true;
            txtProductVAT.IsEnabled = true;
            txtProductTotalCostPrice.IsEnabled = true;
            txtInvoiceNo.IsEnabled = true;
            chkUpdateProductCosts.IsEnabled = true;
            dgProducts.IsHitTestVisible = true;//Enabling the datagrid clicking.
            //cboMenuSupplier.SelectedIndex = -1;//-1 Means nothing is selected.

            if (clickedBtn == clickedNew)
                txtInvoiceNo.Text = ((int)Numbers.InitialIndex).ToString();
        }

        private void ClearProductsDataGrid()
        {
            dgProducts.Items.Clear();
        }

        private void ClearBasketTextBox()
        {
            txtBasketQuantity.Text = ((int)Numbers.InitialIndex).ToString();
            txtBasketGrossCostTotal.Text = ((int)Numbers.InitialIndex).ToString();
            txtBasketDiscount.Text = ((int)Numbers.InitialIndex).ToString();
            txtBasketSubTotal.Text = ((int)Numbers.InitialIndex).ToString();
            txtBasketVat.Text = ((int)Numbers.InitialIndex).ToString();
            txtBasketGrandTotal.Text = ((int)Numbers.InitialIndex).ToString();
        }

        private void ClearProductEntranceTextBox()
        {
            txtProductId.Text = "";
            txtProductName.Text = "";
            cboProductUnit.ItemsSource = null;
            txtProductGrossCostPrice.Text = "";
            txtProductCostPrice.Text = "";
            txtProductQuantity.Text = "";
            txtProductDiscount.Text = "";
            txtProductVAT.Text = "";
            txtProductGrossTotalCostPrice.Text = "";
            txtProductTotalCostPrice.Text = "";
            Keyboard.Focus(txtProductId); // set keyboard focus
            DisableProductEntranceButtons();
        }

        private void LoadNewInvoice()
        {
            ClearBasketTextBox();
            ClearProductsDataGrid();

            int invoiceId, increment = (int)Numbers.UnitValue;

            invoiceId = commonBLL.GetLastRecordById(calledBy);//Getting the last invoice number and assign it to the variable called invoiceNo.
            invoiceId += increment;//We are adding one to the last invoice number because every new invoice number is one greater tham the previous invoice number.
            lblInvoiceId.Content = invoiceId;//Assigning invoiceNo to the content of the InvoiceNo Label.
        }

        //-1 means user did not clicked either previous or next button which means user just clicked the point of purchase button to open it.
        private void LoadPastInvoice(int invoiceId = (int)Numbers.InitialIndex, int invoiceArrow = -(int)Numbers.UnitValue)//Optional parameter
        {
            string productId, productName, productQuantity, productGrossCostPrice, productCostPrice, productGrossTotalCostPrice, productDiscount, productVAT, productTotalCostPrice;
            decimal basketQuantity=(int)Numbers.InitialIndex, 
                    basketGrossTotalCostPrice = (int)Numbers.InitialIndex, 
                    basketDiscount = (int)Numbers.InitialIndex, 
                    basketSubTotal = (int)Numbers.InitialIndex, 
                    basketVAT = (int)Numbers.InitialIndex, 
                    basketGrandTotal = (int)Numbers.InitialIndex;

            if (invoiceId == (int)Numbers.InitialIndex)//If the ID is 0 came from the optional parameter, that means user just clicked the WinPOP button to open it.
            {
                invoiceId = commonBLL.GetLastRecordById(calledBy);//Getting the last invoice id and assign it to the variable called invoiceId.
            }

            /*WE CANNOT USE ELSE IF FOR THE CODE BELOW! BOTH IF STATEMENTS ABOVE AND BELOVE MUST WORK.*/
            if (invoiceId != (int)Numbers.InitialIndex)// If the invoice number is still 0 even when we get the last invoice number by using code above, that means this is the first sale and do not run this code block.
            {
                DataTable dataTablePop = pointOfPurchaseDAL.GetByInvoiceId(invoiceId);

                if (dataTablePop.Rows.Count != (int)Numbers.InitialIndex)
                {
                    DataTable dtPopDetail = pointOfPurchaseDetailDAL.Search(invoiceId);
                    DataTable dtProduct;
                    
                    #region ASSET INFORMATION FILLING REGION
                    int assetId = Convert.ToInt32(dataTablePop.Rows[(int)Numbers.InitialIndex][coTxtAssetId].ToString());//Getting the id of account.
                    lblAssetId.Content = assetId;

                    DataTable dtAsset = assetDAL.SearchById(assetId);
                    int sourceType = Convert.ToInt32(dtAsset.Rows[(int)Numbers.InitialIndex][colTxtIdSourceType]);

                    if (sourceType == Convert.ToInt32(Assets.Account))
                        rbAccount.IsChecked = true;
                    else
                        rbBank.IsChecked = true;

                    cboMenuAsset.SelectedValue = dtAsset.Rows[(int)Numbers.InitialIndex][colTxtIdSource].ToString();
                    #endregion

                    LoadCboMenuPaymentType();
                    LoadCboMenuSupplier();

                    cboMenuPaymentType.SelectedValue = Convert.ToInt32(dataTablePop.Rows[(int)Numbers.InitialIndex][colTxtPaymentTypeId].ToString());//Getting the id of purchase type.
                    cboMenuSupplier.SelectedValue = Convert.ToInt32(dataTablePop.Rows[(int)Numbers.InitialIndex][colTxtSupplierId].ToString());//Getting the id of supplier.
                    txtInvoiceNo.Text = dataTablePop.Rows[(int)Numbers.InitialIndex][colTxtInvoiceNo].ToString();
                    lblInvoiceId.Content= dataTablePop.Rows[(int)Numbers.InitialIndex][colTxtId].ToString();

                    #region LOADING THE PRODUCT DATA GRID
                    int productCurrentUnitId, productRetailUnitId, productWholesaleUnitId;

                    for (int currentRow = (int)Numbers.InitialIndex; currentRow < dtPopDetail.Rows.Count; currentRow++)
                    {
                        productId = dtPopDetail.Rows[currentRow][colTxtProductId].ToString();
                        productCurrentUnitId = Convert.ToInt32(dtPopDetail.Rows[currentRow][colTxtProductUnitId]);
                        productQuantity = dtPopDetail.Rows[currentRow][colTxtProductQtyPurchased].ToString();
                        productGrossCostPrice = dtPopDetail.Rows[currentRow][colTxtProductGrossCostPrice].ToString();
                        productGrossTotalCostPrice = (Convert.ToDecimal(productGrossCostPrice) * Convert.ToDecimal(productQuantity)).ToString("0.00");//We do NOT store the total price in the db to reduce the storage. Instead of it, we multiply the unit price with the quantity to find the total price.
                        productDiscount = dtPopDetail.Rows[currentRow][colTxtProductDiscount].ToString();
                        productVAT = dtPopDetail.Rows[currentRow][colTxtProductVAT].ToString();
                        productCostPrice = Convert.ToDecimal(dtPopDetail.Rows[currentRow][colTxtProductCostPrice]).ToString("0.00");//Two digits are enough because we only store two float numbers for the product cost price in DB.
                        productTotalCostPrice =((Convert.ToDecimal(productGrossCostPrice) * Convert.ToDecimal(productQuantity)) - Convert.ToDecimal(productDiscount) + Convert.ToDecimal(productVAT)).ToString("0.00");//We do NOT store the total cost in the db to reduce the storage. Instead of it, we multiply the unit cost with the quantity to find the total cost.

                        dtProduct = productDAL.SearchById(productId);
                        productName = dtProduct.Rows[(int)Numbers.InitialIndex][colTxtName].ToString();//We used (int)Numbers.InitialIndex because there can be only one row in the datatable for a specific product.
                        productRetailUnitId = Convert.ToInt32(dtProduct.Rows[(int)Numbers.InitialIndex][colTxtUnitRetailId]);
                        productWholesaleUnitId = Convert.ToInt32(dtProduct.Rows[(int)Numbers.InitialIndex][colTxtUnitWholesaleId]);

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
                            Quantity = productQuantity,
                            GrossCostPrice = productGrossCostPrice,
                            GrossTotalCostPrice = productGrossTotalCostPrice,
                            Discount = productDiscount,
                            VAT = productVAT,
                            CostPrice = productCostPrice,
                            TotalCostPrice = productTotalCostPrice,

                            //BINDING DATAGRID COMBOBOX
                            UnitCboItemsSource = dtUnitInfo.DefaultView,
                            UnitCboSValue = productCurrentUnitId,
                            UnitCboSValuePath = colTxtId,
                            UnitCboDMemberPath = colTxtName,
                        });

                        #region FILLING THE PREVIOUS BASKET INFORMATIONS TO VARIABLES
                        basketQuantity = basketQuantity + Convert.ToDecimal(productQuantity);
                        basketGrossTotalCostPrice = basketGrossTotalCostPrice + Convert.ToDecimal(productGrossTotalCostPrice);
                        basketDiscount = basketDiscount + Convert.ToDecimal(productDiscount);
                        basketSubTotal = basketSubTotal + (Convert.ToDecimal(productGrossTotalCostPrice) - Convert.ToDecimal(productDiscount));
                        basketVAT = basketVAT + Convert.ToDecimal(productVAT);
                        basketGrandTotal =
                            basketGrandTotal + 
                            Convert.ToDecimal((Convert.ToDecimal(productGrossCostPrice) * Convert.ToDecimal(productQuantity)) - 
                            Convert.ToDecimal(productDiscount) + Convert.ToDecimal(productVAT));
                        #endregion
                    }
                    #endregion

                    #region FILLING THE PREVIOUS BASKET INFORMATIONS

                    //We used(int)Numbers.InitialIndex below as a row name because there can be only one row in the datatable for a specific Invoice.
                    txtBasketQuantity.Text = basketQuantity.ToString();
                    txtBasketGrossCostTotal.Text = basketGrossTotalCostPrice.ToString("0.00");
                    txtBasketDiscount.Text = basketDiscount.ToString();
                    txtBasketSubTotal.Text = basketSubTotal.ToString("0.00");
                    txtBasketVat.Text = basketVAT.ToString();
                    txtBasketGrandTotal.Text = basketGrandTotal.ToString("0.00");

                    #endregion
                }
                else if (dataTablePop.Rows.Count == (int)Numbers.InitialIndex)//If the pop detail row quantity is 0, that means there is no such row so decrease or increase the invoice number according to user preference.
                {
                    if (invoiceArrow == 0)//If the invoice arrow is 0, that means user clicked the previous button.
                    {
                        invoiceId = invoiceId - (int)Numbers.UnitValue;
                    }
                    else
                    {
                        invoiceId = invoiceId + (int)Numbers.UnitValue;
                    }

                    if (invoiceArrow != -(int)Numbers.UnitValue)//If the user has not clicked either previous or next button, then the invoiceArrow will be -1 and no need for recursion.
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

            DataGridRow dgRow;
            ContentPresenter cpProduct;
            TextBox tbCellContent;
            ComboBox tbCellContentCbo;

            for (int rowNo = (int)Numbers.InitialIndex; rowNo < rowLength; rowNo++)
            {
                dgRow = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(rowNo);

                for (int colNo = (int)Numbers.InitialIndex; colNo < colLength; colNo++)
                {
                    cpProduct = dgProducts.Columns[colNo].GetCellContent(dgRow) as ContentPresenter;
                    var tmpProduct = cpProduct.ContentTemplate;

                    if (colNo != (int)PopColumns.ColProductUnit)
                    {
                        tbCellContent = tmpProduct.FindName(dgCellNames[colNo], cpProduct) as TextBox;
                        dgProductCells[rowNo, colNo] = tbCellContent.Text;
                    }
                    else
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
            int emptyCboIndex = -(int)Numbers.UnitValue;
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
            if (int.TryParse(txtInvoiceNo.Text, out int number) && /*isDgEqual == false && oldIdAsset != Convert.ToInt32(lblAssetId.Content) && oldIdAssetSupplier != Convert.ToInt32(lblAssetSupplierId.Content) &&*/ cboMenuPaymentType.SelectedIndex != emptyCboIndex && cboMenuSupplier.SelectedIndex != emptyCboIndex && cboMenuAsset.SelectedIndex != emptyCboIndex && dgProducts.Items.Count != (int)Numbers.InitialIndex)
            {
                int invoiceNo = (int)Numbers.InitialIndex;//Defaulty, we are assigning 0 to the variable called invoiceNo in case the user would not enter any number.
                if (txtInvoiceNo.Text != "")
                    invoiceNo = Convert.ToInt32(txtInvoiceNo.Text);

                int invoiceId = Convert.ToInt32(lblInvoiceId.Content); /*lblInvoiceId stands for the invoice id in the database.*/
                int userId = userBLL.GetUserId(WinLogin.loggedInUserName);
                bool isSuccess = false, isSuccessDetail = false,isSuccessAsset=false;
                int productId;
                int unitId;
                decimal productOldQtyInStock, newQuantity,newCostPrice;
                int addedBy = userId;
                string[] cells = new string[colLength];
                DateTime dateTime = DateTime.Now;
                int productRate = (int)Numbers.InitialIndex;//Modify this code dynamically!!!!!!!!!

                DataTable dataTableLastInvoice = pointOfPurchaseBLL.GetLastInvoiceRecord();//Getting the last invoice.
                DataTable dataTableProduct = new DataTable();
                DataTable dataTableUnit = new DataTable();
                DataTable dtAsset= new DataTable();
                decimal oldSourceBalance;

                if (clickedNewOrEdit == clickedEdit)
                {
                    #region SUPPLIER BALANCE REVERTING SECTION
                    dtAsset = assetDAL.SearchById(uneditedIdAssetSupplier);
                    oldSourceBalance = Convert.ToDecimal(dtAsset.Rows[(int)Numbers.InitialIndex][colTxtSourceBalance]);

                    assetCUL.SourceBalance = oldSourceBalance + uneditedBasketGrandTotal;//We have to add the unedited grandTotal to the supplier's balance in order to revert our dept .
                    assetCUL.Id = Convert.ToInt32(uneditedIdAssetSupplier);
                    isSuccessAsset = assetDAL.Update(assetCUL);
                    #endregion
                }

                #region SUPPLIER BALANCE UPDATING SECTION
                dtAsset = assetDAL.SearchById(Convert.ToInt32(lblAssetSupplierId.Content));
                oldSourceBalance = Convert.ToDecimal(dtAsset.Rows[(int)Numbers.InitialIndex][colTxtSourceBalance]);
                
                assetCUL.SourceBalance = oldSourceBalance - Convert.ToDecimal(txtBasketGrandTotal.Text);//We owe the supplier X Quantity for getting this purchase. So we subtract it from the balance to pay it later in the WinExpense.
                assetCUL.Id = Convert.ToInt32(lblAssetSupplierId.Content);
                
                isSuccessAsset = assetDAL.Update(assetCUL);
                #endregion

                #region TABLE POP SAVING SECTION
                //Getting the values from the POP Window and fill them into the pointOfPurchaseCUL.
                pointOfPurchaseCUL.Id = invoiceId;//The column invoice id in the database is not auto incremental. This is for preventing the number increasing when the user deletes an existing invoice and creates a new invoice.
                pointOfPurchaseCUL.InvoiceNo = invoiceNo;
                pointOfPurchaseCUL.PaymentTypeId = Convert.ToInt32(cboMenuPaymentType.SelectedValue);//Selected value contains the id of the item so that no need to get it from DB.
                pointOfPurchaseCUL.SupplierId = Convert.ToInt32(cboMenuSupplier.SelectedValue);
                //pointOfPurchaseCUL.TotalProductQuantity = Convert.ToDecimal(txtBasketQuantity.Text);
                //pointOfPurchaseCUL.GrossCostTotal = Convert.ToDecimal(txtBasketGrossCostTotal.Text);
                //pointOfPurchaseCUL.Discount = Convert.ToDecimal(txtBasketDiscount.Text);
                //pointOfPurchaseCUL.SubTotal = Convert.ToDecimal(txtBasketSubTotal.Text);
                //pointOfPurchaseCUL.Vat = Convert.ToDecimal(txtBasketVat.Text);
                //pointOfPurchaseCUL.GrandTotal = Convert.ToDecimal(txtBasketGrandTotal.Text);
                pointOfPurchaseCUL.AssetId = Convert.ToInt32(lblAssetId.Content);
                pointOfPurchaseCUL.AddedDate = DateTime.Now;
                pointOfPurchaseCUL.AddedBy = userId;

                if (clickedNewOrEdit ==clickedEdit)//If the user clicked the btnEdit, then update the specific invoice information in tbl_pop at once.
                {
                    isSuccess = pointOfPurchaseBLL.UpdatePOP(pointOfPurchaseCUL);
                }

                else
                {
                    //Creating a Boolean variable to insert data into the database.
                    isSuccess = pointOfPurchaseBLL.InsertPOP(pointOfPurchaseCUL);
                }
                #endregion

                #region TABLE POP DETAILS SAVING SECTION

                DataGridRow dgRow;
                ContentPresenter cpProduct;
                TextBox tbCellContent;
                ComboBox tbCellContentCbo;

                for (int rowNo = (int)Numbers.InitialIndex; rowNo < dgProducts.Items.Count; rowNo++)
                {
                    if (clickedNewOrEdit ==clickedEdit)//If the user clicked the btnEdit, then delete the specific invoice's products in tbl_pos_detailed at once.
                    {
                        productBLL.RevertOldQuantityInStock(oldDgProductCells, oldDgItemsRowCount, (int)PopColumns.ColProductQuantity, calledBy);//Reverting the old products' quantity in stock.

                        //We are sending pointOfPurchaseDetailCUL as a parameter to the Delete method just to use the Id property in the SQL Query. So that we can erase all the products which have the specific id.
                        pointOfPurchaseDetailDAL.Delete(invoiceId);

                        //2 means null for this code. We used this in order to prevent running the if block again and again. Because, we erase all of the products belong to one invoice number at once.
                        clickedNewOrEdit = clickedNull;
                    }

                    dgRow = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(rowNo);

                    for (int colNo = (int)Numbers.InitialIndex; colNo < colLength; colNo++)
                    {
                        cpProduct = dgProducts.Columns[colNo].GetCellContent(dgRow) as ContentPresenter;
                        var tmpProduct = cpProduct.ContentTemplate;

                        if (colNo != (int)PopColumns.ColProductUnit)
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

                    dataTableProduct = productDAL.SearchProductByIdBarcode(cells[(int)Numbers.InitialIndex]);//Cell[0] may contain the product id or barcode_retail or barcode_wholesale.
                    productId = Convert.ToInt32(dataTableProduct.Rows[(int)Numbers.InitialIndex][colTxtId]);//Row index is always zero for this situation because there can be only one row of a product which has a unique barcode on the table.


                    dataTableUnit = unitDAL.GetUnitInfoById(Convert.ToInt32(cells[(int)PopColumns.ColProductUnit]));//Cell[2] contains the unit id in the combobox.
                    unitId = Convert.ToInt32(dataTableUnit.Rows[(int)Numbers.InitialIndex][colTxtId]);//Row index is always zero for this situation because there can be only one row of a specific unit.

                    pointOfPurchaseDetailCUL.Id = invoiceId;//No incremental value in the database because there can be multiple goods with the same invoice id.
                    pointOfPurchaseDetailCUL.ProductId = productId;
                    pointOfPurchaseDetailCUL.AddedBy = addedBy;
                    pointOfPurchaseDetailCUL.ProductRate = productRate;
                    pointOfPurchaseDetailCUL.ProductUnitId = unitId;
                    pointOfPurchaseDetailCUL.ProductGrossCostPrice = Convert.ToDecimal(cells[(int)PopColumns.ColProductGrossCostPrice]);//cells[4] contains gross cost price of the product in the list. We have to store the current cost price as well because it may be changed in the future.
                    pointOfPurchaseDetailCUL.ProductQuantity = Convert.ToDecimal(cells[(int)PopColumns.ColProductQuantity]);
                    pointOfPurchaseDetailCUL.ProductDiscount = Convert.ToDecimal(cells[(int)PopColumns.ColProductDiscount]);
                    pointOfPurchaseDetailCUL.ProductVAT = Convert.ToDecimal(cells[(int)PopColumns.ColProductVAT]);
                    pointOfPurchaseDetailCUL.ProductCostPrice = Convert.ToDecimal(cells[(int)PopColumns.ColProductCostPrice]);//cells[8] contains cost price of the product in the list. We have to store the current cost price as well because it may be changed in the future.

                    isSuccessDetail = pointOfPurchaseDetailDAL.Insert(pointOfPurchaseDetailCUL);

                    #region PRODUCT QUANTITY AND COST UPDATE
                    productOldQtyInStock = Convert.ToDecimal(dataTableProduct.Rows[(int)Numbers.InitialIndex][colTxtQtyInStock].ToString());//Getting the old product quantity in stock.

                    newQuantity= productOldQtyInStock + Convert.ToDecimal(cells[(int)PopColumns.ColProductQuantity]);

                    productDAL.UpdateSpecificColumn(productId, colTxtQtyInStock, newQuantity.ToString());

                    if (chkUpdateProductCosts.IsChecked == true)
                    {
                        newCostPrice = Convert.ToDecimal(cells[(int)PopColumns.ColProductCostPrice]);
                        productDAL.UpdateSpecificColumn(productId, colTxtCostPrice, newCostPrice.ToString());
                    }
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
            if (txtProductQuantity.Text!="" &&txtProductCostPrice.Text!="" && cboProductUnit.ItemsSource!=null)
            {
                decimal productQuantity;
                int productRetailUnitId;
                string productIdFromUser = txtProductId.Text;
                DataTable dtProduct = productDAL.SearchProductByIdBarcode(productIdFromUser);
                int productId = Convert.ToInt32(dtProduct.Rows[(int)Numbers.InitialIndex][colTxtId]); //We need to get the Id of the product from the db even if the user enters an id because user may also enter a barcode.

                productRetailUnitId = Convert.ToInt32(dtProduct.Rows[(int)Numbers.InitialIndex][colTxtUnitRetailId]);
                //productWholesaleUnitId = Convert.ToInt32(dtProduct.Rows[(int)Numbers.InitialIndex][colTxtUnitWholesaleId]);

                if (Convert.ToInt32(cboProductUnit.SelectedValue) == productRetailUnitId)
                {
                    productQuantity = (int)Numbers.UnitValue;//If it is a unit retail id, the assign one asa default value.
                }

                else
                {
                    productQuantity = Convert.ToDecimal(dtProduct.Rows[(int)Numbers.InitialIndex][colTxtQtyInUnit]);
                }

                txtProductQuantity.Text = productQuantity.ToString();
                txtProductGrossTotalCostPrice.Text = (productQuantity * Convert.ToDecimal(txtProductGrossCostPrice.Text)).ToString();
                txtProductTotalCostPrice.Text = (
                    (productQuantity * Convert.ToDecimal(txtProductGrossCostPrice.Text)) -
                    Convert.ToDecimal(txtProductDiscount.Text) +
                    Convert.ToDecimal(txtProductVAT.Text)
                    ).ToString();
            }
        }

        private void btnProductAdd_Click(object sender, RoutedEventArgs e)//Try to do this by using listview
        {
            bool addNewProductLine = true;
            int productQuantity;
            int rowQuantity = dgProducts.Items.Count;
            DataTable dtProduct = productDAL.SearchProductByIdBarcode(txtProductId.Text);
            int productId = Convert.ToInt32(dtProduct.Rows[(int)Numbers.InitialIndex][colTxtId]); //We need to get the Id of the product from the db even if the user enters an id because user may also enter a barcode.

            for (int i = 0; i < rowQuantity; i++)
            {
                DataGridRow row = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(i);

                ContentPresenter cpProduct = dgProducts.Columns[(int)Numbers.InitialIndex].GetCellContent(row) as ContentPresenter;
                var tmpProduct = cpProduct.ContentTemplate;
                TextBox tbBarcodeCellContent = tmpProduct.FindName(dgCellNames[(int)Numbers.InitialIndex], cpProduct) as TextBox;

                if (tbBarcodeCellContent.Text == productId.ToString())
                {
                    if (MessageBox.Show("There is already the same item in the list. Would you like to sum them?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        //GETTING THE CELL CONTENT OF THE PRODUCT QUANTITY
                        ContentPresenter cpProductQty = dgProducts.Columns[(int)PopColumns.ColProductQuantity].GetCellContent(row) as ContentPresenter;
                        var tmpProductQty = cpProductQty.ContentTemplate;
                        TextBox txtDgProductQty = tmpProductQty.FindName(dgCellNames[(int)PopColumns.ColProductQuantity], cpProductQty) as TextBox;

                        //GETTING THE CELL CONTENT OF THE PRODUCT GROSS TOTAL COST PRICE
                        ContentPresenter cpProductGrossTotalCostPrice = dgProducts.Columns[(int)PopColumns.ColProductGrossTotalCostPrice].GetCellContent(row) as ContentPresenter;
                        var tmpProductGrossTotalCostPrice = cpProductGrossTotalCostPrice.ContentTemplate;
                        TextBox txtDgProductGrossTotalCostPrice = tmpProductGrossTotalCostPrice.FindName(dgCellNames[(int)PopColumns.ColProductGrossTotalCostPrice], cpProductGrossTotalCostPrice) as TextBox;

                        //GETTING THE CELL CONTENT OF THE PRODUCT TOTAL COST PRICE
                        ContentPresenter cpProductTotalCostPrice = dgProducts.Columns[(int)PopColumns.ColProductTotalCostPrice].GetCellContent(row) as ContentPresenter;
                        var tmpProductTotalCostPrice = cpProductTotalCostPrice.ContentTemplate;
                        TextBox txtDgProductTotalCostPrice = tmpProductTotalCostPrice.FindName(dgCellNames[(int)PopColumns.ColProductTotalCostPrice], cpProductTotalCostPrice) as TextBox;

                        //CALCULATING NEW PRODUCT QUANTITY IN DATAGRID
                        productQuantity = Convert.ToInt32(txtDgProductQty.Text);
                        productQuantity += Convert.ToInt32(txtProductQuantity.Text);//We are adding the quantity entered in the "txtProductQuantity" to the previous quantity cell's quantity.

                        //ASSIGNING NEW VALUES TO THE RELATED DATA GRID CELLS.
                        txtDgProductQty.Text = productQuantity.ToString();
                        txtDgProductGrossTotalCostPrice.Text = (productQuantity * Convert.ToDecimal(txtProductGrossCostPrice.Text)).ToString("0.00");
                        txtDgProductTotalCostPrice.Text = (
                            (productQuantity * Convert.ToDecimal(txtProductGrossCostPrice.Text)) -
                            Convert.ToDecimal(txtProductDiscount.Text) +
                            Convert.ToDecimal(txtProductVAT.Text)
                            ).ToString("0.00");

                        addNewProductLine = false;
                        break;//We have to break the loop if the user clicked "yes" because no need to scan the rest of the rows after confirming.
                    }
                }
            }


            if (addNewProductLine == true)//Use ENUMS instead of this!!!!!!!
            {
                dgProducts.Items.Add(new
                {
                    Id = productId,
                    Name = txtProductName.Text,
                    GrossCostPrice = txtProductGrossCostPrice.Text,
                    Quantity = txtProductQuantity.Text,
                    GrossTotalCostPrice = txtProductGrossTotalCostPrice.Text,
                    Discount = txtProductDiscount.Text,
                    VAT = txtProductVAT.Text,
                    CostPrice = txtProductCostPrice.Text,
                    TotalCostPrice = txtProductTotalCostPrice.Text,

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

            txtBasketGrossCostTotal.Text = (Convert.ToDecimal(txtBasketGrossCostTotal.Text) + Convert.ToDecimal(txtProductGrossTotalCostPrice.Text)).ToString();

            txtBasketDiscount.Text = (Convert.ToDecimal(txtBasketDiscount.Text) + Convert.ToDecimal(txtProductDiscount.Text)).ToString();

            txtBasketSubTotal.Text = (Convert.ToDecimal(txtBasketSubTotal.Text) + ((Convert.ToDecimal(txtProductGrossCostPrice.Text) * quantityFromTextEntry) - Convert.ToDecimal(txtProductDiscount.Text))).ToString();//The previous sub total has already basket discount in it so we only need to subtract the new product's discount.

            txtBasketVat.Text = (Convert.ToDecimal(txtBasketVat.Text) + Convert.ToDecimal(txtProductVAT.Text)).ToString();
            
            txtBasketGrandTotal.Text = (Convert.ToDecimal(txtBasketGrossCostTotal.Text) + Convert.ToDecimal(txtBasketVat.Text) - Convert.ToDecimal(txtBasketDiscount.Text)).ToString();
        }

        private void SubstractBasket(int selectedRowIndex)
        {
            DataGridRow dataGridRow;

            dataGridRow = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(selectedRowIndex);

            //GETTING THE CELL CONTENT OF THE PRODUCT QUANTITY
            ContentPresenter cpProductQuantity = dgProducts.Columns[(int)PopColumns.ColProductQuantity].GetCellContent(dataGridRow) as ContentPresenter;
            var tmpProductQuantity = cpProductQuantity.ContentTemplate;
            TextBox txtDgProductQuantity = tmpProductQuantity.FindName(dgCellNames[(int)PopColumns.ColProductQuantity], cpProductQuantity) as TextBox;

            //GETTING THE CELL CONTENT OF THE PRODUCT GROSS TOTAL COST PRICE
            ContentPresenter cpProductGrossTotalCostPrice = dgProducts.Columns[(int)PopColumns.ColProductGrossTotalCostPrice].GetCellContent(dataGridRow) as ContentPresenter;
            var tmpProductGrossTotalCostPrice = cpProductGrossTotalCostPrice.ContentTemplate;
            TextBox txtDgProductGrossTotalCostPrice = tmpProductGrossTotalCostPrice.FindName(dgCellNames[(int)PopColumns.ColProductGrossTotalCostPrice], cpProductGrossTotalCostPrice) as TextBox;

            //GETTING THE CELL CONTENT OF THE PRODUCT DISCOUNT
            ContentPresenter cpProductDiscount = dgProducts.Columns[(int)PopColumns.ColProductDiscount].GetCellContent(dataGridRow) as ContentPresenter;
            var tmpProductDiscount = cpProductDiscount.ContentTemplate;
            TextBox txtDgProductDiscount = tmpProductDiscount.FindName(dgCellNames[(int)PopColumns.ColProductDiscount], cpProductDiscount) as TextBox;

            //GETTING THE CELL CONTENT OF THE PRODUCT VAT
            ContentPresenter cpProductVAT = dgProducts.Columns[(int)PopColumns.ColProductVAT].GetCellContent(dataGridRow) as ContentPresenter;
            var tmpProductVAT = cpProductVAT.ContentTemplate;
            TextBox txtDgProductVAT = tmpProductVAT.FindName(dgCellNames[(int)PopColumns.ColProductVAT], cpProductVAT) as TextBox;

            //GETTING THE CELL CONTENT OF THE PRODUCT TOTAL COST PRICE
            ContentPresenter cpProductTotalCostPrice = dgProducts.Columns[(int)PopColumns.ColProductTotalCostPrice].GetCellContent(dataGridRow) as ContentPresenter;
            var tmpProductTotalCostPrice = cpProductTotalCostPrice.ContentTemplate;
            TextBox txtDgProductTotalCostPrice = tmpProductTotalCostPrice.FindName(dgCellNames[(int)PopColumns.ColProductTotalCostPrice], cpProductTotalCostPrice) as TextBox;


            //ASSIGNING NEW VALUES TO THE BASKET'S RELATED TEXT BOXES.
            txtBasketQuantity.Text = (Convert.ToDecimal(txtBasketQuantity.Text) - Convert.ToDecimal(txtDgProductQuantity.Text)).ToString();

            txtBasketGrossCostTotal.Text = (Convert.ToDecimal(txtBasketGrossCostTotal.Text) - Convert.ToDecimal(txtDgProductGrossTotalCostPrice.Text)).ToString();

            txtBasketDiscount.Text = (Convert.ToDecimal(txtBasketDiscount.Text) - Convert.ToDecimal(txtDgProductDiscount.Text)).ToString();

            txtBasketSubTotal.Text = (Convert.ToDecimal(txtBasketSubTotal.Text) - (Convert.ToDecimal(txtDgProductGrossTotalCostPrice.Text) - Convert.ToDecimal(txtDgProductDiscount.Text))).ToString();

            txtBasketVat.Text = (Convert.ToDecimal(txtBasketVat.Text) - Convert.ToDecimal(txtDgProductVAT.Text)).ToString();

            txtBasketGrandTotal.Text = (Convert.ToDecimal(txtBasketGrandTotal.Text) - Convert.ToDecimal(txtDgProductTotalCostPrice.Text)).ToString();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            clickedNewOrEdit = clickedNew;//0 stands for the user has entered the btnNew.
            LoadNewInvoice();
            ModifyToolsOnClickBtnNewOrEdit(clickedNewOrEdit);
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
            uneditedIdAssetSupplier = Convert.ToInt32(lblAssetSupplierId.Content);
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

                    pointOfPurchaseDetailDAL.Delete(invoiceId);
                    pointOfPurchaseDAL.Delete(invoiceId);

                    #endregion

                    #region REVERT THE STOCK
                    oldDgProductCells = (string[,])GetDataGridContent().Clone();//Cloning one array into another array.
                    productBLL.RevertOldQuantityInStock(oldDgProductCells, dgProducts.Items.Count, (int)PopColumns.ColProductQuantity, calledBy);
                    #endregion

                    #region REVERT THE ASSET
                    int assetSupplierId = Convert.ToInt32(lblAssetSupplierId.Content);
                    decimal oldSupplierBalance;

                    //CODE DUPLICATION!!!! SIMILAR EXISTS IN SAVE SECTION

                    DataTable dtAssetSupplier = assetDAL.SearchById(assetSupplierId);
                    oldSupplierBalance = Convert.ToDecimal(dtAssetSupplier.Rows[(int)Numbers.InitialIndex]["source_balance"]);

                    //We need to revert the supplier balance so it means we are adding the price to the dept balance of the supplier. Ex: if the supplier balance is -500(means we owe it 500) then -500+200=-300.
                    //You also need to delete the expenseInvoice in WinExpense in order to give the price back to the company's asset. 
                    assetCUL.SourceBalance = oldSupplierBalance + Convert.ToDecimal(txtBasketGrandTotal.Text);
                    assetCUL.Id = Convert.ToInt32(lblAssetSupplierId.Content);

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
            int firstInvoiceId = (int)Numbers.UnitValue, currentInvoiceId = Convert.ToInt32(lblInvoiceId.Content);

            if (currentInvoiceId != firstInvoiceId)
            {
                ClearProductsDataGrid();
                int prevInvoiceId = currentInvoiceId - (int)Numbers.UnitValue;
                clickedArrow = clickedPrev;//0 means customer has clicked the previous button.
                LoadPastInvoice(prevInvoiceId, clickedArrow);
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            int lastInvoiceId = commonBLL.GetLastRecordById(calledBy), currentInvoiceId;

            currentInvoiceId = Convert.ToInt32(lblInvoiceId.Content);

            if (currentInvoiceId != lastInvoiceId)
            {
                ClearProductsDataGrid();
                int nextInvoice = currentInvoiceId + (int)Numbers.UnitValue;
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
        private void DgTextChanged()
        {
            ////GETTING TEXTBOX FROM DATAGRID.
            ContentPresenter cpProductCostPrice = dgProducts.Columns[(int)PopColumns.ColProductCostPrice].GetCellContent(dgProducts.SelectedItem) as ContentPresenter;
            var tmpProductCostPrice = cpProductCostPrice.ContentTemplate;
            TextBox txtDgProductCostPrice = tmpProductCostPrice.FindName(dgCellNames[(int)PopColumns.ColProductCostPrice], cpProductCostPrice) as TextBox;

            ////GETTING TEXTBOX FROM DATAGRID.
            ContentPresenter cpProductGrossCostPrice = dgProducts.Columns[(int)PopColumns.ColProductGrossCostPrice].GetCellContent(dgProducts.SelectedItem) as ContentPresenter;
            var tmpProductGrossCostPrice = cpProductGrossCostPrice.ContentTemplate;
            TextBox txtDgProductGrossCostPrice = tmpProductGrossCostPrice.FindName(dgCellNames[(int)PopColumns.ColProductGrossCostPrice], cpProductGrossCostPrice) as TextBox;

            ////GETTING TEXTBOX FROM DATAGRID.
            ContentPresenter cpProductQuantity = dgProducts.Columns[(int)PopColumns.ColProductQuantity].GetCellContent(dgProducts.SelectedItem) as ContentPresenter;
            var tmpProductQuantity = cpProductQuantity.ContentTemplate;
            TextBox txtDgProductQuantity = tmpProductQuantity.FindName(dgCellNames[(int)PopColumns.ColProductQuantity], cpProductQuantity) as TextBox;

            //GETTING THE CELL CONTENT OF THE PRODUCT GROSS TOTAL SALE PRICE
            ContentPresenter cpProductGrossTotalCostPrice = dgProducts.Columns[(int)PopColumns.ColProductGrossTotalCostPrice].GetCellContent(dgProducts.SelectedItem) as ContentPresenter;
            var tmpProductGrossTotalCostPrice = cpProductGrossTotalCostPrice.ContentTemplate;
            TextBox txtDgProductGrossTotalCostPrice = tmpProductGrossTotalCostPrice.FindName(dgCellNames[(int)PopColumns.ColProductGrossTotalCostPrice], cpProductGrossTotalCostPrice) as TextBox;

            //GETTING TEXTBOX FROM DATAGRID.
            ContentPresenter cpProductDiscount = dgProducts.Columns[(int)PopColumns.ColProductDiscount].GetCellContent(dgProducts.SelectedItem) as ContentPresenter;
            var tmpProductDiscount = cpProductDiscount.ContentTemplate;
            TextBox txtDgProductDiscount = tmpProductDiscount.FindName(dgCellNames[(int)PopColumns.ColProductDiscount], cpProductDiscount) as TextBox;

            //GETTING TEXTBOX FROM DATAGRID.
            ContentPresenter cpProductVAT = dgProducts.Columns[(int)PopColumns.ColProductVAT].GetCellContent(dgProducts.SelectedItem) as ContentPresenter;
            var tmpProductVAT = cpProductVAT.ContentTemplate;
            TextBox txtDgProductVAT = tmpProductVAT.FindName(dgCellNames[(int)PopColumns.ColProductVAT], cpProductVAT) as TextBox;

            //GETTING TEXTBOX FROM DATAGRID
            ContentPresenter cpProductTotalCostPrice = dgProducts.Columns[(int)PopColumns.ColProductTotalCostPrice].GetCellContent(dgProducts.SelectedItem) as ContentPresenter;
            var tmpProductTotalCostPrice = cpProductTotalCostPrice.ContentTemplate;
            TextBox txtDgProductTotalCostPrice = tmpProductTotalCostPrice.FindName(dgCellNames[(int)PopColumns.ColProductTotalCostPrice], cpProductTotalCostPrice) as TextBox;

            if (txtDgProductQuantity.Text != "" && txtDgProductGrossCostPrice.Text != "" && txtDgProductDiscount.Text != "" && txtDgProductVAT.Text != "")
            {
                txtDgProductCostPrice.Text = ((
                    (Convert.ToDecimal(txtDgProductGrossCostPrice.Text) * Convert.ToDecimal(txtDgProductQuantity.Text)) - 
                    Convert.ToDecimal(txtDgProductDiscount.Text) + 
                    Convert.ToDecimal(txtDgProductVAT.Text)) / Convert.ToDecimal(txtDgProductQuantity.Text)).ToString("0.00");

                txtDgProductQuantity.Text = txtDgProductQuantity.Text.ToString();//We need to reassign it otherwise it will not be affected.
                txtDgProductGrossTotalCostPrice.Text = (Convert.ToDecimal(txtDgProductGrossCostPrice.Text) * Convert.ToDecimal(txtDgProductQuantity.Text)).ToString("0.00");
                txtDgProductTotalCostPrice.Text = ((Convert.ToDecimal(txtDgProductGrossCostPrice.Text) * Convert.ToDecimal(txtDgProductQuantity.Text)) - Convert.ToDecimal(txtDgProductDiscount.Text) + Convert.ToDecimal(txtDgProductVAT.Text)).ToString("0.00");

                txtBasketQuantity.Text = (oldBasketQuantity + Convert.ToDecimal(txtDgProductQuantity.Text)).ToString();
                txtBasketGrossCostTotal.Text = (oldBasketGrossCostTotal + Convert.ToDecimal(txtDgProductGrossTotalCostPrice.Text)).ToString();
                txtBasketSubTotal.Text = (oldBasketSubTotal + Convert.ToDecimal(txtDgProductGrossTotalCostPrice.Text) - Convert.ToDecimal(txtDgProductDiscount.Text)).ToString();
                txtBasketDiscount.Text = (oldBasketDiscount + Convert.ToDecimal(txtDgProductDiscount.Text)).ToString();
                txtBasketVat.Text = (oldBasketVAT + Convert.ToDecimal(txtDgProductVAT.Text)).ToString();
                txtBasketGrandTotal.Text = (oldBasketGrandTotal + Convert.ToDecimal(txtDgProductTotalCostPrice.Text)).ToString();//VAT and Discount are already in the old grand total so no need to calculate them.
            }
        }

        private void dgTxtProductGrossCostPrice_KeyUp(object sender, KeyEventArgs e)
        {
            DgTextChanged();
        }

        private void dgTxtProductQuantity_KeyUp(object sender, KeyEventArgs e)
        {
            DgTextChanged();
        }

        private void dgTxtProductDiscount_KeyUp(object sender, KeyEventArgs e)
        {
            DgTextChanged();
        }

        private void dgTxtProductVAT_KeyUp(object sender, KeyEventArgs e)
        {
            DgTextChanged();
        }

        private void txtProductId_KeyUp(object sender, KeyEventArgs e)
        {
            string productIdFromUser = txtProductId.Text;
            long number;

            DataTable dtProduct = productDAL.SearchProductByIdBarcode(productIdFromUser);

            if (e.Key == Key.Enter && productIdFromUser != "")
            {
                if (btnProductAdd.IsEnabled == true && productIdFromUser != "")
                {
                    btnProductAdd_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("You cannot enter because the Id/Barcode is wrong!");
                    ClearProductEntranceTextBox();
                    //Keyboard.ClearFocus();//It is necessary to clear focus from the txtId because this method works recursively once we click the enter button after getting the error message.
                    //txtProductId.Focus();
                    //txtProductId.SelectAll();//Selecting all of the text to correct it easily at once.
                }
            }

            else if (productIdFromUser != ((int)Numbers.InitialIndex).ToString() && long.TryParse(productIdFromUser, out number) && dtProduct.Rows.Count != (int)Numbers.InitialIndex)//Validating the barcode if it is a number(except zero) or not.
            {
                decimal productQuantity, productDiscount = (int)Numbers.InitialIndex, productVAT = (int)Numbers.InitialIndex;
                int productId;
                int productCurrentUnitId, productRetailUnitId, productWholesaleUnitId;
                string productBarcodeRetail/*, productBarcodeWholesale*/;
                string grossCostPrice, costPrice;

                btnProductAdd.IsEnabled = true; //Enabling the add button if any valid barcode is entered.
                btnProductClear.IsEnabled = true;//Enabling the clear button if any valid barcode is entered.

                productId = Convert.ToInt32(dtProduct.Rows[(int)Numbers.InitialIndex][colTxtId]);
                productBarcodeRetail = dtProduct.Rows[(int)Numbers.InitialIndex][colTxtBarcodeRetail].ToString();
                //productBarcodeWholesale = dtProduct.Rows[(int)Numbers.InitialIndex][colTxtBarcodeWholesale].ToString();
                txtProductName.Text = dtProduct.Rows[(int)Numbers.InitialIndex][colTxtName].ToString();//Filling the product name textbox from the database
                productRetailUnitId = Convert.ToInt32(dtProduct.Rows[(int)Numbers.InitialIndex][colTxtUnitRetailId]);
                productWholesaleUnitId= Convert.ToInt32(dtProduct.Rows[(int)Numbers.InitialIndex][colTxtUnitWholesaleId]);

                if (productBarcodeRetail == productIdFromUser || productId.ToString() == productIdFromUser)//If the barcode equals the product's barcode_retail or id, then take the product's retail unit id.
                {
                    productCurrentUnitId = productRetailUnitId;
                    productQuantity = (int)Numbers.UnitValue;//If it is a unit retail id, the assign one as a default value.
                }

                else //If the barcode equals to the barcode_wholesale, then take the product's wholesale unit id.
                {
                    productCurrentUnitId = productWholesaleUnitId;
                    productQuantity = Convert.ToDecimal(dtProduct.Rows[(int)Numbers.InitialIndex][colTxtQtyInUnit]);
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

                //Product's cost price in the db(which already has discount and vat from the previous purchases)is the gross cost price for the current purchase(which doesn't have the new discount and vat yet.)
                grossCostPrice = dtProduct.Rows[(int)Numbers.InitialIndex][colTxtCostPrice].ToString();

                txtProductGrossCostPrice.Text = grossCostPrice;
                txtProductCostPrice.Text = grossCostPrice;//Defaultly, cost price is gross cost price before we enter new discount and vat.
                txtProductQuantity.Text = productQuantity.ToString();
                txtProductGrossTotalCostPrice.Text = (Convert.ToDecimal(grossCostPrice) * productQuantity).ToString();
                txtProductDiscount.Text = productDiscount.ToString();
                txtProductVAT.Text = productVAT.ToString();
                txtProductTotalCostPrice.Text = (Convert.ToDecimal(grossCostPrice) * productQuantity).ToString();//Defaultly, total cost price is gross total cost price before we enter discount and vat.
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
                txtProductDiscount.Text = ((int)Numbers.InitialIndex).ToString();
        }

        private void txtProductVAT_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtProductVAT.Text == "")
                txtProductVAT.Text = ((int)Numbers.InitialIndex).ToString();
        }

        private void CalculateEntryProductCostPrice()
        {
            decimal number,productTotalCostPrice=(int)Numbers.InitialIndex;

            if (decimal.TryParse(txtProductDiscount.Text, out number) && decimal.TryParse(txtProductVAT.Text, out number) && txtProductDiscount.Text != "" && txtProductVAT.Text != "")
                productTotalCostPrice = Convert.ToDecimal(txtProductGrossTotalCostPrice.Text) - Convert.ToDecimal(txtProductDiscount.Text) + Convert.ToDecimal(txtProductVAT.Text);

            else if (txtProductDiscount.Text == "")
                productTotalCostPrice = Convert.ToDecimal(txtProductGrossTotalCostPrice.Text) + Convert.ToDecimal(txtProductVAT.Text);

            else if (txtProductVAT.Text == "")
                productTotalCostPrice = Convert.ToDecimal(txtProductGrossTotalCostPrice.Text) - Convert.ToDecimal(txtProductDiscount.Text);

            txtProductCostPrice.Text = (productTotalCostPrice / Convert.ToDecimal(txtProductQuantity.Text)).ToString("0.00");
            txtProductTotalCostPrice.Text = productTotalCostPrice.ToString();
        }

        private void CalculateDgProductCostPrice()
        {
            int rowQuantity = dgProducts.Items.Count;

            for (int i = 0; i < rowQuantity; i++)
            {
                DataGridRow row = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(i);

                ////GETTING TEXTBOX FROM DATAGRID.
                ContentPresenter cpProductQuantity = dgProducts.Columns[(int)PopColumns.ColProductQuantity].GetCellContent(row) as ContentPresenter;
                var tmpProductQuantity = cpProductQuantity.ContentTemplate;
                TextBox txtDgProductQuantity = tmpProductQuantity.FindName(dgCellNames[(int)PopColumns.ColProductQuantity], cpProductQuantity) as TextBox;

                ////GETTING TEXTBOX FROM DATAGRID.
                ContentPresenter cpProductCostPrice = dgProducts.Columns[(int)PopColumns.ColProductCostPrice].GetCellContent(row) as ContentPresenter;
                var tmpProductCostPrice = cpProductCostPrice.ContentTemplate;
                TextBox txtDgProductCostPrice = tmpProductCostPrice.FindName(dgCellNames[(int)PopColumns.ColProductCostPrice], cpProductCostPrice) as TextBox;

                //GETTING THE CELL CONTENT OF THE PRODUCT GROSS TOTAL SALE PRICE
                ContentPresenter cpProductGrossTotalCostPrice = dgProducts.Columns[(int)PopColumns.ColProductGrossTotalCostPrice].GetCellContent(row) as ContentPresenter;
                var tmpProductGrossTotalCostPrice = cpProductGrossTotalCostPrice.ContentTemplate;
                TextBox txtDgProductGrossTotalCostPrice = tmpProductGrossTotalCostPrice.FindName(dgCellNames[(int)PopColumns.ColProductGrossTotalCostPrice], cpProductGrossTotalCostPrice) as TextBox;

                //GETTING TEXTBOX FROM DATAGRID.
                ContentPresenter cpProductDiscount = dgProducts.Columns[(int)PopColumns.ColProductDiscount].GetCellContent(row) as ContentPresenter;
                var tmpProductDiscount = cpProductDiscount.ContentTemplate;
                TextBox txtDgProductDiscount = tmpProductDiscount.FindName(dgCellNames[(int)PopColumns.ColProductDiscount], cpProductDiscount) as TextBox;

                //GETTING TEXTBOX FROM DATAGRID.
                ContentPresenter cpProductVAT = dgProducts.Columns[(int)PopColumns.ColProductVAT].GetCellContent(row) as ContentPresenter;
                var tmpProductVAT = cpProductVAT.ContentTemplate;
                TextBox txtDgProductVAT = tmpProductVAT.FindName(dgCellNames[(int)PopColumns.ColProductVAT], cpProductVAT) as TextBox;

                //GETTING TEXTBOX FROM DATAGRID
                ContentPresenter cpProductTotalCostPrice = dgProducts.Columns[(int)PopColumns.ColProductTotalCostPrice].GetCellContent(row) as ContentPresenter;
                var tmpProductTotalCostPrice = cpProductTotalCostPrice.ContentTemplate;
                TextBox txtDgProductTotalCostPrice = tmpProductTotalCostPrice.FindName(dgCellNames[(int)PopColumns.ColProductTotalCostPrice], cpProductTotalCostPrice) as TextBox;

                txtDgProductTotalCostPrice.Text = (Convert.ToDecimal(txtDgProductGrossTotalCostPrice.Text) - Convert.ToDecimal(txtDgProductDiscount.Text) + Convert.ToDecimal(txtDgProductVAT.Text)).ToString("0.00");
                txtDgProductCostPrice.Text = ((Convert.ToDecimal(txtDgProductGrossTotalCostPrice.Text) - Convert.ToDecimal(txtDgProductDiscount.Text) + Convert.ToDecimal(txtDgProductVAT.Text)) / Convert.ToDecimal(txtDgProductQuantity.Text)).ToString("0.00");
            }
        }

        private void CalculateDgDiscountPerProduct()
        {
            int rowQuantity = dgProducts.Items.Count;

            for (int i = 0; i < rowQuantity; i++)
            {
                DataGridRow row = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(i);

                //GETTING THE CELL CONTENT OF THE PRODUCT GROSS TOTAL COST PRICE
                ContentPresenter cpProductGrossTotalCostPrice = dgProducts.Columns[(int)PopColumns.ColProductGrossTotalCostPrice].GetCellContent(row) as ContentPresenter;
                var tmpProductGrossTotalCostPrice = cpProductGrossTotalCostPrice.ContentTemplate;
                TextBox txtDgProductGrossTotalCostPrice = tmpProductGrossTotalCostPrice.FindName(dgCellNames[(int)PopColumns.ColProductGrossTotalCostPrice], cpProductGrossTotalCostPrice) as TextBox;

                //GETTING THE CELL CONTENT OF THE PRODUCT DISCOUNT
                ContentPresenter cpProductDiscount = dgProducts.Columns[(int)PopColumns.ColProductDiscount].GetCellContent(row) as ContentPresenter;
                var tmpProductDiscount = cpProductDiscount.ContentTemplate;
                TextBox txtDgProductDiscount = tmpProductDiscount.FindName(dgCellNames[(int)PopColumns.ColProductDiscount], cpProductDiscount) as TextBox;

                if (txtBasketDiscount.Text != "")
                    //DISCOUNT PER PRODUCT = PRODUCT UNIT PRICE/GROSS TOTAL * DISCOUNT TOTAL
                    txtDgProductDiscount.Text = String.Format("{0:0.00}", (Convert.ToDecimal(txtDgProductGrossTotalCostPrice.Text) / Convert.ToDecimal(txtBasketGrossCostTotal.Text)) * Convert.ToDecimal(txtBasketDiscount.Text));
                else
                    txtDgProductDiscount.Text = ((int)Numbers.InitialIndex).ToString();
            }
        }

        private void CalculateDgVatPerProduct()
        {
            int rowQuantity = dgProducts.Items.Count;

            for (int i = 0; i < rowQuantity; i++)
            {
                DataGridRow row = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(i);

                //GETTING THE CELL CONTENT OF THE PRODUCT GROSS TOTAL COST PRICE
                ContentPresenter cpProductGrossTotalCostPrice = dgProducts.Columns[(int)PopColumns.ColProductGrossTotalCostPrice].GetCellContent(row) as ContentPresenter;
                var tmpProductGrossTotalCostPrice = cpProductGrossTotalCostPrice.ContentTemplate;
                TextBox txtDgProductGrossTotalCostPrice = tmpProductGrossTotalCostPrice.FindName(dgCellNames[(int)PopColumns.ColProductGrossTotalCostPrice], cpProductGrossTotalCostPrice) as TextBox;

                //GETTING THE CELL CONTENT OF THE PRODUCT VAT
                ContentPresenter cpProductVAT = dgProducts.Columns[(int)PopColumns.ColProductVAT].GetCellContent(row) as ContentPresenter;
                var tmpProductVAT = cpProductVAT.ContentTemplate;
                TextBox txtDgProductVAT = tmpProductVAT.FindName(dgCellNames[(int)PopColumns.ColProductVAT], cpProductVAT) as TextBox;

                if (txtBasketVat.Text != "")
                    //VAT PER PRODUCT = PRODUCT UNIT PRICE/GROSS TOTAL * VAT TOTAL
                    txtDgProductVAT.Text = String.Format("{0:0.00}", (Convert.ToDecimal(txtDgProductGrossTotalCostPrice.Text) / Convert.ToDecimal(txtBasketGrossCostTotal.Text)) * Convert.ToDecimal(txtBasketVat.Text));
                else
                    txtDgProductVAT.Text = ((int)Numbers.InitialIndex).ToString();
            }
        }

        private void CalculateBasketGrandTotal(int calledByVatOrDiscount)
        {
            if (decimal.TryParse(txtBasketVat.Text, out decimal number) && txtBasketVat.Text != "" && txtBasketDiscount.Text != "")
            {
                txtBasketSubTotal.Text = (Convert.ToDecimal(txtBasketGrossCostTotal.Text) - Convert.ToDecimal(txtBasketDiscount.Text)).ToString();
                txtBasketGrandTotal.Text = (Convert.ToDecimal(txtBasketGrossCostTotal.Text) - Convert.ToDecimal(txtBasketDiscount.Text) + Convert.ToDecimal(txtBasketVat.Text)).ToString();
            }
            else
            {
                if (calledByVatOrDiscount==calledByVAT)
                {
                    txtBasketVat.Text = ((int)Numbers.InitialIndex).ToString();//We need to assign zero in order to prevent null entry into the database.
                    txtBasketGrandTotal.Text = (Convert.ToDecimal(txtBasketGrossCostTotal.Text) - Convert.ToDecimal(txtBasketDiscount.Text)).ToString();
                    txtBasketVat.SelectAll();//Select all automatically for the user to re-enter new value easily.
                }
                else
                {
                    txtBasketDiscount.Text = ((int)Numbers.InitialIndex).ToString();
                    txtBasketSubTotal.Text = txtBasketGrossCostTotal.Text;//Since the Discount is zero, we only need to assign the basket cost total into the sub total. Normally, sub total = cost total - total discount.
                    txtBasketGrandTotal.Text = (Convert.ToDecimal(txtBasketGrossCostTotal.Text) + Convert.ToDecimal(txtBasketVat.Text)).ToString();
                    txtBasketDiscount.SelectAll();
                }
            }
        }

        private void txtProductGrossCostPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextMenuGrossCostPriceChanged();
        }

        /*----THIS IS NOT AN EFFICIENT CODE----*/
        private void TextMenuGrossCostPriceChanged()
        {
            if (txtProductGrossCostPrice.IsFocused == true)//If the cursor is not focused on this textbox, then no need to check this code.
            {
                if (txtProductGrossCostPrice.Text != "")
                {
                    decimal number;
                    decimal productQuantity = Convert.ToDecimal(txtProductQuantity.Text);
                    string productGrossCostPrice = txtProductGrossCostPrice.Text;
                    char lastCharacter = char.Parse(productGrossCostPrice.Substring(productGrossCostPrice.Length - (int)Numbers.UnitValue));//Getting the last character to check if the user has entered a missing cost price like " 3, ".
                    bool result = Char.IsDigit(lastCharacter);//Checking if the last digit of the number is a number or not.

                    if (decimal.TryParse(productGrossCostPrice, out number) && result == true)
                    {
                        txtProductGrossTotalCostPrice.Text = (Convert.ToDecimal(productGrossCostPrice) * productQuantity).ToString();

                        txtProductCostPrice.Text = ((
                            (Convert.ToDecimal(productGrossCostPrice) * Convert.ToDecimal(productQuantity)) -
                            Convert.ToDecimal(txtProductDiscount.Text) +
                            Convert.ToDecimal(txtProductVAT.Text)
                            ) / Convert.ToDecimal(productQuantity)).ToString("0.00");

                        txtProductTotalCostPrice.Text = (
                            (Convert.ToDecimal(productGrossCostPrice) * productQuantity) -
                            Convert.ToDecimal(txtProductDiscount.Text) +
                            Convert.ToDecimal(txtProductVAT.Text)
                            ).ToString();

                        btnProductAdd.IsEnabled = true;
                    }

                    else//Reverting the quantity to the default value.
                    {
                        MessageBox.Show("Please enter a valid number");

                        using (DataTable dataTable = productDAL.SearchProductByIdBarcode(txtProductId.Text))
                        {
                            int rowIndex = 0;
                            txtProductGrossCostPrice.Text = dataTable.Rows[rowIndex][colTxtCostPrice].ToString();//We are reverting the cost price of the product to default if the user has pressed a wrong key such as "a-b-c".
                        }
                    }
                }

                /* If the user left the txtProductGrossCostPrice as empty, wait for him to enter a new value and block the btnProductAdd. 
                   Note: Because the "TextChanged" function works immediately, we don't revert the value into the default. User may click on the "backspace" to correct it by himself"*/
                else
                {
                    btnProductAdd.IsEnabled = false;
                }
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

                    char lastCharacter = char.Parse(productQuantity.Substring(productQuantity.Length - (int)Numbers.UnitValue));//Getting the last character to check if the user has entered a missing quantity like " 3, "

                    bool result = Char.IsDigit(lastCharacter);//Checking if the last digit of the number is a number or not.

                    if (decimal.TryParse(productQuantity, out number) && result == true)
                    {
                        string unitKg = "Kilogram", unitLt = "Liter";
                        string productGrossCostPrice = txtProductGrossCostPrice.Text;

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

                        txtProductGrossTotalCostPrice.Text = (Convert.ToDecimal(productGrossCostPrice) * Convert.ToDecimal(productQuantity)).ToString();

                        txtProductCostPrice.Text = ((
                            (Convert.ToDecimal(productGrossCostPrice) * Convert.ToDecimal(productQuantity)) -
                            Convert.ToDecimal(txtProductDiscount.Text) +
                            Convert.ToDecimal(txtProductVAT.Text)
                            )/Convert.ToDecimal(productQuantity)).ToString("0.00");

                        txtProductTotalCostPrice.Text = (
                            (Convert.ToDecimal(productGrossCostPrice) * Convert.ToDecimal(productQuantity)) -
                            Convert.ToDecimal(txtProductDiscount.Text) +
                            Convert.ToDecimal(txtProductVAT.Text)
                            ).ToString();
                        btnProductAdd.IsEnabled = true;
                    }

                    else//Reverting the quantity to the default value.
                    {
                        MessageBox.Show("Please enter a valid number");
                        txtProductQuantity.Text = ((int)Numbers.UnitValue).ToString();//We are reverting the quantity of the product to default if the user has pressed a wrong key such as "a-b-c".  
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

        private void txtInvoiceNo_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtInvoiceNo.Text == ((int)Numbers.InitialIndex).ToString())
            {
                txtInvoiceNo.Text = "";
            }
        }

        private void txtInvoiceNo_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtInvoiceNo.Text == "" || !Int32.TryParse(txtInvoiceNo.Text, out int value))//The code will work if the text is empty or does NOT contain a numeric value.
            {
                txtInvoiceNo.Text = ((int)Numbers.InitialIndex).ToString();
            }
        }

        private void txtProductDiscount_KeyUp(object sender, KeyEventArgs e)
        {
            CalculateEntryProductCostPrice();
        }

        private void txtProductVAT_KeyUp(object sender, KeyEventArgs e)
        {
            CalculateEntryProductCostPrice();
        }

        private void txtBasketVat_KeyUp(object sender, KeyEventArgs e)
        {
            CalculateDgVatPerProduct();
            CalculateDgProductCostPrice();//Because VAT is divided per product in CalculateDgVatPerProduct() method above, we need to recalculate the total product sale price in the datagrid.
            CalculateBasketGrandTotal(calledByVAT);
        }

        private void txtBasketDiscount_KeyUp(object sender, KeyEventArgs e)
        {
            CalculateDgDiscountPerProduct();
            CalculateDgProductCostPrice();//Because Discount is divided per product in CalculateDgDiscountPerProduct() method above, we need to recalculate the total product sale price in the datagrid.
            CalculateBasketGrandTotal(calledByDiscount);
        }

        private void dgProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgProducts.SelectedItem!=null)
            {
                //GETTING TEXTBOX FROM DATAGRID.
                ContentPresenter cpProductQuantity = dgProducts.Columns[(int)PopColumns.ColProductQuantity].GetCellContent(dgProducts.SelectedItem) as ContentPresenter;
                var tmpProductQuantity = cpProductQuantity.ContentTemplate;
                TextBox productQuantity = tmpProductQuantity.FindName(dgCellNames[(int)PopColumns.ColProductQuantity], cpProductQuantity) as TextBox;
                
                //GETTING TEXTBOX FROM DATAGRID.
                ContentPresenter cpProductGrossTotalCostPrice = dgProducts.Columns[(int)PopColumns.ColProductGrossTotalCostPrice].GetCellContent(dgProducts.SelectedItem) as ContentPresenter;
                var tmpProductGrossTotalCostPrice = cpProductGrossTotalCostPrice.ContentTemplate;
                TextBox productGrossTotalCostPrice = tmpProductGrossTotalCostPrice.FindName(dgCellNames[(int)PopColumns.ColProductGrossTotalCostPrice], cpProductGrossTotalCostPrice) as TextBox;

                //GETTING TEXTBOX FROM DATAGRID.
                ContentPresenter cpProductDiscount = dgProducts.Columns[(int)PopColumns.ColProductDiscount].GetCellContent(dgProducts.SelectedItem) as ContentPresenter;
                var tmpProductDiscount = cpProductDiscount.ContentTemplate;
                TextBox txtDgProductDiscount = tmpProductDiscount.FindName(dgCellNames[(int)PopColumns.ColProductDiscount], cpProductDiscount) as TextBox;

                //GETTING TEXTBOX FROM DATAGRID.
                ContentPresenter cpProductVAT = dgProducts.Columns[(int)PopColumns.ColProductVAT].GetCellContent(dgProducts.SelectedItem) as ContentPresenter;
                var tmpProductVAT = cpProductVAT.ContentTemplate;
                TextBox txtDgProductVAT = tmpProductVAT.FindName(dgCellNames[(int)PopColumns.ColProductVAT], cpProductVAT) as TextBox;

                //GETTING TEXTBOX FROM DATAGRID.
                ContentPresenter cpProductTotalCostPrice = dgProducts.Columns[(int)PopColumns.ColProductTotalCostPrice].GetCellContent(dgProducts.SelectedItem) as ContentPresenter;
                var tmpProductTotalCostPrice = cpProductTotalCostPrice.ContentTemplate;
                TextBox productTotalCostPrice = tmpProductTotalCostPrice.FindName(dgCellNames[(int)PopColumns.ColProductTotalCostPrice], cpProductTotalCostPrice) as TextBox;

                oldBasketQuantity = Convert.ToDecimal(txtBasketQuantity.Text) - Convert.ToDecimal(productQuantity.Text);
                oldBasketGrossCostTotal = Convert.ToDecimal(txtBasketGrossCostTotal.Text) - Convert.ToDecimal(productGrossTotalCostPrice.Text);
                oldBasketSubTotal = Convert.ToDecimal(txtBasketSubTotal.Text) - (Convert.ToDecimal(productGrossTotalCostPrice.Text) - Convert.ToDecimal(txtDgProductDiscount.Text));
                oldBasketDiscount = Convert.ToDecimal(txtBasketDiscount.Text) - Convert.ToDecimal(txtDgProductDiscount.Text);
                oldBasketVAT = Convert.ToDecimal(txtBasketVat.Text) - Convert.ToDecimal(txtDgProductVAT.Text);
                oldBasketGrandTotal = Convert.ToDecimal(txtBasketGrandTotal.Text) - Convert.ToDecimal(productTotalCostPrice.Text);
            }
        }

        private void LoadCboMenuAsset(int checkStatus)
        {
            DataTable dtAccount;
            if (checkStatus == Convert.ToInt32(Assets.Account))
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

        private void LoadCboMenuSupplier()
        {
            //Creating Data Table to hold the products from Database
            DataTable dtSupplier = supplierDAL.Select();

            //Specifying Items Source for product combobox
            cboMenuSupplier.ItemsSource = dtSupplier.DefaultView;

            //Here DisplayMemberPath helps to display Text in the ComboBox.
            cboMenuSupplier.DisplayMemberPath = colTxtName;

            //SelectedValuePath helps to store values like a hidden field.
            cboMenuSupplier.SelectedValuePath = colTxtId;
        }

        private void cboMenuSupplier_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int sourceId;
            int sourceType= Convert.ToInt32(Assets.Supplier);

            sourceId = Convert.ToInt32(cboMenuSupplier.SelectedValue);
            lblAssetSupplierId.Content = assetDAL.GetAssetIdBySource(sourceId, sourceType);
        }

        private void cboMenuAsset_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int sourceId;
            int sourceType;

            if (rbAccount.IsChecked == true)//DO NOT REPEAT YOURSELF!!!!! YOU HAVE ALREADY HAVE THESE SECTION ABOVE!
                sourceType = Convert.ToInt32(Assets.Account);
            else
                sourceType = Convert.ToInt32(Assets.Bank);

            sourceId = Convert.ToInt32(cboMenuAsset.SelectedValue);
            lblAssetId.Content = assetDAL.GetAssetIdBySource(sourceId,sourceType);
        }

        private void rbAccount_Checked(object sender, RoutedEventArgs e)
        {
            //cboMenuAsset.ItemsSource = null;
            LoadCboMenuAsset(Convert.ToInt32(Assets.Account));
        }

        private void rbBank_Checked(object sender, RoutedEventArgs e)
        {
            //cboMenuAsset.ItemsSource = null;
            LoadCboMenuAsset(Convert.ToInt32(Assets.Bank));
        }
    }
}