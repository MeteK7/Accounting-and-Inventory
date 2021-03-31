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
            cboBankName.ItemsSource = dataTable.DefaultView;

            //Here DisplayMemberPath helps to display Text in the ComboBox.
            cboBankName.DisplayMemberPath = "name";

            //SelectedValuePath helps to store values like a hidden field.
            cboBankName.SelectedValuePath = "id";
        }


    }
}
