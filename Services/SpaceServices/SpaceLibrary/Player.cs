using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceLibrary
{
    public class Player
    {
        public static ServiceWrapper PlayerWrapper()
        {
            ServiceWrapper retVal = new ServiceWrapper();
            retVal.ReturnedDataType = "Player";
            retVal.ReturnedDataTypeVersion = "1.0";
            return retVal;
        }
        public Player()
        {
        }
        public int PlayerId { get; set; }
        public string PlayerName { get; set; }
        public double CreditBalance { get; set; }
        public int MaxCrew { get; set; }
        public int AvailableCrew { get; set; }
        public Ship ShipDetails { get; set; }
    }
}
