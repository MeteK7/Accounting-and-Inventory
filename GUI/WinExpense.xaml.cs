using BLL;
using CUL;
using DAL;
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
    /// Interaction logic for WinExpense.xaml
    /// </summary>
    public partial class WinExpense : Window
    {
        AccountDAL accountDAL = new AccountDAL();
        SupplierDAL supplierDAL = new SupplierDAL();
        UserBLL userBLL = new UserBLL();
        ExpenseCUL expenseCUL = new ExpenseCUL();
        ExpenseBLL expenseBLL = new ExpenseBLL();
        ExpenseDAL expenseDAL = new ExpenseDAL();
        AssetCUL assetCUL = new AssetCUL();
        AssetDAL assetDAL = new AssetDAL();
        BankDAL bankDAL = new BankDAL();
        CommonBLL commonBLL = new CommonBLL();

        int unitValue = 1;
        int clickedNewOrEdit, clickedNew=0,clickedEdit=1;//0 stands for user clicked the button New, and 1 stands for user clicked the button Edit.
        int account = 1, bank = 2, supplier = 3;
        bool isCboSelectionDisabled = false; 
        int clickedArrow,  clickedPrev = 0, clickedNext = 1;
        string calledBy = "WinExpense";

        public WinExpense()
        {
            InitializeComponent();
            DisableTools();
            LoadPastExpense();
        }
        private void FirstTimeRun()
        {
            MessageBox.Show("Welcome!\n Thank you for choosing Kaba Accounting and Inventory System.");
            btnPrev.IsEnabled = false;//Disabling the btnPrev button because there is no any records in the database for the first time.
            btnNext.IsEnabled = false;//Disabling the btnNext button because there is no any records in the database for the first time.
        }

        //-1 means user did not clicked either previous or next button which means user just clicked the point of purchase button to open it.
        private void LoadPastExpense(int expenseId = 0, int expenseArrow = -1)//Optional parameter
        {
            int initalIndex = 0,idAssetFrom;

            if (expenseId == initalIndex)//If the ID is 0 came from the optional parameter, that means user just clicked the WinPOP button to open it.
            {
                expenseId = commonBLL.GetLastRecordById(calledBy);//Getting the last invoice id and assign it to the variable called invoiceId.
            }

            /*WE CANNOT USE ELSE IF FOR THE CODE BELOW! BOTH IF STATEMENTS ABOVE AND BELOVE MUST WORK.*/
            if (expenseId != initalIndex)// If the invoice number is still 0 even when we get the last invoice number by using code above, that means this is the first sale and do not run this code block.
            {
                DataTable dtExpense = expenseDAL.GetByExpenseId(expenseId);

                if (dtExpense.Rows.Count != initalIndex)
                {
                    #region ASSET INFORMATION FILLING REGION
                    idAssetFrom = Convert.ToInt32(dtExpense.Rows[initalIndex]["id_asset_from"].ToString());
                    //lblAssetIdFrom.Content = idAssetFrom;

                    DataTable dtAsset = assetDAL.SearchById(idAssetFrom);
                    int sourceType = Convert.ToInt32(dtAsset.Rows[initalIndex]["id_source_type"]);

                    if (sourceType == account)
                        rbAccount.IsChecked = true;
                    else
                        rbBank.IsChecked = true;

                    LoadCboFrom(sourceType);//This function works twice when you open the WinExpense because the rb selection is being changed. But if the previous selection is same, rbSelection change does not work so the LoadCboFrom method does not work as well.

                    cboFrom.SelectedValue = dtAsset.Rows[initalIndex]["id_source"].ToString();
                    #endregion

                    expenseId = Convert.ToInt32(dtExpense.Rows[initalIndex]["id"].ToString());//Getting the id of account.
                    lblExpenseId.Content = expenseId;

                    LoadCboTo();

                    cboTo.SelectedValue = Convert.ToInt32(dtExpense.Rows[initalIndex]["id_to"].ToString());//Getting the id of supplier.

                    txtAmount.Text= dtExpense.Rows[initalIndex]["amount"].ToString();
                }
                else if (dtExpense.Rows.Count == initalIndex)//If the pop detail row quantity is 0, that means there is no such row so decrease or increase the invoice number according to user preference.
                {
                    if (expenseArrow == 0)//If the invoice arrow is 0, that means user clicked the previous button.
                    {
                        expenseId = expenseId - 1;
                    }
                    else
                    {
                        expenseId = expenseId + 1;
                    }

                    if (expenseArrow != -1)//If the user has not clicked either previous or next button, then the invoiceArrow will be -1 and no need for recursion.
                    {
                        LoadPastExpense(expenseId, expenseArrow);//Call the method again to get the new past invoice.
                    }

                }
            }
            else
            {
                FirstTimeRun();//This method is called when it is the first time of using this program.
            }
        }

        public void DisableTools()
        {
            btnMenuSave.IsEnabled = false;
            btnMenuCancel.IsEnabled = false;
            cboFrom.IsEnabled = false;
            cboTo.IsEnabled = false;
            txtAmount.IsEnabled = false;
            rbAccount.IsEnabled = false;
            rbBank.IsEnabled = false;

        }

        private void EnableButtonsOnClickSaveCancel()
        {
            btnMenuNew.IsEnabled = true;
            btnMenuEdit.IsEnabled = true;
            btnMenuDelete.IsEnabled = true;
            btnPrev.IsEnabled = true;
            btnNext.IsEnabled = true;
        }

        private void ModifyToolsOnClickBtnNewEdit()
        {
            btnMenuSave.IsEnabled = true;
            btnMenuCancel.IsEnabled = true;
            btnMenuNew.IsEnabled = false;
            btnMenuEdit.IsEnabled = false;
            btnMenuDelete.IsEnabled = false;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            cboTo.IsEnabled = true;
            txtAmount.IsEnabled = true;
            rbAccount.IsEnabled = true;
            rbBank.IsEnabled = true;

            if (clickedNewOrEdit == clickedNew)
            {
                txtAmount.Text = "";
                lblAssetIdFrom.Content = "";
                lblAssetIdTo.Content = "";
                lblBalanceFrom.Content = "";
                lblBalanceTo.Content = "";
                rbAccount.IsChecked = false;
                rbBank.IsChecked = false;
                cboFrom.IsEnabled = true;
            }
        }

        private void LoadNewExpense()
        {
            //ClearBasketTextBox();
            //ClearProductsDataGrid();

            int expenseNo, increment = 1;

            expenseNo = expenseBLL.GetLastExpenseNumber();//Getting the last invoice number and assign it to the variable called expenseNo.
            expenseNo += increment;//We are adding one to the last expense number because every new expense number is one greater tham the previous expense number.
            lblExpenseId.Content = expenseNo;//Assigning expenseNo to the content of the expenseNo Label.
        }

        private void cboFrom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isCboSelectionDisabled == false)
            {
                #region LBLASSETIDFROM POPULATING SECTION
                int sourceId, assetId;
                int sourceType;

                if (rbAccount.IsChecked == true)
                    sourceType = account;
                else
                    sourceType = bank;

                sourceId = Convert.ToInt32(cboFrom.SelectedValue);
                assetId = assetDAL.GetAssetIdBySource(sourceId, sourceType);
                lblAssetIdFrom.Content = assetId;
                #endregion

                #region LBLBALANCEFROM POPULATING SECTION
                int rowIndex = 0;

                DataTable dtAsset = assetDAL.SearchById(assetId);

                string balance = dtAsset.Rows[rowIndex]["source_balance"].ToString();

                lblBalanceFrom.Content = balance;
                #endregion
            }
        }

        private void cboTo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isCboSelectionDisabled==false)
            {
                #region LBLASSETIDTO POPULATING SECTION
                int sourceId, assetId;
                int sourceType = supplier;

                sourceId = Convert.ToInt32(cboTo.SelectedValue);
                assetId = assetDAL.GetAssetIdBySource(sourceId, sourceType);
                lblAssetIdTo.Content = assetId;
                #endregion

                #region LBLBALANCETO POPULATING SECTION
                int rowIndex = 0;

                DataTable dtAsset = assetDAL.SearchById(assetId);

                string balance = dtAsset.Rows[rowIndex]["source_balance"].ToString();

                lblBalanceTo.Content = balance;
                #endregion
            }
        }

        private void btnMenuNew_Click(object sender, RoutedEventArgs e)
        {
            isCboSelectionDisabled = true;//We need to disable the function cboselectionchange because they are being launched once we clear them.
            ClearTools();
            isCboSelectionDisabled = false;//We need to enable the function cboselectionchange after clearing them.

            clickedNewOrEdit = clickedNew;//0 stands for the user has entered the btnNew.
            LoadNewExpense();
            LoadCboTo();
            ModifyToolsOnClickBtnNewEdit();
        }

        private void btnMenuCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Would you really like to cancel the expense page, you piece of shit?", "Cancel Invoice", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    DisableTools();
                    //LoadPastPayment();
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnMenuSave_Click(object sender, RoutedEventArgs e)
        {
            int emptyIndex = -1;

            if (cboFrom.SelectedIndex!= emptyIndex && cboTo.SelectedIndex!= emptyIndex && txtAmount.Text!="")
            {
                int expenseId = Convert.ToInt32(lblExpenseId.Content); /*lblExpenseNumber stands for the expense id in the database.*/
                int userId = userBLL.GetUserId(WinLogin.loggedInUserName);
                bool isSuccess = false, isSuccessAsset = false, isSuccessAssetSupplier = false;

                #region ASSIGNING CUL SECTION
                expenseCUL.Id = expenseId;
                expenseCUL.IdFrom = Convert.ToInt32(cboFrom.SelectedValue);
                expenseCUL.IdTo = Convert.ToInt32(cboTo.SelectedValue);
                expenseCUL.IdAssetFrom = Convert.ToInt32(lblAssetIdFrom.Content);
                expenseCUL.IdAssetTo = Convert.ToInt32(lblAssetIdTo.Content);
                expenseCUL.Amount =Convert.ToDecimal(txtAmount.Text);
                expenseCUL.AddedBy = userId;
                expenseCUL.AddedDate = DateTime.Now;
                #endregion

                #region TABLE ASSET UPDATING SECTION
                //UPDATING THE ASSET FOR EXPENSE OF THE CORPORATION.
                assetCUL.Id = Convert.ToInt32(lblAssetIdFrom.Content);
                assetCUL.SourceBalance = Convert.ToDecimal(lblBalanceFrom.Content) - Convert.ToDecimal(txtAmount.Text);//We have to subtract this amount from company's balance in order to make the payment to the supplier.
                isSuccessAsset = assetDAL.Update(assetCUL);

                //UPDATING THE ASSET FOR BALANCE OF THE SUPPLIER.
                assetCUL.Id = Convert.ToInt32(lblAssetIdTo.Content);
                assetCUL.SourceBalance = Convert.ToDecimal(lblBalanceTo.Content)+Convert.ToDecimal(txtAmount.Text);//We have to add this amount to the supplier's balance in order to reset our dept.
                isSuccessAssetSupplier = assetDAL.Update(assetCUL);
                #endregion

                if (clickedNewOrEdit==clickedEdit)
                {
                    isSuccess = expenseBLL.UpdateExpense(expenseCUL);
                }
                else
                {
                    isSuccess = expenseBLL.InsertExpense(expenseCUL);
                }

                //If the data is inserted successfully, then the value of the variable isSuccess will be true; otherwise it will be false.
                if (isSuccess == true && isSuccessAsset == true && isSuccessAssetSupplier==true)//IsSuccessDetail is always CHANGING in every loop above! IMPROVE THIS!!!!
                {
                    DisableTools();
                    EnableButtonsOnClickSaveCancel();
                }
                else
                {
                    MessageBox.Show("Something went wrong :(");
                }
            }
        }

        private void ClearTools()
        {
            cboFrom.ItemsSource = null;
            cboTo.ItemsSource = null;
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            int firstExpenseId = 1, currentExpenseId = Convert.ToInt32(lblExpenseId.Content); ;

            if (currentExpenseId != firstExpenseId)
            {
                int prevExpenseId = currentExpenseId - unitValue;

                isCboSelectionDisabled = true;//We need to disable the function cboselectionchange because they are being launched once we clear them.
                ClearTools();
                isCboSelectionDisabled = false;//We need to enable the function cboselectionchange after clearing them.

                clickedArrow = clickedPrev;//0 means customer has clicked the previous button.
                LoadPastExpense(prevExpenseId, clickedArrow);
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            int lastExpenseId = commonBLL.GetLastRecordById(calledBy), currentInvoiceId;

            currentInvoiceId = Convert.ToInt32(lblExpenseId.Content);

            if (currentInvoiceId != lastExpenseId)
            {
                int nextInvoice = currentInvoiceId + unitValue;

                isCboSelectionDisabled = true;//We need to disable the function cboselectionchange because they are being launched once we clear them.
                ClearTools();
                isCboSelectionDisabled = false;//We need to enable the function cboselectionchange after clearing them.

                clickedArrow = clickedNext;//1 means customer has clicked the next button.
                LoadPastExpense(nextInvoice, clickedArrow);
            }
        }

        private void LoadCboFrom(int checkStatus)
        {
            DataTable dtAccount;//Creating Data Table to hold the products from Database.
            if (checkStatus == account)
                dtAccount = accountDAL.Select();


            else
                dtAccount = bankDAL.Select();

            //Specifying Items Source for product combobox
            cboFrom.ItemsSource = dtAccount.DefaultView;

            //Here DisplayMemberPath helps to display Text in the ComboBox.
            cboFrom.DisplayMemberPath = "name";

            //SelectedValuePath helps to store values like a hidden field.
            cboFrom.SelectedValuePath = "id";
        }

        private void LoadCboTo()
        {
            DataTable dtTo;//Creating Data Table to hold the products from Database.

            dtTo = supplierDAL.Select();

            //Specifying Items Source for product combobox
            cboTo.ItemsSource = dtTo.DefaultView;

            //Here DisplayMemberPath helps to display Text in the ComboBox.
            cboTo.DisplayMemberPath = "name";

            //SelectedValuePath helps to store values like a hidden field.
            cboTo.SelectedValuePath = "id";
        }

        private void rbAccount_Checked(object sender, RoutedEventArgs e)
        {
            cboFrom.IsEnabled = true;
            LoadCboFrom(account);
        }

        private void rbBank_Checked(object sender, RoutedEventArgs e)
        {
            cboFrom.IsEnabled = true;
            LoadCboFrom(bank);
        }
    }
}
