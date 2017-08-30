using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SpaceServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SpaceService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SpaceService.svc or SpaceService.svc.cs at the Solution Explorer and start debugging.
    public class SpaceService : ISpaceService
    {
        public List<Player> GetPlayerList()
        {
            return Players.Instance.PlayerList;
        }
    }
}
