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
    /// Interaction logic for WinDeposit.xaml
    /// </summary>
    public partial class WinDeposit : Window
    {
        DepositBLL depositBLL = new DepositBLL();
        DepositCUL depositCUL = new DepositCUL();
        UserBLL userBLL = new UserBLL();
        BankDAL bankDAL = new BankDAL();
        AccountDAL accountDAL = new AccountDAL();
        public WinDeposit()
        {
            InitializeComponent();
            LoadUserInformations();
            DisableTools();
        }

        int depositArrow;
        int btnNewOrEdit;//0 stands for user clicked the button New, and 1 stands for user clicked the button Edit.
        string[,] dgOldProductCells = new string[,] { };

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void cboBankName_Loaded(object sender, RoutedEventArgs e)
        {
            //Creating Data Table to hold the products from Database
            DataTable dataTable = bankDAL.Select();

            //Specifying Items Source for product combobox
            cboEntranceBankName.ItemsSource = dataTable.DefaultView;

            //Here DisplayMemberPath helps to display Text in the ComboBox.
            cboEntranceBankName.DisplayMemberPath = "name";

            //SelectedValuePath helps to store values like a hidden field.
            cboEntranceBankName.SelectedValuePath = "id";
        }

        private void txtId_KeyUp(object sender, KeyEventArgs e)
        {
            int rowIndex = 0;

            DataTable dtBank = bankDAL.SearchById(Convert.ToInt32(txtEntranceBankId.Text));

            if (dtBank.Rows.Count!=rowIndex)//If there is a data in the db, there cannot be a datatable with index of 0.
            {
                cboEntranceBankName.SelectedValue = Convert.ToInt32(dtBank.Rows[rowIndex]["id"]);
            }
            else
            {
                MessageBox.Show("There is no such item!");
            }
        }

        private void cboBankName_KeyUp(object sender, KeyEventArgs e)
        {
            txtEntranceBankId.Text = cboEntranceBankName.SelectedValue.ToString();
        }

        private void btnEntranceEnter_Click(object sender, RoutedEventArgs e)
        {
            if (txtEntranceBankId.Text != "" && txtEntranceAmount.Text != "")
            {
                bool addNewProductLine = true;
                int colId = 0, colAmount = 3;
                int amount = 0;
                int rowQuntity = dgDeposits.Items.Count;

                for (int i = 0; i < rowQuntity; i++)
                {
                    DataGridRow row = (DataGridRow)dgDeposits.ItemContainerGenerator.ContainerFromIndex(i);

                    TextBlock barcodeCellContent = dgDeposits.Columns[colId].GetCellContent(row) as TextBlock;    //Try to understand this code!!!  

                    if (barcodeCellContent.Text == txtEntranceBankId.Text)
                    {
                        if (MessageBox.Show("There is already the same item in the list. Would you like to sum them?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            TextBlock tbCellAmountContent = dgDeposits.Columns[colAmount].GetCellContent(row) as TextBlock;    //Try to understand this code!!!                         

                            amount = Convert.ToInt32(tbCellAmountContent.Text);
                            amount += Convert.ToInt32(txtEntranceAmount.Text);//We are adding the amount entered in the "txtProductAmount" to the previous amount cell's amount.

                            tbCellAmountContent.Text = amount.ToString();//Assignment of the new amount to the related cell.
                            addNewProductLine = false;
                            break;//We have to break the loop if the user clicked "yes" because no need to scan the rest of the rows after confirming.
                        }
                    }
                }

                if (addNewProductLine == true)//Use ENUMS instead of this!!!!!!!
                {
                    dgDeposits.Items.Add(new { Id = txtEntranceBankId.Text, BankName = cboEntranceBankName.Text, Description = txtEntranceDescription.Text, Amount = txtEntranceAmount.Text });
                }

                dgDeposits.UpdateLayout();

                PopulateSummary();

                ClearEntrance();
            }

            else
                MessageBox.Show("You have a missing part!");
        }

        private void btnMenuNew_Click(object sender, RoutedEventArgs e)
        {
            ModifyToolsOnClickBtnMenuNew();
        }

        private void LoadUserInformations()
        {
            txtUsername.Text = WinLogin.loggedInUserName;
            txtUserType.Text = WinLogin.loggedInUserType;
        }

        private void DisableTools()
        {
            btnMenuSave.IsEnabled = false;
            btnMenuCancel.IsEnabled = false;
            cboMenuAccount.IsEnabled = false;
            txtEntranceBankId.IsEnabled = false;
            txtEntranceDescription.IsEnabled = false;
            txtEntranceAmount.IsEnabled = false;
            cboEntranceBankName.IsEnabled = false;
            btnEntranceEnter.IsEnabled = false;
            btnEntranceClear.IsEnabled = false;
            dgDeposits.IsEnabled = false;
        }

        private void ModifyToolsOnClickBtnMenuNew()
        {
            btnMenuEdit.IsEnabled = false;//Edit button should be disabled while entering a new deposit.
            btnMenuDelete.IsEnabled = false;//Delete button should be disabled while entering a new deposit.
            
            btnMenuSave.IsEnabled = true;
            btnMenuCancel.IsEnabled = true;
            cboMenuAccount.IsEnabled = true;
            txtEntranceBankId.IsEnabled = true;
            txtEntranceDescription.IsEnabled = true;
            txtEntranceAmount.IsEnabled = true;
            cboEntranceBankName.IsEnabled = true;
            btnEntranceEnter.IsEnabled = true;
            btnEntranceClear.IsEnabled = true;
            dgDeposits.IsEnabled = true;
        }

        private void ClearEntrance()
        {
            txtEntranceBankId.Text = "";
            txtEntranceDescription.Text = "";
            txtEntranceAmount.Text = "";
            cboEntranceBankName.Text = "";
        }

        private void PopulateSummary()
        {
            decimal amount = Convert.ToDecimal(txtEntranceAmount.Text);

            txtTotalAmount.Text = (Convert.ToDecimal(txtTotalAmount.Text) + amount).ToString();
        }

        private string[,] GetDataGridContent()
        {
            int rowLength = dgDeposits.Items.Count;
            int colLength = 8;
            string[,] dgProductCells = new string[rowLength, colLength];

            for (int rowIndex = 0; rowIndex < rowLength; rowIndex++)
            {
                DataGridRow dgRow = (DataGridRow)dgDeposits.ItemContainerGenerator.ContainerFromIndex(rowIndex);

                for (int colNo = 0; colNo < colLength; colNo++)
                {
                    TextBlock tbCellContent = dgDeposits.Columns[colNo].GetCellContent(dgRow) as TextBlock;

                    dgProductCells[rowIndex, colNo] = tbCellContent.Text;
                }
            }

            return dgProductCells;
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

        private void txtEntranceAmount_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtEntranceAmount.Text != "")
            {
                string amount = txtEntranceAmount.Text;
                char lastCharacter = char.Parse(amount.Substring(amount.Length - 1));//Getting the last character to check if the user has entered a missing amount like " 3, ".
                bool isValidAmount = Char.IsDigit(lastCharacter);//Checking if the last digit of the number is a number or not.
                bool isNumeric = int.TryParse(amount, out _);

                if (isNumeric != true && isValidAmount != true)
                {
                    MessageBox.Show("Please enter a valid number");
                    txtEntranceAmount.Text = "";
                    //Keyboard.Focus(txtEntranceAmount); // set keyboard focus
                }
            }
        }

        private void btnMenuSave_Click(object sender, RoutedEventArgs e)
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
            if (isDgEqual == false &&  cboMenuAccount.SelectedIndex != emptyIndex)
            {
                int userClickedNewOrEdit = btnNewOrEdit;
                int userId = userBLL.GetUserId(WinLogin.loggedInUserName);
                bool isSuccess = false;

                DataTable dataTableLastInvoice = depositBLL.GetLastDepositInfo();//Getting the last invoice number and assign it to the variable called invoiceId.
                DataTable dataTableProduct = new DataTable();
                DataTable dataTableUnit = new DataTable();

                #region TABLE DEPOSIT SAVING SECTION
                //Getting the values from the POS Window and fill them into the depositCUL.
                depositCUL.Id = Convert.ToInt32(lblDepositNumber.Content);
                depositCUL.AccountId = Convert.ToInt32(cboMenuAccount.SelectedValue);
                depositCUL.BankId = Convert.ToInt32(cboMenuBank.SelectedValue);
                depositCUL.Amount = Convert.ToDecimal(txtTotalAmount.Text);
                depositCUL.AddedDate = DateTime.Now;
                depositCUL.AddedBy = userId;

                userClickedNewOrEdit = btnNewOrEdit;// We are reassigning the btnNewOrEdit value into userClickedNewOrEdit.

                if (userClickedNewOrEdit == 1)//If the user clicked the btnEdit, then update the specific invoice information in tbl_pos at once.
                {
                    isSuccess = depositBLL.UpdateDeposit(depositCUL);
                }

                else
                {
                    //Creating a Boolean variable to insert data into the database.
                    isSuccess = depositBLL.InsertDeposit(depositCUL);
                }
                #endregion

                #region TABLE DEPOSIT DETAILS SAVING SECTION
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
                int productRate = 0;//Modify this code dynamically!!!!!!!!!

                for (int rowNo = 0; rowNo < dgProducts.Items.Count; rowNo++)
                {
                    if (userClickedNewOrEdit == 1)//If the user clicked the btnEdit, then edit the specific invoice's products in tbl_pos_detailed at once.
                    {
                        productBLL.RevertOldAmountInStock(dgOldProductCells, dgProducts.Items.Count, calledBy);//Reverting the old products' amount in stock.

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

                    depositDetailCUL.Id = invoiceId;
                    depositDetailCUL.ProductId = productId;
                    depositDetailCUL.AddedBy = addedBy;
                    depositDetailCUL.ProductRate = productRate;
                    depositDetailCUL.ProductUnitId = unitId;
                    depositDetailCUL.ProductCostPrice = Convert.ToDecimal(cells[cellCostPrice]);//cells[3] contains cost price of the product in the list.
                    depositDetailCUL.ProductSalePrice = Convert.ToDecimal(cells[cellSalePrice]);//cells[4] contains sale price of the product in the list.
                    depositDetailCUL.ProductAmount = Convert.ToDecimal(cells[cellProductAmount]);

                    isSuccessDetail = pointOfSaleDetailDAL.Insert(depositDetailCUL);

                    #region PRODUCT AMOUNT UPDATE
                    productOldAmountInStock = Convert.ToDecimal(dataTableProduct.Rows[initialRowIndex]["amount_in_stock"].ToString());//Getting the old product amount in stock.

                    productCUL.AmountInStock = productOldAmountInStock - Convert.ToDecimal(cells[cellProductAmount]);

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
    }
}
