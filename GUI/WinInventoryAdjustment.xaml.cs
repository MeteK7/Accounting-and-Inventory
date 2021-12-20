using BLL;
using DAL;
using CUL;
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
using CUL.Enums;

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
        UserBLL userBLL = new UserBLL();
        PointOfSaleDAL pointOfSaleDAL = new PointOfSaleDAL();
        PointOfSaleCUL pointOfSaleCUL = new PointOfSaleCUL();
        PointOfSaleDetailDAL pointOfSaleDetailDAL = new PointOfSaleDetailDAL();
        PointOfSaleDetailCUL pointOfSaleDetailCUL = new PointOfSaleDetailCUL();
        InventoryAdjustmentDAL inventoryAdjustmentDAL = new InventoryAdjustmentDAL();
        InventoryAdjustmentCUL inventoryAdjustmentCUL = new InventoryAdjustmentCUL();
        InventoryAdjustmentDetailDAL inventoryAdjustmentDetailDAL = new InventoryAdjustmentDetailDAL();
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
        string colQtyNameInDb = "quantity_in_stock", colCostPriceNameInDb = "costprice";
        decimal newQuantity;
        int colLength = 10;

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FillStaffInformations()
        {
            txtUsername.Text = WinLogin.loggedInUserName;
            txtUserType.Text = WinLogin.loggedInUserType;
        }

        private void LoadPastInventoryAdjustmentPage(int inventoryAdjustmentId = (int)Numbers.InitialIndex, int invoiceArrow = -(int)Numbers.UnitValue)//Optional parameter
        {
            int productUnitId;
            string productId, productName, productUnitName;
            decimal productCostPrice, productSalePrice, productQuantityInReal, productQuantityInStock, productQuantityDifference, productTotalCostPrice, productTotalSalePrice;

            if (inventoryAdjustmentId == (int)Numbers.InitialIndex)
            {
                inventoryAdjustmentId = GetLastInventoryAdjustmentId();//Getting the last invoice number and assign it to the variable called invoiceNo.
            }

            /*WE CANNOT USE ELSE IF FOR THE CODE BELOW! BOTH IF STATEMENTS ABOVE AND BELOVE MUST WORK.*/
            if (inventoryAdjustmentId != (int)Numbers.InitialIndex)// If the invoice number is still 0 even when we get the last invoice number by using code above, that means this is the first sale and do not run this code block.
            {
                DataTable dataTableInventoryAdjustment = inventoryAdjustmentDAL.Search(inventoryAdjustmentId);
                DataTable dataTableInventoryAdjustmentDetail = inventoryAdjustmentDetailDAL.Search(inventoryAdjustmentId);
                DataTable dataTableUnitInfo;
                DataTable dataTableProduct;

                if (dataTableInventoryAdjustmentDetail.Rows.Count != (int)Numbers.InitialIndex)
                {
                    #region LOADING THE PRODUCT DATA GRID

                    for (int currentRow = (int)Numbers.InitialIndex; currentRow < dataTableInventoryAdjustmentDetail.Rows.Count; currentRow++)
                    {
                        lblIventoryAdjustmentId.Content = dataTableInventoryAdjustment.Rows[(int)Numbers.InitialIndex]["id"].ToString();

                        productId = dataTableInventoryAdjustmentDetail.Rows[currentRow]["product_id"].ToString();
                        productUnitId = Convert.ToInt32(dataTableInventoryAdjustmentDetail.Rows[currentRow]["product_unit_id"]);

                        dataTableUnitInfo = unitDAL.GetUnitInfoById(productUnitId);//Getting the unit name by unit id.
                        productUnitName = dataTableUnitInfo.Rows[(int)Numbers.InitialIndex]["name"].ToString();//We use firstRowIndex value for the index number in every loop because there can be only one unit name of a specific id.

                        productCostPrice = Convert.ToDecimal(dataTableInventoryAdjustmentDetail.Rows[currentRow]["product_cost_price"]);
                        productSalePrice = Convert.ToDecimal(dataTableInventoryAdjustmentDetail.Rows[currentRow]["product_sale_price"]);
                        productQuantityInReal = Convert.ToDecimal(dataTableInventoryAdjustmentDetail.Rows[currentRow]["product_quantity_in_real"]);
                        productQuantityInStock = Convert.ToDecimal(dataTableInventoryAdjustmentDetail.Rows[currentRow]["product_quantity_in_stock"]);
                        productQuantityDifference = productQuantityInReal - productQuantityInStock;//There is already a same formula in the method named "txtProductQuantityInReal_TextChanged"!!!.
                        productTotalCostPrice = productCostPrice * productQuantityInReal;//We do NOT store the total cost in the db to reduce the storage. Instead of it, we multiply the unit cost with the quantity to find the total cost.
                        productTotalSalePrice = productSalePrice * productQuantityInReal;//We do NOT store the total price in the db to reduce the storage. Instead of it, we multiply the unit price with the quantity to find the total price.

                        dataTableProduct = productDAL.SearchById(productId);

                        productName = dataTableProduct.Rows[(int)Numbers.InitialIndex]["name"].ToString();//We used firstRowIndex because there can be only one row in the datatable for a specific product.

                        dgProducts.Items.Add(new { Id = productId, Name = productName, Unit = productUnitName, CostPrice = productCostPrice, SalePrice = productSalePrice, QuantityInReal = productQuantityInReal, QuantityInStock = productQuantityInStock, QuantityDifference= productQuantityDifference, TotalCostPrice = productTotalCostPrice, TotalSalePrice = productTotalSalePrice });
                    }
                    #endregion

                    #region FILLING THE PREVIOUS BASKET INFORMATIONS

                    //We used firstRowIndex below as a row name because there can be only one row in the datatable for a specific Invoice.
                    txtBasketQuantity.Text = dataTableInventoryAdjustment.Rows[(int)Numbers.InitialIndex]["total_product_quantity"].ToString();
                    txtBasketGrandTotal.Text = dataTableInventoryAdjustment.Rows[(int)Numbers.InitialIndex]["grand_total"].ToString();

                    #endregion
                }
                else if (dataTableInventoryAdjustmentDetail.Rows.Count == (int)Numbers.InitialIndex)//If the pos detail row quantity is 0, that means there is no such row so decrease or increase the invoice number according to user preference.
                {
                    if (invoiceArrow == 0)//If the invoice arrow is 0, that means user clicked the previous button.
                    {
                        inventoryAdjustmentId = inventoryAdjustmentId - (int)Numbers.UnitValue;
                    }
                    else
                    {
                        inventoryAdjustmentId = inventoryAdjustmentId + (int)Numbers.UnitValue;
                    }

                    if (invoiceArrow != -(int)Numbers.UnitValue)//If the user has not clicked either previous or next button, then the invoiceArrow will be -1 and no need for recursion.
                    {
                        LoadPastInventoryAdjustmentPage(inventoryAdjustmentId, invoiceArrow);//Call the method again to get the new past invoice.
                    }

                }
            }
            else
            {
                FirstTimeRun();//This method is called when it is the first time of using this program.
            }

        }

        private void FirstTimeRun()
        {
            MessageBox.Show("Welcome!\n Thank you for choosing Accounting and Inventory System.");
            btnEditRecord.IsEnabled = false;//There cannot be any editable records for the first run.
            btnDeleteRecord.IsEnabled = false;//There cannot be any deletible records for the first run.
            btnPrev.IsEnabled = false;//Disabling the btnPrev button because there is no any records in the database for the first time.
            btnNext.IsEnabled = false;//Disabling the btnNext button because there is no any records in the database for the first time.
        }

        private void ClearProductEntranceTextBox()
        {
            txtProductId.Text = "";
            txtProductName.Text = "";
            txtProductUnit.Text = "";
            txtProductCostPrice.Text = "";
            txtProductSalePrice.Text = "";
            txtProductQuantityInReal.Text = "";
            txtProductQuantityInStock.Text = "";
            txtProductQuantityDifference.Text = "";
            txtProductTotalCostPrice.Text = "";
            Keyboard.Focus(txtProductId); // set keyboard focus
            DisableProductEntranceButtons();
        }

        private void ClearBasketTextBox()
        {
            txtBasketQuantity.Text = ((int)Numbers.InitialIndex).ToString();
            txtBasketGrandTotal.Text = ((int)Numbers.InitialIndex).ToString();
        }

        private void ClearInventoryAdjustmentDataGrid()
        {
            dgProducts.Items.Clear();
        }

        private int GetLastInventoryAdjustmentId()
        {
            int inventoryAdjustmentId;

            DataTable dataTable = inventoryAdjustmentDAL.Search();//Searching the last id number in the tbl_inventory_adjustment.

            if (dataTable.Rows.Count != (int)Numbers.InitialIndex)//If there is an inventory adjustment id in the database, that means the database table's first row cannot be null, and the datatable table's first index is 0.
            {
                inventoryAdjustmentId = Convert.ToInt32(dataTable.Rows[(int)Numbers.InitialIndex]["id"]);//We defined this code out of the for loop below because all of the products has the same invoice number in every sale. So, no need to call this method for every products again and again.
            }
            else//If there is no any inventory adjustment id in the db, that means it is the first record. So, assing inventoryAdjustmentId with 0;
            {
                inventoryAdjustmentId = (int)Numbers.InitialIndex;
            }
            return inventoryAdjustmentId;
        }

        private void LoadNewInventoryAdjustment()/*INVENTORY ADJUSTMENT ID REFERS TO THE ID NUMBER IN THE DATABASE FOR WinInventoryAdjustment.*/
        {
            ClearBasketTextBox();
            ClearInventoryAdjustmentDataGrid();

            int inventoryAdjustmentId;

            inventoryAdjustmentId = GetLastInventoryAdjustmentId();//Getting the last inventory adjustment record id and assign it to the variable called inventoryAdjustmentId.
            inventoryAdjustmentId += (int)Numbers.UnitValue;//We are adding one to the last inventory adjustment record id because every new inventory adjustment record id is one greater tham the previous one.
            lblIventoryAdjustmentId.Content = inventoryAdjustmentId;//Assigning inventory adjustment record id to the content of the inventory adjustment record id label.
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
            txtProductUnit.IsEnabled = true;
            txtProductId.IsEnabled = true;
            txtProductName.IsEnabled = true;
            txtProductCostPrice.IsEnabled = true;
            txtProductSalePrice.IsEnabled = true;
            txtProductQuantityInReal.IsEnabled = true;
            txtProductQuantityInStock.IsEnabled = true;
            txtProductQuantityDifference.IsEnabled = true;
            txtProductTotalCostPrice.IsEnabled = true;
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
            txtProductQuantityInReal.IsEnabled = false;
            txtProductUnit.IsEnabled = false;
            txtProductCostPrice.IsEnabled = false;
            txtProductSalePrice.IsEnabled = false;
            txtProductQuantityInReal.IsEnabled = false;
            txtProductQuantityInStock.IsEnabled = false;
            txtProductQuantityDifference.IsEnabled = false;
            txtProductTotalCostPrice.IsEnabled = false;
            dgProducts.IsHitTestVisible = false;//Disabling the datagrid clicking.
        }

        private void EnableButtonsOnClickSaveCancel()
        {
            btnNew.IsEnabled = true;//If the products are saved successfully, enable the new button to be able to add new products.
            btnEditRecord.IsEnabled = true;//If the products are saved successfully, enable the edit button to be able to edit an existing invoice.
            btnDeleteRecord.IsEnabled = true;
            btnPrev.IsEnabled = true;
            btnNext.IsEnabled = true;
        }

        private void DisableProductEntranceButtons()
        {
            btnProductAdd.IsEnabled = false; //Disabling the add button if all text boxes are cleared.
            btnProductClear.IsEnabled = false; //Disabling the clear button if all text boxes are cleared.
        }

        private void txtProductId_KeyUp(object sender, KeyEventArgs e)
        {
            int number;
            DataTable dataTableProduct = productDAL.SearchProductByIdBarcode(txtProductId.Text);

            if (txtProductId.Text != Numbers.InitialIndex.ToString() && int.TryParse(txtProductId.Text, out number) && dataTableProduct.Rows.Count != (int)Numbers.InitialIndex)//Validating the barcode if it is a number(except zero) or not.
            {
                int productUnitId;
                decimal productQuantityInStock;
                string productId, productBarcodeRetail, productName, productUnitName, productCostPrice, productSalePrice;

                productId = dataTableProduct.Rows[(int)Numbers.InitialIndex]["id"].ToString();
                productBarcodeRetail = dataTableProduct.Rows[(int)Numbers.InitialIndex]["barcode_retail"].ToString();
                productName = dataTableProduct.Rows[(int)Numbers.InitialIndex]["name"].ToString();//Filling the product name textbox from the database.
                productCostPrice = dataTableProduct.Rows[(int)Numbers.InitialIndex]["costprice"].ToString();
                productSalePrice = dataTableProduct.Rows[(int)Numbers.InitialIndex]["saleprice"].ToString();
                productQuantityInStock = Convert.ToDecimal(dataTableProduct.Rows[(int)Numbers.InitialIndex][colQtyNameInDb]);


                if (productBarcodeRetail == txtProductId.Text || productId.ToString() == txtProductId.Text)//If the barcode equals the product's barcode_retail or id, then take the product's retail unit id.
                {
                    productUnitId = Convert.ToInt32(dataTableProduct.Rows[(int)Numbers.InitialIndex]["unit_retail_id"]);
                }

                else //If the barcode equals to the barcode_wholesale, then take the product's wholesale unit id.
                {
                    productUnitId = Convert.ToInt32(dataTableProduct.Rows[(int)Numbers.InitialIndex]["unit_wholesale_id"]);
                }

                DataTable dataTableProductUnit = unitDAL.GetUnitInfoById(productUnitId);//Datatable for finding the unit name by unit id.
                productUnitName = dataTableProductUnit.Rows[(int)Numbers.InitialIndex]["name"].ToString();//Populating the textbox with the related unit name from dataTableUnit.

                txtProductName.Text = productName;
                txtProductUnit.Text = productUnitName;
                txtProductCostPrice.Text = productCostPrice;
                txtProductSalePrice.Text = productSalePrice;
                txtProductQuantityInStock.Text = productQuantityInStock.ToString();


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
            btnNewOrEdit = (int)Numbers.InitialIndex;//0 stands for the user has entered the btnNew.
            LoadNewInventoryAdjustment();
            ModifyToolsOnClickBtnNewOrEdit();
        }

        private void btnProductAdd_Click(object sender, RoutedEventArgs e)
        {
            bool addNewProductLine = true;
            int barcodeColNo = (int)Numbers.InitialIndex;
            int quantityInReal,quantityDifference;
            int rowQuntity = dgProducts.Items.Count;

            for (int i = (int)Numbers.InitialIndex; i < rowQuntity; i++)
            {
                DataGridRow row = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(i);

                TextBlock barcodeCellContent = dgProducts.Columns[barcodeColNo].GetCellContent(row) as TextBlock;    //Try to understand this code!!!  

                if (barcodeCellContent.Text == txtProductId.Text)
                {
                    if (MessageBox.Show("There is already the same item in the list. Would you like to sum them?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        TextBlock tbCellQuantityInRealContent = dgProducts.Columns[(int)InvAdjColumns.ColProductQuantityInReal].GetCellContent(row) as TextBlock;
                        TextBlock tbCellQuantityDifferenceContent = dgProducts.Columns[(int)InvAdjColumns.ColProductQuantityDifference].GetCellContent(row) as TextBlock;
                        TextBlock tbCellTotalCostPriceContent = dgProducts.Columns[(int)InvAdjColumns.ColProductTotalCostPrice].GetCellContent(row) as TextBlock;
                        TextBlock tbCellTotalSalePriceContent = dgProducts.Columns[(int)InvAdjColumns.ColProductTotalSalePrice].GetCellContent(row) as TextBlock;

                        quantityInReal = Convert.ToInt32(tbCellQuantityInRealContent.Text);
                        quantityInReal += Convert.ToInt32(txtProductQuantityInReal.Text);//We are adding the quantity entered in the "txtProductQuantity" to the previous quantity cell's content.

                        tbCellQuantityInRealContent.Text = quantityInReal.ToString();//Assignment of the new quantity to the related cell.

                        quantityDifference = Convert.ToInt32(tbCellQuantityDifferenceContent.Text);
                        quantityDifference += Convert.ToInt32(txtProductQuantityDifference.Text);//We are adding the quantity entered in the "txtProductQuantityDifference" to the previous quantity difference cell's content.

                        tbCellQuantityDifferenceContent.Text = quantityDifference.ToString();//Assignment of the new quantity difference to the related cell.


                        tbCellTotalCostPriceContent.Text = (quantityDifference * Convert.ToDecimal(txtProductCostPrice.Text)).ToString();
                        tbCellTotalSalePriceContent.Text = (quantityDifference * Convert.ToDecimal(txtProductSalePrice.Text)).ToString();//Calculating the new total price according to the new entry.

                        addNewProductLine = false;
                        break;//We have to break the loop if the user clicked "yes" because no need to scan the rest of the rows after confirming.
                    }
                }
            }


            if (addNewProductLine == true)//Use ENUMS instead of this!!!!!!!
            {
                dgProducts.Items.Add(new 
                { 
                    Id = txtProductId.Text, 
                    Name = txtProductName.Text, 
                    Unit = txtProductUnit.Text, 
                    CostPrice = txtProductCostPrice.Text, 
                    SalePrice = txtProductSalePrice.Text, 
                    QuantityInReal = txtProductQuantityInReal.Text, 
                    QuantityInStock=txtProductQuantityInStock.Text, 
                    QuantityDifference=txtProductQuantityDifference.Text, 
                    TotalCostPrice = (Convert.ToDecimal(txtProductCostPrice.Text) * Convert.ToDecimal(txtProductQuantityDifference.Text)).ToString("0.00"), 
                    TotalSalePrice = (Convert.ToDecimal(txtProductSalePrice.Text) * Convert.ToDecimal(txtProductQuantityDifference.Text)).ToString("0.00")
                });
            }

            dgProducts.UpdateLayout();
            rowQuntity = dgProducts.Items.Count;//Renewing the row quantity after adding a new product.

            PopulateBasket();

            ClearProductEntranceTextBox();

            //items[0].BarcodeRetail = "EXAMPLECODE"; This code can change the 0th row's data on the column called BarcodeRetail.
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
                    ClearInventoryAdjustmentDataGrid();
                    LoadNewInventoryAdjustment();
                    EnableButtonsOnClickSaveCancel();
                    break;
                case MessageBoxResult.No:
                    MessageBox.Show("Enjoy!", "Enjoy");
                    break;
                case MessageBoxResult.Cancel:
                    MessageBox.Show("Nevermind then...", "Accounting");
                    break;
            }
        }

        int inventoryAdjustmentArrow;
        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            int currentInvoiceNo = Convert.ToInt32(lblIventoryAdjustmentId.Content);

            if (currentInvoiceNo != (int)Numbers.InitialIndex)
            {
                ClearInventoryAdjustmentDataGrid();
                int prevInventoryAdjustment = Convert.ToInt32(lblIventoryAdjustmentId.Content) - (int)Numbers.UnitValue;
                inventoryAdjustmentArrow = (int)Numbers.InitialIndex;//0 means customer has clicked the previous button.
                LoadPastInventoryAdjustmentPage(prevInventoryAdjustment, inventoryAdjustmentArrow);
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            int lastInventoryAdjustmentId = GetLastInventoryAdjustmentId(), currentInvoiceNo = Convert.ToInt32(lblIventoryAdjustmentId.Content);

            if (currentInvoiceNo != lastInventoryAdjustmentId)
            {
                ClearInventoryAdjustmentDataGrid();
                int nextInvoice = Convert.ToInt32(lblIventoryAdjustmentId.Content) + (int)Numbers.UnitValue;
                inventoryAdjustmentArrow = (int)Numbers.UnitValue;//1 means customer has clicked the next button.
                LoadPastInventoryAdjustmentPage(nextInvoice, inventoryAdjustmentArrow);
            }
        }

        private string[,] GetDataGridContent()//This method stores the previous list in a global array variable called "cells" when we press the Edit button.
        {
            int rowLength = dgProducts.Items.Count;
            string[,] dgProductCells = new string[rowLength, colLength];

            for (int rowNo = (int)Numbers.InitialIndex; rowNo < rowLength; rowNo++)
            {
                DataGridRow dgRow = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(rowNo);

                for (int colNo = (int)Numbers.InitialIndex; colNo < colLength; colNo++)
                {
                    TextBlock tbCellContent = dgProducts.Columns[colNo].GetCellContent(dgRow) as TextBlock;

                    dgProductCells[rowNo, colNo] = tbCellContent.Text;

                    //dgOldProductCells[rowNo, colNo] = cells[rowNo, colNo];//Assigning the old products' informations to the global array called "dgOldProductCells" so that we can access to the old products to revert the changes.
                }
            }

            return dgProductCells;
        }

        private DataTable GetLastInventoryAdjustment()
        {
            //int specificRowIndex = 0, invoiceNo;

            DataTable dataTable = inventoryAdjustmentDAL.Search();//A METHOD WHICH HAS AN OPTIONAL PARAMETER

            return dataTable;
        }

        private void RevertOldQuantityInStock()
        {
            int productId;
            int colProductId = (int)Numbers.InitialIndex;
            decimal productQtyFromDB;


            DataTable dtProduct = new DataTable();

            for (int rowNo = (int)Numbers.InitialIndex; rowNo < oldItemsRowCount; rowNo++)
            {
                dtProduct = productDAL.SearchProductByIdBarcode(dgOldProductCells[rowNo, colProductId]);

                productQtyFromDB = Convert.ToInt32(dtProduct.Rows[(int)Numbers.InitialIndex][colQtyNameInDb]);

                newQuantity = productQtyFromDB - Convert.ToDecimal(dgOldProductCells[rowNo, (int)InvAdjColumns.ColProductQuantityDifference]);//Revert the quantity in stock.

                productId = Convert.ToInt32(dgOldProductCells[rowNo, colProductId]);

                productDAL.UpdateSpecificColumn(productId,colQtyNameInDb, newQuantity.ToString());
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
                #region TABLE INVENTORY ADJUSTMENT SAVING SECTION
                int inventoryAdjustmentId = Convert.ToInt32(lblIventoryAdjustmentId.Content);
                int userId = userBLL.GetUserId(WinLogin.loggedInUserName);

                DataTable dataTableLastInvoice = GetLastInventoryAdjustment();//Getting the last invoice number and assign it to the variable called invoiceId.
                DataTable dataTableProduct = new DataTable();
                DataTable dataTableUnit = new DataTable();

                //Getting the values from the POS Window and fill them into the pointOfSaleCUL.
                inventoryAdjustmentCUL.Id = inventoryAdjustmentId;//The column invoice id in the database is not auto incremental. This is to prevent the number from increasing when the user deletes an existing invoice and creates a new invoice.
                inventoryAdjustmentCUL.TotalProductQuantity = Convert.ToDecimal(txtBasketQuantity.Text);
                inventoryAdjustmentCUL.GrandTotal = Convert.ToDecimal(txtBasketGrandTotal.Text);
                inventoryAdjustmentCUL.AddedDate = DateTime.Now;
                inventoryAdjustmentCUL.AddedBy = userId;
                #endregion

                #region TABLE INVENTORY ADJUSTMENT DETAILS SAVING SECTION

                int userClickedNewOrEdit = btnNewOrEdit;
                int productId;
                int unitId;
                string[] cells = new string[colLength];
                bool isSuccessProductQty = false;
                bool isSuccessDetail = false;
                bool isSuccess = false;

                for (int rowNo = (int)Numbers.InitialIndex; rowNo < dgProducts.Items.Count; rowNo++)
                {
                    if (userClickedNewOrEdit == (int)Numbers.UnitValue)//If the user clicked the btnEditRecord, then edit the specific invoice's products in tbl_inventory_adjustment_detailed at once.
                    {
                        RevertOldQuantityInStock();//Reverting the old products' quantity in stock.

                        //We are sending invoiceNo as a parameter to the "Delete" Method. So that we can erase all the products which have the specific invoice number.
                        inventoryAdjustmentDetailDAL.Delete(inventoryAdjustmentId);

                        //2 means null for this code. We used this in order to prevent running the if block again and again. Because, we erase all of the products belong to one invoice number at once.
                        userClickedNewOrEdit = (int)Buttons.Null;
                    }

                    DataGridRow row = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(rowNo);

                    for (int colNo = (int)Numbers.InitialIndex; colNo < colLength; colNo++)
                    {
                        TextBlock cellContent = dgProducts.Columns[colNo].GetCellContent(row) as TextBlock;

                        cells[colNo] = cellContent.Text;
                    }

                    dataTableProduct = productDAL.SearchProductByIdBarcode(cells[(int)Numbers.InitialIndex]);//Cell[0] may contain the product id or barcode_retail or barcode_wholesale.
                    productId = Convert.ToInt32(dataTableProduct.Rows[(int)Numbers.InitialIndex]["id"]);//Row index is always zero for this situation because there can be only one row of a product which has a unique barcode on the table.


                    dataTableUnit = unitDAL.GetUnitInfoByName(cells[(int)InvAdjColumns.ColProductUnit]);//Cell[2] contains the unit name.
                    unitId = Convert.ToInt32(dataTableUnit.Rows[(int)Numbers.InitialIndex]["id"]);//Row index is always zero for this situation because there can be only one row of a specific unit.

                    inventoryAdjustmentDetailCUL.ProductId = productId;
                    inventoryAdjustmentDetailCUL.InventoryAdjustmentId = inventoryAdjustmentId;
                    inventoryAdjustmentDetailCUL.ProductUnitId = unitId;
                    inventoryAdjustmentDetailCUL.ProductCostPrice = Convert.ToDecimal(cells[(int)InvAdjColumns.ColProductCostPrice]);
                    inventoryAdjustmentDetailCUL.ProductSalePrice = Convert.ToDecimal(cells[(int)InvAdjColumns.ColProductSalePrice]);
                    inventoryAdjustmentDetailCUL.ProductQuantityInReal = Convert.ToDecimal(cells[(int)InvAdjColumns.ColProductQuantityInReal]);
                    inventoryAdjustmentDetailCUL.ProductQuantityInStock = Convert.ToDecimal(cells[(int)InvAdjColumns.ColProductQuantityInStock]);
                    inventoryAdjustmentDetailCUL.ProductQuantityDifference = Convert.ToDecimal(cells[(int)InvAdjColumns.ColProductQuantityDifference]);

                    isSuccessDetail = inventoryAdjustmentDetailDAL.Insert(inventoryAdjustmentDetailCUL);

                    #region TABLE PRODUCT INVENTORY ADJUSTMENT SECTION
                    newQuantity = Convert.ToDecimal(cells[(int)InvAdjColumns.ColProductQuantityInReal]);//Assigning the real quantity of the product in the facility to the system's stock.
                    isSuccessProductQty = productDAL.UpdateSpecificColumn(productId, colQtyNameInDb, newQuantity.ToString());
                    #endregion
                }
                #endregion

                userClickedNewOrEdit = btnNewOrEdit;// We are reassigning the btnNewOrEdit value into userClickedNewOrEdit.

                if (userClickedNewOrEdit == (int)Numbers.UnitValue)//If the user clicked the btnEditRecord, then update the specific invoice information in tbl_pos at once.
                {
                    isSuccess = inventoryAdjustmentDAL.Update(inventoryAdjustmentCUL);
                }

                else
                {
                    //Creating a Boolean variable to insert data into the database.
                    isSuccess = inventoryAdjustmentDAL.Insert(inventoryAdjustmentCUL);
                }


                //If the data is inserted successfully, then the value of the variable isSuccess will be true; otherwise it will be false.
                if (isSuccess == true && isSuccessDetail == true && isSuccessProductQty==true)//IsSuccessDetail is always CHANGING in every loop above! IMPROVE THIS!!!!
                {
                    //ClearBasketTextBox();
                    //ClearInventoryAdjustmentDataGrid();
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

        private void BtnDeleteRecord_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Would you really like to delete this record, "+ WinLogin.loggedInUserName+"?", "Delete Record", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:

                    #region DELETE INVOICE
                    int inventoryAdjustmentId = Convert.ToInt32(lblIventoryAdjustmentId.Content); //GetLastInvoiceNumber(); You can also call this method and add number 1 to get the current invoice number, but getting the ready value is faster than getting the last invoice number from the database and adding a number to it to get the current invoice number.

                    inventoryAdjustmentCUL.Id = inventoryAdjustmentId;//Assigning the invoice number into the Id in the pointofSaleCUL.
                    inventoryAdjustmentDetailCUL.InventoryAdjustmentId = inventoryAdjustmentId;

                    inventoryAdjustmentDAL.Delete(inventoryAdjustmentCUL);
                    inventoryAdjustmentDetailDAL.Delete(inventoryAdjustmentId);
                    #endregion

                    #region REVERT THE STOCK
                    oldItemsRowCount = dgProducts.Items.Count;//When the user clicks Edit, the index of old(previously saved) items row will be assigned to oldItemsRowCount.
                    dgOldProductCells = (string[,])(GetDataGridContent().Clone());//Cloning one array into another array.
                    RevertOldQuantityInStock();
                    #endregion

                    #region PREPARE TO THE LAST PAGE
                    DisableTools();
                    ClearProductEntranceTextBox();
                    ClearInventoryAdjustmentDataGrid();
                    LoadPastInventoryAdjustmentPage();
                    EnableButtonsOnClickSaveCancel();
                    #endregion

                    break;
                case MessageBoxResult.No:
                    MessageBox.Show("Enjoy!", "Enjoy");
                    break;
                case MessageBoxResult.Cancel:
                    MessageBox.Show("Nevermind then...", "Accounting");
                    break;
            }
        }

        private void txtProductQuantityInReal_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtProductQuantityInReal.IsFocused == true)//If the cursor is not focused on this textbox, then no need to check this code.
            {
                if (txtProductQuantityInReal.Text != "")
                {
                    decimal number;
                    string textProductQuantityInReal = txtProductQuantityInReal.Text;
                    char lastCharacter = char.Parse(textProductQuantityInReal.Substring(textProductQuantityInReal.Length - 1));//Getting the last character to check if the user has entered a missing quantity like " 3, ".
                    bool result = Char.IsDigit(lastCharacter);//Checking if the last digit of the number is a number or not.

                    if (decimal.TryParse(textProductQuantityInReal, out number) && result == true)
                    {
                        DataTable dtProduct = productDAL.SearchProductByIdBarcode(txtProductId.Text);

                        string unitKg = "Kilogram", unitLt = "Liter";
                        decimal productQuantity, productQuantityInStock = Convert.ToDecimal(txtProductQuantityInStock.Text);
                        string productCostPrice = dtProduct.Rows[(int)Numbers.InitialIndex]["costprice"].ToString();

                        #region Checking the unit type
                        if (txtProductUnit.Text != unitKg && txtProductUnit.Text != unitLt)
                        {
                            /*If the user entered any unit except kilogram or liter, there cannot be a decimal quantity. 
                            So, convert the quantity to integer even the user has entered a decimal quantity as a mistake.*/
                            productQuantity = Convert.ToInt32(Convert.ToDecimal(txtProductQuantityInReal.Text));
                            txtProductQuantityInReal.Text = productQuantity.ToString();
                        }
                        else//If the user has defined the unit as kilogram or liter, then there can be a decimal quantity like "3,5 liter."
                        {
                            productQuantity = Convert.ToDecimal(txtProductQuantityInReal.Text);
                        }
                        #endregion

                        //#region Checking the sign of the quantity in stock. NO NEED FOR THIS!!!!!
                        //if (productQuantityInStock < (int)Numbers.InitialIndex) //If it is a negative quantity, convert it into positive.
                        //    productQuantityInStock = Math.Abs(productQuantityInStock);
                        //#endregion

                        txtProductQuantityDifference.Text = (productQuantity - productQuantityInStock).ToString();//Getting the quantity difference by subtracting the quantity in stock from the current quantity.
                        txtProductTotalCostPrice.Text = (Convert.ToDecimal(productCostPrice) * Convert.ToDecimal(txtProductQuantityDifference.Text)).ToString("0.00");
                    }

                    else//Reverting the quantity to the default value.
                    {
                        MessageBox.Show("Please enter a valid number");
                        txtProductQuantityInReal.Text = Numbers.UnitValue.ToString();//We are reverting the quantity of the product to default if the user has pressed a wrong key such as "a-b-c".
                        btnProductAdd.IsEnabled = true;//Once the mistake has been corrected, enable btnProductAdd.
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

        private void BtnDeleteDataGridRow_Click(object sender, RoutedEventArgs e)
        {
            var selectedRow = dgProducts.SelectedItem;
            int selectedRowIndex = dgProducts.SelectedIndex;

            if (selectedRow != null)
            {
                SubstractBasket(selectedRowIndex);

                dgProducts.Items.Remove(selectedRow);
            }
        }

        private void PopulateBasket()
        {
            decimal quantityDifference = Convert.ToDecimal(txtProductQuantityDifference.Text);

            txtBasketQuantity.Text = (Convert.ToDecimal(txtBasketQuantity.Text) + quantityDifference).ToString();

            txtBasketGrandTotal.Text = (Convert.ToDecimal(txtBasketGrandTotal.Text) + (Convert.ToDecimal(txtProductCostPrice.Text) * quantityDifference)).ToString();
        }

        private void SubstractBasket(int selectedRowIndex)
        {
            DataGridRow dtProduct;
            TextBlock tbCellQuantityDifference;
            TextBlock tbCellTotalCostPrice;

            dtProduct = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(selectedRowIndex);

            tbCellQuantityDifference = dgProducts.Columns[(int)InvAdjColumns.ColProductQuantityDifference].GetCellContent(dtProduct) as TextBlock;

            tbCellTotalCostPrice = dgProducts.Columns[(int)InvAdjColumns.ColProductTotalCostPrice].GetCellContent(dtProduct) as TextBlock;    //Try to understand this code!!!  

            txtBasketQuantity.Text = (Convert.ToDecimal(txtBasketQuantity.Text) - Convert.ToDecimal(tbCellQuantityDifference.Text)).ToString();

            txtBasketGrandTotal.Text = ((Convert.ToDecimal(txtBasketGrandTotal.Text) - Convert.ToDecimal(tbCellTotalCostPrice)).ToString());
        }
    }
}
