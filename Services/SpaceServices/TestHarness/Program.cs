using Newtonsoft.Json;
using SpaceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestHarness
{
    class Program
    {
        static void Main(string[] args)
        {
            SpaceServiceReference.SpaceServiceSoapClient client = new SpaceServiceReference.SpaceServiceSoapClient();
            string sWrapperData = client.GetMarketOrders(1);
            Console.WriteLine(sWrapperData);
            //ServiceWrapper Wrapper = JsonConvert.DeserializeObject<ServiceWrapper>(sWrapperData);
            //Player PlayerData = JsonConvert.DeserializeObject<Player>(Wrapper.ReturnedJsonData);
            //PlayerData.CreditBalance += 100;
            //client.SavePlayer(1, JsonConvert.SerializeObject(PlayerData));
            Console.ReadLine();
        }
    }
}
