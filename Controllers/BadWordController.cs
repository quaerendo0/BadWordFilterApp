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
        private readonly IWordFilter _filter;
        public BadWordController(IWordFilter filter, ILogger<BadWordController> logger)
        {
            _logger = logger;
            _filter = filter;
        }
        [HttpPost]
        public IActionResult Post([FromForm]string input)
        {
            try
            {
                var start = DateTime.Now;
                var censoredText = _filter.FilterText(input);
                TimeSpan timeDiff = DateTime.Now - start;
                _logger.LogInformation("Time spent on filtering {0}:{1}:{2}.{3}", timeDiff.Hours, timeDiff.Minutes, timeDiff.Seconds, timeDiff.Milliseconds);
                return new ContentResult { Content = censoredText };
            }
            catch (WordFilterNotConfigured error)
            {
                _logger.LogInformation(error.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Something went ****ing wrong, can't filter ****");
            }
        }
    }    
}