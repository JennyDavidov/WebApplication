﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControl.Models
{
    interface IServersManager
    {
        ConcurrentDictionary<string, Servers> GetAllServers();
        void AddServer(Servers f);
        void DeleteServer(Servers p);
        public ConcurrentDictionary<string, Servers> GetServerToFlightDic();
        public Task<ActionResult<List<Servers>>> GetAllActiveServers(string relative_to);
    }
}
