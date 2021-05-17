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
    /// Interaction logic for WinCustomer.xaml
    /// </summary>
    public partial class WinCustomer : Window
    {
        public WinCustomer()
        {
            InitializeComponent();
            LoadCustomerDataGrid();
        }
        CustomerCUL customerCUL = new CustomerCUL();
        CustomerDAL customerDAL = new CustomerDAL();

        UserDAL userDAL = new UserDAL();
        UserBLL userBLL = new UserBLL();

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
            customerCUL.AddedBy = userBLL.GetUserId(WinLogin.loggedInUserName);


            //Creating a Boolean variable to insert data into the database.
            bool isSuccess = customerDAL.Insert(customerCUL);

            //If the data is inserted successfully, then the value of the variable isSuccess will be true; otherwise it will be false.
            if (isSuccess == true)
            {
                MessageBox.Show("New data inserted successfully.");
                dtgCustomers.Items.Clear();
                ClearCustomerTextBox();
                LoadCustomerDataGrid();
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
            customerCUL.AddedBy = userBLL.GetUserId(WinLogin.loggedInUserName);

            //Updating Data into the database
            bool isSuccess = customerDAL.Update(customerCUL);

            if (isSuccess == true)
            {
                MessageBox.Show("Data successfully updated.");
                dtgCustomers.Items.Clear();
                ClearCustomerTextBox();
                LoadCustomerDataGrid();
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
                dtgCustomers.Items.Clear();
                ClearCustomerTextBox();
                LoadCustomerDataGrid();
            }
            else
            {
                MessageBox.Show("Something went wrong:/");
            }
        }

        private void LoadCustomerDataGrid()//Try to modify it by creating an optional parameter.
        {
            //Refreshing Data Grid View
            //DataTable dataTable = customerDAL.Select();
            //dtgCustomer.ItemsSource = dataTable.DefaultView;
            //dtgCustomer.AutoGenerateColumns = true;
            //dtgCustomer.CanUserAddRows = false;

            int firstRowIndex = 0, customerId, addedById;
            string customerName,customerEmail, customerContact,customerAddress, addedDate, addedByUsername;
            DataTable dataTable = customerDAL.Select();
            DataTable dataTableUserInfo;

            //dtgs.ItemsSource = dataTable.DefaultView; Adds everything at once.
            dtgCustomers.AutoGenerateColumns = true;
            dtgCustomers.CanUserAddRows = false;

            #region LOADING THE PRODUCT DATA GRID

            for (int currentRow = firstRowIndex; currentRow < dataTable.Rows.Count; currentRow++)
            {
                customerId = Convert.ToInt32(dataTable.Rows[currentRow]["id"]);
                customerName = dataTable.Rows[currentRow]["name"].ToString();
                customerEmail= dataTable.Rows[currentRow]["email"].ToString();
                customerContact= dataTable.Rows[currentRow]["contact"].ToString();
                customerAddress = dataTable.Rows[currentRow]["address"].ToString();
                addedDate = dataTable.Rows[currentRow]["added_date"].ToString();
                addedById = Convert.ToInt32(dataTable.Rows[currentRow]["added_by"]);
                dataTableUserInfo = userDAL.GetUserInfoById(addedById);
                addedByUsername = dataTableUserInfo.Rows[firstRowIndex]["first_name"].ToString() + " " + dataTableUserInfo.Rows[firstRowIndex]["last_name"].ToString();

                dtgCustomers.Items.Add(new { Id = customerId, Name = customerName,Email=customerEmail,Contact=customerContact,Address=customerAddress, AddedDate = addedDate, AddedBy = addedByUsername });
            }
            #endregion
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
            object row = dtgCustomers.SelectedItem;
            int colCustomerId = 0, colCustomerName = 1, colCustomerEmail = 2,colCustomerContact=3, colCustomerAddress = 4;

            txtId.Text = (dtgCustomers.Columns[colCustomerId].GetCellContent(row) as TextBlock).Text;
            txtName.Text = (dtgCustomers.Columns[colCustomerName].GetCellContent(row) as TextBlock).Text;
            txtEmail.Text = (dtgCustomers.Columns[colCustomerEmail].GetCellContent(row) as TextBlock).Text;
            txtContact.Text = (dtgCustomers.Columns[colCustomerContact].GetCellContent(row) as TextBlock).Text;
            txtAddress.Text = (dtgCustomers.Columns[colCustomerAddress].GetCellContent(row) as TextBlock).Text;

            //DataRowView drv = (DataRowView)dtgCustomer.SelectedItem;

            //txtId.Text = (drv[0]).ToString();//Selecting the specific row
            //txtName.Text = (drv[1]).ToString();
            //txtEmail.Text = (drv[2]).ToString();
            //txtContact.Text = (drv[3]).ToString();
            //txtAddress.Text = (drv[4]).ToString();
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
                dtgCustomers.ItemsSource = dataTable.DefaultView;
                dtgCustomers.AutoGenerateColumns = true;
                dtgCustomers.CanUserAddRows = false;
            }
            else
            {
                //Show all categories from the database
                LoadCustomerDataGrid();
            }
        }
    }
}