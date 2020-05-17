using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    interface IServersManager
    {
        IEnumerable<Servers> GetAllServers();
        void AddServer(Servers f);
        void DeleteServer(Servers p);
    }
}
