﻿using System;
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
            Console.WriteLine(client.HelloWorld());
            Console.ReadLine();
        }
    }
}
