using CUL;
using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class DepositBLL
    {
        DepositDAL depositDAL = new DepositDAL();

        public DataTable GetLastDepositInfo()
        {
            DataTable dataTable = depositDAL.GetByIdOrLastId();//A METHOD WHICH HAS AN OPTIONAL PARAMETER

            return dataTable;
        }

        public bool InsertDeposit(DepositCUL depositCUL)
        {
            return depositDAL.Insert(depositCUL);
        }

        public bool UpdateDeposit(DepositCUL depositCUL)
        {
            return depositDAL.Update(depositCUL);
        }
    }
}
