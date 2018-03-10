using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using dinopays.web.Models;
using dinopays.web.Starling;
using dinopays.web.Starling.Models;

namespace dinopays.web.ApplicationServices
{
    public class SummaryBuilder : ISummaryBuilder
    {
        private readonly IStarlingClient _starlingClient;

        public SummaryBuilder(IStarlingClient starlingClient)
        {
            _starlingClient = starlingClient;
        }


        public async Task<Summary> Summarise(DateTimeOffset from, DateTimeOffset to, CancellationToken cancel)
        {
            var response = await _starlingClient.GetTransactions(from, to, cancel);

            return response.Transactions.Aggregate(new Summary(), Folder);

            Summary Folder(Summary summary, TransactionSummary transaction)
            {
                switch (transaction.Direction)
                {
                    case Direction.Inbound:
                        return new Summary
                        {
                            TotalIncoming = summary.TotalIncoming + transaction.Amount,
                            TotalOutgoing = summary.TotalOutgoing,
                            PositiveOutgoing = summary.PositiveOutgoing,
                            NegativeOutgoing = summary.NegativeOutgoing
                        };
                    case Direction.Outbound:
                        var amount = Math.Abs(transaction.Amount);
                        var category = Categorise(transaction, cancel).GetAwaiter().GetResult();

                        return new Summary
                        {
                            TotalIncoming = summary.TotalIncoming,
                            TotalOutgoing = summary.TotalOutgoing + Math.Abs(transaction.Amount),
                            PositiveOutgoing = summary.PositiveOutgoing + (category == PositivityCategory.Positive ? amount : 0),
                            NegativeOutgoing = summary.NegativeOutgoing + (category == PositivityCategory.Negative ? amount : 0),
                        };
                    default:
                        return summary;
                }
            }
        }

        async Task<PositivityCategory> Categorise(TransactionSummary transaction, CancellationToken cancel)
        {

            var spendingCategory = await GetSpendingCategory(transaction, cancel);

            switch (spendingCategory)
            {
                case SpendingCategory.BillsAndServices:
                    break;
                case SpendingCategory.EatingOut:
                    return PositivityCategory.Negative;
                case SpendingCategory.Entertainment:
                    break;
                case SpendingCategory.Expenses:
                    break;
                case SpendingCategory.General:
                    break;
                case SpendingCategory.Groceries:
                    break;
                case SpendingCategory.Shopping:
                    break;
                case SpendingCategory.Holidays:
                    return PositivityCategory.Positive;
                case SpendingCategory.Payments:
                    break;
                case SpendingCategory.Transport:
                    break;
                case SpendingCategory.Lifestyle:
                    break;
                case SpendingCategory.None:
                    break;
                default:
                    return PositivityCategory.Neutral;
            }

            return PositivityCategory.Neutral;
        }

        private Task<SpendingCategory> GetSpendingCategory(TransactionSummary transaction, CancellationToken cancel)
        {
            if (transaction.Source == Source.MasterCard)
            {
                return _starlingClient.GetMastercardTransactionCategory(transaction.Id, cancel);
            }
            else if (transaction.Source == Source.DirectDebit)
            {
                return _starlingClient.GetDirectDebitTransactionCategory(transaction.Id, cancel);
            }
            else
            {
                return Task.FromResult(SpendingCategory.None);
            }
        }
    }
}