using Newtonsoft.Json;
using SpaceLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace SpaceService
{
    /// <summary>
    /// Summary description for SpaceService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SpaceService : System.Web.Services.WebService
    {

        [WebMethod]
        public string GetPlayer(int PlayerId)
        {
            ServiceWrapper retVal = Player.PlayerWrapper();
            retVal.ResponseSuccessful = true;

            Player staticPlayer = new Player();
            string sPlayer = JsonConvert.SerializeObject(staticPlayer);
            retVal.ReturnedJsonData = sPlayer;
            return JsonConvert.SerializeObject(retVal);
        }
    }
}
