using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using dinopays.web.ApplicationServices;
using dinopays.web.Models;
using dinopays.web.Starling;
using dinopays.web.Starling.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace dinopays.web.Controllers
{
    [Route("api/[controller]")]
    public class PetsController : Controller
    {
        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<Pet> Get(Guid id,
                                   [FromServices] ISummaryBuilder summaryBuilder,
                                   CancellationToken cancel)
        {
            var now = DateTimeOffset.UtcNow;
            var then = now.AddMonths(-1);

            var finalSummary = await summaryBuilder.Summarise(then, now, cancel);

            return new Pet
            {
                Id = id,
                Health = CalculateHealth(finalSummary),
                BonusHealth = CalculateBonus(finalSummary),
                Summary = finalSummary,
            };
        }

        private int CalculateBonus(Summary summary)
        {
            var bonus = PositiveNegativeBonus() + GoalBonus();

            if (bonus < -3)
            {
                return -3;
            }
            else if (bonus > 3)
            {
                return 3;
            }

            return bonus;

            int PositiveNegativeBonus()
            {
                var diff = summary.PositiveOutgoing - summary.NegativeOutgoing;

                if (diff > 0)
                {
                    return 1;
                }

                if (diff < 0)
                {
                    return -1;
                }

                return 0;
            }

            int GoalBonus()
            {
                return summary.Goals
                              .Select(g => g.OnTarget ? 1 : -1)
                              .Sum();

            }
        }

        int CalculateHealth(Summary summary)
        {
            if (summary.TotalIncoming == 0)
            {
                return 0;
            }

            return 10 - (int) Math.Round((summary.TotalOutgoing / summary.TotalIncoming) * 10,
                                         MidpointRounding.AwayFromZero);
        }
    }
}
