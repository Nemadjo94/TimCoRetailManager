using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRMDesktopUI.Library.Models
{
    public class SaleDetailModel
    {
        // We are sending only these two values, the rest are picked up 
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
