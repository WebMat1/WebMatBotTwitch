using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMatBotV3.Shared;

namespace WebMatBotV3.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommandController : ControllerBase
    {
        private readonly DataContext dataContext;

        [HttpGet("{command}")]
        public async Task<string> Get(string command)
        {
            return await WebMatBot.Program.SendCommand(command);
        }
    }
}
