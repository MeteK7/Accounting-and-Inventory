using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUL
{
    public class CategoryCUL
    {
        public int Id { get; set; } //Properties are naming with Pascal Case.
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime AddedDate { get; set; }
        public int AddedBy { get; set; }

    }
}
