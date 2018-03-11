using System;
using dinopays.web.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace dinopays.web.Controllers
{
    [Route("api/{username}/[controller]")]
    public class GoalsController : Controller
    {
        // GET: api/<controller>
        [HttpPost]
        public void Post(string username,
                         [FromBody] CreateGoalRequest request,
                         [FromServices] IMongoCollection<User> users)
        {
            var update = Builders<User>.Update.Push(u => u.Goals,
                                                    new Goal
                                                    {
                                                        Id = Guid.NewGuid(),
                                                        Frequency = request.Frequency,
                                                        GoalDirection = request.GoalDirection,
                                                        Name = request.Name,
                                                        Threshold = request.Threshold,
                                                        MatchingTransactions = request.MatchingTransactions
                                                    });

            users.FindOneAndUpdate(u => u.Username == username, update);
        }
    }
}
