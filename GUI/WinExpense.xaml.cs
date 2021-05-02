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

        int clickedNewOrEdit, btnNew=0,btnEdit=1;//0 stands for user clicked the button New, and 1 stands for user clicked the button Edit.

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

        public void ModifyToolsOnClickBtnNewEdit()
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

        private void cboFrom_Loaded(object sender, RoutedEventArgs e)
        {
            //Creating Data Table to hold the products from Database
            DataTable dtAccount = accountDAL.Select();

            //Specifying Items Source for product combobox
            cboFrom.ItemsSource = dtAccount.DefaultView;

            //Here DisplayMemberPath helps to display Text in the ComboBox.
            cboFrom.DisplayMemberPath = "name";

            //SelectedValuePath helps to store values like a hidden field.
            cboFrom.SelectedValuePath = "id";
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
            int rowIndex=0, selectedValue = Convert.ToInt32(cboFrom.SelectedValue);

            DataTable dtAccount = accountDAL.SearchById(selectedValue);

            string balance = dtAccount.Rows[rowIndex]["balance"].ToString();

            lblBalanceFrom.Content = "Balance: " + balance;
        }

        private void cboTo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int rowIndex = 0, selectedValue = Convert.ToInt32(cboTo.SelectedValue);

            DataTable dtSupplier = supplierDAL.SearchById(selectedValue);

            string balance = dtSupplier.Rows[rowIndex]["balance"].ToString();

            lblBalanceTo.Content = "Balance: " + balance;
        }

        private void btnMenuNew_Click(object sender, RoutedEventArgs e)
        {
            clickedNewOrEdit = btnNew;//0 stands for the user has entered the btnNew.
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

        private void btnMenuSave_Click(object sender, RoutedEventArgs e)
        {
            int emptyIndex = -1;

            if (cboFrom.SelectedIndex!= emptyIndex && cboTo.SelectedIndex!= emptyIndex && txtAmount.Text!="")
            {
                int expenseId = Convert.ToInt32(lblExpenseNumber.Content); /*lblExpenseNumber stands for the expense id in the database.*/
                int userId = userBLL.GetUserId(WinLogin.loggedInUserName);
                bool isSuccess = false;

                #region ASSIGNING CUL SECTION
                expenseCUL.Id = expenseId;
                expenseCUL.IdFrom = Convert.ToInt32(cboFrom.SelectedValue);
                expenseCUL.IdTo = Convert.ToInt32(cboTo.SelectedValue);
                expenseCUL.Amount =Convert.ToDecimal(txtAmount.Text);
                expenseCUL.AddedBy = userId;
                expenseCUL.AddedDate = DateTime.Now;
                #endregion

                if (clickedNewOrEdit==btnEdit)
                {
                    isSuccess = expenseBLL.Update(expenseCUL);
                }
            }
        }
    }
}
