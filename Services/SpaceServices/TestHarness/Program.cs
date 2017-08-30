using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestHarness.SpaceServiceReference;

namespace TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            SpaceServiceClient client = new SpaceServiceClient();
            Player[] Players = client.GetPlayerList();
            foreach (Player player in Players)
            {
                Console.WriteLine(string.Format("{0}: {1}", player.PlayerId.ToString(), player.PlayerName));
            }
            Console.ReadLine();
            
        }
    }
}
