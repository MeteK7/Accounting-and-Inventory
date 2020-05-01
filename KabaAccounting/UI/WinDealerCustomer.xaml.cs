using KabaAccounting.BLL;
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

namespace KabaAccounting.UI
{
    /// <summary>
    /// Interaction logic for WinDealerCustomer.xaml
    /// </summary>
    public partial class WinDealerCustomer : Window
    {
        public WinDealerCustomer()
        {
            InitializeComponent();
            RefreshDealerCustomerDataGrid();
        }

        DealerCustomerBLL dealerCustomerBLL = new DealerCustomerBLL();
        DealerCustomerDAL dealerCustomerDAL = new DealerCustomerDAL();

        UserDAL userDAL = new UserDAL();

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //Get the values from the DealerCustomer Window and fill them into the dealerCustomerBLL.
            dealerCustomerBLL.Type = cboType.Text;
            dealerCustomerBLL.Name = txtName.Text;
            dealerCustomerBLL.Email = txtEmail.Text;
            dealerCustomerBLL.Contact = txtContact.Text;
            dealerCustomerBLL.Address = txtAddress.Text;
            dealerCustomerBLL.AddedDate = DateTime.Now;
            dealerCustomerBLL.AddedBy = GetUserId();


            //Creating a Boolean variable to insert data into the database.
            bool isSuccess = dealerCustomerDAL.Insert(dealerCustomerBLL);

            //If the data is inserted successfully, then the value of the variable isSuccess will be true; otherwise it will be false.
            if (isSuccess == true)
            {
                MessageBox.Show("New data inserted successfully.");
                ClearDealerCustomerTextBox();
                RefreshDealerCustomerDataGrid();
            }
            else
            {
                MessageBox.Show("Something went wrong :(");
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            //Getting values from the WinDealerCustomer

            dealerCustomerBLL.Id = Convert.ToInt32(txtId.Text);
            dealerCustomerBLL.Type = cboType.Text;
            dealerCustomerBLL.Name = txtName.Text;
            dealerCustomerBLL.Email = txtEmail.Text;
            dealerCustomerBLL.Contact = txtContact.Text;
            dealerCustomerBLL.Address = txtAddress.Text;
            dealerCustomerBLL.AddedDate = DateTime.Now;
            dealerCustomerBLL.AddedBy = GetUserId();

            //Updating Data into the database
            bool isSuccess = dealerCustomerDAL.Update(dealerCustomerBLL);

            if (isSuccess == true)
            {
                MessageBox.Show("Data successfully updated.");
                ClearDealerCustomerTextBox();
                RefreshDealerCustomerDataGrid();
            }
            else
            {
                MessageBox.Show("Failed to update category");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            dealerCustomerBLL.Id = Convert.ToInt32(txtId.Text);

            bool isSuccess = dealerCustomerDAL.Delete(dealerCustomerBLL);

            if (isSuccess == true)
            {
                MessageBox.Show("Data has been deleted successfully.");
                ClearDealerCustomerTextBox();
                RefreshDealerCustomerDataGrid();
            }
            else
            {
                MessageBox.Show("Something went wrong:/");
            }
        }

        private int GetUserId()//You used this method in WinProducts, as well. You can Make an external class just for this!!!.
        {
            //Getting the name of the user from the Login Window and fill it into a string variable;
            string loggedUser = WinLogin.loggedIn;

            //Calling the method named GetIdFromUsername in the userDAL and sending the variable loggedUser as a parameter into it.
            //Then, fill the result into the userBLL;
            UserBLL userBLL = userDAL.GetIdFromUsername(loggedUser);

            int userId = userBLL.Id;

            return userId;
        }

        private void RefreshDealerCustomerDataGrid()//Try to modify it by creating an optional parameter.
        {
            //Refreshing Data Grid View
            DataTable dataTable = dealerCustomerDAL.Select();
            dtgDealerCustomer.ItemsSource = dataTable.DefaultView;
            dtgDealerCustomer.AutoGenerateColumns = true;
            dtgDealerCustomer.CanUserAddRows = false;
        }

        private void ClearDealerCustomerTextBox()
        {
            txtId.Text = "";
            cboType.SelectedIndex = -1;
            txtName.Text = "";
            txtEmail.Text = "";
            txtContact.Text = "";
            txtAddress.Text = "";
            txtSearch.Text = "";
            
        }

        private void DtgDealerCustomerIndexChanged()
        {
            //Getting the index of a particular row and fill the text boxes with the related columns of the row.

            //int rowIndex = dtgCategories.SelectedIndex;

            DataRowView drv = (DataRowView)dtgDealerCustomer.SelectedItem;

            txtId.Text = (drv[0]).ToString();//Selecting the specific row
            cboType.Text = (drv["type"]).ToString();//You could also define the column name from your data grid.
            txtName.Text = (drv[2]).ToString();
            txtEmail.Text = (drv[3]).ToString();
            txtContact.Text = (drv[4]).ToString();
            txtAddress.Text = (drv[5]).ToString();
        }


        private void dtgDealerCustomer_KeyUp(object sender, KeyEventArgs e)
        {
            DtgDealerCustomerIndexChanged();
        }

        private void dtgDealerCustomer_KeyDown(object sender, KeyEventArgs e)
        {
            DtgDealerCustomerIndexChanged();
        }

        private void dtgDealerCustomer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DtgDealerCustomerIndexChanged();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Get Keyword from Text box
            string keyword = txtSearch.Text;

            //Check if the keyword has value or not

            if (keyword != null) /*Do NOT Repeat yourself!!! Improve if statement block!!! You have similar codes in the RefreshDealerCustomerDataGrid method!!! */
            {
                //Show category informations based on the keyword
                DataTable dataTable = dealerCustomerDAL.Search(keyword);
                dtgDealerCustomer.ItemsSource = dataTable.DefaultView;
                dtgDealerCustomer.AutoGenerateColumns = true;
                dtgDealerCustomer.CanUserAddRows = false;
            }
            else
            {
                //Show all categories from the database
                RefreshDealerCustomerDataGrid();
            }
        }
    }
}
