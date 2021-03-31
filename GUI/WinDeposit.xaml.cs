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
        BankDAL bankDAL = new BankDAL();
        public WinDeposit()
        {
            InitializeComponent();
            DisableTools();
        }

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
            int number, rowIndex = 0;

            DataTable dtBank = bankDAL.SearchById(Convert.ToInt32(txtEntranceBankId.Text));

            cboEntranceBankName.SelectedValue = dtBank.Rows[rowIndex]["id"];
        }

        private void cboBankName_KeyUp(object sender, KeyEventArgs e)
        {
            txtEntranceBankId.Text = cboEntranceBankName.SelectedValue.ToString();
        }

        private void DisableTools()
        {
            txtEntranceBankId.IsEnabled = false;
            txtEntranceDescription.IsEnabled = false;
            txtEntranceAmount.IsEnabled = false;
            cboEntranceBankName.IsEnabled = false;
            cboEntranceAccountNumber.IsEnabled = false;
            btnEntranceEnter.IsEnabled = false;
            btnEntranceClear.IsEnabled = false;
        }
    }
}
