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
    /// Interaction logic for WinCategories.xaml
    /// </summary>
    public partial class WinCategories : Window
    {
        public WinCategories()
        {
            InitializeComponent();
            RefreshCategoryDataGrid();
        }

        CategoryBLL categoryBLL = new CategoryBLL();
        CategoryDAL categoryDAL = new CategoryDAL();
        UserDAL userDAL = new UserDAL();

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCategoryAdd_Click(object sender, RoutedEventArgs e)
        {
            //Get the values from the Category Window and fill them into the categoryBLL.
            categoryBLL.Title = txtTitle.Text;
            categoryBLL.Description = txtDescription.Text;
            categoryBLL.AddedDate = DateTime.Now;
            categoryBLL.AddedBy= GetUserId();


            //Creating a Boolean variable to insert data into the database.
            bool isSuccess = categoryDAL.Insert(categoryBLL);

            //If the category is inserted successfull, then the value of the variable isSuccess will be true; otherwise it will be false.
            if (isSuccess==true)
            {
                MessageBox.Show("New Category inserted successfully.");
                ClearCategoryTextBox();
                RefreshCategoryDataGrid();
            }
            else
            {
                MessageBox.Show("Something went wrong :(");
            }
        }
        private int GetUserId()
        {
            //Getting the name of the user from the Login Window and fill it into a string variable;
            string loggedUser = WinLogin.loggedIn;

            //Calling the method named GetIdFromUsername in the userDAL and sending the variable loggedUser as a parameter into it.
            //Then, fill the result into the userBLL;
            UserBLL userBLL = userDAL.GetIdFromUsername(loggedUser);

            int userId = userBLL.Id;

            return userId;
        }

        private void RefreshCategoryDataGrid()
        {
            //Refreshing Data Grid View
            DataTable dataTable = categoryDAL.Select();
            dtgCategories.ItemsSource = dataTable.DefaultView;
            dtgCategories.AutoGenerateColumns = true;
            dtgCategories.CanUserAddRows = false;
        }

        private void ClearCategoryTextBox()
        {
            txtCategoryId.Text = "";
            txtTitle.Text = "";
            txtDescription.Text = "";
            txtCategorySearch.Text = "";
        }

        private void dtgCategories_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DtgCategoriesIndexChanged();
        }
        private void dtgCategories_KeyUp(object sender, KeyEventArgs e)
        {
            DtgCategoriesIndexChanged();
        }
        private void dtgCategories_KeyDown(object sender, KeyEventArgs e)
        {
            DtgCategoriesIndexChanged();
        }

        private void DtgCategoriesIndexChanged()
        {
            //Getting the index of a particular row and fill the text boxes with the related columns of the row.

            //int rowIndex = dtgCategories.SelectedIndex;

            DataRowView drv = (DataRowView)dtgCategories.SelectedItem;

            txtCategoryId.Text = (drv[0]).ToString();//Selecting the specific row
            txtTitle.Text = (drv["title"]).ToString();//You could also define the column name from your table.
            txtDescription.Text = (drv[2]).ToString();
        }

        private void btnCategoryUpdate_Click(object sender, RoutedEventArgs e)
        {
            //Getting values from the CategoryUI

            categoryBLL.Id = Convert.ToInt32(txtCategoryId.Text);
            categoryBLL.Title = txtTitle.Text;
            categoryBLL.Description = txtDescription.Text;
            categoryBLL.AddedDate = DateTime.Now;
            categoryBLL.AddedBy = GetUserId();

            //Updating Data into the database
            bool isSuccess = categoryDAL.Update(categoryBLL);

            if (isSuccess == true)
            {
                MessageBox.Show("Category successfully updated");
                ClearCategoryTextBox();
                RefreshCategoryDataGrid();
            }
            else
            {
                MessageBox.Show("Failed to update category");
            }
        }

        private void btnCategoryDelete_Click(object sender, RoutedEventArgs e)
        {
            categoryBLL.Id = Convert.ToInt32(txtCategoryId.Text);

            bool isSuccess = categoryDAL.Delete(categoryBLL);

            if (isSuccess == true)
            {
                MessageBox.Show("Category has been deleted successfully.");
                ClearCategoryTextBox();
                RefreshCategoryDataGrid();
            }
            else
            {
                MessageBox.Show("Something went wrong:/");
            } 
        }

        private void txtCategorySearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Get Keyword from Text box
            string keyword = txtCategorySearch.Text;

            //Check if the keyword has value or not

            if (keyword != null) /*Do NOT Repeat yourself!!! Improve if statement block!!! You have similar codes in the RefreshCategoryDataGrid method!!! */
            {
                //Show category informations based on the keyword
                DataTable dataTable = categoryDAL.Search(keyword);
                dtgCategories.ItemsSource = dataTable.DefaultView;
                dtgCategories.AutoGenerateColumns = true;
                dtgCategories.CanUserAddRows = false;
            }
            else
            {
                //Show all categories from the database
                RefreshCategoryDataGrid();
            }
        }
    }
}
