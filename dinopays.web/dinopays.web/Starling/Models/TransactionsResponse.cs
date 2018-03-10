using System.Collections.Generic;

namespace dinopays.web.Starling.Models
{
    public class TransactionsResponse
    {
        public IEnumerable<CategorisedTransaction> Transactions { get; set; }
    }
}