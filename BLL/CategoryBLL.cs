using KabaAccounting.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class CategoryBLL
    {
        CategoryDAL categoryDAL = new CategoryDAL();

        public string GetCategoryName(DataTable dataTableProduct, int rowIndex)
        {
            DataTable dataTableCategory;

            int categoryId;
            int rowFirstIndex = 0;
            string categoryName;

            categoryId = Convert.ToInt32(dataTableProduct.Rows[rowIndex]["category_id"]);//Getting the category id first to find its name.
            dataTableCategory = categoryDAL.GetCategoryInfoById(categoryId);//Getting all of the category infos by using id.
            categoryName = dataTableCategory.Rows[rowFirstIndex]["name"].ToString();//Fetching the name of the category from dataTableCategory. Index is always zero since we are dealing with a unique category only.

            return categoryName;
        }
    }
}
