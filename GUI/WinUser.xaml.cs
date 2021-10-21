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
    /// Interaction logic for WinUser.xaml
    /// </summary>
    public partial class WinUser : Window
    {
        public WinUser()
        {
            InitializeComponent();
            RefreshUserDataGrid();
        }

        UserCUL userCUL = new UserCUL();
        UserDAL userDAL = new UserDAL();

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnUserAdd_Click(object sender, RoutedEventArgs e)
        {
            //Gettting Data from UI
            userCUL.FirstName = txtFirstName.Text;
            userCUL.LastName = txtLastName.Text;
            userCUL.Email = txtUserEmail.Text;
            userCUL.UserName = txtUserName.Text;
            userCUL.Password = txtUserPassword.Text;
            userCUL.Contact = txtUserContact.Text;
            userCUL.Address = txtUserAddress.Text;
            userCUL.Gender = cboUserGender.Text;
            userCUL.UserType = cboUserType.Text;
            userCUL.AddedDate = DateTime.Now;

            //Getting username of the logged in user
            string loggedUser = WinLogin.loggedInUserName;

            //Getting ID of the user who is logged in
            UserCUL userAddedBy = userDAL.GetIdFromUsername(loggedUser);

            userCUL.AddedBy = userAddedBy.Id;

            //Inserting Data into the Database
            bool isSuccess = userDAL.Insert(userCUL);

            if (isSuccess == true)
            {
                MessageBox.Show("Data inserted successfully.");
                ClearUserTextBox();
                RefreshUserDataGrid();
            }
            else
            {
                MessageBox.Show("Something went wrong:(");
            }
        }

        private void RefreshUserDataGrid()
        {
            //Refreshing Data Grid View
            DataTable dataTable = userDAL.Select();
            dtgUsers.ItemsSource = dataTable.DefaultView;
            dtgUsers.AutoGenerateColumns = true;
            dtgUsers.CanUserAddRows = false;
        }

        private void ClearUserTextBox()
        {
            txtUserId.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtUserEmail.Text = "";
            txtUserName.Text = "";
            txtUserPassword.Text = "";
            txtUserContact.Text = "";
            txtUserAddress.Text = "";
            cboUserGender.Text = "";
            cboUserType.Text = "";
        }

        private void btnUserUpdate_Click(object sender, RoutedEventArgs e)
        {
            //Getting values from the UserUI
            userCUL.Id = Convert.ToInt32(txtUserId.Text);
            userCUL.FirstName = txtFirstName.Text;
            userCUL.LastName = txtLastName.Text;
            userCUL.Email = txtUserEmail.Text;
            userCUL.UserName = txtUserName.Text;
            userCUL.Password = txtUserPassword.Text;
            userCUL.Contact = txtUserContact.Text;
            userCUL.Address = txtUserAddress.Text;
            userCUL.Gender = cboUserGender.Text;
            userCUL.UserType = cboUserType.Text;
            userCUL.AddedDate = DateTime.Now;
            userCUL.AddedBy = 1;

            //Updating Data into the database
            bool isSuccess = userDAL.Update(userCUL);

            if (isSuccess == true)
            {
                MessageBox.Show("User successfully updated");
                ClearUserTextBox();
                RefreshUserDataGrid();
            }
            else
            {
                MessageBox.Show("Failed to update user");
            }
        }

        private void btnUserDelete_Click(object sender, RoutedEventArgs e)
        {
            userCUL.Id = Convert.ToInt32(txtUserId.Text);

            bool isSuccess = userDAL.Delete(userCUL);

            if (isSuccess == true)
            {
                MessageBox.Show("User has been deleted successfully.");
                ClearUserTextBox();
                RefreshUserDataGrid();
            }
            else
            {
                MessageBox.Show("Something went wrong:/");
            }
        }

        private void txtUserSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Get Keyword from Text box
            string keyword = txtUserSearch.Text;

            //Check if the keyword has value or not

            if (keyword != null) /*Do NOT Repeat yourself!!! Improve if statement block!!! You have similar codes in the RefreshDataGridUsers method!!! */
            {
                //Show user informations based on the keyword
                DataTable dataTable = userDAL.Search(keyword);
                dtgUsers.ItemsSource = dataTable.DefaultView;
                dtgUsers.AutoGenerateColumns = true;
                dtgUsers.CanUserAddRows = false;
            }
            else
            {
                //Show all users from the database
                RefreshUserDataGrid();
            }
        }

        private void dtgUsers_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DtgUsersIndexChanged();
        }
        private void dtgUsers_KeyUp(object sender, KeyEventArgs e)
        {
            DtgUsersIndexChanged();
        }
        private void dtgUsers_KeyDown(object sender, KeyEventArgs e)
        {
            DtgUsersIndexChanged();
        }
        private void DtgUsersIndexChanged()
        {
            //Getting the index of a particular row

            //int rowIndex = dtgUsers.SelectedIndex;

            DataRowView drv = (DataRowView)dtgUsers.SelectedItem;

            txtUserId.Text = (drv[0]).ToString();//Selecting the specific row
            txtFirstName.Text = (drv["first_name"]).ToString();//You could also define the column name from your table.
            txtLastName.Text = (drv[2]).ToString();
            txtUserEmail.Text = (drv[3]).ToString();
            txtUserName.Text = (drv[4]).ToString();
            txtUserPassword.Text = (drv[5]).ToString();
            txtUserContact.Text = (drv[6]).ToString();
            txtUserAddress.Text = (drv[7]).ToString();
            cboUserGender.Text = (drv[8]).ToString();
            cboUserType.Text = (drv[9]).ToString();
        }

    }
}
