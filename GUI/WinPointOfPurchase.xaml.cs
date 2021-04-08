﻿using BLL;
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

        int btnNewOrEdit;//0 stands for user clicked the button New, and 1 stands for user clicked the button Edit.
        string[,] dgOldProductCells = new string[,] { };
        string calledBy= "POP";
        int oldItemsRowCount;
        int invoiceArrow;

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
            cboProductUnit.IsEnabled = false;
            txtProductId.IsEnabled = false;
            txtProductName.IsEnabled = false;
            txtProductCostPrice.IsEnabled = false;
            txtProductAmount.IsEnabled = false;
            txtProductTotalCostPrice.IsEnabled = false;
            txtInvoiceNo.IsEnabled = false;
        }

        private void ModifyToolsOnClickBtnNewOrEdit()//Do NOT repeat yourself! You have used IsEnabled function for these toolbox contents many times! And in the other pages as well!
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
            cboProductUnit.IsEnabled = true;
            txtProductId.IsEnabled = true;
            txtProductName.IsEnabled = true;
            txtProductCostPrice.IsEnabled = true;
            txtProductAmount.IsEnabled = true;
            txtProductTotalCostPrice.IsEnabled = true;
            txtInvoiceNo.IsEnabled = true;
            chkUpdateProductCosts.IsEnabled = true;
            dgProducts.IsHitTestVisible = true;//Enabling the datagrid clicking.
            cboMenuSupplier.SelectedIndex = -1;//-1 Means nothing is selected.
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
            txtProductAmount.Text = "";
            txtProductTotalCostPrice.Text = "";
            Keyboard.Focus(txtProductId); // set keyboard focus
            DisableProductEntranceButtons();
        }

        private void LoadNewInvoice()
        {
            ClearBasketTextBox();
            ClearProductsDataGrid();
        }

        //-1 means user did not clicked either previous or next button which means user just clicked the point of purchase button to open it.
        private void LoadPastInvoice(int invoiceId = 0, int invoiceArrow = -1)//Optional parameter
        {
            int firstRowIndex = 0, productUnitId;
            string productId, productName, productUnitName, productCostPrice, productAmount, productTotalCostPrice;

            if (invoiceId == 0)//If the ID is 0 came from the optional parameter, that means user just clicked the pop button to open it.
            {
                DataTable dataTableLastInvoice = pointOfPurchaseBLL.GetLastInvoiceRecord();//Getting the last invoice id and assign it to the variable called invoiceId.

                if (dataTableLastInvoice.Rows.Count != 0)
                {
                    invoiceId = Convert.ToInt32(dataTableLastInvoice.Rows[firstRowIndex]["id"]);
                }
            }

            /*WE CANNOT USE ELSE IF FOR THE CODE BELOW! BOTH IF STATEMENTS ABOVE AND BELOVE MUST WORK.*/
            if (invoiceId != 0)// If the invoice number is still 0 even when we get the last invoice number by using code above, that means this is the first sale and do not run this code block.
            {
                DataTable dataTablePop = pointOfPurchaseDAL.GetByInvoiceId(invoiceId);
                DataTable dataTablePopDetail = pointOfPurchaseDetailDAL.Search(invoiceId);
                DataTable dataTableUnitInfo;
                DataTable dataTableProduct;

                if (dataTablePopDetail.Rows.Count != 0)
                {
                    #region LOADING THE PRODUCT DATA GRID

                    for (int currentRow = firstRowIndex; currentRow < dataTablePopDetail.Rows.Count; currentRow++)
                    {
                        cboMenuPaymentType.SelectedValue = Convert.ToInt32(dataTablePop.Rows[firstRowIndex]["payment_type_id"].ToString());//Getting the id of purchase type.
                        cboMenuSupplier.SelectedValue = Convert.ToInt32(dataTablePop.Rows[firstRowIndex]["supplier_id"].ToString());//Getting the id of supplier.
                        txtInvoiceNo.Text = dataTablePop.Rows[firstRowIndex]["invoice_no"].ToString();

                        productId = dataTablePopDetail.Rows[currentRow]["product_id"].ToString();
                        productUnitId = Convert.ToInt32(dataTablePopDetail.Rows[currentRow]["product_unit_id"]);

                        dataTableUnitInfo = unitDAL.GetUnitInfoById(productUnitId);//Getting the unit name by unit id.
                        productUnitName = dataTableUnitInfo.Rows[firstRowIndex]["name"].ToString();//We use firstRowIndex value for the index number in every loop because there can be only one unit name of a specific id.

                        productCostPrice = dataTablePopDetail.Rows[currentRow]["product_cost_price"].ToString();
                        productAmount = dataTablePopDetail.Rows[currentRow]["amount"].ToString();
                        productTotalCostPrice = (Convert.ToDecimal(productCostPrice) * Convert.ToDecimal(productAmount)).ToString();//We do NOT store the total cost in the db to reduce the storage. Instead of it, we multiply the unit cost with the amount to find the total cost.

                        dataTableProduct = productDAL.SearchById(productId);

                        productName = dataTableProduct.Rows[firstRowIndex]["name"].ToString();//We used firstRowIndex because there can be only one row in the datatable for a specific product.

                        dgProducts.Items.Add(new { Id = productId, Name = productName, Unit = productUnitName, CostPrice = productCostPrice, Amount = productAmount, TotalCostPrice = productTotalCostPrice });
                    }
                    #endregion

                    #region FILLING THE PREVIOUS BASKET INFORMATIONS

                    //We used firstRowIndex below as a row name because there can be only one row in the datatable for a specific Invoice.
                    txtBasketAmount.Text = dataTablePop.Rows[firstRowIndex]["total_product_amount"].ToString();
                    txtBasketCostTotal.Text = dataTablePop.Rows[firstRowIndex]["cost_total"].ToString();
                    txtBasketSubTotal.Text = dataTablePop.Rows[firstRowIndex]["sub_total"].ToString();
                    txtBasketVat.Text = dataTablePop.Rows[firstRowIndex]["vat"].ToString();
                    txtBasketDiscount.Text = dataTablePop.Rows[firstRowIndex]["discount"].ToString();
                    txtBasketGrandTotal.Text = dataTablePop.Rows[firstRowIndex]["grand_total"].ToString();

                    #endregion
                }
                else if (dataTablePopDetail.Rows.Count == 0)//If the pop detail row quantity is 0, that means there is no such row so decrease or increase the invoice number according to user preference.
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
            int colLength = 6;
            string[,] dgProductCells = new string[rowLength, colLength];


            for (int rowNo = 0; rowNo < rowLength; rowNo++)
            {
                DataGridRow dgRow = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(rowNo);

                for (int colNo = 0; colNo < colLength; colNo++)
                {
                    TextBlock tbCellContent = dgProducts.Columns[colNo].GetCellContent(dgRow) as TextBlock;

                    dgProductCells[rowNo, colNo] = tbCellContent.Text;

                    //dgOldProductCells[rowNo, colNo] = cells[rowNo, colNo];//Assigning the old products' informations to the global array called "dgOldProductCells" so that we can access to the old products to revert the changes.
                }
            }

            return dgProductCells;
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
            //-1 means nothing has been chosen in the combobox. Note: We had to add the --&& txtInvoiceNo.Text.ToString()!= "0"-- into the if statement because the invoice text does not have the restriction so that the user may enter wrongly..
            if (isDgEqual == false && int.TryParse(txtInvoiceNo.Text, out int number) && txtInvoiceNo.Text != "0" && cboMenuPaymentType.SelectedIndex != -1 && cboMenuSupplier.SelectedIndex != -1)
            {
                int userClickedNewOrEdit = btnNewOrEdit;
                int invoiceNo = Convert.ToInt32(txtInvoiceNo.Text);
                int userId = userBLL.GetUserId(WinLogin.loggedInUserName);
                int initialIndex = 0;
                bool isSuccess = false;

                int currentInvoiceId = 1, firstRowIndex = 0;//Current invoice id is 1 number greater than the previous id. So that we assign 1 as a default value to add it to the previous id later.
                DataTable dataTableLastInvoice = pointOfPurchaseBLL.GetLastInvoiceRecord();//Getting the last invoice.
                DataTable dataTableProduct = new DataTable();
                DataTable dataTableUnit = new DataTable();


                //If there is a row in the datatable, then fetch the id of the invoice. Otherwise, it is the first run and keep the default value.
                if (userClickedNewOrEdit == 0 && dataTableLastInvoice.Rows.Count != 0)
                {
                    currentInvoiceId = currentInvoiceId + Convert.ToInt32(dataTableLastInvoice.Rows[firstRowIndex]["id"]);//Getting the new invoice id.
                }

                /*ONLY ELSE MAY BE MORE SUITABLE!!!*/
                else if (userClickedNewOrEdit == 1)//If it is in the Edit Mode, then assign the old invoice id in order to update the same invoice id later.
                {
                    currentInvoiceId = pointOfPurchaseBLL.GetInvoiceIdByNo(txtInvoiceNo.Text);
                }

                #region TABLE POP SAVING SECTION
                //Getting the values from the POP Window and fill them into the pointOfPurchaseCUL.
                pointOfPurchaseCUL.Id = currentInvoiceId;//The column invoice id in the database is not auto incremental. This is for preventing the number increasing when the user deletes an existing invoice and creates a new invoice.
                pointOfPurchaseCUL.InvoiceNo = invoiceNo;
                pointOfPurchaseCUL.PaymentTypeId = Convert.ToInt32(cboMenuPaymentType.SelectedValue);//Selected value contains the id of the item so that no need to get it from DB.
                pointOfPurchaseCUL.SupplierId = Convert.ToInt32(cboMenuSupplier.SelectedValue);
                pointOfPurchaseCUL.TotalProductAmount = Convert.ToInt32(txtBasketAmount.Text);
                pointOfPurchaseCUL.CostTotal = Convert.ToDecimal(txtBasketCostTotal.Text);
                pointOfPurchaseCUL.SubTotal = Convert.ToDecimal(txtBasketSubTotal.Text);
                pointOfPurchaseCUL.Vat = Convert.ToDecimal(txtBasketVat.Text);
                pointOfPurchaseCUL.Discount = Convert.ToDecimal(txtBasketDiscount.Text);
                pointOfPurchaseCUL.GrandTotal = Convert.ToDecimal(txtBasketGrandTotal.Text);
                pointOfPurchaseCUL.AddedDate = DateTime.Now;
                pointOfPurchaseCUL.AddedBy = userId;

                userClickedNewOrEdit = btnNewOrEdit;// We are reassigning the btnNewOrEdit value into userClickedNewOrEdit.

                if (userClickedNewOrEdit == 1)//If the user clicked the btnEdit, then update the specific invoice information in tbl_pop at once.
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
                int cellUnit = 2, cellCostPrice = 3, cellProductAmount = 4;
                int productId;
                int unitId;
                decimal productOldAmountInStock;
                int cellLength = 6;
                int addedBy = userId;
                string[] cells = new string[cellLength];
                DateTime dateTime = DateTime.Now;
                bool isSuccessDetail = false;
                int productRate = 0;//Modify this code dynamically!!!!!!!!!

                for (int rowNo = 0; rowNo < dgProducts.Items.Count; rowNo++)
                {
                    if (userClickedNewOrEdit == 1)//If the user clicked the btnEdit, then delete the specific invoice's products in tbl_pos_detailed at once.
                    {
                        productBLL.RevertOldAmountInStock(dgOldProductCells, dgProducts.Items.Count, calledBy);//Reverting the old products' amount in stock.

                        //We are sending pointOfPurchaseDetailCUL as a parameter to the Delete method just to use the Id property in the SQL Query. So that we can erase all the products which have the specific id.
                        pointOfPurchaseDetailDAL.Delete(pointOfPurchaseDetailCUL);

                        //2 means null for this code. We used this in order to prevent running the if block again and again. Because, we erase all of the products belong to one invoice number at once.
                        userClickedNewOrEdit = 2;
                    }

                    DataGridRow row = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(rowNo);

                    for (int colNo = 0; colNo < cellLength; colNo++)
                    {
                        TextBlock cellContent = dgProducts.Columns[colNo].GetCellContent(row) as TextBlock;    //Try to understand this code!!!  

                        cells[colNo] = cellContent.Text;
                    }

                    dataTableProduct = productDAL.SearchProductByIdBarcode(cells[initialIndex]);//Cell[0] contains the product barcode.
                    productId = Convert.ToInt32(dataTableProduct.Rows[initialIndex]["id"]);//Row index is always zero for this situation because there can be only one row of a product which has a unique barcode on the table.


                    dataTableUnit = unitDAL.GetUnitInfoByName(cells[cellUnit]);//Cell[0] contains the product barcode.
                    unitId = Convert.ToInt32(dataTableUnit.Rows[initialIndex]["id"]);//Row index is always zero for this situation because there can be only one row of a specific unit.

                    pointOfPurchaseDetailCUL.Id = currentInvoiceId;//No incremental value in the database because there can be multiple goods with the same invoice id.
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

                    if (chkUpdateProductCosts.IsChecked==true)
                        productCUL.CostPrice = Convert.ToDecimal(cells[cellCostPrice]);

                    productCUL.Id = productId;//Assigning the Id in the productCUL to update the product columns in the DB using a specific product.

                    productDAL.UpdateAmountInStock(productCUL);
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
            int barcodeColNo = 0;
            //int costColNo = 3; NO NEED TO GET THE COST CONTENT AGAIN SINCE WE HAVE ALREADY GOT IT FROM THE FIRST ENTRY OF THIS PRODUCT.
            //int priceColNo = 4;
            int amountColNo = 4;
            int totalCostColNo = 5;
            int amount = 0;
            int rowQuntity = dgProducts.Items.Count;

            for (int i = 0; i < rowQuntity; i++)
            {
                DataGridRow row = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(i);

                TextBlock barcodeCellContent = dgProducts.Columns[barcodeColNo].GetCellContent(row) as TextBlock;    //Try to understand this code!!!  

                if (barcodeCellContent.Text == txtProductId.Text)
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

                dgProducts.Items.Add(new { Id = txtProductId.Text, Name = txtProductName.Text, Unit = cboProductUnit.SelectedItem, CostPrice = txtProductCostPrice.Text, Amount = txtProductAmount.Text, TotalCostPrice = totalCostPrice.ToString() });
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

            //You may think that it would be better to get the total cost price instead of multiplying the amount by unit cost price. However, the total cost price is updated only when the txtAmount is lost the focus.
            txtBasketSubTotal.Text = (Convert.ToDecimal(txtBasketSubTotal.Text) + (Convert.ToDecimal(txtProductTotalCostPrice.Text))).ToString();

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

        private void cboMenuSupplier_Loaded(object sender, RoutedEventArgs e)
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
            btnNewOrEdit = 0;//0 stands for the user has entered the btnNew.
            LoadNewInvoice();
            ModifyToolsOnClickBtnNewOrEdit();
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
            btnNewOrEdit = 1;//1 stands for the user has entered the btnEdit.
            oldItemsRowCount = dgProducts.Items.Count;//When the user clicks Edit, the index of old(previously saved) items row will be assigned to oldItemsRowCount.
            dgOldProductCells = (string[,])(GetDataGridContent().Clone());//Cloning one array into another array.
            ModifyToolsOnClickBtnNewOrEdit();
        }

        private void btnDeleteRecord_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Would you really like to delete the invoice, you piece of shit?", "Delete Invoice", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:

                    #region DELETE INVOICE
                    int invoiceNo = Convert.ToInt32(txtInvoiceNo.Text);

                    pointOfPurchaseCUL.InvoiceNo = invoiceNo;//Assigning the invoice number into the Id in the pointofSaleCUL.
                    //pointOfPurchaseDetailCUL.InvoiceNo = invoiceNo; REFACTOR THIS CODE!!!

                    pointOfPurchaseDAL.Delete(pointOfPurchaseCUL);
                    pointOfPurchaseDetailDAL.Delete(pointOfPurchaseDetailCUL);
                    #endregion

                    #region REVERT THE STOCK
                    oldItemsRowCount = dgProducts.Items.Count;//When the user clicks Edit, the index of old(previously saved) items row will be assigned to oldItemsRowCount.
                    dgOldProductCells = (string[,])(GetDataGridContent().Clone());//Cloning one array into another array.
                    productBLL.RevertOldAmountInStock(dgOldProductCells, dgProducts.Items.Count, calledBy);
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
            int  firstInvoiceId = 1, currentInvoiceId;

            currentInvoiceId = pointOfPurchaseBLL.GetInvoiceIdByNo(txtInvoiceNo.Text);

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
            int voidInvoiceId = 0, lastInvoiceId, currentInvoiceId;

            currentInvoiceId = pointOfPurchaseBLL.GetInvoiceIdByNo(txtInvoiceNo.Text);

            DataTable dataTableLastInvoice = pointOfPurchaseBLL.GetLastInvoiceRecord();
            lastInvoiceId = Convert.ToInt32(dataTableLastInvoice.Rows[voidInvoiceId]["id"]);//Getting the last invoice id.


            if (currentInvoiceId != lastInvoiceId)
            {
                ClearProductsDataGrid();
                int nextInvoice = Convert.ToInt32(currentInvoiceId) + 1;
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

        private void txtProductId_KeyUp(object sender, KeyEventArgs e)
        {
            int number;

            DataTable dataTable = productDAL.SearchProductByIdBarcode(txtProductId.Text);

            if (txtProductId.Text != 0.ToString() && int.TryParse(txtProductId.Text, out number) && dataTable.Rows.Count != 0)//Validating the barcode if it is a number(except zero) or not.
            {
                int productAmount = 1;
                int rowIndex = 0;
                int productId;
                int productUnit = 0;
                string productBarcodeRetail/*, productBarcodeWholesale*/;
                string costPrice;

                btnProductAdd.IsEnabled = true; //Enabling the add button if any valid barcode is entered.
                btnProductClear.IsEnabled = true;//Enabling the clear button if any valid barcode is entered.


                productId = Convert.ToInt32(dataTable.Rows[rowIndex]["id"]);
                productBarcodeRetail = dataTable.Rows[rowIndex]["barcode_retail"].ToString();
                //productBarcodeWholesale = dataTable.Rows[rowIndex]["barcode_wholesale"].ToString();


                if (productBarcodeRetail == txtProductId.Text || productId.ToString() == txtProductId.Text)//If the barcode equals the product's barcode_retail or id, then take the product's retail unit id.
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
                cboProductUnit.SelectedIndex = 0;//For selecting the combobox's first element. We selected 0 index because we have just one unit of a retail product.

                costPrice = dataTable.Rows[rowIndex]["costprice"].ToString();

                txtProductCostPrice.Text = costPrice;
                txtProductAmount.Text = productAmount.ToString();
                txtProductTotalCostPrice.Text = (Convert.ToDecimal(costPrice) * productAmount).ToString();
            }

            /*--->If the txtProductId is empty which means user has clicked the backspace button and if the txtProductName is filled once before, then erase all the text contents.
            Note: I just checked the btnProductAdd to know if there was a product entry before or not.
                  If the btnProductAdd is not enabled in the if block above once before, then no need to call the method ClearProductEntranceTextBox.*/
            else if (txtProductId.Text == "" && btnProductAdd.IsEnabled == true)
            {
                ClearProductEntranceTextBox();
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
        private void txtProductCostPrice_TextChanged(object sender, TextChangedEventArgs e)
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
                        decimal productAmount= Convert.ToDecimal(txtProductAmount.Text);

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
    }
}