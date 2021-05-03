using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUL
{
    public class AssetCUL
    {
        public int Id { get; set; }
        public int AssetId { get; set; }
        public string AssetType { get; set; }
        public decimal AssetBalance { get; set; }
    }
}
