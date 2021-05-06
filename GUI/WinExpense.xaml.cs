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
        AssetCUL assetCUL = new AssetCUL();
        AssetDAL assetDAL = new AssetDAL();
        BankDAL bankDAL = new BankDAL();

        int clickedNewOrEdit, clickedNew=0,clickedEdit=1;//0 stands for user clicked the button New, and 1 stands for user clicked the button Edit.
        int account = 1, bank = 2, supplier = 3;

        public WinExpense()
        {
            InitializeComponent();
            DisableTools();
        }

        public void DisableTools()
        {
            btnMenuSave.IsEnabled = false;
            btnMenuCancel.IsEnabled = false;
            cboFrom.IsEnabled = false;
            cboTo.IsEnabled = false;
            txtAmount.IsEnabled = false;

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
        }

        private void LoadNewExpense()
        {
            //ClearBasketTextBox();
            //ClearProductsDataGrid();

            int expenseNo, increment = 1;

            expenseNo = expenseBLL.GetLastExpenseNumber();//Getting the last invoice number and assign it to the variable called expenseNo.
            expenseNo += increment;//We are adding one to the last expense number because every new expense number is one greater tham the previous expense number.
            lblExpenseNumber.Content = expenseNo;//Assigning expenseNo to the content of the expenseNo Label.
        }

        private void cboTo_Loaded(object sender, RoutedEventArgs e)
        {
            //Creating Data Table to hold the products from Database
            DataTable dtSupplier = supplierDAL.Select();

            //Specifying Items Source for product combobox
            cboTo.ItemsSource = dtSupplier.DefaultView;

            //Here DisplayMemberPath helps to display Text in the ComboBox.
            cboTo.DisplayMemberPath = "name";

            //SelectedValuePath helps to store values like a hidden field.
            cboTo.SelectedValuePath = "id";
        }

        private void cboFrom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            #region LBLASSETID POPULATING SECTION
            int sourceId;
            int sourceType;

            if (rbAccount.IsChecked == true)
                sourceType = account;
            else
                sourceType = bank;

            sourceId = Convert.ToInt32(cboFrom.SelectedValue);
            lblAssetId.Content = assetDAL.GetAssetIdBySource(sourceId, sourceType);
            #endregion

            #region LBLBALANCEFROM POPULATING SECTION
            int rowIndex = 0, assetId = Convert.ToInt32(lblAssetId.Content);

            DataTable dtAsset = assetDAL.SearchById(assetId);

            string balance = dtAsset.Rows[rowIndex]["source_balance"].ToString();

            lblBalanceFrom.Content = balance;
            #endregion
        }

        private void cboTo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            #region LBLASSETID POPULATING SECTION
            int sourceId;
            int sourceType=supplier;

            sourceId = Convert.ToInt32(cboTo.SelectedValue);
            lblAssetSupplierId.Content = assetDAL.GetAssetIdBySource(sourceId, sourceType);
            #endregion

            #region LBLBALANCETO POPULATING SECTION
            int rowIndex = 0, assetId = Convert.ToInt32(lblAssetSupplierId.Content);

            DataTable dtAsset = assetDAL.SearchById(assetId);

            string balance = dtAsset.Rows[rowIndex]["source_balance"].ToString();

            lblBalanceTo.Content = balance;
            #endregion
        }

        private void btnMenuNew_Click(object sender, RoutedEventArgs e)
        {
            clickedNewOrEdit = clickedNew;//0 stands for the user has entered the btnNew.
            LoadNewExpense();
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
                int expenseId = Convert.ToInt32(lblExpenseNumber.Content); /*lblExpenseNumber stands for the expense id in the database.*/
                int userId = userBLL.GetUserId(WinLogin.loggedInUserName);
                bool isSuccess = false, isSuccessAsset = false, isSuccessAssetSupplier = false;

                #region ASSIGNING CUL SECTION
                expenseCUL.Id = expenseId;
                expenseCUL.IdFrom = Convert.ToInt32(cboFrom.SelectedValue);
                expenseCUL.IdTo = Convert.ToInt32(cboTo.SelectedValue);
                expenseCUL.Amount =Convert.ToDecimal(txtAmount.Text);
                expenseCUL.AddedBy = userId;
                expenseCUL.AddedDate = DateTime.Now;
                #endregion

                #region TABLE ASSET UPDATING SECTION
                //UPDATING THE ASSET FOR EXPENSE OF THE CORPORATION.
                assetCUL.Id = Convert.ToInt32(lblAssetId.Content);
                assetCUL.SourceBalance = Convert.ToDecimal(lblBalanceFrom.Content) - Convert.ToDecimal(txtAmount.Text);//We have to subtract this amount from company's balance in order to make the payment to the supplier.
                isSuccessAsset = assetDAL.Update(assetCUL);

                //UPDATING THE ASSET FOR BALANCE OF THE SUPPLIER.
                assetCUL.Id = Convert.ToInt32(lblAssetSupplierId.Content);
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

        private void rbAccount_Checked(object sender, RoutedEventArgs e)
        {
            //cboMenuAsset.ItemsSource = null;
            LoadCboFrom(account);
        }

        private void rbBank_Checked(object sender, RoutedEventArgs e)
        {
            //cboMenuAsset.ItemsSource = null;
            LoadCboFrom(bank);
        }
    }
}
