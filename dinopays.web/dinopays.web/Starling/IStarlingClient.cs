using System;
using System.Threading;
using System.Threading.Tasks;
using dinopays.web.Starling.Models;

namespace dinopays.web.Starling
{
    public interface IStarlingClient
    {
        Task<TransactionsResponse> GetTransactions(DateTimeOffset from,
                                                   DateTimeOffset to,
                                                   CancellationToken cancel);

        Task<SpendingCategory> GetMastercardTransactionCategory(Guid id, CancellationToken cancel);

        Task<SpendingCategory> GetDirectDebitTransactionCategory(Guid id, CancellationToken cancel);
    }
}
