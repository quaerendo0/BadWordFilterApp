using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BadWordFilterApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BadWordFilterApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BadWordController : ControllerBase
    {
        private readonly ILogger<BadWordController> _logger;
        public BadWordController(ILogger<BadWordController> logger)
        {
            _logger = logger;
        }
        [HttpPost]
        public string Post([FromForm]string input)
        {
            WordFilter filter = new WordFilter();
            return filter.FilterText(input);
        }
    }    
}