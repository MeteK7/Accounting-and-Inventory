using BLL;
using CUL;
using CUL.Enums;
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
    /// Interaction logic for WinCategories.xaml
    /// </summary>
    public partial class WinCategory : Window
    {
        public WinCategory()
        {
            InitializeComponent();
            LoadCategoryDataGrid();
        }

        CategoryCUL categoryCUL = new CategoryCUL();
        CategoryDAL categoryDAL = new CategoryDAL();
        UserDAL userDAL = new UserDAL();
        UserBLL userBLL = new UserBLL();

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnCategoryAdd_Click(object sender, RoutedEventArgs e)
        {
            //Get the values from the Category Window and fill them into the categoryCUL.
            categoryCUL.Name = txtTitle.Text;
            categoryCUL.Description = txtDescription.Text;
            categoryCUL.AddedDate = DateTime.Now;
            categoryCUL.AddedBy = userBLL.GetUserId(WinLogin.loggedInUserName);


            //Creating a Boolean variable to insert data into the database.
            bool isSuccess = categoryDAL.Insert(categoryCUL);

            //If the category is inserted successfully, then the value of the variable isSuccess will be true; otherwise it will be false.
            if (isSuccess == true)
            {
                MessageBox.Show("New Category inserted successfully.");
                dtgCategories.Items.Clear();
                ClearCategoryTextBox();
                LoadCategoryDataGrid();
            }
            else
            {
                MessageBox.Show("Something went wrong :(");
            }
        }

        private void btnCategoryUpdate_Click(object sender, RoutedEventArgs e)
        {
            //Getting values from the CategoryUI

            categoryCUL.Id = Convert.ToInt32(txtCategoryId.Text);
            categoryCUL.Name = txtTitle.Text;
            categoryCUL.Description = txtDescription.Text;
            categoryCUL.AddedDate = DateTime.Now;
            categoryCUL.AddedBy = userBLL.GetUserId(WinLogin.loggedInUserName);

            //Updating Data into the database
            bool isSuccess = categoryDAL.Update(categoryCUL);

            if (isSuccess == true)
            {
                MessageBox.Show("Category successfully updated");
                dtgCategories.Items.Clear();
                ClearCategoryTextBox();
                LoadCategoryDataGrid();
            }
            else
            {
                MessageBox.Show("Failed to update category");
            }
        }

        private void btnCategoryDelete_Click(object sender, RoutedEventArgs e)
        {
            categoryCUL.Id = Convert.ToInt32(txtCategoryId.Text);

            bool isSuccess = categoryDAL.Delete(categoryCUL);

            if (isSuccess == true)
            {
                MessageBox.Show("Category has been deleted successfully.");
                dtgCategories.Items.Clear();
                ClearCategoryTextBox();
                LoadCategoryDataGrid();
            }
            else
            {
                MessageBox.Show("Something went wrong:/");
            }
        }

        private void LoadCategoryDataGrid()
        {
            //Refreshing Data Grid View
            //DataTable dataTable = categoryDAL.Select();
            //dtgCategories.ItemsSource = dataTable.DefaultView;
            //dtgCategories.AutoGenerateColumns = true;
            //dtgCategories.CanUserAddRows = false;

            int categoryId, addedById;
            string categoryName,categoryDescription, addedDate, addedByUsername;
            DataTable dataTable = categoryDAL.Select();
            DataTable dataTableUserInfo;

            //dtgs.ItemsSource = dataTable.DefaultView; Adds everything at once.
            dtgCategories.AutoGenerateColumns = true;
            dtgCategories.CanUserAddRows = false;

            #region LOADING THE PRODUCT DATA GRID

            for (int currentRow = (int)Numbers.InitialIndex; currentRow < dataTable.Rows.Count; currentRow++)
            {
                categoryId = Convert.ToInt32(dataTable.Rows[currentRow]["id"]);
                categoryName = dataTable.Rows[currentRow]["name"].ToString();
                categoryDescription = dataTable.Rows[currentRow]["description"].ToString();
                addedDate = dataTable.Rows[currentRow]["added_date"].ToString();

                addedById = Convert.ToInt32(dataTable.Rows[currentRow]["added_by"]);
                dataTableUserInfo = userDAL.GetUserInfoById(addedById);
                addedByUsername = dataTableUserInfo.Rows[(int)Numbers.InitialIndex]["first_name"].ToString() + " " + dataTableUserInfo.Rows[(int)Numbers.InitialIndex]["last_name"].ToString();

                dtgCategories.Items.Add(new { Id = categoryId, Name = categoryName, Description= categoryDescription, AddedDate = addedDate, AddedBy = addedByUsername });
            }
            #endregion
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
            object row = dtgCategories.SelectedItem;
            int rowCategoryId = 0, rowCategoryTitle = 1, rowCategoryDescription = 2;

            txtCategoryId.Text = (dtgCategories.Columns[rowCategoryId].GetCellContent(row) as TextBlock).Text;
            txtTitle.Text = (dtgCategories.Columns[rowCategoryTitle].GetCellContent(row) as TextBlock).Text;
            txtDescription.Text = (dtgCategories.Columns[rowCategoryDescription].GetCellContent(row) as TextBlock).Text;


            //DataRowView drv = (DataRowView)dtgCategories.SelectedItem;

            //txtCategoryId.Text = (drv[0]).ToString();//Selecting the specific row
            //txtTitle.Text = (drv["name"]).ToString();//You could also define the column name from your table.
            //txtDescription.Text = (drv[2]).ToString();
        }

        private void txtCategorySearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Get Keyword from Text box
            string keyword = txtCategorySearch.Text;

            //Check if the keyword has value or not

            if (keyword != null) /*Do NOT Repeat yourself!!! Improve if statement block!!! You have similar codes in the RefreshCategoryDataGrid method!!! */
            {
                dtgCategories.Items.Clear();

                //Show category informations based on the keyword.
                DataTable dtCategory = categoryDAL.Search(keyword);

                for (int rowIndex = (int)Numbers.InitialIndex; rowIndex < dtCategory.Rows.Count; rowIndex++)
                {
                    dtgCategories.Items.Add(
                        new CategoryCUL()
                        {
                            Id = Convert.ToInt32(dtCategory.Rows[rowIndex]["id"]),
                            Name = dtCategory.Rows[rowIndex]["name"].ToString(),
                            Description = dtCategory.Rows[rowIndex]["description"].ToString(),
                            AddedDate = Convert.ToDateTime(dtCategory.Rows[rowIndex]["added_date"]),
                            AddedBy = Convert.ToInt32(dtCategory.Rows[rowIndex]["added_by"])
                        });
                }
            }
            else
            {
                //Show all categories from the database.
                LoadCategoryDataGrid();
            }
        }
    }
}
