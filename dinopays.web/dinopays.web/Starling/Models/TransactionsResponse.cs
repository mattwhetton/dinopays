using System.Collections.Generic;

namespace dinopays.web.Starling.Models
{
    public class TransactionsResponse
    {
        public IEnumerable<TransactionSummary> Transactions { get; set; }
    }
}