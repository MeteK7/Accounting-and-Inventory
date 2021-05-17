using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUL
{
    public class ExpenseCUL
    {
        public int Id { get; set; }
        public int IdFrom { get; set; }
        public int IdTo { get; set; }
        public int IdAssetFrom { get; set; }
        public int IdAssetTo { get; set; }
        public int AssetId { get; set; }
        public decimal Amount { get; set; }
        public int AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
    }
}
