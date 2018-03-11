using System.Collections.Generic;
using System.Linq;
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
                        [FromBody] IEnumerable<CategoryThinger> categories,
                        [FromServices] IMongoCollection<User> users)
        {
            users.FindOneAndUpdate(u => u.Username == username,
                                   Builders<User>.Update.Set(u => u.Categories,
                                                             categories.ToDictionary(t => t.Name, t => t.Status).Stringify()));
        }

        [HttpGet]
        public IEnumerable<CategoryThinger> Get(string username,
                                                                    [FromServices] IMongoCollection<User> users)
        {
            var user = users.AsQueryable().First(u => u.Username == username);

            return user.Categories.Enumify<SpendingCategory, PositivityCategory>().Select(kvp => new CategoryThinger
            {
                Name = kvp.Key,
                Status = kvp.Value
            });
        }
    }

    public class CategoryThinger
    {
        public SpendingCategory Name { get; set; }

        public PositivityCategory Status { get; set; }
    }
}
