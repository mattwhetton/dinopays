using System;
using dinopays.web.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace dinopays.web.Controllers
{
    [Route("api/[controller]")]
    public class PetsController : Controller
    {
        // GET api/<controller>/5
        [HttpGet("{id}")]
        public Pet Get(Guid id)
        {
            return new Pet
            {
                Id = id,
                Health = 5
            };
        }
    }
}
