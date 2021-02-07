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
    /// Interaction logic for WinInventoryAdjustment.xaml
    /// </summary>
    public partial class WinInventoryAdjustment : Window
    {
        public WinInventoryAdjustment()
        {
            InitializeComponent();
        }

        UserDAL userDAL = new UserDAL();
        PointOfSaleDAL pointOfSaleDAL = new PointOfSaleDAL();
        PointOfSaleCUL pointOfSaleCUL = new PointOfSaleCUL();
        PointOfSaleDetailDAL pointOfSaleDetailDAL = new PointOfSaleDetailDAL();
        PointOfSaleDetailCUL pointOfSaleDetailCUL = new PointOfSaleDetailCUL();
        InventoryAdjustmentDAL inventoryAdjustmentDAL = new InventoryAdjustmentDAL();
        ProductDAL productDAL = new ProductDAL();
        ProductCUL productCUL = new ProductCUL();
        PaymentDAL paymentDAL = new PaymentDAL();
        PaymentCUL paymentCUL = new PaymentCUL();
        CustomerDAL customerDAL = new CustomerDAL();
        CustomerCUL customerCUL = new CustomerCUL();
        UnitDAL unitDAL = new UnitDAL();
        UnitCUL unitCUL = new UnitCUL();

        int btnNewOrEdit;//0 stands for user clicked the button New, and 1 stands for user clicked the button Edit.

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ClearProductEntranceTextBox()
        {
            txtProductId.Text = "";
            txtProductName.Text = "";
            txtProductUnit.Text = "";
            txtProductCostPrice.Text = "";
            txtProductSalePrice.Text = "";
            txtProductAmount.Text = "";
            txtProductTotalPrice.Text = "";
            Keyboard.Focus(txtProductId); // set keyboard focus
            DisableProductEntranceButtons();
        }

        private void ClearBasketTextBox()
        {
            txtBasketAmount.Text = "0";
            txtBasketGrandTotal.Text = "0";
        }

        private void ClearInventoryAdjustmentListView()
        {
            dgProducts.Items.Clear();
        }

        private int GetLastInventoryAdjustmentId()
        {
            int specificRowIndex = 0, inventoryAdjustmentId;

            DataTable dataTable = inventoryAdjustmentDAL.Search();//Searching the last id number in the tbl_inventory_adjustment.

            if (dataTable.Rows.Count != 0)//If there is an inventory adjustment id in the database, that means the database table's first row cannot be null, and the datatable table's first index is 0.
            {
                inventoryAdjustmentId = Convert.ToInt32(dataTable.Rows[specificRowIndex]["id"]);//We defined this code out of the for loop below because all of the products has the same invoice number in every sale. So, no need to call this method for every products again and again.
            }
            else//If there is no any inventory adjustment id in the db, that means it is the first record. So, assing inventoryAdjustmentId with 0;
            {
                inventoryAdjustmentId = 0;
            }
            return inventoryAdjustmentId;
        }

        private void LoadNewInventoryAdjustment()/*INVENTORY ADJUSTMENT ID REFERS TO THE ID NUMBER IN THE DATABASE FOR WinInventoryAdjustment.*/
        {
            ClearBasketTextBox();
            ClearInventoryAdjustmentListView();

            int inventoryAdjustmentId, increment = 1;

            inventoryAdjustmentId = GetLastInventoryAdjustmentId();//Getting the last inventory adjustment record id and assign it to the variable called inventoryAdjustmentId.
            inventoryAdjustmentId += increment;//We are adding one to the last inventory adjustment record id because every new inventory adjustment record id is one greater tham the previous one.
            lblIventoryAdjustmentId.Content = inventoryAdjustmentId;//Assigning inventory adjustment record id to the content of the inventory adjustment record id label.
        }

        private void ModifyTools()//Do NOT repeat yourself! You have used IsEnabled function for these toolbox contents many times!
        {
            btnNew.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnPrint.IsEnabled = true;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            txtProductUnit.IsEnabled = true;
            txtProductId.IsEnabled = true;
            txtProductName.IsEnabled = true;
            txtProductSalePrice.IsEnabled = true;
            txtProductAmount.IsEnabled = true;
            txtProductTotalPrice.IsEnabled = true;
            dgProducts.IsHitTestVisible = true;//Enabling the datagrid clicking.

        }
        private void DisableProductEntranceButtons()
        {
            btnProductAdd.IsEnabled = false; //Disabling the add button if all text boxes are cleared.
            btnProductClear.IsEnabled = false; //Disabling the clear button if all text boxes are cleared.
        }

        private void txtProductId_KeyUp(object sender, KeyEventArgs e)
        {
            int number;

            DataTable dataTable = productDAL.SearchProductByIdBarcode(txtProductId.Text);

            if (txtProductId.Text != 0.ToString() && int.TryParse(txtProductId.Text, out number) && dataTable.Rows.Count != 0)//Validating the barcode if it is a number(except zero) or not.
            {
                int productAmount = 1;
                int rowIndex = 0;
                int productId;
                int productUnit = 0;
                string productBarcodeRetail;
                string costPrice, salePrice;

                btnProductAdd.IsEnabled = true; //Enabling the add button if any valid barcode is entered.
                btnProductClear.IsEnabled = true;//Enabling the clear button if any valid barcode is entered.


                productId = Convert.ToInt32(dataTable.Rows[rowIndex]["id"]);
                productBarcodeRetail = dataTable.Rows[rowIndex]["barcode_retail"].ToString();
                txtProductName.Text = dataTable.Rows[rowIndex]["name"].ToString();//Filling the product name textbox from the database

                if (productBarcodeRetail == txtProductId.Text || productId.ToString() == txtProductId.Text)//If the barcode equals the product's barcode_retail or id, then take the product's retail unit id.
                {
                    productUnit = Convert.ToInt32(dataTable.Rows[rowIndex]["unit_retail"]);
                }

                else //If the barcode equals to the barcode_wholesale, then take the product's wholesale unit id.
                {
                    productUnit = Convert.ToInt32(dataTable.Rows[rowIndex]["unit_wholesale"]);
                }

                DataTable dataTableUnit = unitDAL.GetUnitInfoById(productUnit);//Datatable for finding the unit name by unit id.

                txtProductUnit.Text=dataTableUnit.Rows[rowIndex]["name"].ToString();//Populating the textbox with the related unit name from dataTableUnit.

                costPrice = dataTable.Rows[rowIndex]["costprice"].ToString();
                salePrice = dataTable.Rows[rowIndex]["saleprice"].ToString();

                txtProductCostPrice.Text = costPrice;
                txtProductSalePrice.Text = salePrice;
                txtProductAmount.Text = productAmount.ToString();
                txtProductTotalPrice.Text = (Convert.ToDecimal(salePrice) * productAmount).ToString();
            }


            /*
            If the txtProductId is empty which means user has clicked the backspace button and if the txtProductName is filled once before entering an id, then erase all the text contents.
            I just checked the btnProductAdd to know if there was a product entry before or not.
            If the btnProductAdd is not enabled in the if block above once before, then no need to call the method ClearProductEntranceTextBox.*/
            else if (txtProductId.Text == "" && btnProductAdd.IsEnabled == true)
            {
                ClearProductEntranceTextBox();
            }
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            btnNewOrEdit = 0;//0 stands for the user has entered the btnNew.
            LoadNewInventoryAdjustment();
            ModifyTools();
        }

    }
}
