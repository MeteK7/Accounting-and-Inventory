using BLL;
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
    /// Interaction logic for WinSupplier.xaml
    /// </summary>
    public partial class WinSupplier : Window
    {
        public WinSupplier()
        {
            InitializeComponent();
            RefreshSupplierDataGrid();
        }

        SupplierCUL supplierCUL = new SupplierCUL();
        SupplierDAL supplierDAL = new SupplierDAL();

        UserDAL userDAL = new UserDAL();
        UserBLL userBLL = new UserBLL();

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            //Get the values from the Supplier Window and fill them into the supplierCUL.

            supplierCUL.Name = txtName.Text;
            supplierCUL.Email = txtEmail.Text;
            supplierCUL.Contact = txtContact.Text;
            supplierCUL.Address = txtAddress.Text;
            supplierCUL.AddedDate = DateTime.Now;
            supplierCUL.AddedBy = userBLL.GetUserId(WinLogin.loggedInUserName);


            //Creating a Boolean variable to insert data into the database.
            bool isSuccess = supplierDAL.Insert(supplierCUL);

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

            supplierCUL.Id = Convert.ToInt32(txtId.Text);
            supplierCUL.Name = txtName.Text;
            supplierCUL.Email = txtEmail.Text;
            supplierCUL.Contact = txtContact.Text;
            supplierCUL.Address = txtAddress.Text;
            supplierCUL.AddedDate = DateTime.Now;
            supplierCUL.AddedBy = userBLL.GetUserId(WinLogin.loggedInUserName);

            //Updating Data into the database
            bool isSuccess = supplierDAL.Update(supplierCUL);

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
            supplierCUL.Id = Convert.ToInt32(txtId.Text);

            bool isSuccess = supplierDAL.Delete(supplierCUL);

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
            if (drv != null)
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