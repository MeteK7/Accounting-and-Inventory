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
    /// Interaction logic for WinSupplierCustomer.xaml
    /// </summary>
    public partial class WinSupplier : Window
    {
        public WinSupplier()
        {
            InitializeComponent();
            RefreshSupplierDataGrid();
        }

        SupplierBLL supplierBLL = new SupplierBLL();
        SupplierDAL supplierDAL = new SupplierDAL();

        UserDAL userDAL = new UserDAL();

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //Get the values from the Supplier Window and fill them into the supplierBLL.

            supplierBLL.Name = txtName.Text;
            supplierBLL.Email = txtEmail.Text;
            supplierBLL.Contact = txtContact.Text;
            supplierBLL.Address = txtAddress.Text;
            supplierBLL.AddedDate = DateTime.Now;
            supplierBLL.AddedBy = GetUserId();


            //Creating a Boolean variable to insert data into the database.
            bool isSuccess = supplierDAL.Insert(supplierBLL);

            //If the data is inserted successfully, then the value of the variable isSuccess will be true; otherwise it will be false.
            if (isSuccess == true)
            {
                MessageBox.Show("New data inserted successfully.");
                ClearSupplierTextBox();
                RefreshSupplierDataGrid();
            }
            else
            {
                MessageBox.Show("Something went wrong :(");
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            //Getting values from the WinSupplier

            supplierBLL.Id = Convert.ToInt32(txtId.Text);
            supplierBLL.Name = txtName.Text;
            supplierBLL.Email = txtEmail.Text;
            supplierBLL.Contact = txtContact.Text;
            supplierBLL.Address = txtAddress.Text;
            supplierBLL.AddedDate = DateTime.Now;
            supplierBLL.AddedBy = GetUserId();

            //Updating Data into the database
            bool isSuccess = supplierDAL.Update(supplierBLL);

            if (isSuccess == true)
            {
                MessageBox.Show("Data successfully updated.");
                ClearSupplierTextBox();
                RefreshSupplierDataGrid();
            }
            else
            {
                MessageBox.Show("Failed to update category");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            supplierBLL.Id = Convert.ToInt32(txtId.Text);

            bool isSuccess = supplierDAL.Delete(supplierBLL);

            if (isSuccess == true)
            {
                MessageBox.Show("Data has been deleted successfully.");
                ClearSupplierTextBox();
                RefreshSupplierDataGrid();
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

        private void RefreshSupplierDataGrid()//Try to modify it by creating an optional parameter.
        {
            //Refreshing Data Grid View
            DataTable dataTable = supplierDAL.Select();
            dgSupplier.ItemsSource = dataTable.DefaultView;
            dgSupplier.AutoGenerateColumns = true;
            dgSupplier.CanUserAddRows = false;
        }

        private void ClearSupplierTextBox()
        {
            txtId.Text = "";
            txtName.Text = "";
            txtEmail.Text = "";
            txtContact.Text = "";
            txtAddress.Text = "";
            txtSearch.Text = "";
            
        }

        private void dgSupplierIndexChanged()
        {
            //Getting the index of a particular row and fill the text boxes with the related columns of the row.

            //int rowIndex = dgCategories.SelectedIndex;

            DataRowView drv = (DataRowView)dgSupplier.SelectedItem;
            if (drv!=null)
            {
                txtId.Text = (drv[0]).ToString();//Selecting the specific row
                txtName.Text = (drv[2]).ToString();
                txtEmail.Text = (drv[3]).ToString();
                txtContact.Text = (drv[4]).ToString();
                txtAddress.Text = (drv[5]).ToString();
            }
        }


        private void dgSupplier_KeyUp(object sender, KeyEventArgs e)
        {
                dgSupplierIndexChanged();
        }

        private void dgSupplier_KeyDown(object sender, KeyEventArgs e)
        {
            dgSupplierIndexChanged();
        }

        private void dgSupplier_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            dgSupplierIndexChanged();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Get Keyword from Text box
            string keyword = txtSearch.Text;

            //Check if the keyword has value or not

            if (keyword != null) /*Do NOT Repeat yourself!!! Improve if statement block!!! You have similar codes in the RefreshSupplierCustomerDataGrid method!!! */
            {
                //Show category informations based on the keyword
                DataTable dataTable = supplierDAL.Search(keyword);
                dgSupplier.ItemsSource = dataTable.DefaultView;
                dgSupplier.AutoGenerateColumns = true;
                dgSupplier.CanUserAddRows = false;
            }
            else
            {
                //Show all categories from the database
                RefreshSupplierDataGrid();
            }
        }

        private void dgSupplier_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ClearSupplierTextBox();
            dgSupplier.UnselectAll();
            
        }
    }
}
