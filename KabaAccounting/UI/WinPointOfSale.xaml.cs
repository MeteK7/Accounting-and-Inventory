using KabaAccounting.BLL;
using KabaAccounting.DAL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for WinPointOfSale.xaml
    /// </summary>
    public partial class WinPointOfSale : Window
    {
        public WinPointOfSale()
        {
            InitializeComponent();
            btnProductAdd.IsEnabled = false;//Disabling the add button for the first run.
            btnProductClear.IsEnabled = false;//Disabling the clear button for the first run.
        }
        UserDAL userDAL = new UserDAL();
        PointOfSaleDAL pointOfSaleDAL = new PointOfSaleDAL();
        PointOfSaleBLL pointOfSaleBLL = new PointOfSaleBLL();
        PointOfSaleDetailDAL pointOfSaleDetailDAL = new PointOfSaleDetailDAL();
        PointOfSaleDetailBLL pointOfSaleDetailBLL = new PointOfSaleDetailBLL();
        ProductDAL productDAL = new ProductDAL();
        ProductBLL productBLL = new ProductBLL();
        CustomerDAL customerDAL = new CustomerDAL();
        CustomerBLL customerBLL = new CustomerBLL();
        UnitDAL unitDAL = new UnitDAL();
        UnitBLL unitBLL = new UnitBLL();
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            //Get the values from the POS Window and fill them into the pointOfSaleBLL.
            pointOfSaleBLL.SaleType = cboSaleType.Text;
            pointOfSaleBLL.CustomerId = Convert.ToInt32(cboCustomer.SelectedItem);
            pointOfSaleBLL.SubTotal = Convert.ToDecimal(txtSubTotal.Text);
            pointOfSaleBLL.Vat = Convert.ToDecimal(txtVat.Text);
            pointOfSaleBLL.Discount = Convert.ToDecimal(txtDiscount.Text);
            pointOfSaleBLL.GrandTotal = Convert.ToDecimal(txtTotal.Text);
            pointOfSaleBLL.AddedDate = DateTime.Now;
            pointOfSaleBLL.AddedBy = GetUserId();


            //Creating a Boolean variable to insert data into the database.
            bool isSuccess = pointOfSaleDAL.Insert(pointOfSaleBLL);


            #region TABLE POS DETAILS SAVING SECTION

            #endregion


            //If the data is inserted successfully, then the value of the variable isSuccess will be true; otherwise it will be false.
            if (isSuccess == true)
            {
                ClearPointOfSaleTextBox();
                ClearPointOfSaleListView();
            }
            else
            {
                MessageBox.Show("Something went wrong :(");
            }
        }

        private void ClearPointOfSaleTextBox()
        {
            txtTotalProducts.Text = "";
            txtSubTotal.Text = "";
            txtVat.Text = "";
            txtDiscount.Text = "";
            txtTotal.Text = "";
        }

        private void ClearPointOfSaleListView()
        {
            dgProducts.Items.Clear();
        }

        private void ClearProductEntranceTextBox()
        {
            txtProductBarcode.Text="";
            txtProductName.Text = "";
            cboProductUnit.SelectedIndex = -1;
            txtProductPrice.Text = "";
            txtProductAmount.Text = "";
            txtProductTotalPrice.Text = "";
            Keyboard.Focus(txtProductBarcode); // set keyboard focus
            btnProductAdd.IsEnabled = false; //Disabling the add button if all text boxes are cleared.
            btnProductClear.IsEnabled = false; //Disabling the clear button if all text boxes are cleared.
        }
        private void txtProductBarcode_KeyUp(object sender, KeyEventArgs e)
        {
            DataTable dataTable = productDAL.SearchProductId(txtProductBarcode.Text);

            int number;
            if (txtProductBarcode.Text != 0.ToString() && int.TryParse(txtProductBarcode.Text, out number) && dataTable.Rows.Count!=0)//Validating the barcode if it is a number(except zero) or not.
            {
                btnProductAdd.IsEnabled = true; //Enabling the add button if any valid barcode is entered.
                btnProductClear.IsEnabled = true;//Enabling the clear button if any valid barcode is entered.

                int productAmount = 1;
                int rowIndex = 0;
                int productId;
                int productUnit = 0;
                string productBarcodeRetail/*, productBarcodeWholesale*/;

                productId = Convert.ToInt32(dataTable.Rows[rowIndex]["id"]);
                productBarcodeRetail = dataTable.Rows[rowIndex]["barcode_retail"].ToString();
                //productBarcodeWholesale = dataTable.Rows[rowIndex]["barcode_wholesale"].ToString();


                if (productBarcodeRetail == txtProductBarcode.Text || productId.ToString() == txtProductBarcode.Text)//If the barcode equals the product's barcode_retail or id, then take the product's retail unit id.
                {
                    productUnit = Convert.ToInt32(dataTable.Rows[rowIndex]["unit_retail"]);
                }
                else //If the barcode equals to the barcode_wholesale, then take the product's wholesale unit id.
                {
                    productUnit = Convert.ToInt32(dataTable.Rows[rowIndex]["unit_wholesale"]);
                }

                txtProductName.Text = dataTable.Rows[rowIndex]["name"].ToString();//Filling the product name textbox from the database

                DataTable dataTableUnit = unitDAL.GetNameById(productUnit);//Datatable for finding the unit name by unit id.

                cboProductUnit.Items.Add(dataTableUnit.Rows[rowIndex]["name"].ToString());//Populating the combobox with related unit names from dataTableUnit.
                cboProductUnit.SelectedIndex = 0;//For selecting the combobox's first element. We selected 0 index because we have just one unit of a retail product.
                
                string productPrice = dataTable.Rows[rowIndex]["saleprice"].ToString();

                txtProductPrice.Text = productPrice;
                txtProductAmount.Text = productAmount.ToString();
                txtProductTotalPrice.Text = (Convert.ToDecimal(productPrice) * productAmount).ToString();
            }

            else
            {
                if (txtProductBarcode.Text != "")
                {
                    MessageBox.Show("You have entered a wrong barcode.");
                }

                ClearProductEntranceTextBox();
            }
        }


        private void btnProductAdd_Click(object sender, RoutedEventArgs e)//Try to do this by using listview
        {
            bool addNewProductLine = true;
            int barcodeColNo=0;
            int amountColNo = 4;
            int amount = 0;

            for (int i = 0; i < dgProducts.Items.Count; i++)
            {
                DataGridRow row = (DataGridRow)dgProducts.ItemContainerGenerator.ContainerFromIndex(i);

                TextBlock barcodeCellContent = dgProducts.Columns[barcodeColNo].GetCellContent(row) as TextBlock;    //Try to understand this code!!!  

                if (barcodeCellContent.Text==txtProductBarcode.Text)
                {
                    if (MessageBox.Show("There is already the same item in the list. Would you like to sum them?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        TextBlock cellContent = dgProducts.Columns[amountColNo].GetCellContent(row) as TextBlock;    //Try to understand this code!!!              
                        //MessageBox.Show(cellContent.Text);
                        amount = Convert.ToInt32(cellContent.Text);
                        amount += 1;
                        cellContent.Text = amount.ToString();
                        addNewProductLine = false;
                    }
                }
            }

            if (addNewProductLine == true)
            {
                //dgProducts.Items.Add(new ProductBLL(){ Id = Convert.ToInt32(txtProductBarcode.Text), Name = txtProductName.Text });// You can also apply this code instead of the code below. Note that you have to change the binding name from the datagrid with the name of the property in ProductBLL if you wish to use this code.
                dgProducts.Items.Add(new { Barcode = txtProductBarcode.Text, Name = txtProductName.Text,  Unit=cboProductUnit.SelectedItem, Price=txtProductPrice.Text, Amount=txtProductAmount.Text, Total=txtProductTotalPrice.Text});
            }

            ClearProductEntranceTextBox();

            //items[0].BarcodeRetail = "EXAMPLECODE"; This code can change the 0th row's data on the column called BarcodeRetail.
        }

        private void btnProductClear_Click(object sender, RoutedEventArgs e)
        {
            ClearProductEntranceTextBox();
            
        }

        private void cboCustomer_Loaded(object sender, RoutedEventArgs e)
        {
            //Creating Data Table to hold the products from Database
            DataTable dataTable = customerDAL.Select();

            //Specifying Items Source for product combobox
            cboCustomer.ItemsSource = dataTable.DefaultView;

            //Here DisplayMemberPath helps to display Text in the ComboBox.
            cboCustomer.DisplayMemberPath = "name";

            //SelectedValuePath helps to store values like a hidden field.
            cboCustomer.SelectedValuePath = "id";
        }

        private void txtProductAmount_LostFocus(object sender, RoutedEventArgs e)
        {
            string strProductAmount = txtProductAmount.Text;
            char lastCharacter = char.Parse(strProductAmount.Substring(strProductAmount.Length-1));

            bool result = Char.IsDigit(lastCharacter);//Checking if the last digit of the number is a number or not.

            decimal number;

            if (strProductAmount != "" && decimal.TryParse(strProductAmount, out number) && result==true)
            {
                DataTable dataTable = productDAL.SearchProductId(txtProductBarcode.Text);

                string unitKg = "Kilogram", unitLt = "Liter";
                int rowIndex = 0;
                decimal productAmount;
                string productPrice = dataTable.Rows[rowIndex]["saleprice"].ToString();

                if (cboProductUnit.Text != unitKg && cboProductUnit.Text != unitLt)
                {
                    /*If the user entered any unit except kilogram or liter, there cannot be a decimal quantity. 
                    So, convert the quantity to integer even the user has entered a decimal quantity as a mistake.*/
                    productAmount = Convert.ToInt32(Convert.ToDecimal(txtProductAmount.Text));
                    txtProductAmount.Text = productAmount.ToString();
                }
                else
                {
                    productAmount = Convert.ToDecimal(txtProductAmount.Text);
                }

                txtProductPrice.Text = productPrice;
                
                txtProductTotalPrice.Text = (Convert.ToDecimal(productPrice) * productAmount).ToString();
            }
            else
            {
                MessageBox.Show("Please enter a valid number");
            }
        }
    }
}
