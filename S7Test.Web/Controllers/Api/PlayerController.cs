using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S7Test.Core.Entity.Domain;
using S7Test.Core.Interface.Service.Domain;
using S7Test.Core.Model;
using S7Test.Web.Models;

namespace S7Test.Web.Controllers.Api
{
    [Authorize(Policy = "CustomApiAuthzPolicy")]
    [Produces("application/json")]
    [Route("api/Player")]
    public class PlayerController : Controller
    {
        private readonly IPlayerService _playerService;

        public PlayerController(
            IPlayerService playerService)
        {
            _playerService = playerService;
        }

        //GET: api/Player/model
        [HttpGet]
        public PagingViewModel Get([FromQuery]PlayerFilterModel model)
        {
            return new PagingViewModel
            {
                Data = _playerService.GetPlayersByFilterModel(model, out int pageCount),
                PageCount = pageCount
            };
        }

        // POST: api/Player
        [HttpPost]
        public IActionResult Post([FromForm]PlayerViewModel model)
        {
            List<string> errors = null;
            try
            {
                if (!ModelState.IsValid)
                {
                    errors = new List<string>();
                    errors.AddRange(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                }
                else
                {
                    var player = new Player
                    {
                        Id = model.Id,
                        IsActive = model.IsActive,
                        Picture = model.Picture,
                        Age = model.Age,
                        YellowCards = model.YellowCards,
                        RedCards = model.RedCards,
                        Goals = model.Goals,
                        Appearances = model.Appearances,
                        Name = model.Name,
                        Gender = model.Gender,
                        Team = new Team { Id = model.Team .Id, Name = model.Team.Name }
                    };
                    if (_playerService.AddOrUpdatePlayer(player) > 0)
                    {
                        return Json(new { success = true, id = player.Id });
                    }
                    else errors.Add("Could not save changes, please try again later.");
                }
            }
            catch (System.Exception ex)
            {
                //if (errors == null) errors = new List<string>();
                //errors.Add(ex.InnerException?.Message ?? ex.Message);
            }
            return errors == null ? Json(new { success = true }) : Json(new { errors });
        }

        // DELETE: api/Player/5
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            List<string> errors = null;
            try
            {
                if (!ModelState.IsValid || !_playerService.DeletePlayerById(id))
                {
                    errors = new List<string>();
                    errors.AddRange(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                }
            }
            catch (System.Exception ex)
            {
                //if (errors == null) errors = new List<string>();
                //errors.Add(ex.InnerException?.Message ?? ex.Message);
            }
            return errors == null ? Json(new { success = true }) : Json(new { errors });

        }
    }
}
