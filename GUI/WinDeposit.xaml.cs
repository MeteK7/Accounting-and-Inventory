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
        DepositDAL depositDAL = new DepositDAL();
        DepositCUL depositCUL = new DepositCUL();
        DepositDetailDAL depositDetailDAL = new DepositDetailDAL();
        DepositDetailCUL depositDetailCUL = new DepositDetailCUL();
        UserBLL userBLL = new UserBLL();
        BankDAL bankDAL = new BankDAL();
        AccountDAL accountDAL = new AccountDAL();
        public WinDeposit()
        {
            InitializeComponent();
            LoadUserInformations();
            LoadPastRecord();
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
        private void ClearSummary()
        {
            txtSummaryTotalAmount.Text = "0";
        }

        private void ClearDepositsDataGrid()
        {
            dgDeposits.Items.Clear();
        }
        private void LoadNewInvoice()/*INVOICE NUMBER REFERS TO THE ID NUMBER IN THE DATABASE FOR POINT OF SALE.*/
        {
            ClearSummary();
            ClearDepositsDataGrid();

            int depositId, increment = 1;

            depositId = depositBLL.GetLastDepositId();//Getting the last invoice number and assign it to the variable called invoiceNo.
            depositId += increment;//We are adding one to the last invoice number because every new invoice number is one greater tham the previous invoice number.
            lblDepositNumber.Content = depositId;//Assigning invoiceNo to the content of the InvoiceNo Label.
        }

        private void btnMenuNew_Click(object sender, RoutedEventArgs e)
        {
            btnNewOrEdit = 0;//0 stands for the user has entered the btnNew.
            LoadNewInvoice();
            ToolsOnClickBtnNewEdit();
        }

        private void LoadUserInformations()
        {
            txtUsername.Text = WinLogin.loggedInUserName;
            txtUserType.Text = WinLogin.loggedInUserType;
        }

        private void LoadPastRecord(int depositId = 0, int depositArrow = -1)
        {
            int firstRowIndex = 0, bankId;
            string bankName, depositDescription, depositAmount;

            if (depositId == 0)
            {
                depositId = depositBLL.GetLastDepositId();//Getting the last deposit id and assign it to the variable called depositId.
            }

            /*WE CANNOT USE ELSE IF FOR THE CODE BELOW! BOTH IF STATEMENTS ABOVE AND BELOVE MUST WORK.*/
            if (depositId != 0)// If the invoice number is still 0 even when we get the last invoice number by using code above, that means this is the first sale and do not run this code block.
            {
                DataTable dataTableDeposit = depositDAL.GetByIdOrLastId(depositId);

                if (dataTableDeposit.Rows.Count != 0)
                {
                    DataTable dataTableDepositDetail = depositDetailDAL.SearchById(depositId);
                    DataTable dataTableBankInfo;

                    cboMenuAccount.SelectedValue = Convert.ToInt32(dataTableDeposit.Rows[firstRowIndex]["account_id"].ToString());//Getting the id of account.
                    lblDepositNumber.Content = dataTableDeposit.Rows[firstRowIndex]["id"].ToString();

                    #region LOADING THE DEPOSIT DATA GRID
                    for (int currentRow = firstRowIndex; currentRow < dataTableDepositDetail.Rows.Count; currentRow++)
                    {
                        bankId = Convert.ToInt32(dataTableDepositDetail.Rows[currentRow]["bank_id"]);

                        dataTableBankInfo = bankDAL.SearchById(bankId);//Getting the bank name by bank id.
                        bankName = dataTableBankInfo.Rows[firstRowIndex]["name"].ToString();//We use firstRowIndex value for the index number in every loop because there can be only one bank name of a specific id.
                        
                        depositDescription= dataTableDepositDetail.Rows[currentRow]["description"].ToString();
                        depositAmount = dataTableDepositDetail.Rows[currentRow]["amount"].ToString();

                        dgDeposits.Items.Add(new { BankId = bankId, BankName = bankName, Description = depositDescription, Amount = depositAmount});
                    }
                    #endregion

                    #region FILLING THE PREVIOUS SUMMARY INFORMATIONS
                    txtSummaryTotalAmount.Text = dataTableDeposit.Rows[firstRowIndex]["total_amount"].ToString();
                    #endregion
                }

                else if (dataTableDeposit.Rows.Count == 0)//If the pos detail row quantity is 0, that means there is no such row so decrease or increase the invoice number according to user preference.
                {
                    if (depositArrow == 0)//If the invoice arrow is 0, that means user clicked the previous button.
                    {
                        depositId = depositId - 1;
                    }
                    else
                    {
                        depositId = depositId + 1;
                    }

                    if (depositArrow != -1)//If the user has not clicked either previous or next button, then the invoiceArrow will be -1 and no need for recursion.
                    {
                        LoadPastRecord(depositId, depositArrow);//Call the method again to get the new past invoice.
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
            MessageBox.Show("Welcome!\n Thank you for choosing Kaba Accounting and Inventory System.");
            btnPrev.IsEnabled = false;//Disabling the btnPrev button because there is no any records in the database for the first time.
            btnNext.IsEnabled = false;//Disabling the btnNext button because there is no any records in the database for the first time.
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

        private void ClearDepositEntrance()
        {
            txtEntranceBankId.Text = "";
            cboEntranceBankName.SelectedItem = "";
            txtEntranceDescription.Text = "";
            txtEntranceAmount.Text = "";
        }

        private void EnableButtonsOnClickSaveCancel()
        {
            btnMenuNew.IsEnabled = true;//If the deposits are saved successfully, enable the new button to be able to add new products.
            btnMenuEdit.IsEnabled = true;//If the deposits are saved successfully, enable the edit button to be able to edit an existing invoice.
            btnMenuDelete.IsEnabled = true;
            btnPrev.IsEnabled = true;
            btnNext.IsEnabled = true;
        }

        private void ToolsOnClickBtnNewEdit()
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

            txtSummaryTotalAmount.Text = (Convert.ToDecimal(txtSummaryTotalAmount.Text) + amount).ToString();
        }

        private string[,] GetDataGridContent()
        {
            int rowLength = dgDeposits.Items.Count;
            int colLength = 4;
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

            //If the old datagrid equals new datagrid, no need for saving because the user did not change anything.(ONLY VALID IN CASE OF CLICKING TO THE EDIT BUTTON!!!)
            //-1 means nothing has been chosen in the combobox. Note: We don't add the --&& lblInvoiceNo.Content.ToString()!= "0"-- into the if statement because the invoice label cannot be 0 due to the restrictions.
            if (isDgEqual == false &&  cboMenuAccount.SelectedIndex != emptyIndex)
            {
                int userClickedNewOrEdit = btnNewOrEdit;
                int depositId = Convert.ToInt32(lblDepositNumber.Content);
                int userId = userBLL.GetUserId(WinLogin.loggedInUserName);
                bool isSuccess = false;

                //DataTable dataTableLastDeposit = depositBLL.GetLastDepositInfo();//Getting the last invoice number and assign it to the variable called invoiceId.
                DataTable dataTableProduct = new DataTable();
                DataTable dataTableUnit = new DataTable();

                #region TABLE DEPOSIT SAVING SECTION
                //Getting the values from the POS Window and fill them into the depositCUL.
                depositCUL.Id = Convert.ToInt32(depositId);
                depositCUL.AccountId = Convert.ToInt32(cboMenuAccount.SelectedValue);
                depositCUL.TotalAmount = Convert.ToDecimal(txtSummaryTotalAmount.Text);
                depositCUL.AddedDate = DateTime.Now;
                depositCUL.AddedBy = userId;

                //userClickedNewOrEdit = btnNewOrEdit;// We are reassigning the btnNewOrEdit value into userClickedNewOrEdit.

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
                int cellBankId = 0, cellDescription = 2, cellAmount = 3;
                int colLength = 4;
                string[] cells = new string[colLength];
                DateTime dateTime = DateTime.Now;
                bool isSuccessDetail = false;

                for (int rowNo = 0; rowNo < dgDeposits.Items.Count; rowNo++)
                {
                    if (userClickedNewOrEdit == 1)//If the user clicked the btnEdit, then edit the specific deposit's items in tbl_deposit_detailed at once.
                    {
                        //We are sending deposit id as a parameter to the "Delete" Method. So that we can erase all the entrances which have the specific deposit id.
                        depositDetailDAL.Delete(depositId);

                        //2 means null for this code. We used this in order to prevent running the if block again and again. Because, we erase all of the products belong to one invoice number at once.
                        userClickedNewOrEdit = 2;
                    }

                    DataGridRow row = (DataGridRow)dgDeposits.ItemContainerGenerator.ContainerFromIndex(rowNo);

                    for (int colNo = 0; colNo < colLength; colNo++)
                    {
                        TextBlock cellContent = dgDeposits.Columns[colNo].GetCellContent(row) as TextBlock;

                        cells[colNo] = cellContent.Text;
                    }

                    depositDetailCUL.Id = depositId;
                    depositDetailCUL.BankId = cells[cellBankId];
                    depositDetailCUL.Description = cells[cellDescription];
                    depositDetailCUL.Amount = Convert.ToDecimal(cells[cellAmount]);

                    isSuccessDetail = depositDetailDAL.Insert(depositDetailCUL);
                }
                #endregion

                //If the data is inserted successfully, then the value of the variable isSuccess will be true; otherwise it will be false.
                if (isSuccess == true && isSuccessDetail == true)//IsSuccessDetail is always CHANGING in every loop above! IMPROVE THIS!!!!
                {
                    //ClearBasketTextBox();
                    //ClearPointOfSaleDataGrid();
                    ClearDepositEntrance();
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

        private void btnMenuCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Would you really like to cancel the deposit?", "Cancel Deposit", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    DisableTools();
                    ClearDepositEntrance();
                    ClearDepositsDataGrid();
                    LoadPastRecord();
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

        private void btnMenuEdit_Click(object sender, RoutedEventArgs e)
        {
            btnNewOrEdit = 1;//1 stands for the user has entered the btnEdit.
            dgOldProductCells = (string[,])(GetDataGridContent().Clone());//Cloning one array into another array.
            ToolsOnClickBtnNewEdit();
        }

        private void btnMenuDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Would you really like to delete the invoice?", "Delete Invoice", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:

                    #region DELETE DEPOSIT
                    int depositId = Convert.ToInt32(lblDepositNumber.Content);

                    depositDAL.Delete(depositId);
                    depositDetailDAL.Delete(depositId);
                    #endregion

                    #region PREPARE TO THE LAST PAGE
                    DisableTools();
                    ClearDepositEntrance();
                    ClearDepositsDataGrid();
                    LoadPastRecord();
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
    }
}
