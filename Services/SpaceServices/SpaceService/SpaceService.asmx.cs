using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using SpaceLibrary;
using System;
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

            using (DataConnection dbConnection = new DataConnection())
            {
                string query = "SELECT PlayerId, PlayerData FROM Player";
                MySqlCommand cmd = new MySqlCommand(query, dbConnection.Connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    retVal.ResponseSuccessful = true;
                    Player returnPlayer = new Player();

                    returnPlayer.PlayerId = reader.GetInt32(0);
                    retVal.ReturnedJsonData = reader.GetString(1);
                    reader.Close();
                    return JsonConvert.SerializeObject(retVal);
                }
                else
                {
                    retVal.ResponseSuccessful = false;
                    retVal.ResponseMessage = "Player not found";
                    retVal.ResponseCode = 1000;
                    reader.Close();
                    return JsonConvert.SerializeObject(retVal);
                }
            }
        }

        [WebMethod]
        public void SavePlayer(int PlayerId, string PlayerData)
        {
            ServiceWrapper retVal = Player.PlayerWrapper();

            using (DataConnection dbConnection = new DataConnection())
            {
                string query = "UPDATE Player SET PlayerData=@PlayerData WHERE PlayerId=@PlayerId";
                MySqlCommand cmd = new MySqlCommand(query, dbConnection.Connection);
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@PlayerId", PlayerId);
                cmd.Parameters.AddWithValue("@PlayerData", PlayerData);
                cmd.ExecuteNonQuery();
            }
        }
    }
}