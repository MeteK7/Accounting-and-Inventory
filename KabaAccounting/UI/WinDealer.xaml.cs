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
    public partial class WinDealer : Window
    {
        public WinDealer()
        {
            InitializeComponent();
            RefreshDealerDataGrid();
        }

        DealerBLL dealerBLL = new DealerBLL();
        DealerDAL dealerDAL = new DealerDAL();

        UserDAL userDAL = new UserDAL();

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //Get the values from the Dealer Window and fill them into the dealerBLL.

            dealerBLL.Name = txtName.Text;
            dealerBLL.Email = txtEmail.Text;
            dealerBLL.Contact = txtContact.Text;
            dealerBLL.Address = txtAddress.Text;
            dealerBLL.AddedDate = DateTime.Now;
            dealerBLL.AddedBy = GetUserId();


            //Creating a Boolean variable to insert data into the database.
            bool isSuccess = dealerDAL.Insert(dealerBLL);

            //If the data is inserted successfully, then the value of the variable isSuccess will be true; otherwise it will be false.
            if (isSuccess == true)
            {
                MessageBox.Show("New data inserted successfully.");
                ClearDealerTextBox();
                RefreshDealerDataGrid();
            }
            else
            {
                MessageBox.Show("Something went wrong :(");
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            //Getting values from the WinDealer

            dealerBLL.Id = Convert.ToInt32(txtId.Text);
            dealerBLL.Name = txtName.Text;
            dealerBLL.Email = txtEmail.Text;
            dealerBLL.Contact = txtContact.Text;
            dealerBLL.Address = txtAddress.Text;
            dealerBLL.AddedDate = DateTime.Now;
            dealerBLL.AddedBy = GetUserId();

            //Updating Data into the database
            bool isSuccess = dealerDAL.Update(dealerBLL);

            if (isSuccess == true)
            {
                MessageBox.Show("Data successfully updated.");
                ClearDealerTextBox();
                RefreshDealerDataGrid();
            }
            else
            {
                MessageBox.Show("Failed to update category");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            dealerBLL.Id = Convert.ToInt32(txtId.Text);

            bool isSuccess = dealerDAL.Delete(dealerBLL);

            if (isSuccess == true)
            {
                MessageBox.Show("Data has been deleted successfully.");
                ClearDealerTextBox();
                RefreshDealerDataGrid();
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

        private void RefreshDealerDataGrid()//Try to modify it by creating an optional parameter.
        {
            //Refreshing Data Grid View
            DataTable dataTable = dealerDAL.Select();
            dgDealer.ItemsSource = dataTable.DefaultView;
            dgDealer.AutoGenerateColumns = true;
            dgDealer.CanUserAddRows = false;
        }

        private void ClearDealerTextBox()
        {
            txtId.Text = "";
            txtName.Text = "";
            txtEmail.Text = "";
            txtContact.Text = "";
            txtAddress.Text = "";
            txtSearch.Text = "";
            
        }

        private void dgDealerIndexChanged()
        {
            //Getting the index of a particular row and fill the text boxes with the related columns of the row.

            //int rowIndex = dgCategories.SelectedIndex;

            DataRowView drv = (DataRowView)dgDealer.SelectedItem;
            if (drv!=null)
            {
                txtId.Text = (drv[0]).ToString();//Selecting the specific row
                txtName.Text = (drv[2]).ToString();
                txtEmail.Text = (drv[3]).ToString();
                txtContact.Text = (drv[4]).ToString();
                txtAddress.Text = (drv[5]).ToString();
            }
        }


        private void dgDealer_KeyUp(object sender, KeyEventArgs e)
        {
                dgDealerIndexChanged();
        }

        private void dgDealer_KeyDown(object sender, KeyEventArgs e)
        {
            dgDealerIndexChanged();
        }

        private void dgDealer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            dgDealerIndexChanged();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Get Keyword from Text box
            string keyword = txtSearch.Text;

            //Check if the keyword has value or not

            if (keyword != null) /*Do NOT Repeat yourself!!! Improve if statement block!!! You have similar codes in the RefreshDealerCustomerDataGrid method!!! */
            {
                //Show category informations based on the keyword
                DataTable dataTable = dealerDAL.Search(keyword);
                dgDealer.ItemsSource = dataTable.DefaultView;
                dgDealer.AutoGenerateColumns = true;
                dgDealer.CanUserAddRows = false;
            }
            else
            {
                //Show all categories from the database
                RefreshDealerDataGrid();
            }
        }

        private void dgDealer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ClearDealerTextBox();
            dgDealer.UnselectAll();
            
        }
    }
}
