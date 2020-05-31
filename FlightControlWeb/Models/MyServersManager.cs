using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControl.Models
{
    public class MyServersManager : IServersManager
    {
        public static ConcurrentDictionary<string, Servers> CacheServers = new ConcurrentDictionary<string, Servers>();
        public static ConcurrentDictionary<string, Servers> ServerToFlightDic= new ConcurrentDictionary<string, Servers>();

        public void AddServer(Servers s)
        {
            if (!CacheServers.TryAdd(s.ServerId,s))
            {
                Console.WriteLine("Error adding item to cache");
            }
            Console.WriteLine("add plan succeeded");
        }

        public void DeleteServer(Servers s)
        {
            string key = CacheServers.FirstOrDefault(x => x.Value == s).Key;
            if (key == null)
            {
                throw new Exception("Server Plan not found");
            }
            else
            {
                if (!CacheServers.TryRemove(key, out s))
                {
                    Console.WriteLine("Error removing item from cache");
                }
            }
        }

        public ConcurrentDictionary<string, Servers> GetAllServers()
        {
            return CacheServers;
        }

        public ConcurrentDictionary<string, Servers> GetServerToFlightDic()
        {
            return ServerToFlightDic;
        }
    }
}
