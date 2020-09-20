using KabaAccounting.BLL;
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

namespace KabaAccounting.UI
{
    /// <summary>
    /// Interaction logic for WinPointOfPurchase.xaml
    /// </summary>
    public partial class WinPointOfPurchase : Window
    {
        public WinPointOfPurchase()
        {
            InitializeComponent();
            DisableButtonsTools();
            FillStaffInformations();
            LoadPastInvoice();
        }

        UserDAL userDAL = new UserDAL();
        PointOfPurchaseDAL pointOfPurchaseDAL = new PointOfPurchaseDAL();
        PointOfPurchaseBLL pointOfPurchaseBLL = new PointOfPurchaseBLL();
        PointOfPurchaseDetailDAL pointOfPurchaseDetailDAL = new PointOfPurchaseDetailDAL();
        PointOfPurchaseDetailBLL pointOfPurchaseDetailBLL = new PointOfPurchaseDetailBLL();
        ProductDAL productDAL = new ProductDAL();
        ProductBLL productBLL = new ProductBLL();
        CustomerDAL customerDAL = new CustomerDAL();
        CustomerBLL customerBLL = new CustomerBLL();
        UnitDAL unitDAL = new UnitDAL();
        UnitBLL unitBLL = new UnitBLL();

        int btnNewOrEdit;//0 stands for user clicked the button New, and 1 stands for user clicked the button Edit.

        private void LoadPastInvoice(int invoiceNo = 0, int invoiceArrow = -1)//Optional parameter
        {
            int firstRowIndex = 0, productUnitId;
            string productId, productBarcode, productName, productUnitName, productCost, productPrice, productAmount, productTotalCost, productTotalPrice;

            if (invoiceNo == 0)
            {
                invoiceNo = GetLastInvoiceNumber();//Getting the last invoice number and assign it to the variable called invoiceNo.
            }

            if (invoiceNo != 0)// If the invoice number is still 0 even when we get the last invoice number by using code above, that means this is the first sale and do not run this code block.
            {
                DataTable dataTablePopDetail = pointOfPurchaseDetailDAL.Search(invoiceNo);
                DataTable dataTableUnitInfo;

                if (dataTablePopDetail.Rows.Count != 0)
                {
                    #region LOADING THE PRODUCT DATA GRID

                    for (int currentRow = firstRowIndex; currentRow < dataTablePopDetail.Rows.Count; currentRow++)
                    {
                        productId = dataTablePopDetail.Rows[currentRow]["product_id"].ToString();
                        productUnitId = Convert.ToInt32(dataTablePopDetail.Rows[currentRow]["product_unit_id"]);

                        dataTableUnitInfo = unitDAL.GetUnitInfoById(productUnitId);//Getting the unit name by unit id.
                        productUnitName = dataTableUnitInfo.Rows[firstRowIndex]["name"].ToString();//We use firstRowIndex value for the index number in every loop because there can be only one unit name of a specific id.

                        productCost = dataTablePopDetail.Rows[currentRow]["product_cost_price"].ToString();
                        productPrice = dataTablePopDetail.Rows[currentRow]["product_sale_price"].ToString();
                        productAmount = dataTablePopDetail.Rows[currentRow]["amount"].ToString();
                        productTotalCost = (Convert.ToDecimal(productCost) * Convert.ToDecimal(productAmount)).ToString();//We do NOT store the total cost in the db to reduce the storage. Instead of it, we multiply the unit cost with the amount to find the total cost.
                        productTotalPrice = (Convert.ToDecimal(productPrice) * Convert.ToDecimal(productAmount)).ToString();//We do NOT store the total price in the db to reduce the storage. Instead of it, we multiply the unit price with the amount to find the total price.

                        DataTable dataTableProduct = productDAL.SearchById(productId);

                        productBarcode = dataTableProduct.Rows[firstRowIndex]["id"].ToString();//The id column in the products table stands for the barcode of the product.
                        productName = dataTableProduct.Rows[firstRowIndex]["name"].ToString();//We used firstRowIndex because there can be only one row in the datatable for a specific product.

                        dgProducts.Items.Add(new { Barcode = productBarcode, Name = productName, Unit = productUnitName, Cost = productCost, Price = productPrice, Amount = productAmount, TotalCost = productTotalCost, TotalPrice = productTotalPrice });
                    }
                    #endregion

                    #region FILLING THE PREVIOUS BASKET INFORMATIONS

                    DataTable dataTablePop = pointOfPurchaseDAL.Search(invoiceNo);//This Search method gets the id and row informations in the table which belong to the last invoice.

                    //We used firstRowIndex below as a row name because there can be only one row in the datatable for a specific Invoice.
                    txtInvoiceNo.Text = dataTablePop.Rows[firstRowIndex]["id"].ToString();
                    //txtBasketGrandTotalProducts.Text= dataTablePos.Rows[rowIndex]["id"].ToString();
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
                        invoiceNo = invoiceNo - 1;
                    }
                    else
                    {
                        invoiceNo = invoiceNo + 1;
                    }

                    if (invoiceArrow != -1)//If the user has not clicked either previous or next button, then the invoiceArrow will be -1 and no need for recursion.
                    {
                        LoadPastInvoice(invoiceNo, invoiceArrow);//Call the method again to get the new past invoice.
                    }

                }
            }

        }

        private void FillStaffInformations()
        {
            txtStaffName.Text = WinLogin.loggedIn;
            txtStaffPosition.Text = WinLogin.loggedInPosition;
        }

        private void RefreshProductDataGrid()
        {
            //Refreshing Data Grid View
            DataTable dataTable = productDAL.Select();
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
            btnEdit.IsEnabled = true;//If the products are saved successfully, enable the edit button to be able to edit an existing invoice.
            btnDelete.IsEnabled = true;
            btnPrev.IsEnabled = true;
            btnNext.IsEnabled = true;
        }

        private void DisableButtonsTools()
        {
            DisableProductEntranceButtons();
            dgProducts.IsHitTestVisible = false;//Disabling the datagrid clicking.
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;
            btnPrint.IsEnabled = false;
            cboPurchaseType.IsEnabled = false;
            cboSupplier.IsEnabled = false;
            cboProductUnit.IsEnabled = false;
            txtProductBarcode.IsEnabled = false;
            txtProductName.IsEnabled = false;
            txtProductCostPrice.IsEnabled = false;
            txtProductAmount.IsEnabled = false;
            txtProductTotalCostPrice.IsEnabled = false;
            txtInvoiceNo.IsEnabled = false;
        }
        private int GetUserId()//You used this method in WinProducts, as well. You can Make an external class just for this to prevent repeatings!!!.
        {
            //Getting the name of the user from the Login Window and fill it into a string variable;
            string loggedUser = WinLogin.loggedIn;

            //Calling the method named GetIdFromUsername in the userDAL and sending the variable loggedUser as a parameter into it.
            //Then, fill the result into the userBLL;
            UserBLL userBLL = userDAL.GetIdFromUsername(loggedUser);

            int userId = userBLL.Id;

            return userId;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RevertOldAmountInStock()
        {
            //productBLL.AmountInStock = productOldAmountInStock;//Revert the amount in stock.
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //-1 means nothing has been chosen in the combobox. Note: We had to add the --&& txtInvoiceNo.Text.ToString()!= "0"-- into the if statement because the invoice text does not have the restriction so that the user may enter wrongly..
            if (int.TryParse(txtInvoiceNo.Text, out int number) && txtInvoiceNo.Text!="0" && cboPurchaseType.SelectedIndex!=-1 &&cboSupplier.SelectedIndex!=-1)
            {
                int invoiceNo = Convert.ToInt32(txtInvoiceNo.Text); //GetLastInvoiceNumber(); You can also call this method and add number 1 to get the current invoice number, but getting the ready value is faster than getting the last invoice number from the database and adding a number to it to get the current invoice number.
                int userId = GetUserId();

                //Getting the values from the POS Window and fill them into the pointOfPurchaseBLL.
                pointOfPurchaseBLL.InvoiceNo = invoiceNo;
                pointOfPurchaseBLL.SaleType = cboPurchaseType.Text;
                pointOfPurchaseBLL.CustomerId = Convert.ToInt32(cboSupplier.SelectedValue);
                pointOfPurchaseBLL.CostTotal = Convert.ToDecimal(txtBasketCostTotal.Text);
                pointOfPurchaseBLL.SubTotal = Convert.ToDecimal(txtBasketSubTotal.Text);
                pointOfPurchaseBLL.Vat = Convert.ToDecimal(txtBasketVat.Text);
                pointOfPurchaseBLL.Discount = Convert.ToDecimal(txtBasketDiscount.Text);
                pointOfPurchaseBLL.GrandTotal = Convert.ToDecimal(txtBasketGrandTotal.Text);
                pointOfPurchaseBLL.AddedDate = DateTime.Now;
                pointOfPurchaseBLL.AddedBy = userId;

                #region TABLE POS DETAILS SAVING SECTION

                int userClickedNewOrEdit = btnNewOrEdit;
                int cellUnit = 2, cellCostPrice = 3, cellProductAmount = 4;
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
                    DataGridRow row = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(rowNo);

                    for (int colNo = 0; colNo < cellLength; colNo++)
                    {
                        TextBlock cellContent = dgProducts.Columns[colNo].GetCellContent(row) as TextBlock;    //Try to understand this code!!!  

                        cells[colNo] = cellContent.Text;
                    }

                    DataTable dataTableProduct = new DataTable();
                    dataTableProduct = productDAL.SearchSpecificProductById(cells[initialRowIndex]);//Cell[0] contains the product barcode.
                    productId = Convert.ToInt32(dataTableProduct.Rows[initialRowIndex]["id"]);//Row index is always zero for this situation because there can be only one row of a product which has a unique barcode on the table.


                    DataTable dataTableUnit = new DataTable();
                    dataTableUnit = unitDAL.GetUnitInfoByName(cells[cellUnit]);//Cell[0] contains the product barcode.
                    unitId = Convert.ToInt32(dataTableUnit.Rows[initialRowIndex]["id"]);//Row index is always zero for this situation because there can be only one row of a specific unit.

                    //dataTable.Rows[rowIndex]["saleprice"].ToString();
                    pointOfPurchaseDetailBLL.ProductId = productId;
                    pointOfPurchaseDetailBLL.InvoiceNo = invoiceNo;
                    pointOfPurchaseDetailBLL.AddedDate = dateTime;
                    pointOfPurchaseDetailBLL.AddedBy = addedBy;
                    pointOfPurchaseDetailBLL.ProductRate = productRate;
                    pointOfPurchaseDetailBLL.ProductUnitId = unitId;
                    pointOfPurchaseDetailBLL.ProductCostPrice = Convert.ToDecimal(cells[cellCostPrice]);//cells[3] contains cost price of the product in the list.
                    pointOfPurchaseDetailBLL.ProductAmount = Convert.ToDecimal(cells[cellProductAmount]);


                    if (userClickedNewOrEdit == 1)//If the user clicked the btnEdit, then delete the specific invoice's products in tbl_pos_detailed at once.
                    {
                        //We are sending pointOfPurchaseDetailBLL as a parameter to the Delete method just to use the Invoice Number property in the SQL Query. So that we can erase all the products which have the specific invoice number.
                        pointOfPurchaseDetailDAL.Delete(pointOfPurchaseDetailBLL);

                        //2 means null for this code. We used this in order to prevent running the if block again and again. Because, we erase all of the products belong to one invoice number at once.
                        userClickedNewOrEdit = 2;
                    }


                    productBLL.Id = productId;

                    productOldAmountInStock = Convert.ToDecimal(dataTableProduct.Rows[initialRowIndex]["amount_in_stock"].ToString());//Getting the old product amount in stock.

                    productBLL.AmountInStock = productOldAmountInStock + Convert.ToDecimal(cells[cellProductAmount]);

                    productDAL.UpdateAmountInStock(productBLL);

                    isSuccessDetail = pointOfPurchaseDetailDAL.Insert(pointOfPurchaseDetailBLL);
                }
                #endregion

                userClickedNewOrEdit = btnNewOrEdit;// We are reassigning the btnNewOrEdit value into userClickedNewOrEdit.

                if (userClickedNewOrEdit == 1)//If the user clicked the btnEdit, then update the specific invoice information in tbl_pop at once.
                {
                    isSuccess = pointOfPurchaseDAL.Update(pointOfPurchaseBLL);
                }

                else
                {
                    //Creating a Boolean variable to insert data into the database.
                    isSuccess = pointOfPurchaseDAL.Insert(pointOfPurchaseBLL);
                }


                //If the data is inserted successfully, then the value of the variable isSuccess will be true; otherwise it will be false.
                if (isSuccess == true && isSuccessDetail == true)//IsSuccessDetail is always CHANGING in every loop above! IMPROVE THIS!!!!
                {
                    //ClearBasketTextBox();
                    //ClearPointOfSaleListView();
                    ClearProductEntranceTextBox();
                    DisableButtonsTools();
                    EnableButtonsOnClickSaveCancel();
                }
                else
                {
                    MessageBox.Show("Something went wrong :(");
                }
            }

            else
            {
                MessageBox.Show("You have a missing part!");
            }
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

        private void ClearPointOfSaleListView()
        {
            dgProducts.Items.Clear();
        }

        private void ClearProductEntranceTextBox()
        {
            txtProductBarcode.Text = "";
            txtProductName.Text = "";
            cboProductUnit.SelectedIndex = -1;
            txtProductCostPrice.Text = "";
            txtProductAmount.Text = "";
            txtProductTotalCostPrice.Text = "";
            Keyboard.Focus(txtProductBarcode); // set keyboard focus
            DisableProductEntranceButtons();
        }

        private void txtProductBarcode_KeyUp(object sender, KeyEventArgs e)
        {
            int number;

            DataTable dataTable = productDAL.SearchSpecificProductById(txtProductBarcode.Text);

            if (txtProductBarcode.Text != 0.ToString() && int.TryParse(txtProductBarcode.Text, out number) && dataTable.Rows.Count != 0)//Validating the barcode if it is a number(except zero) or not.
            {
                int productAmount = 1;
                int rowIndex = 0;
                int productId;
                int productUnit = 0;
                string productBarcodeRetail/*, productBarcodeWholesale*/;

                btnProductAdd.IsEnabled = true; //Enabling the add button if any valid barcode is entered.
                btnProductClear.IsEnabled = true;//Enabling the clear button if any valid barcode is entered.


                productId = Convert.ToInt32(dataTable.Rows[rowIndex]["id"]);
                productBarcodeRetail = dataTable.Rows[rowIndex]["barcode_retail"].ToString();
                //productBarcodeWholesale = dataTable.Rows[rowIndex]["barcode_wholesale"].ToString();


                if (productBarcodeRetail == txtProductBarcode.Text || productId.ToString() == txtProductBarcode.Text)//If the barcode equals the product's barcode_retail or id, then take the product's retail unit id.
                {
                    productUnit = Convert.ToInt32(dataTable.Rows[rowIndex]["unit_retail"]);
                }

                else //If the barcode equals to the barcode_wholesale, then take the product's wholesale unit id.
                {
                    productUnit = Convert.ToInt32(dataTable.Rows[rowIndex]["unit_wholesale"]);
                }

                txtProductName.Text = dataTable.Rows[rowIndex]["name"].ToString();//Filling the product name textbox from the database

                DataTable dataTableUnit = unitDAL.GetUnitInfoById(productUnit);//Datatable for finding the unit name by unit id.

                cboProductUnit.Items.Add(dataTableUnit.Rows[rowIndex]["name"].ToString());//Populating the combobox with related unit names from dataTableUnit.
                cboProductUnit.SelectedIndex = 0;//For selecting the combobox's first element. We selected 0 index because we have just one unit of a retail product.

                string costPrice = dataTable.Rows[rowIndex]["costprice"].ToString();

                txtProductCostPrice.Text = costPrice;
                txtProductAmount.Text = productAmount.ToString();
                txtProductTotalCostPrice.Text = (Convert.ToDecimal(costPrice) * productAmount).ToString();
            }

            //if (Keyboard.IsKeyDown(Key.Tab))//PLEASE TRY TO TRIG THE TAB BUTTON!!!
            //{
            //    if (verifyBarcode!=true)
            //    {
            //        MessageBox.Show("You have entered a wrong barcode");
            //    }
            //}


            //If the txtProductBarcode is empty which means user has clicked the backspace button and if the txtProductName is filled once before, then erase all the text contents.
            /*Note: I just checked the btnProductAdd to know if there was a product entry before or not.
                    If the btnProductAdd is not enabled in the if block above once before, then no need to call the method ClearProductEntranceTextBox.*/
            else if (txtProductBarcode.Text == "" && btnProductAdd.IsEnabled == true)
            {
                ClearProductEntranceTextBox();
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

                if (barcodeCellContent.Text == txtProductBarcode.Text)
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

                dgProducts.Items.Add(new { Barcode = txtProductBarcode.Text, Name = txtProductName.Text, Unit = cboProductUnit.SelectedItem, CostPrice = txtProductCostPrice.Text, Amount = txtProductAmount.Text, TotalCostPrice = totalCostPrice.ToString()});
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
            TextBlock tbTotalCostCell;
            TextBlock tbAmountCell;
            TextBlock tbTotalPriceCell;
            int productTotalCostCol = 6;
            int productAmountCol = 5;
            int productTotalPriceCol = 7;

            dataGridRow = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(selectedRowIndex);

            tbAmountCell = dgProducts.Columns[productAmountCol].GetCellContent(dataGridRow) as TextBlock;

            tbTotalCostCell = dgProducts.Columns[productTotalCostCol].GetCellContent(dataGridRow) as TextBlock;    //Try to understand this code!!!  

            tbTotalPriceCell = dgProducts.Columns[productTotalPriceCol].GetCellContent(dataGridRow) as TextBlock;    //Try to understand this code!!!  


            txtBasketAmount.Text = (Convert.ToDecimal(txtBasketAmount.Text) - Convert.ToDecimal(tbAmountCell.Text)).ToString();

            txtBasketCostTotal.Text = (Convert.ToDecimal(txtBasketCostTotal.Text) - Convert.ToDecimal(tbTotalCostCell.Text)).ToString();

            txtBasketSubTotal.Text = (Convert.ToDecimal(txtBasketSubTotal.Text) - Convert.ToDecimal(tbTotalPriceCell.Text)).ToString();

            txtBasketGrandTotal.Text = (Convert.ToDecimal(txtBasketSubTotal.Text) + Convert.ToDecimal(txtBasketVat.Text) - Convert.ToDecimal(txtBasketDiscount.Text)).ToString();
        }

        private void btnProductClear_Click(object sender, RoutedEventArgs e)
        {
            ClearProductEntranceTextBox();

        }

        private void cboSupplier_Loaded(object sender, RoutedEventArgs e)
        {
            //Creating Data Table to hold the products from Database
            DataTable dataTable = customerDAL.Select();

            //Specifying Items Source for product combobox
            cboSupplier.ItemsSource = dataTable.DefaultView;

            //Here DisplayMemberPath helps to display Text in the ComboBox.
            cboSupplier.DisplayMemberPath = "name";

            //SelectedValuePath helps to store values like a hidden field.
            cboSupplier.SelectedValuePath = "id";
        }

        private void LoadNewInvoice()
        {
            ClearBasketTextBox();
            ClearPointOfSaleListView();

            //int invoiceNo, increment = 1;

            //invoiceNo = GetLastInvoiceNumber();//Getting the last invoice number and assign it to the variable called invoiceNo.
            //invoiceNo += increment;//We are adding one to the last invoice number because every new invoice number is one greater tham the previous invoice number.
            //txtInvoiceNo.Text = invoiceNo.ToString();//Assigning invoiceNo to the content of the InvoiceNo Label.
        }

        private int GetLastInvoiceNumber()
        {
            int specificRowIndex = 0, invoiceNo;

            DataTable dataTable = pointOfPurchaseDAL.Search();//Searching the last id number in the tbl_pop which actually stands for the current invoice number to save it to tbl_pop_details as an invoice number for this sale.

            if (dataTable.Rows.Count != 0)//If there is an invoice number in the database, that means the datatable's first row cannot be null, and the datatable's first index is 0.
            {
                invoiceNo = Convert.ToInt32(dataTable.Rows[specificRowIndex]["id"]);//We defined this code out of the for loop below because all of the products has the same invoice number in every sale. So, no need to call this method for every products again and again.
            }
            else//If there is no any invoice number, that means it is the first sale. So, assing invoiceNo with 0;
            {
                invoiceNo = 0;
            }
            return invoiceNo;
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            btnNewOrEdit = 0;//0 stands for the user has entered the btnNew.
            LoadNewInvoice();
            EnteredBtnNewOrEdit();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            btnNewOrEdit = 1;//1 stands for the user has entered the btnEdit.
            EnteredBtnNewOrEdit();
        }

        private void EnteredBtnNewOrEdit()//Do NOT repeat yourself! You have used IsEnabled function for these toolbox contents many times!
        {
            btnNew.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnPrint.IsEnabled = true;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            cboPurchaseType.IsEnabled = true;
            cboSupplier.IsEnabled = true;
            cboProductUnit.IsEnabled = true;
            txtProductBarcode.IsEnabled = true;
            txtProductName.IsEnabled = true;
            txtProductCostPrice.IsEnabled = true;
            txtProductAmount.IsEnabled = true;
            txtProductTotalCostPrice.IsEnabled = true;
            txtInvoiceNo.IsEnabled = true;
            dgProducts.IsHitTestVisible = true;//Enabling the datagrid clicking.

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Would you really like to cancel the invoice, you piece of shit?", "Cancel Invoice", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    DisableButtonsTools();
                    ClearProductEntranceTextBox();
                    ClearPointOfSaleListView();
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

        int invoiceArrow;
        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            int firstInvoiceNo = 0, currentInvoiceNo = Convert.ToInt32(txtInvoiceNo.Text);

            if (currentInvoiceNo != firstInvoiceNo)
            {
                ClearPointOfSaleListView();
                int prevInvoice = Convert.ToInt32(txtInvoiceNo.Text) - 1;
                invoiceArrow = 0;//0 means customer has clicked the previous button.
                LoadPastInvoice(prevInvoice, invoiceArrow);
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            int lastInvoiceNo = GetLastInvoiceNumber(), currentInvoiceNo = Convert.ToInt32(txtInvoiceNo.Text);

            if (currentInvoiceNo != lastInvoiceNo)
            {
                ClearPointOfSaleListView();
                int nextInvoice = Convert.ToInt32(txtInvoiceNo.Text) + 1;
                invoiceArrow = 1;//1 means customer has clicked the next button.
                LoadPastInvoice(nextInvoice, invoiceArrow);
            }
        }

        private void btnDeleteListRow_Click(object sender, RoutedEventArgs e)
        {
            var selectedRow = dgProducts.SelectedItem;
            int selectedRowIndex = dgProducts.SelectedIndex;

            if (selectedRow != null)
            {
                SubstractBasket(selectedRowIndex);

                dgProducts.Items.Remove(selectedRow);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Would you really like to delete the invoice, you piece of shit?", "Delete Invoice", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:

                    int invoiceNo = Convert.ToInt32(txtInvoiceNo.Text); //GetLastInvoiceNumber(); You can also call this method and add number 1 to get the current invoice number, but getting the ready value is faster than getting the last invoice number from the database and adding a number to it to get the current invoice number.

                    pointOfPurchaseBLL.InvoiceNo = invoiceNo;//Assigning the invoice number into the Id in the pointofSaleBLL.
                    pointOfPurchaseDetailBLL.InvoiceNo = invoiceNo;

                    pointOfPurchaseDAL.Delete(pointOfPurchaseBLL);
                    pointOfPurchaseDetailDAL.Delete(pointOfPurchaseDetailBLL);

                    DisableButtonsTools();
                    ClearProductEntranceTextBox();
                    ClearPointOfSaleListView();
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

        private void txtInvoiceNo_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtInvoiceNo.Text=="0")
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

        private void txtProductAmount_TextChanged(object sender, TextChangedEventArgs e)/*----THIS IS NOT A PRODUCTIVE CODE----*/
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
                        DataTable dataTable = productDAL.SearchSpecificProductById(txtProductBarcode.Text);

                        string unitKg = "Kilogram", unitLt = "Liter";
                        int rowIndex = 0;
                        decimal productAmount;
                        string productCostPrice = dataTable.Rows[rowIndex]["costprice"].ToString();

                        if (cboProductUnit.Text != unitKg && cboProductUnit.Text != unitLt)
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

                        txtProductTotalCostPrice.Text = (Convert.ToDecimal(productCostPrice) * productAmount).ToString();
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
