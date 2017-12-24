using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using S7Test.Core.Entity.App;
using S7Test.Core.Entity.Domain;
using S7Test.Core.Interface.Service.App;
using S7Test.Core.Interface.Service.Domain;

namespace S7Test.Web.Controllers.Api
{
    [Authorize(Policy = "CustomApiAuthzPolicy")]
    [Produces("application/json")]
    [Route("api/Team")]
    public class TeamController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly IPlayerService _playerService;

        public TeamController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,            
            IEmailSender emailSender,
            ISmsSender smsSender,
            IPlayerService playerService,
            ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _playerService = playerService;
            _logger = loggerFactory.CreateLogger<TeamController>();
        }

        // GET: api/Teams
        [HttpGet("{id:int?}")]
        public IEnumerable<Team> Get(int? id = 0)
        {
            return _playerService.GetTeamOrList(id.Value);
        }
                
        // POST: api/Team
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // DELETE: api/Team/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
