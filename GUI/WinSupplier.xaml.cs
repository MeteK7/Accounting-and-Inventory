using BLL;
using KabaAccounting.CUL;
using DAL;
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
            LoadSupplierDataGrid();
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
            #region NEW SUPPLIER INSERTION

            #endregion
            //Get the values from the Supplier Window and fill them into the supplierCUL.

            supplierCUL.Name = txtName.Text;
            supplierCUL.Email = txtEmail.Text;
            supplierCUL.Contact = txtContact.Text;
            supplierCUL.Address = txtAddress.Text;
            supplierCUL.AddedDate = DateTime.Now;
            supplierCUL.AddedBy = userBLL.GetUserId(WinLogin.loggedInUserName);


            //Creating a Boolean variable to insert data into the database.
            bool isSuccessSupplier = supplierDAL.Insert(supplierCUL);

            //If the data is inserted successfully, then the value of the variable isSuccess will be true; otherwise it will be false.
            if (isSuccessSupplier == true)
            {
                MessageBox.Show("New data inserted successfully.");
                dtgSuppliers.Items.Clear();
                ClearSupplierTextBox();
                LoadSupplierDataGrid();
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
                dtgSuppliers.Items.Clear();
                ClearSupplierTextBox();
                LoadSupplierDataGrid();
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
                dtgSuppliers.Items.Clear();
                ClearSupplierTextBox();
                LoadSupplierDataGrid();
            }
            else
            {
                MessageBox.Show("Something went wrong:/");
            }
        }

        private void LoadSupplierDataGrid()//Try to modify it by creating an optional parameter.
        {
            //Refreshing Data Grid View
            //DataTable dataTable = supplierDAL.Select();
            //dtgSupplier.ItemsSource = dataTable.DefaultView;
            //dtgSupplier.AutoGenerateColumns = true;
            //dtgSupplier.CanUserAddRows = false;

            int firstRowIndex = 0, supplierId, addedById;
            string supplierName, supplierEmail, supplierContact, supplierAddress, addedDate, addedByUsername;
            DataTable dataTable = supplierDAL.Select();
            DataTable dataTableUserInfo;

            //dtgs.ItemsSource = dataTable.DefaultView; Adds everything at once.
            dtgSuppliers.AutoGenerateColumns = true;
            dtgSuppliers.CanUserAddRows = false;

            #region LOADING THE PRODUCT DATA GRID

            for (int currentRow = firstRowIndex; currentRow < dataTable.Rows.Count; currentRow++)
            {
                supplierId = Convert.ToInt32(dataTable.Rows[currentRow]["id"]);
                supplierName = dataTable.Rows[currentRow]["name"].ToString();
                supplierEmail = dataTable.Rows[currentRow]["email"].ToString();
                supplierContact = dataTable.Rows[currentRow]["contact"].ToString();
                supplierAddress = dataTable.Rows[currentRow]["address"].ToString();
                addedDate = dataTable.Rows[currentRow]["added_date"].ToString();
                addedById = Convert.ToInt32(dataTable.Rows[currentRow]["added_by"]);
                dataTableUserInfo = userDAL.GetUserInfoById(addedById);
                addedByUsername = dataTableUserInfo.Rows[firstRowIndex]["first_name"].ToString() + " " + dataTableUserInfo.Rows[firstRowIndex]["last_name"].ToString();

                dtgSuppliers.Items.Add(new { Id = supplierId, Name = supplierName, Email = supplierEmail, Contact = supplierContact, Address = supplierAddress, AddedDate = addedDate, AddedBy = addedByUsername });
            }
            #endregion
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

        private void dtgSupplierIndexChanged()
        {
            object row = dtgSuppliers.SelectedItem;
            int colSupplierId = 0, colSupplierName = 1, colSupplierEmail = 2, colSupplierContact = 3, colSupplierAddress = 4;

            txtId.Text = (dtgSuppliers.Columns[colSupplierId].GetCellContent(row) as TextBlock).Text;
            txtName.Text = (dtgSuppliers.Columns[colSupplierName].GetCellContent(row) as TextBlock).Text;
            txtEmail.Text = (dtgSuppliers.Columns[colSupplierEmail].GetCellContent(row) as TextBlock).Text;
            txtContact.Text = (dtgSuppliers.Columns[colSupplierContact].GetCellContent(row) as TextBlock).Text;
            txtAddress.Text = (dtgSuppliers.Columns[colSupplierAddress].GetCellContent(row) as TextBlock).Text;


            //DataRowView drv = (DataRowView)dtgSupplier.SelectedItem;
            //if (drv != null)
            //{
            //    txtId.Text = (drv[0]).ToString();//Selecting the specific row
            //    txtName.Text = (drv[2]).ToString();
            //    txtEmail.Text = (drv[3]).ToString();
            //    txtContact.Text = (drv[4]).ToString();
            //    txtAddress.Text = (drv[5]).ToString();
            //}
        }


        private void dtgSupplier_KeyUp(object sender, KeyEventArgs e)
        {
            dtgSupplierIndexChanged();
        }

        private void dtgSupplier_KeyDown(object sender, KeyEventArgs e)
        {
            dtgSupplierIndexChanged();
        }

        private void dtgSupplier_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            dtgSupplierIndexChanged();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Get Keyword from Text box
            string keyword = txtSearch.Text;

            //Check if the keyword has value or not

            if (keyword != null) /*Do NOT Repeat yourself!!! Improve if statement block!!! You have similar codes in the RefreshSupplierCustomerDataGrid method!!! */
            {
                dtgSuppliers.Items.Clear();

                //Show supplier informations based on the keyword.
                DataTable dtSupplier = supplierDAL.Search(keyword);

                for (int rowIndex = 0; rowIndex < dtSupplier.Rows.Count; rowIndex++)
                {
                    dtgSuppliers.Items.Add(
                        new SupplierCUL()
                        {
                            Id = Convert.ToInt32(dtSupplier.Rows[rowIndex]["id"]),
                            Name = dtSupplier.Rows[rowIndex]["name"].ToString(),
                            Email = dtSupplier.Rows[rowIndex]["email"].ToString(),
                            Contact = dtSupplier.Rows[rowIndex]["contact"].ToString(),
                            Address = dtSupplier.Rows[rowIndex]["address"].ToString(),
                            AddedDate = Convert.ToDateTime(dtSupplier.Rows[rowIndex]["added_date"]),
                            AddedBy = Convert.ToInt32(dtSupplier.Rows[rowIndex]["added_by"])
                        });
                }
            }
            else
            {
                //Show all suppliers from the database.
                LoadSupplierDataGrid();
            }
        }

        private void dtgSupplier_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ClearSupplierTextBox();
            dtgSuppliers.UnselectAll();

        }
    }
}