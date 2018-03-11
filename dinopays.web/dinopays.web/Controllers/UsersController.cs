using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dinopays.web.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace dinopays.web.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]CreateUserRequest request,
                         [FromServices] IMongoCollection<User> users)
        {
            if (users.AsQueryable().Any(u => u.Username == request.Username))
            {
                throw new Exception("Username Taken");
            }

            var user = new User {Id = Guid.NewGuid(), AccessToken = request.AccessToken, Username = request.Username};
            users.InsertOne(user);
        }
    }
}
