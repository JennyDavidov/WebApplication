using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class MyServersManager : IServersManager
    {
        private static List<Servers> ServersList = new List<Servers>();
        public void AddServer(Servers s)
        {
            ServersList.Add(s);
        }

        public void DeleteServer(Servers s)
        {
            ServersList.Remove(s);
        }

        public IEnumerable<Servers> GetAllServers()
        {
            return ServersList;
        }
    }
}
