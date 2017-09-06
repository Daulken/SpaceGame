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
            string sWrapperData = client.CreateMarketOrder(1, 1, 1, 2, 250, 40.23);
            Console.WriteLine(sWrapperData);
            Console.ReadLine();
        }
    }
}
