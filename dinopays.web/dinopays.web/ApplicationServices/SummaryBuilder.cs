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
            var response = await _starlingClient.GetTransactions(from, to, cancel);

            var transactions = MapTransactions(response, cancel);

            var finalSummary = transactions.Aggregate(new Summary(), Folder);
            finalSummary.PositiveTransactions = transactions.Where(t => t.Direction == Direction.Outbound &&
                                                                        t.PositivityCategory == PositivityCategory.Positive)
                                                            .OrderByDescending(t => t.CreatedAt)
                                                            .Take(5);
            finalSummary.NegativeTransactions = transactions.Where(t => t.Direction == Direction.Outbound &&
                                                                        t.PositivityCategory == PositivityCategory.Negative)
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

        private List<Transaction> MapTransactions(TransactionsResponse response, CancellationToken cancel)
        {
            return response.Transactions.AsParallel().Select(t =>
            {
                var spendingCategory = GetSpendingCategory(t, cancel).GetAwaiter().GetResult();
                return new Transaction
                {
                    CreatedAt = t.Created,
                    Amount = Math.Abs(t.Amount),
                    Description = t.Narrative,
                    Direction = t.Direction,
                    PositivityCategory = Categorise(spendingCategory),
                    SpendingCategory = spendingCategory
                };
            }).ToList();
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