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
    /// Interaction logic for WinCustomer.xaml
    /// </summary>
    public partial class WinCustomer : Window
    {
        public WinCustomer()
        {
            InitializeComponent();
        }
        CustomerCUL customerCUL = new CustomerCUL();
        CustomerDAL customerDAL = new CustomerDAL();

        UserDAL userDAL = new UserDAL();

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //Get the values from the Supplier Window and fill them into the supplierCUL.

            customerCUL.Name = txtName.Text;
            customerCUL.Email = txtEmail.Text;
            customerCUL.Contact = txtContact.Text;
            customerCUL.Address = txtAddress.Text;
            customerCUL.AddedDate = DateTime.Now;
            customerCUL.AddedBy = GetUserId();


            //Creating a Boolean variable to insert data into the database.
            bool isSuccess = customerDAL.Insert(customerCUL);

            //If the data is inserted successfully, then the value of the variable isSuccess will be true; otherwise it will be false.
            if (isSuccess == true)
            {
                MessageBox.Show("New data inserted successfully.");
                ClearCustomerTextBox();
                RefreshCustomerDataGrid();
            }
            else
            {
                MessageBox.Show("Something went wrong :(");
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            //Getting values from the WinCustomer

            customerCUL.Id = Convert.ToInt32(txtId.Text);
            customerCUL.Name = txtName.Text;
            customerCUL.Email = txtEmail.Text;
            customerCUL.Contact = txtContact.Text;
            customerCUL.Address = txtAddress.Text;
            customerCUL.AddedDate = DateTime.Now;
            customerCUL.AddedBy = GetUserId();

            //Updating Data into the database
            bool isSuccess = customerDAL.Update(customerCUL);

            if (isSuccess == true)
            {
                MessageBox.Show("Data successfully updated.");
                ClearCustomerTextBox();
                RefreshCustomerDataGrid();
            }
            else
            {
                MessageBox.Show("Failed to update category");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            customerCUL.Id = Convert.ToInt32(txtId.Text);

            bool isSuccess = customerDAL.Delete(customerCUL);

            if (isSuccess == true)
            {
                MessageBox.Show("Data has been deleted successfully.");
                ClearCustomerTextBox();
                RefreshCustomerDataGrid();
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
            //Then, fill the result into the userCUL;
            UserCUL userCUL = userDAL.GetIdFromUsername(loggedUser);

            int userId = userCUL.Id;

            return userId;
        }

        private void RefreshCustomerDataGrid()//Try to modify it by creating an optional parameter.
        {
            //Refreshing Data Grid View
            DataTable dataTable = customerDAL.Select();
            dtgCustomer.ItemsSource = dataTable.DefaultView;
            dtgCustomer.AutoGenerateColumns = true;
            dtgCustomer.CanUserAddRows = false;
        }

        private void ClearCustomerTextBox()
        {
            txtId.Text = "";
            txtName.Text = "";
            txtEmail.Text = "";
            txtContact.Text = "";
            txtAddress.Text = "";
            txtSearch.Text = "";

        }

        private void DtgCustomerIndexChanged()
        {
            //Getting the index of a particular row and fill the text boxes with the related columns of the row.

            //int rowIndex = dtgCategories.SelectedIndex;

            DataRowView drv = (DataRowView)dtgCustomer.SelectedItem;

            txtId.Text = (drv[0]).ToString();//Selecting the specific row
            txtName.Text = (drv[2]).ToString();
            txtEmail.Text = (drv[3]).ToString();
            txtContact.Text = (drv[4]).ToString();
            txtAddress.Text = (drv[5]).ToString();
        }

        private void dtgCustomer_KeyUp(object sender, KeyEventArgs e)
        {
            DtgCustomerIndexChanged();
        }

        private void dtgCustomer_KeyDown(object sender, KeyEventArgs e)
        {
            DtgCustomerIndexChanged();
        }

        private void dtgCustomer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DtgCustomerIndexChanged();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Get Keyword from Text box
            string keyword = txtSearch.Text;

            //Check if the keyword has value or not

            if (keyword != null) /*Do NOT Repeat yourself!!! Improve if statement block!!! You have similar codes in the RefreshSupplierCustomerDataGrid method!!! */
            {
                //Show category informations based on the keyword
                DataTable dataTable = customerDAL.Search(keyword);
                dtgCustomer.ItemsSource = dataTable.DefaultView;
                dtgCustomer.AutoGenerateColumns = true;
                dtgCustomer.CanUserAddRows = false;
            }
            else
            {
                //Show all categories from the database
                RefreshCustomerDataGrid();
            }
        }
    }
}