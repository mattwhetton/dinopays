using System.Collections.Generic;
using dinopays.web.Data;
using dinopays.web.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace dinopays.web.Controllers
{
    [Route("api/[controller]")]
    public class GoalsController : Controller
    {
        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<Goal> Get([FromServices] IGoalRepository goalRepository)
        {
            return goalRepository.Goals;
        }
    }
}
