using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
                                   [FromServices] IStarlingClient starlingClient,
                                   CancellationToken cancel)
        {
            var now = DateTimeOffset.UtcNow;
            var then = now.AddMonths(-1);

            var response = await starlingClient.GetTransactions(then, now, cancel);

            var finalSummary = response.Transactions.Aggregate(new Summary {TotalIncoming = 0m, TotalOutgoing = 0m}, Folder);

            return new Pet
            {
                Id = id,
                Health = CalculateHealth(finalSummary)
            };

            Summary Folder(Summary summary, TransactionSummary transaction)
            {
                switch(transaction.Direction)
                {
                    case Direction.Inbound:
                        return new Summary
                        {
                            TotalIncoming = summary.TotalIncoming + transaction.Amount,
                            TotalOutgoing = summary.TotalOutgoing
                        };
                    case Direction.Outbound:
                        return new Summary
                        {
                            TotalIncoming = summary.TotalIncoming,
                            TotalOutgoing = summary.TotalOutgoing + Math.Abs(transaction.Amount)
                        };
                    default:
                        return summary;
                }
            }
        }

        int CalculateHealth(Summary summary)
        {
            if (summary.TotalIncoming == 0)
            {
                return 0;
            }

            return 10 - (int)Math.Round((summary.TotalOutgoing / summary.TotalIncoming) * 10, 
                                        MidpointRounding.AwayFromZero);
        }
    }

    public class Summary
    {
        public decimal TotalIncoming { get; set; }

        public decimal TotalOutgoing { get; set; }
    }
}
