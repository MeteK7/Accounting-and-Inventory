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
        BankDAL bankDAL = new BankDAL();
        AccountDAL accountDAL = new AccountDAL();
        public WinDeposit()
        {
            InitializeComponent();
            LoadUserInformations();
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

            txtTotal.Text = (Convert.ToDecimal(txtTotal.Text) + amount).ToString();
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
    }
}
