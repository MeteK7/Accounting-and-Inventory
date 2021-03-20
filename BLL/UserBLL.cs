using KabaAccounting.CUL;
using KabaAccounting.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class UserBLL
    {
        UserDAL userDAL = new UserDAL();
        UserCUL userCUL = new UserCUL();
        public int GetUserId(string userName)//You used this method in WinProducts, as well. You can Make an external class just for this to prevent repeatings!!!.
        {
            //Getting the name of the user from the Login Window and fill it into a string variable;
            //string loggedUser = WinLogin.loggedInUserName;

            //Calling the method named GetIdFromUsername in the userDAL and sending the variable loggedUser as a parameter into it.
            //Then, fill the result into the userCUL;
            userCUL = userDAL.GetIdFromUsername(userName);

            int userId = userCUL.Id;

            return userId;
        }
    }
}
