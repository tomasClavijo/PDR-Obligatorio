using LogsServer.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiRabbitMQ.Service;

namespace LogsServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoggerController : ControllerBase
    {
        IMQService _mq;

        public LoggerController(IMQService mq)
        {
            _mq = mq;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] string username, [FromQuery] DateTime date, [FromQuery] string word)
        {
            if (username is null)
                username = "";
            if (word is null)
                word = "";
            string response = JsonConvert.SerializeObject(_mq.GetMessages(username, date, word));
            return Ok(response);
        }
    }
}
