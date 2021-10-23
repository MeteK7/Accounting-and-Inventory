using BLL;
using CUL;
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
        UserBLL userBLL = new UserBLL();
        ReceiptCUL receiptCUL = new ReceiptCUL();
        ReceiptBLL receiptBLL = new ReceiptBLL();
        ReceiptDAL receiptDAL = new ReceiptDAL();
        AssetCUL assetCUL = new AssetCUL();
        AssetDAL assetDAL = new AssetDAL();
        BankDAL bankDAL = new BankDAL();
        CommonBLL commonBLL = new CommonBLL();

        const string calledBy = "WinReceipt", colTxtName = "name", colTxtId = "id", colTxtIdTo = "id_to", colTxtAmount = "amount", colTxtDetails = "details", colTxtAddedDate = "added_date";
        const int initialIndex = 0, unitValue = 1;
        const int clickedNothing = -1, clickedNew = 0, clickedPrev = 0, clickedNext = 1, clickedEdit = 1, clickedNull = 2;//0 stands for user clicked the button New, and 1 stands for user clicked the button Edit.
        const int account = 1, bank = 2, supplier = 3;
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
            isCboSelectionEnabled = false;
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

        public void DisableTools()
        {
            btnMenuSave.IsEnabled = false;
            btnMenuCancel.IsEnabled = false;
            cboFrom.IsEnabled = false;
            cboTo.IsEnabled = false;
            txtAmount.IsEnabled = false;
            rbAccount.IsEnabled = false;
            rbBank.IsEnabled = false;
        }

        public void EnableTools()
        {
            btnMenuSave.IsEnabled = true;
            btnMenuCancel.IsEnabled = true;
            cboFrom.IsEnabled = true;
            cboTo.IsEnabled = true;
            txtAmount.IsEnabled = true;
            rbAccount.IsEnabled = true;
            rbBank.IsEnabled = true;
        }

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
            cboFrom.IsEnabled = true;
            cboTo.IsEnabled = true;
            txtAmount.IsEnabled = true;
            rbAccount.IsEnabled = true;
            rbBank.IsEnabled = true;
        }

        //-1 means user did not clicked either previous or next button which means user just clicked the point of purchase button to open it.
        private void LoadPastReceipt(int receiptId = initialIndex, int receiptArrow = clickedNothing)//Optional parameter
        {
            int idAssetFrom;

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

                    #region ASSET INFORMATION FILLING REGION
                    idAssetFrom = Convert.ToInt32(dtReceipt.Rows[initialIndex]["id_asset_from"].ToString());
                    //lblAssetIdFrom.Content = idAssetFrom;

                    DataTable dtAsset = assetDAL.SearchById(idAssetFrom);
                    int sourceType = Convert.ToInt32(dtAsset.Rows[initialIndex]["id_source_type"]);

                    if (sourceType == account)
                        rbAccount.IsChecked = true;
                    else
                        rbBank.IsChecked = true;

                    LoadCboFrom(sourceType);//This function works twice when you open the WinReceipt because the rb selection is being changed. But if the previous selection is same, rbBank_Checked does not work so the method LoadCboFrom called by rbBank_Checked does not work as well.
                    cboFrom.SelectedValue = dtAsset.Rows[initialIndex]["id_source"].ToString();
                    #endregion

                    LoadCboTo();
                    cboTo.SelectedValue = Convert.ToInt32(dtReceipt.Rows[initialIndex][colTxtIdTo].ToString());//Getting the id of supplier.

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

        private void LoadNewReceipt()
        {
            //ClearBasketTextBox();
            //ClearProductsDataGrid();

            int expenseNo;

            expenseNo = receiptBLL.GetLastExpenseNumber();//Getting the last invoice number and assign it to the variable called expenseNo.
            expenseNo += unitValue;//We are adding one to the last expense number because every new receipt number is one greater tham the previous expense number.
            lblReceiptId.Content = expenseNo;//Assigning receiptNo to the content of the receiptNo Label.
        }

        private void btnMenuNew_Click(object sender, RoutedEventArgs e)
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
    }
}
