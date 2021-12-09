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
    /// Interaction logic for WinReceipt.xaml
    /// </summary>
    public partial class WinReceipt : Window
    {
        AccountDAL accountDAL = new AccountDAL();
        SupplierDAL supplierDAL = new SupplierDAL();
        CustomerDAL customerDAL = new CustomerDAL();
        PaymentDAL paymentDAL = new PaymentDAL();
        SourceTypeDAL sourceTypeDAL = new SourceTypeDAL();
        UserBLL userBLL = new UserBLL();
        ReceiptCUL receiptCUL = new ReceiptCUL();
        ReceiptBLL receiptBLL = new ReceiptBLL();
        ReceiptDAL receiptDAL = new ReceiptDAL();
        AssetCUL assetCUL = new AssetCUL();
        AssetDAL assetDAL = new AssetDAL();
        BankDAL bankDAL = new BankDAL();
        CommonBLL commonBLL = new CommonBLL();

        const string calledBy = "tbl_receipts", colTxtName = "name", colTxtId = "id",colTxtIdFrom="id_from", colTxtIdTo="id_to", colTxtIdAssetFrom= "id_asset_from", colTxtIdAssetTo = "id_asset_to", colTxtAmount = "amount", colTxtDetails = "details", colTxtAddedDate = "added_date", colTxtPaymentType = "payment_type", colTxtPaymentTypeId = "id_payment_type";
        const int initialIndex = 0, unitValue = 1;
        const int clickedNothing = -1, clickedNew = 0, clickedPrev = 0, clickedNext = 1, clickedEdit = 1, clickedNull = 2;//0 stands for user clicked the button New, and 1 stands for user clicked the button Edit.
        const int account = 1, bank = 2, supplier = 3,customer=4;
        const int receiptSize = 5;
        const int oldBalanceFrom = 0, oldBalanceTo = 1, oldAssetIdFrom = 2, oldAssetIdTo = 3, oldAmount = 4;

        int clickedNewOrEdit, clickedArrow;
        string[] oldReceipt = new string[receiptSize];
        bool isCboSelectionEnabled = true;

        public WinReceipt()
        {
            InitializeComponent();
            DisableTools();
            LoadPastReceipt();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FirstTimeRun()
        {
            MessageBox.Show("Welcome!\n Thank you for choosing Kaba Accounting and Inventory System.");
            btnPrev.IsEnabled = false;//Disabling the btnPrev button because there is no any records in the database for the first time.
            btnNext.IsEnabled = false;//Disabling the btnNext button because there is no any records in the database for the first time.
        }

        private void ClearTools()
        {
            isCboSelectionEnabled = false;//We need to disable the SelectionChanged methods in case any methods below would trig them.
            cboSourceFrom.ItemsSource = null;
            cboSourceTo.ItemsSource = null;
            cboFrom.ItemsSource = null;
            cboTo.ItemsSource = null;
            isCboSelectionEnabled = true;

            lblBalanceFrom.Content = "";
            lblBalanceTo.Content = "";
            lblAssetIdFrom.Content = "";
            lblAssetIdTo.Content = "";
            lblDateAdded.Content = "";
            txtDetails.Text = "";
            txtAmount.Text = "";
        }

        private void ClearLabels()
        {
            lblBalanceTo.Content = "";
            lblAssetIdTo.Content = "";
        }

        public void DisableTools()
        {
            btnMenuSave.IsEnabled = false;
            btnMenuCancel.IsEnabled = false;
            cboSourceFrom.IsEnabled = false;
            cboSourceTo.IsEnabled = false;
            cboPaymentType.IsEnabled = false;
            cboFrom.IsEnabled = false;
            cboTo.IsEnabled = false;
            txtAmount.IsEnabled = false;
            txtDetails.IsEnabled = false;
        }

        //public void EnableTools()
        //{
        //    btnMenuSave.IsEnabled = true;
        //    btnMenuCancel.IsEnabled = true;
        //    cboSourceFrom.IsEnabled = true;
        //    cboSourceTo.IsEnabled = true;
        //    cboPaymentType.IsEnabled = true;
        //    cboFrom.IsEnabled = true;
        //    cboTo.IsEnabled = true;
        //    txtAmount.IsEnabled = true;
        //    txtDetails.IsEnabled = true;
        //}

        private void EnableButtonsOnClickSaveCancel()
        {
            btnMenuNew.IsEnabled = true;
            btnMenuEdit.IsEnabled = true;
            btnMenuDelete.IsEnabled = true;
            btnPrev.IsEnabled = true;
            btnNext.IsEnabled = true;
        }

        private void ModifyToolsOnClickBtnNewEdit()
        {
            btnMenuSave.IsEnabled = true;
            btnMenuCancel.IsEnabled = true;
            btnMenuNew.IsEnabled = false;
            btnMenuEdit.IsEnabled = false;
            btnMenuDelete.IsEnabled = false;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;
            cboPaymentType.IsEnabled = true;
            cboSourceFrom.IsEnabled = true;
            cboSourceTo.IsEnabled = true;
            cboFrom.IsEnabled = true;
            cboTo.IsEnabled = true;
            txtAmount.IsEnabled = true;
            txtDetails.IsEnabled = true;
        }

        private void LoadNewReceipt()
        {
            //ClearBasketTextBox();
            //ClearProductsDataGrid();

            int receiptNo;

            receiptNo = receiptBLL.GetLastReceiptNumber();//Getting the last receipt number and assign it to the variable called receiptNo.
            receiptNo += unitValue;//We are adding one to the last receipt number because every new receipt number is one greater tham the previous receipt number.
            lblReceiptId.Content = receiptNo;//Assigning receiptNo to the content of the receiptNo Label.
        }

        //-1 means user did not clicked either previous or next button which means user just clicked the point of purchase button to open it.
        private void LoadPastReceipt(int receiptId = initialIndex, int receiptArrow = clickedNothing)//Optional parameter
        {
            int idAssetFrom,idAssetTo,idFrom,idTo;

            if (receiptId == initialIndex)//If the ID is 0 came from the optional parameter, that means user just clicked the WinPOP button to open it.
            {
                receiptId = commonBLL.GetLastRecordById(calledBy);//Getting the last invoice id and assign it to the variable called invoiceId.
            }

            /*WE CANNOT USE ELSE IF FOR THE CODE BELOW! BOTH IF STATEMENTS ABOVE AND BELOVE MUST WORK.*/
            if (receiptId != initialIndex)// If the invoice number is still 0 even when we get the last invoice number by using code above, that means this is the first sale and do not run this code block.
            {
                DataTable dtReceipt = receiptDAL.GetByReceiptId(receiptId);

                if (dtReceipt.Rows.Count != initialIndex)
                {
                    receiptId = Convert.ToInt32(dtReceipt.Rows[initialIndex][colTxtId].ToString());//Getting the id of account.
                    lblReceiptId.Content = receiptId;

                    #region SOURCE TYPE CBO INFORMATION FILLING REGION
                    //CBO SOURCE-FROM FILLING
                    idAssetFrom = Convert.ToInt32(dtReceipt.Rows[initialIndex][colTxtIdAssetFrom].ToString()); //Fetching the id_asset_from in order to get full details about the specific asset later.

                    DataTable dtAssetFrom = assetDAL.SearchById(idAssetFrom);//Sending the idAssetFrom in order the fetch full details of the asset.
                    int idSourceTypeFrom = Convert.ToInt32(dtAssetFrom.Rows[initialIndex]["id_source_type"]);

                    LoadCboSourceFrom();//We need to load the cboSourceFrom first in order to get which source type the user has clicked below.
                    cboSourceFrom.SelectedValue = idSourceTypeFrom;//This code trigs the method LoadCboFrom in the method cboSourceFrom_SelectionChanged!


                    //CBO SOURCE-TO FILLING
                    idAssetTo = Convert.ToInt32(dtReceipt.Rows[initialIndex][colTxtIdAssetTo].ToString()); //Fetching the id_asset_to in order to get full details about the specific asset later.

                    DataTable dtAssetTo = assetDAL.SearchById(idAssetTo);//Sending the idAssetFrom in order the fetch full details of the asset.
                    int idSourceTypeTo = Convert.ToInt32(dtAssetTo.Rows[initialIndex]["id_source_type"]);

                    LoadCboSourceTo();//We need to load the cboSourceTo first in order to get which source type the user has clicked below.
                    cboSourceTo.SelectedValue = idSourceTypeTo;//This code trigs the method LoadCboTo in the method cboSourceTo_SelectionChanged!
                    #endregion

                    LoadCboPaymentType();//Loading payment types.
                    cboPaymentType.SelectedValue= Convert.ToInt32(dtReceipt.Rows[(int)Numbers.InitialIndex][colTxtPaymentTypeId].ToString());//Getting the id of payment type.

                    #region SOURCE CBO INFORMATION FILLING REGION 
                    idFrom = Convert.ToInt32(dtReceipt.Rows[initialIndex][colTxtIdFrom].ToString());
                    //LoadCboFrom(idSourceTypeFrom);No need for this code because it is automatically trigged by the code line --cboSourceFrom.SelectedValue = idSourceTypeFrom-- above.
                    cboFrom.SelectedValue = idFrom;

                    idTo = Convert.ToInt32(dtReceipt.Rows[initialIndex][colTxtIdTo].ToString());
                    //LoadCboTo(idSourceTypeTo);No need for this code because it is automatically trigged by the code line --cboSourceTo.SelectedValue = idSourceTypeTo-- above.
                    cboTo.SelectedValue = idTo;
                    #endregion

                    txtAmount.Text = dtReceipt.Rows[initialIndex][colTxtAmount].ToString();
                    txtDetails.Text = dtReceipt.Rows[initialIndex][colTxtDetails].ToString();
                    lblDateAdded.Content = Convert.ToDateTime(dtReceipt.Rows[initialIndex][colTxtAddedDate]).ToString("f");
                }
                else if (dtReceipt.Rows.Count == initialIndex)//If the pop detail row quantity is 0, that means there is no such row so decrease or increase the invoice number according to user preference.
                {
                    if (receiptArrow == initialIndex)//If the invoice arrow is 0, that means user clicked the previous button.
                    {
                        receiptId = receiptId - unitValue;
                    }
                    else
                    {
                        receiptId = receiptId + unitValue;
                    }

                    if (receiptArrow != -unitValue)//If the user has not clicked either previous or next button, then the invoiceArrow will be -1 and no need for recursion.
                    {
                        LoadPastReceipt(receiptId, receiptArrow);//Call the method again to get the new past invoice.
                    }

                }
            }
            else
            {
                FirstTimeRun();//This method is called when it is the first time of using this program.
            }
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            int firstReceiptId = unitValue, currentReceiptId = Convert.ToInt32(lblReceiptId.Content);

            if (currentReceiptId != firstReceiptId)
            {
                int prevReceiptId = currentReceiptId - unitValue;

                clickedArrow = clickedPrev;//0 means customer has clicked the previous button.
                ClearTools();
                LoadPastReceipt(prevReceiptId, clickedArrow);
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            int lastReceiptId = commonBLL.GetLastRecordById(calledBy), currentInvoiceId;

            currentInvoiceId = Convert.ToInt32(lblReceiptId.Content);

            if (currentInvoiceId != lastReceiptId)
            {
                int nextInvoice = currentInvoiceId + unitValue;

                clickedArrow = clickedNext;//1 means customer has clicked the next button.
                ClearTools();
                LoadPastReceipt(nextInvoice, clickedArrow);
            }
        }

        private void btnMenuNew_Click(object sender, RoutedEventArgs e)
        {
            clickedNewOrEdit = clickedNew;//0 stands for the user has entered the btnNew.

            ClearTools();
            LoadNewReceipt();
            LoadCboSourceFrom();
            LoadCboSourceTo();
            ModifyToolsOnClickBtnNewEdit();
        }

        private void btnMenuSave_Click(object sender, RoutedEventArgs e)
        {
            int emptyIndex = -1;

            if (cboFrom.SelectedIndex != emptyIndex && cboTo.SelectedIndex != emptyIndex && txtAmount.Text != "")
            {
                int receiptId = Convert.ToInt32(lblReceiptId.Content); /*lblReceiptId stands for the receipt id in the database.*/
                int userId = userBLL.GetUserId(WinLogin.loggedInUserName);
                bool isSuccess = false, isSuccessAsset = false, isSuccessAssetSupplier = false;

                #region ASSIGNING CUL SECTION
                receiptCUL.Id = receiptId;
                receiptCUL.IdPaymentType= Convert.ToInt32(cboPaymentType.SelectedValue);
                receiptCUL.IdFrom = Convert.ToInt32(cboFrom.SelectedValue);
                receiptCUL.IdTo = Convert.ToInt32(cboTo.SelectedValue);
                receiptCUL.IdAssetFrom = Convert.ToInt32(lblAssetIdFrom.Content);
                receiptCUL.IdAssetTo = Convert.ToInt32(lblAssetIdTo.Content);
                receiptCUL.Amount = Convert.ToDecimal(txtAmount.Text);
                receiptCUL.Details = txtDetails.Text;
                receiptCUL.AddedBy = userId;
                receiptCUL.AddedDate = DateTime.Now;
                #endregion

                if (clickedNewOrEdit == clickedEdit)
                {
                    isSuccess = receiptBLL.UpdateReceipt(receiptCUL);

                    #region TABLE ASSET REVERTING AND UPDATING SECTION
                    //UPDATING THE ASSET FOR SOURCE BALANCE.
                    assetCUL.Id = Convert.ToInt32(oldReceipt[oldAssetIdFrom]);
                    assetCUL.SourceBalance = Convert.ToDecimal(oldReceipt[oldBalanceFrom]) + Convert.ToDecimal(oldReceipt[oldAmount]);//We have to add this amount into source's balance in order to revert the old receipt.
                    isSuccessAsset = assetDAL.Update(assetCUL);

                    //UPDATING THE ASSET FOR COMPANY BALANCE.
                    assetCUL.Id = Convert.ToInt32(oldReceipt[oldAssetIdTo]);
                    assetCUL.SourceBalance = Convert.ToDecimal(oldReceipt[oldBalanceTo]) - Convert.ToDecimal(oldReceipt[oldAmount]);//We have to subtract this amount from company's balance in order to revert our balance.
                    isSuccessAssetSupplier = assetDAL.Update(assetCUL);
                    #endregion
                }

                else
                {
                    isSuccess = receiptBLL.InsertReceipt(receiptCUL);
                }

                #region TABLE ASSET UPDATING SECTION
                //UPDATING THE ASSET FOR EXPENSE OF THE CORPORATION.
                assetCUL.Id = Convert.ToInt32(lblAssetIdFrom.Content);
                assetCUL.SourceBalance = Convert.ToDecimal(GetBalance(Convert.ToInt32(lblAssetIdFrom.Content))) - Convert.ToDecimal(txtAmount.Text);//We have to subtract this amount from company's balance in order to make the payment to the supplier.
                isSuccessAsset = assetDAL.Update(assetCUL);

                //UPDATING THE ASSET FOR BALANCE OF THE SUPPLIER.
                assetCUL.Id = Convert.ToInt32(lblAssetIdTo.Content);
                assetCUL.SourceBalance = Convert.ToDecimal(GetBalance(Convert.ToInt32(lblAssetIdTo.Content))) + Convert.ToDecimal(txtAmount.Text);//We have to add this amount to supplier's balance in order to reset our dept.
                isSuccessAssetSupplier = assetDAL.Update(assetCUL);
                #endregion

                //If the data is inserted successfully, then the value of the variable isSuccess will be true; otherwise it will be false.
                if (isSuccess == true && isSuccessAsset == true && isSuccessAssetSupplier == true)//IsSuccessDetail is always CHANGING in every loop above! IMPROVE THIS!!!!
                {
                    DisableTools();
                    EnableButtonsOnClickSaveCancel();
                }
                else
                {
                    MessageBox.Show("Something went wrong :(");
                }
            }
        }

        private void btnMenuCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Would you really like to cancel the expense page?", "Cancel Receipt", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    DisableTools();
                    ClearTools();
                    LoadPastReceipt();
                    EnableButtonsOnClickSaveCancel();
                    break;
                case MessageBoxResult.No:
                    MessageBox.Show("Enjoy!", "Enjoy");
                    break;
                case MessageBoxResult.Cancel:
                    MessageBox.Show("Nevermind then...", "KABA Accounting");
                    break;
            }
        }

        private void btnMenuEdit_Click(object sender, RoutedEventArgs e)
        {
            ModifyToolsOnClickBtnNewEdit();

            oldReceipt[oldBalanceFrom] = lblBalanceFrom.Content.ToString();
            oldReceipt[oldBalanceTo] = lblBalanceTo.Content.ToString();
            oldReceipt[oldAssetIdFrom] = lblAssetIdFrom.Content.ToString();
            oldReceipt[oldAssetIdTo] = lblAssetIdTo.Content.ToString();
            oldReceipt[oldAmount] = txtAmount.Text.ToString();

            clickedNewOrEdit = clickedEdit;//Changing the state of the clicked NewOrEdit in order to update the old receipt page.
        }

        private void btnMenuDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Would you really like to delete the receipt?", "Delete Receipt", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:

                    #region TABLE ASSET REVERTING AND UPDATING SECTION
                    //REVERTING THE ASSET FOR SOURCE BALANCE.
                    assetCUL.Id = Convert.ToInt32(lblAssetIdFrom.Content);
                    assetCUL.SourceBalance = Convert.ToDecimal(GetBalance(Convert.ToInt32(lblAssetIdFrom.Content))) + Convert.ToDecimal(txtAmount.Text);//We have to add this amount into source's balance in order to revert the old receipt.
                    assetDAL.Update(assetCUL);

                    //REVERTING THE ASSET FOR COMPANY BALANCE.
                    assetCUL.Id = Convert.ToInt32(lblAssetIdTo.Content);
                    assetCUL.SourceBalance = Convert.ToDecimal(GetBalance(Convert.ToInt32(lblAssetIdTo.Content))) - Convert.ToDecimal(txtAmount.Text);//We have to subtract this amount from company's balance in order to revert our balance.
                    assetDAL.Update(assetCUL);
                    #endregion

                    #region DELETE EXPENSE RECORD
                    int receiptId = Convert.ToInt32(lblReceiptId.Content);

                    receiptDAL.Delete(receiptId);
                    #endregion

                    LoadPastReceipt();
                    break;
                case MessageBoxResult.No:
                    MessageBox.Show("Enjoy!", "Enjoy");
                    break;
                case MessageBoxResult.Cancel:
                    MessageBox.Show("Nevermind then...", "KABA Accounting");
                    break;
            }
        }

        private void cboSourceFrom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isCboSelectionEnabled == true)
            {
                ClearLabels();

                LoadCboFrom(Convert.ToInt32(cboSourceFrom.SelectedValue));
            }
        }

        private void cboSourceTo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isCboSelectionEnabled == true)
            {
                ClearLabels();

                LoadCboTo(Convert.ToInt32(cboSourceTo.SelectedValue));
            }
        }

        private void cboFrom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isCboSelectionEnabled == true)
            {
                CboFromSelectionChanged();
            }
        }

        private void cboTo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isCboSelectionEnabled == true)
            {
                CboToSelectionChanged();
            }
        }

        private void CboFromSelectionChanged()
        {
            #region LBLASSETIDFROM POPULATING SECTION
            int sourceId, assetId, sourceTypeId;
            
            sourceTypeId = Convert.ToInt32(cboSourceFrom.SelectedValue);
            sourceId = Convert.ToInt32(cboFrom.SelectedValue);
            assetId = assetDAL.GetAssetIdBySource(sourceId, sourceTypeId);
            lblAssetIdFrom.Content = assetId;
            #endregion

            #region LBLBALANCEFROM POPULATING SECTION
            DataTable dtAsset = assetDAL.SearchById(assetId);

            string balance = dtAsset.Rows[initialIndex]["source_balance"].ToString();

            lblBalanceFrom.Content = balance;
            #endregion
        }

        private void CboToSelectionChanged()
        {
            #region LBLASSETIDTO POPULATING SECTION
            int sourceId, assetId, sourceTypeId;

            sourceTypeId= Convert.ToInt32(cboSourceTo.SelectedValue);
            sourceId = Convert.ToInt32(cboTo.SelectedValue);
            assetId = assetDAL.GetAssetIdBySource(sourceId, sourceTypeId);
            lblAssetIdTo.Content = assetId;
            #endregion

            #region LBLBALANCETO POPULATING SECTION
            DataTable dtAsset = assetDAL.SearchById(assetId);

            string balance = dtAsset.Rows[initialIndex]["source_balance"].ToString();

            lblBalanceTo.Content = balance;
            #endregion
        }

        private decimal GetBalance(int assetId)
        {
            DataTable dtAsset = assetDAL.SearchById(assetId);

            decimal balance = Convert.ToDecimal(dtAsset.Rows[initialIndex]["source_balance"]);

            return balance;
        }

        private DataTable FetchSourceData(int idSourceType)
        {
            DataTable dtSource;

            switch (idSourceType)
            {
                case account:
                    dtSource = accountDAL.Select();
                    break;
                case bank:
                    dtSource = bankDAL.Select();
                    break;
                case supplier:
                    dtSource = supplierDAL.Select();
                    break;
                default:
                    dtSource = customerDAL.Select();
                    break;
            }
            return dtSource;
        }

        private void LoadCboSourceFrom()
        {
            isCboSelectionEnabled = false;//Disabling the selection changed method in order to prevent them to work when we reassign the combobox with unselected status.

            DataTable dtSourceFrom= sourceTypeDAL.Select();


            //Specifying Items Source for product combobox
            cboSourceFrom.ItemsSource = dtSourceFrom.DefaultView;

            //Here DisplayMemberPath helps to display Text in the ComboBox.
            cboSourceFrom.DisplayMemberPath = colTxtName;

            //SelectedValuePath helps to store values like a hidden field.
            cboSourceFrom.SelectedValuePath = colTxtId;

            isCboSelectionEnabled = true;//Enabling the selection changed method in order to allow them to work in case of any future selections.
        }

        private void LoadCboSourceTo()
        {
            isCboSelectionEnabled = false;//Disabling the selection changed method in order to prevent them to work when we reassign the combobox with unselected status.

            DataTable dtSourceTo = sourceTypeDAL.Select();


            //Specifying Items Source for product combobox
            cboSourceTo.ItemsSource = dtSourceTo.DefaultView;

            //Here DisplayMemberPath helps to display Text in the ComboBox.
            cboSourceTo.DisplayMemberPath = colTxtName;

            //SelectedValuePath helps to store values like a hidden field.
            cboSourceTo.SelectedValuePath = colTxtId;

            isCboSelectionEnabled = true;//Enabling the selection changed method in order to allow them to work in case of any future selections.
        }

        private void LoadCboPaymentType()
        {
            //Creating Data Table to hold the products from Database
            DataTable dtPayment = paymentDAL.Select();

            //Specifying Items Source for product combobox
            cboPaymentType.ItemsSource = dtPayment.DefaultView;

            //Here DisplayMemberPath helps to display Text in the ComboBox.
            cboPaymentType.DisplayMemberPath = colTxtPaymentType;

            //SelectedValuePath helps to store values like a hidden field.
            cboPaymentType.SelectedValuePath = colTxtId;
        }

        private void LoadCboFrom(int idSourceType)
        {
            isCboSelectionEnabled = false;//Disabling the selection changed method in order to prevent them to work when we reassign the combobox with unselected status.

            DataTable dtFrom = FetchSourceData(idSourceType);

            //Specifying Items Source for product combobox
            cboFrom.ItemsSource = dtFrom.DefaultView;

            //Here DisplayMemberPath helps to display Text in the ComboBox.
            cboFrom.DisplayMemberPath = colTxtName;

            //SelectedValuePath helps to store values like a hidden field.
            cboFrom.SelectedValuePath = colTxtId;

            isCboSelectionEnabled = true;//Enabling the selection changed method in order to allow them to work in case of any future selections.
        }

        private void LoadCboTo(int idSourceType)
        {
            isCboSelectionEnabled = false;//Disabling the selection changed method in order to prevent them to work when we reassign the combobox with unselected status.

            DataTable dtTo = FetchSourceData(idSourceType);

            //Specifying Items Source for product combobox
            cboTo.ItemsSource = dtTo.DefaultView;

            //Here DisplayMemberPath helps to display Text in the ComboBox.
            cboTo.DisplayMemberPath = colTxtName;

            //SelectedValuePath helps to store values like a hidden field.
            cboTo.SelectedValuePath = colTxtId;

            isCboSelectionEnabled = true;//Enabling the selection changed method in order to allow them to work in case of any future selections.
        }
    }
}