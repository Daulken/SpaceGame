using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace SpaceServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISpaceService" in both code and config file together.
    [ServiceContract]
    public interface ISpaceService
    {
        [OperationContract]
        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Xml,
            BodyStyle = WebMessageBodyStyle.Bare,
            UriTemplate = "GetPlayerList/")]
        List<Player> GetPlayerList();
    }
}
