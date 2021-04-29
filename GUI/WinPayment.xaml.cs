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
    /// Interaction logic for WinPayment.xaml
    /// </summary>
    public partial class WinPayment : Window
    {
        AccountDAL accountDAL = new AccountDAL();
        SupplierDAL supplierDAL = new SupplierDAL();

        int btnNewOrEdit;//0 stands for user clicked the button New, and 1 stands for user clicked the button Edit.

        public WinPayment()
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
            txtAmountPayment.IsEnabled = false;

        }

        public void ModifyToolsOnClickBtnNewOrEdit()
        {
            btnMenuSave.IsEnabled = true;
            btnMenuCancel.IsEnabled = true;
            btnMenuNew.IsEnabled = false;
            btnMenuEdit.IsEnabled = false;
            btnMenuDelete.IsEnabled = false;
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
            btnNewOrEdit = 0;//0 stands for the user has entered the btnNew.
            ModifyToolsOnClickBtnNewOrEdit();
        }
    }
}
