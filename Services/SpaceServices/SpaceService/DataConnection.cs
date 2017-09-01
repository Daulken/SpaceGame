using MySql.Data;
using MySql.Data.MySqlClient;
using System;

namespace SpaceService
{
    public class DataConnection : IDisposable
    {
        public MySqlConnection Connection { get; set; }
        public DataConnection()
        {
            if (Connection == null)
            {
                string connstring = string.Format("Server=www.shootonsite.net; database=daulken_Space; UID=daulken_daulken; password=Perfect7");
                Connection = new MySqlConnection(connstring);
                Connection.Open();
            }
        }

        public void Dispose()
        {
            Connection.Close();
        }
    }
}