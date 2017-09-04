using System;
using System.Collections.Generic;
using System.Text;

namespace SpaceLibrary
{
	/// <summary>
	/// Description of a player
	/// </summary>
	public class Player
    {
		/// <summary>
		/// Wrapper for a player that allows it to be serialised and used as a JSON blob in a web service communication
		/// </summary>
		public static ServiceWrapper PlayerWrapper()
        {
            ServiceWrapper retVal = new ServiceWrapper();
            retVal.ReturnedDataType = "Player";
            retVal.ReturnedDataTypeVersion = "1.0";
            return retVal;
        }

		/// <summary>
		/// Default constructor for a new player
		/// </summary>
		public Player()
        {
        }

		/// <summary>
		/// The unique ID of the player for database lookups
		/// </summary>
		public int PlayerId
		{
			get; set;
		}

		/// <summary>
		/// The name of the player
		/// </summary>
		public string PlayerName
		{
			get; set;
		}

		/// <summary>
		/// The current monetary balance the player has
		/// </summary>
		public double CreditBalance
		{
			get; set;
		}

		/// <summary>
		/// The maximum number of crew the player can use
		/// </summary>
		public int MaxCrew
		{
			get; set;
		}

		/// <summary>
		/// The number of crew the player owns
		/// </summary>
		public int AvailableCrew
		{
			get; set;
		}

		/// <summary>
		/// The ship that the player owns
		/// </summary>
		public Ship ShipDetails
		{
			get; set;
		}
    }
}
