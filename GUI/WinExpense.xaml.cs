﻿using BLL;
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

        int initialIndex = 0, unitValue = 1;
        int clickedNewOrEdit, clickedNothing = -1, clickedNew = 0, clickedEdit = 1, clickedNull = 2;//0 stands for user clicked the button New, and 1 stands for user clicked the button Edit.
        int account = 1, bank = 2, supplier = 3;
        int clickedArrow,  clickedPrev = 0, clickedNext = 1;
        string calledBy = "WinExpense";
        string colTxtName = "name",colTxtId= "id", colTxtIdTo = "id_to", colTxtAmount= "amount",colTxtDetails="details",colTxtAddedDate= "added_date";
        const int expenseSize = 5;
        const int oldBalanceFrom=0, oldBalanceTo=1, oldAssetIdFrom=2, oldAssetIdTo=3, oldAmount=4;
        string[] oldExpense = new string[expenseSize];
        bool isCboSelectionEnabled = true;

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
            int idAssetFrom;

            if (expenseId == initialIndex)//If the ID is 0 came from the optional parameter, that means user just clicked the WinPOP button to open it.
            {
                expenseId = commonBLL.GetLastRecordById(calledBy);//Getting the last invoice id and assign it to the variable called invoiceId.
            }

            /*WE CANNOT USE ELSE IF FOR THE CODE BELOW! BOTH IF STATEMENTS ABOVE AND BELOVE MUST WORK.*/
            if (expenseId != initialIndex)// If the invoice number is still 0 even when we get the last invoice number by using code above, that means this is the first sale and do not run this code block.
            {
                DataTable dtExpense = expenseDAL.GetByExpenseId(expenseId);

                if (dtExpense.Rows.Count != initialIndex)
                {
                    expenseId = Convert.ToInt32(dtExpense.Rows[initialIndex][colTxtId].ToString());//Getting the id of account.
                    lblExpenseId.Content = expenseId;

                    #region ASSET INFORMATION FILLING REGION
                    idAssetFrom = Convert.ToInt32(dtExpense.Rows[initialIndex]["id_asset_from"].ToString());
                    //lblAssetIdFrom.Content = idAssetFrom;

                    DataTable dtAsset = assetDAL.SearchById(idAssetFrom);
                    int sourceType = Convert.ToInt32(dtAsset.Rows[initialIndex]["id_source_type"]);

                    if (sourceType == account)
                        rbAccount.IsChecked = true;
                    else
                        rbBank.IsChecked = true;

                    LoadCboFrom(sourceType);//This function works twice when you open the WinExpense because the rb selection is being changed. But if the previous selection is same, rbBank_Checked does not work so the method LoadCboFrom called by rbBank_Checked does not work as well.
                    cboFrom.SelectedValue = dtAsset.Rows[initialIndex]["id_source"].ToString();
                    #endregion

                    LoadCboTo();
                    cboTo.SelectedValue = Convert.ToInt32(dtExpense.Rows[initialIndex][colTxtIdTo].ToString());//Getting the id of supplier.

                    txtAmount.Text= dtExpense.Rows[initialIndex][colTxtAmount].ToString();
                    txtDetails.Text = dtExpense.Rows[initialIndex][colTxtDetails].ToString();
                    lblDateAdded.Content =Convert.ToDateTime(dtExpense.Rows[initialIndex][colTxtAddedDate]).ToString("f");
                }
                else if (dtExpense.Rows.Count == initialIndex)//If the pop detail row quantity is 0, that means there is no such row so decrease or increase the invoice number according to user preference.
                {
                    if (expenseArrow == initialIndex)//If the invoice arrow is 0, that means user clicked the previous button.
                    {
                        expenseId = expenseId - unitValue;
                    }
                    else
                    {
                        expenseId = expenseId + unitValue;
                    }

                    if (expenseArrow != -unitValue)//If the user has not clicked either previous or next button, then the invoiceArrow will be -1 and no need for recursion.
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

        public void EnableTools()
        {
            btnMenuSave.IsEnabled = true;
            btnMenuCancel.IsEnabled = true;
            cboFrom.IsEnabled = true;
            cboTo.IsEnabled = true;
            txtAmount.IsEnabled = true;
            rbAccount.IsEnabled = true;
            rbBank.IsEnabled = true;
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
            cboFrom.IsEnabled = true;
            cboTo.IsEnabled = true;
            txtAmount.IsEnabled = true;
            rbAccount.IsEnabled = true;
            rbBank.IsEnabled = true;
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

        private void btnMenuDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Would you really like to delete the expense?", "Delete Expense", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:

                    #region TABLE ASSET REVERTING AND UPDATING SECTION
                    //REVERTING THE ASSET FOR EXPENSE OF THE CORPORATION.
                    assetCUL.Id = Convert.ToInt32(lblAssetIdFrom.Content);
                    assetCUL.SourceBalance = Convert.ToDecimal(GetBalance(Convert.ToInt32(lblAssetIdFrom.Content))) + Convert.ToDecimal(txtAmount.Text);//We have to add this amount into company's balance in order to revert the old expense.
                    assetDAL.Update(assetCUL);

                    //REVERTING THE ASSET FOR BALANCE OF THE SUPPLIER.
                    assetCUL.Id = Convert.ToInt32(lblAssetIdTo.Content);
                    assetCUL.SourceBalance = Convert.ToDecimal(GetBalance(Convert.ToInt32(lblAssetIdTo.Content))) - Convert.ToDecimal(txtAmount.Text);//We have to subtract this amount from supplier's balance in order to revert our dept.
                    assetDAL.Update(assetCUL);
                    #endregion

                    #region DELETE EXPENSE RECORD
                    int expenseId = Convert.ToInt32(lblExpenseId.Content);

                    expenseDAL.Delete(expenseId);
                    #endregion

                    LoadPastExpense();
                    break;
                case MessageBoxResult.No:
                    MessageBox.Show("Enjoy!", "Enjoy");
                    break;
                case MessageBoxResult.Cancel:
                    MessageBox.Show("Nevermind then...", "KABA Accounting");
                    break;
            }
        }

        private void cboFrom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isCboSelectionEnabled == true)
            {
                CboFromSelectionChanged();
            }
        }

        private void cboTo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isCboSelectionEnabled == true)
            {
                CboToSelectionChanged();
            }
        }

        private void CboFromSelectionChanged()
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
                DataTable dtAsset = assetDAL.SearchById(assetId);

                string balance = dtAsset.Rows[initialIndex]["source_balance"].ToString();

                lblBalanceFrom.Content = balance;
                #endregion
        }

        private void CboToSelectionChanged()
        {
                #region LBLASSETIDTO POPULATING SECTION
                int sourceId, assetId;
                int sourceType = supplier;

                sourceId = Convert.ToInt32(cboTo.SelectedValue);
                assetId = assetDAL.GetAssetIdBySource(sourceId, sourceType);
                lblAssetIdTo.Content = assetId;
                #endregion

                #region LBLBALANCETO POPULATING SECTION
                DataTable dtAsset = assetDAL.SearchById(assetId);

                string balance = dtAsset.Rows[initialIndex]["source_balance"].ToString();

                lblBalanceTo.Content = balance;
                #endregion
        }

        private void btnMenuNew_Click(object sender, RoutedEventArgs e)
        {
            clickedNewOrEdit = clickedNew;//0 stands for the user has entered the btnNew.

            ClearTools();
            LoadNewExpense();
            LoadCboTo();
            ModifyToolsOnClickBtnNewEdit();
        }

        private void btnMenuCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Would you really like to cancel the expense page?", "Cancel Invoice", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    DisableTools();
                    ClearTools();
                    LoadPastExpense();
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

            if (cboFrom.SelectedIndex != emptyIndex && cboTo.SelectedIndex != emptyIndex && txtAmount.Text != "")
            {
                int expenseId = Convert.ToInt32(lblExpenseId.Content); /*lblExpenseId stands for the expense id in the database.*/
                int userId = userBLL.GetUserId(WinLogin.loggedInUserName);
                bool isSuccess = false, isSuccessAsset = false, isSuccessAssetSupplier = false;

                #region ASSIGNING CUL SECTION
                expenseCUL.Id = expenseId;
                expenseCUL.IdFrom = Convert.ToInt32(cboFrom.SelectedValue);
                expenseCUL.IdTo = Convert.ToInt32(cboTo.SelectedValue);
                expenseCUL.IdAssetFrom = Convert.ToInt32(lblAssetIdFrom.Content);
                expenseCUL.IdAssetTo = Convert.ToInt32(lblAssetIdTo.Content);
                expenseCUL.Amount = Convert.ToDecimal(txtAmount.Text);
                expenseCUL.Details = txtDetails.Text;
                expenseCUL.AddedBy = userId;
                expenseCUL.AddedDate = DateTime.Now;
                #endregion

                if (clickedNewOrEdit == clickedEdit)
                {
                    isSuccess = expenseBLL.UpdateExpense(expenseCUL);

                    #region TABLE ASSET REVERTING AND UPDATING SECTION
                    //UPDATING THE ASSET FOR EXPENSE OF THE CORPORATION.
                    assetCUL.Id = Convert.ToInt32(oldExpense[oldAssetIdFrom]);
                    assetCUL.SourceBalance = Convert.ToDecimal(oldExpense[oldBalanceFrom]) + Convert.ToDecimal(oldExpense[oldAmount]);//We have to add this amount into company's balance in order to revert the old expense.
                    isSuccessAsset = assetDAL.Update(assetCUL);

                    //UPDATING THE ASSET FOR BALANCE OF THE SUPPLIER.
                    assetCUL.Id = Convert.ToInt32(oldExpense[oldAssetIdTo]);
                    assetCUL.SourceBalance = Convert.ToDecimal(oldExpense[oldBalanceTo]) - Convert.ToDecimal(oldExpense[oldAmount]);//We have to subtract this amount from supplier's balance in order to revert our dept.
                    isSuccessAssetSupplier = assetDAL.Update(assetCUL);
                    #endregion
                }

                else
                {
                    isSuccess = expenseBLL.InsertExpense(expenseCUL);
                }

                #region TABLE ASSET UPDATING SECTION
                //UPDATING THE ASSET FOR EXPENSE OF THE CORPORATION.
                assetCUL.Id = Convert.ToInt32(lblAssetIdFrom.Content);
                assetCUL.SourceBalance = Convert.ToDecimal(GetBalance(Convert.ToInt32(lblAssetIdFrom.Content))) - Convert.ToDecimal(txtAmount.Text);//We have to subtract this amount from company's balance in order to make the payment to the supplier.
                isSuccessAsset = assetDAL.Update(assetCUL);

                //UPDATING THE ASSET FOR BALANCE OF THE SUPPLIER.
                assetCUL.Id = Convert.ToInt32(lblAssetIdTo.Content);
                assetCUL.SourceBalance = Convert.ToDecimal(GetBalance(Convert.ToInt32(lblAssetIdTo.Content))) + Convert.ToDecimal(txtAmount.Text);//We have to add this amount to supplier's balance in order to reset our dept.
                isSuccessAssetSupplier = assetDAL.Update(assetCUL);
                #endregion

                //If the data is inserted successfully, then the value of the variable isSuccess will be true; otherwise it will be false.
                if (isSuccess == true && isSuccessAsset == true && isSuccessAssetSupplier == true)//IsSuccessDetail is always CHANGING in every loop above! IMPROVE THIS!!!!
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

        private decimal GetBalance(int assetId)
        {
            DataTable dtAsset = assetDAL.SearchById(assetId);

            decimal balance = Convert.ToDecimal(dtAsset.Rows[initialIndex]["source_balance"]);

            return balance;
        }

        private void btnMenuEdit_Click(object sender, RoutedEventArgs e)
        {
            EnableTools();

            oldExpense[oldBalanceFrom] = lblBalanceFrom.Content.ToString();
            oldExpense[oldBalanceTo] = lblBalanceTo.Content.ToString();
            oldExpense[oldAssetIdFrom] = lblAssetIdFrom.Content.ToString();
            oldExpense[oldAssetIdTo] = lblAssetIdTo.Content.ToString();
            oldExpense[oldAmount] = txtAmount.Text.ToString();

            clickedNewOrEdit = clickedEdit;//Changing the state of the clicked NewOrEdit in order to update the old expense page.
        }

        private void ClearTools()
        {
            isCboSelectionEnabled = false;
            cboFrom.ItemsSource = null;
            cboTo.ItemsSource = null;
            isCboSelectionEnabled = true;

            lblBalanceFrom.Content = "";
            lblBalanceTo.Content = "";
            lblAssetIdFrom.Content = "";
            lblAssetIdTo.Content = "";
            lblDateAdded.Content = "";
            txtDetails.Text = "";
            txtAmount.Text = "";
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            int firstExpenseId = unitValue, currentExpenseId = Convert.ToInt32(lblExpenseId.Content);

            if (currentExpenseId != firstExpenseId)
            {
                int prevExpenseId = currentExpenseId - unitValue;
                
                clickedArrow = clickedPrev;//0 means customer has clicked the previous button.
                ClearTools();
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

                clickedArrow = clickedNext;//1 means customer has clicked the next button.
                ClearTools();
                LoadPastExpense(nextInvoice, clickedArrow);
            }
        }

        private void LoadCboFrom(int checkStatus)
        {
            isCboSelectionEnabled = false;//Disabling the selection changed method in order to prevent them to work when we reassign the combobox with unselected status.

            DataTable dtAccount;//Creating Data Table to hold the products from Database.
            if (checkStatus == account)
                dtAccount = accountDAL.Select();

            else
                dtAccount = bankDAL.Select();

            //Specifying Items Source for product combobox
            cboFrom.ItemsSource = dtAccount.DefaultView;

            //Here DisplayMemberPath helps to display Text in the ComboBox.
            cboFrom.DisplayMemberPath = colTxtName;

            //SelectedValuePath helps to store values like a hidden field.
            cboFrom.SelectedValuePath = colTxtId;

            isCboSelectionEnabled = true;//Enabling the selection changed method in order to allow them to work in case of any future selections.
        }

        private void LoadCboTo()
        {
            isCboSelectionEnabled = false;//Disabling the selection changed method in order to prevent them to work when we reassign the combobox with unselected status.

            DataTable dtTo;//Creating Data Table to hold the products from Database.

            dtTo = supplierDAL.Select();

            //Specifying Items Source for product combobox
            cboTo.ItemsSource = dtTo.DefaultView;

            //Here DisplayMemberPath helps to display Text in the ComboBox.
            cboTo.DisplayMemberPath = colTxtName;

            //SelectedValuePath helps to store values like a hidden field.
            cboTo.SelectedValuePath = colTxtId;

            isCboSelectionEnabled = true;//Enabling the selection changed method in order to allow them to work in case of any future selections.
        }

        private void rbAccount_Checked(object sender, RoutedEventArgs e)
        {
            LoadCboFrom(account);
        }

        private void rbBank_Checked(object sender, RoutedEventArgs e)
        {
            LoadCboFrom(bank);
        }
    }
}
