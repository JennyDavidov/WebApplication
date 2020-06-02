using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightControl.Models;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Text.Json;
using Newtonsoft.Json;
using System.Net;

namespace FlightControl.Models
{
    public class MyServersManager : IServersManager
    {
        public static ConcurrentDictionary<string, Servers> CacheServers = new ConcurrentDictionary<string, Servers>();
        public static ConcurrentDictionary<string, Servers> ServerToFlightDic = new ConcurrentDictionary<string, Servers>();
        public static List<Servers> activeServers = new List<Servers>();

        public void AddServer(Servers s)
        {
            if (!CacheServers.TryAdd(s.ServerId, s))
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

        public async Task<ActionResult<List<Servers>>> GetAllActiveServers(string relative_to)
        {
            var client = new HttpClient();
            foreach (var server in this.GetAllServers())
            {
                try
                {
                    string url = server.Value.ServerURL + "/api/Flights?relative_to=" + relative_to;
                    var contentt = await client.GetStringAsync(url);
                }
                catch
                {
                    continue;
                }
                activeServers.Add(server.Value);
            }
            return activeServers;
        }
    }
}
        