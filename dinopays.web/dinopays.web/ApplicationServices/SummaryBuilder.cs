using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
            var transactions = await GetTransactions(from, to, cancel);

            var finalSummary = transactions.Aggregate(new Summary(), Folder);
            finalSummary.RecentBonusTransactions = transactions.Where(t => t.Direction == Direction.Outbound &&
                                                                           t.PositivityCategory != PositivityCategory.Neutral)
                                                               .OrderByDescending(t => t.CreatedAt)
                                                               .Take(5);
            return finalSummary;

            Summary Folder(Summary summary, Transaction transaction)
            {
                var amount = transaction.Amount;
                var category = transaction.PositivityCategory;
                switch (transaction.Direction)
                {
                    case Direction.Inbound:
                        return new Summary
                        {
                            TotalIncoming = summary.TotalIncoming + amount,
                            TotalOutgoing = summary.TotalOutgoing,
                            PositiveOutgoing = summary.PositiveOutgoing,
                            NegativeOutgoing = summary.NegativeOutgoing
                        };
                    case Direction.Outbound:
                        return new Summary
                        {
                            TotalIncoming = summary.TotalIncoming,
                            TotalOutgoing = summary.TotalOutgoing + amount,
                            PositiveOutgoing = summary.PositiveOutgoing + (category == PositivityCategory.Positive ? amount : 0),
                            NegativeOutgoing = summary.NegativeOutgoing + (category == PositivityCategory.Negative ? amount : 0)
                        };
                    default:
                        return summary;
                }
            }
        }


        private async Task<IEnumerable<Transaction>> GetTransactions(DateTimeOffset from, 
                                                                                DateTimeOffset to, 
                                                                                CancellationToken cancel)
        {
            var allTransactionsTask = _starlingClient.GetTransactions(from, to, cancel);
            var masterCardTransactionsTask = _starlingClient.GetMasterCardTransactions(from, to, cancel);
            var directDebitTransactionsTask = _starlingClient.GetDirectDebitTransactions(from, to, cancel);

            var allTransactions = await allTransactionsTask;
            var masterCardTransactions = await masterCardTransactionsTask;
            var directDebitTransactions = await directDebitTransactionsTask;

            var filteredTransactions = allTransactions.Transactions
                                                      .Where(t => masterCardTransactions.Transactions.All(mt => mt.Id != t.Id) &&
                                                                  directDebitTransactions.Transactions.All(dt => dt.Id != t.Id));

            return filteredTransactions.Concat(masterCardTransactions.Transactions)
                                       .Concat(directDebitTransactions.Transactions)
                                       .Select(t => new Transaction
                                       {
                                           CreatedAt = t.Created,
                                           Amount = Math.Abs(t.Amount),
                                           Description = t.Narrative,
                                           Direction = t.Direction,
                                           PositivityCategory = Categorise(t.SpendingCategory),
                                           SpendingCategory = t.SpendingCategory
                                       });
        }

        PositivityCategory Categorise(SpendingCategory spendingCategory)
        {
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