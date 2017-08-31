using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceLibrary
{
    public class ServiceWrapper
    {
        public bool ResponseSuccessful { get; set; }
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string ReturnedDataType { get; set; }
        public string ReturnedDataTypeVersion { get; set; }
        public string ReturnedJsonData { get; set; }
    }
}
