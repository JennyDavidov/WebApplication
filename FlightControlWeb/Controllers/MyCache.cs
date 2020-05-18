using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb
{
    public class MyCache
    {
        private IMemoryCache cache;
        public MyCache(IMemoryCache cache1)
        {
            cache = cache1;
        }
    }
}
