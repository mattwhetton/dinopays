using System.Collections.Generic;
using dinopays.web.ApplicationServices;
using dinopays.web.Models;
using dinopays.web.Starling.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace dinopays.web.Controllers
{
    [Route("api/{username}/[controller]")]
    public class CategoriesController : Controller
    {
        // PUT api/<controller>/5
        [HttpPut]
        public void Put(string username,
                        [FromBody] Dictionary<SpendingCategory, PositivityCategory> categories,
                        [FromServices] IMongoCollection<User> users)
        {
            users.FindOneAndUpdate(u => u.Username == username,
                                   Builders<User>.Update.Set(u => u.Categories, categories.Stringify()));
        }
    }
}
