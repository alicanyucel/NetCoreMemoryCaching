using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using in_memory_caching_in_aspnet_core.Context;
using in_memory_caching_in_aspnet_core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace in_memory_caching_in_aspnet_core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IMemoryCache memoryCache;

        private TestDBContext _context;
        public CustomerController(IMemoryCache memoryCache, TestDBContext context)
        {
            this.memoryCache = memoryCache;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cacheKey = "customerList";
            if (!memoryCache.TryGetValue(cacheKey, out List<Customer> customerList))
            {
                customerList = await _context.Customers.ToListAsync();
                var cacheExpiryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(5),
                    Priority = CacheItemPriority.High,
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };
                memoryCache.Set(cacheKey, customerList, cacheExpiryOptions);
            }

            return Ok(customerList);
        }

    }
}
