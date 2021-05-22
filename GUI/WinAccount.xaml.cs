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
    /// Interaction logic for WinAccount.xaml
    /// </summary>
    public partial class WinAccount : Window
    {
        public WinAccount()
        {
            InitializeComponent();
            LoadAccountDataGrid();
        }

        AccountCUL accountCUL = new AccountCUL();
        AccountDAL accountDAL = new AccountDAL();
        UserBLL userBLL = new UserBLL();
        UserDAL userDAL = new UserDAL();

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            accountCUL.Name = txtAccountName.Text;
            accountCUL.AddedDate = DateTime.Now;
            accountCUL.AddedBy = userBLL.GetUserId(WinLogin.loggedInUserName);

            bool isSuccess = accountDAL.Insert(accountCUL);

            if (isSuccess == true)
            {
                MessageBox.Show("New account has been inserted successfully.");
                dtgAccounts.Items.Clear();
                ClearAccountTextBox();
                LoadAccountDataGrid();
            }
            else
            {
                MessageBox.Show("Something went wrong :(");
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            accountCUL.Id = Convert.ToInt32(txtAccountId.Text);
            accountCUL.Name = txtAccountName.Text;
            accountCUL.AddedDate = DateTime.Now;
            accountCUL.AddedBy = userBLL.GetUserId(WinLogin.loggedInUserName);

            bool isSuccess = accountDAL.Update(accountCUL);

            if (isSuccess == true)
            {
                MessageBox.Show("The account is successfully updated.");
                dtgAccounts.Items.Clear();
                ClearAccountTextBox();
                LoadAccountDataGrid();
            }
            else
            {
                MessageBox.Show("Failed to update the account.");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            accountCUL.Id = Convert.ToInt32(txtAccountId.Text);

            bool isSuccess = accountDAL.Delete(accountCUL);

            if (isSuccess == true)
            {
                MessageBox.Show("Bank has been deleted successfully.");
                dtgAccounts.Items.Clear();
                ClearAccountTextBox();
                LoadAccountDataGrid();
            }
            else
            {
                MessageBox.Show("Something went wrong :/");
            }
        }

        private void LoadAccountDataGrid()
        {
            //Refreshing Data Grid View
            //DataTable dataTable = accountDAL.Select();
            //dtgAccounts.ItemsSource = dataTable.DefaultView;
            //dtgAccounts.AutoGenerateColumns = true;
            //dtgAccounts.CanUserAddRows = false;

            int firstRowIndex = 0, accountId, addedById;
            string accountName, addedDate, addedByUsername;
            DataTable dataTable = accountDAL.Select();
            DataTable dataTableUserInfo;

            //dtgs.ItemsSource = dataTable.DefaultView; Adds everything at once.
            dtgAccounts.AutoGenerateColumns = true;
            dtgAccounts.CanUserAddRows = false;

            #region LOADING THE PRODUCT DATA GRID

            for (int currentRow = firstRowIndex; currentRow < dataTable.Rows.Count; currentRow++)
            {
                accountId = Convert.ToInt32(dataTable.Rows[currentRow]["id"]);
                accountName = dataTable.Rows[currentRow]["name"].ToString();
                addedDate = dataTable.Rows[currentRow]["added_date"].ToString();

                addedById = Convert.ToInt32(dataTable.Rows[currentRow]["added_by"]);
                dataTableUserInfo = userDAL.GetUserInfoById(addedById);
                addedByUsername = dataTableUserInfo.Rows[firstRowIndex]["first_name"].ToString() + " " + dataTableUserInfo.Rows[firstRowIndex]["last_name"].ToString();

                dtgAccounts.Items.Add(new { Id = accountId, Name = accountName, AddedDate = addedDate, AddedBy = addedByUsername});
            }
            #endregion
        }

        private void ClearAccountTextBox()
        {
            txtAccountId.Text = "";
            txtAccountName.Text = "";
            txtAccountSearch.Text = "";
        }

        private void DtgAccountsIndexChanged()
        {
            object row = dtgAccounts.SelectedItem;
            int colAccountId = 0, colAccountName = 1;

            txtAccountId.Text = (dtgAccounts.Columns[colAccountId].GetCellContent(row) as TextBlock).Text;
            txtAccountName.Text = (dtgAccounts.Columns[colAccountName].GetCellContent(row) as TextBlock).Text;

            //DataRowView drv = (DataRowView)dtgAccounts.SelectedItem;

            //txtAccountId.Text = (drv[0]).ToString();//Selecting the specific row
            //txtAccountName.Text = (drv["name"]).ToString();//You can also define the column name from your table like here.
        }
        private void dtgAccounts_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DtgAccountsIndexChanged();
        }

        private void dtgAccounts_KeyUp(object sender, KeyEventArgs e)
        {
            DtgAccountsIndexChanged();
        }
        private void dtgAccounts_KeyDown(object sender, KeyEventArgs e)
        {
            DtgAccountsIndexChanged();
        }

        private void txtAccountSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Get Keyword from Text box
            string keyword = txtAccountSearch.Text;

            //Check if the keyword has value or not

            if (keyword != null) /*Do NOT Repeat yourself!!! Improve if statement block!!! You have similar codes in the RefreshAccountDataGrid method!!! */
            {
                dtgAccounts.Items.Clear();

                //Show account informations based on the keyword
                DataTable dataTableProduct = accountDAL.Search(keyword);//The first "keyword" is the parameter name, and the second "keyword" is the local variable.

                for (int rowIndex = 0; rowIndex < dataTableProduct.Rows.Count; rowIndex++)
                {
                    dtgAccounts.Items.Add(
                        new AccountCUL()
                        {
                            Id = Convert.ToInt32(dataTableProduct.Rows[rowIndex]["id"]),
                            Name = dataTableProduct.Rows[rowIndex]["barcode_retail"].ToString(),
                            AddedDate = Convert.ToDateTime(dataTableProduct.Rows[rowIndex]["barcode_wholesale"]),
                            AddedBy = Convert.ToInt32(dataTableProduct.Rows[rowIndex]["name"])
                        });
                }
            }
            else
            {
                //Show all accounts from the database
                LoadAccountDataGrid();
            }
        }
    }
}
