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
    /// Interaction logic for WinUsers.xaml
    /// </summary>
    public partial class WinUsers : Window
    {
        public WinUsers()
        {
            InitializeComponent();
        }

        UserBLL usrBLL = new UserBLL();
        UserDAL usrDAL = new UserDAL();

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnUserAdd_Click(object sender, RoutedEventArgs e)
        {
            //Gettting Data from UI
            usrBLL.FirstName = txtFirstName.Text;
            usrBLL.LastName = txtLastName.Text;
            usrBLL.Email = txtUserEmail.Text;
            usrBLL.UserName = txtUserName.Text;
            usrBLL.Password = txtUserPassword.Text;
            usrBLL.Contact = txtUserContact.Text;
            usrBLL.Address = txtUserAddress.Text;
            usrBLL.Gender = cboUserGender.Text;
            usrBLL.UserType = cboUserType.Text;
            usrBLL.AddedDate = DateTime.Now;
            usrBLL.AddedBy = 1;

            //Inserting Data into the Database
            bool success = usrDAL.Insert(usrBLL);

            if (success==true)
            {
                MessageBox.Show("Data inserted successfully.");
                ClearUserTextBox();
            }
            else
            {
                MessageBox.Show("Something went wrong:(");
            }

            RefreshUserDataGrid();
        }

        private void dtgUsers_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshUserDataGrid();
        }

        private void RefreshUserDataGrid()
        {
            //Refreshing Data Grid View
            DataTable dataTable = usrDAL.Select();
            dtgUsers.ItemsSource = dataTable.DefaultView;
            dtgUsers.AutoGenerateColumns = true;
            dtgUsers.CanUserAddRows = false;
        }

        private void ClearUserTextBox()
        {
            txtUserId.Text = "";
            txtFirstName.Text="";
            txtLastName.Text = "";
            txtUserEmail.Text = "";
            txtUserName.Text = "";
            txtUserPassword.Text = "";
            txtUserContact.Text = "";
            txtUserAddress.Text = "";
            cboUserGender.Text = "";
            cboUserType.Text = "";
        }

        private void dtgUsers_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Getting the index of a particular row

            //int rowIndex = dtgUsers.SelectedIndex;

            DataRowView drv = (DataRowView)dtgUsers.SelectedItem;

            txtUserId.Text = (drv[0]).ToString();//Selecting the specific ro
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

        private void btnUserUpdate_Click(object sender, RoutedEventArgs e)
        {
            //Getting values from the UserUI
            usrBLL.Id = Convert.ToInt32(txtUserId.Text);
            usrBLL.FirstName=txtFirstName.Text;
            usrBLL.LastName = txtLastName.Text;
            usrBLL.Email = txtUserEmail.Text;
            usrBLL.UserName = txtUserName.Text;
            usrBLL.Password = txtUserPassword.Text;
            usrBLL.Contact = txtUserContact.Text;
            usrBLL.Address = txtUserAddress.Text;
            usrBLL.Gender = cboUserGender.Text;
            usrBLL.UserType = cboUserType.Text;
            usrBLL.AddedDate = DateTime.Now;
            usrBLL.AddedBy = 1;

            //Updating Data into the database
            bool success = usrDAL.Update(usrBLL);

            if (success==true)
            {
                MessageBox.Show("User successfully updated");
                ClearUserTextBox();
            }
            else
            {
                MessageBox.Show("Failed to update user");
            }
            RefreshUserDataGrid();
        }

        private void btnUserDelete_Click(object sender, RoutedEventArgs e)
        {
            usrBLL.Id = Convert.ToInt32(txtUserId.Text);

            bool success = usrDAL.Delete(usrBLL);

            if (success==true)
            {
                MessageBox.Show("User has been deleted successfully.");
                ClearUserTextBox();
            }
            else
            {
                MessageBox.Show("Something went wrong:/");
            }
            RefreshUserDataGrid();
        }

        private void txtUserSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Get Keyword from Text box
            string keyword = txtUserSearch.Text;

            //Check if the keyword has value or not

            if (keyword!=null) /*Do NOT Repeat yourself!!! Improve if statement block!!! You have similar codes in the RefreshDataGridUsers method!!! */
            {
                //Show user informations based on the keyword
                DataTable dataTable = usrDAL.Search(keyword);
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
    }
}
