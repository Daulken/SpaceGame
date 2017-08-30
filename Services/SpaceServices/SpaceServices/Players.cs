using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace SpaceServices
{
    [DataContract]
    public class Player
    {
        [DataMember]
        public int PlayerId { get; set; }
        [DataMember]
        public string PlayerName { get; set; }
    }
    public partial class Players
    {
        private static readonly Players _instance = new Players();
        private Players() { }
        public static Players Instance
        {
            get { return _instance; }
        }
        public List<Player> PlayerList
        {
            get { return players; }
        }
        public List<Player> players = new List<Player>()
        {
            new Player() { PlayerId = 1, PlayerName = "Daulken" },
            new Player() { PlayerId = 2, PlayerName = "Matt" },
            new Player() { PlayerId = 3, PlayerName = "Chris" }
        };

    }
}