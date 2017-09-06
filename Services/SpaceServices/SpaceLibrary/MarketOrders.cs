using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceLibrary
{
    /// <summary>
    /// Market Order
    /// </summary>
    public class MarketOrder
    {
        public static ServiceWrapper MarketOrderWrapper()
        {
            ServiceWrapper retVal = new ServiceWrapper();
            retVal.ReturnedDataType = "MarketOrder";
            retVal.ReturnedDataTypeVersion = "1.0";
            return retVal;
        }
        public int MaterialId { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}
