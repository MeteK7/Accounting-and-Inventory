using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUL.Enums
{
    public enum PopColumns
    {
        ColProductId,
        ColProductName,
        ColProductUnit,
        ColProductQuantity,
        ColProductGrossCostPrice,
        ColProductGrossTotalCostPrice,
        ColProductDiscount,
        ColProductVAT,
        ColProductCostPrice,//Product cost price is the cost price which includes discount and vat in it. (productCostPrice=productGrossCostPrice+ProductVAT-ProductDiscount)
        ColProductTotalCostPrice
    }
}
