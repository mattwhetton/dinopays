using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using dinopays.web.Data;
using dinopays.web.Models;
using dinopays.web.Starling;
using dinopays.web.Starling.Models;

namespace dinopays.web.ApplicationServices
{
    public class SummaryBuilder : ISummaryBuilder
    {
        private readonly IStarlingClientFactory _starlingClientFactory;

        public SummaryBuilder(IStarlingClientFactory starlingClientFactory)
        {
            _starlingClientFactory = starlingClientFactory;
        }


        public async Task<Summary> Summarise(User user, DateTimeOffset from, DateTimeOffset to, CancellationToken cancel)
        {
            var client = _starlingClientFactory.CreateClient(user.AccessToken);
            var transactions = (await GetTransactions(client, from, to, cancel)).ToList();

            var finalSummary = transactions.Aggregate(new Summary(), Folder);
            finalSummary.RecentBonusTransactions = transactions.Where(t => t.Direction == Direction.Outbound &&
                                                                           t.PositivityCategory != PositivityCategory.Neutral)
                                                               .OrderByDescending(t => t.CreatedAt)
                                                               .Take(5);
            finalSummary.Goals = AnalyseGoals(user, transactions);

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


        private async Task<IEnumerable<Transaction>> GetTransactions(IStarlingClient client,
                                                                     DateTimeOffset from, 
                                                                     DateTimeOffset to, 
                                                                     CancellationToken cancel)
        {

            var allTransactionsTask = client.GetTransactions(from, to, cancel);
            var masterCardTransactionsTask = client.GetMasterCardTransactions(from, to, cancel);
            var directDebitTransactionsTask = client.GetDirectDebitTransactions(from, to, cancel);

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

        private IEnumerable<GoalStatus> AnalyseGoals(User user, IEnumerable<Transaction> transactions)
        {
            return user.Goals.Select(AnalyseGoal);

            GoalStatus AnalyseGoal(Goal goal)
            {
                var tomorrow = new DateTimeOffset(DateTime.Today.AddDays(1), TimeSpan.Zero);
                var from = goal.Frequency == Frequency.Daily
                    ? tomorrow.AddDays(-1)
                    : tomorrow.AddDays(-7);

                var currentSpend = transactions.Where(t => goal.MatchingTransactions
                                                               .Contains(t.Description) &&
                                                           t.CreatedAt >= from &&
                                                           t.CreatedAt < tomorrow)
                                               .Select(t => t.Amount)
                                               .Sum();

                bool onTarget = goal.GoalDirection == GoalDirection.Over
                    ? currentSpend >= goal.Threshold
                    : currentSpend <= goal.Threshold;

                return new GoalStatus
                {
                    CurrentSpend = currentSpend,
                    Goal = goal,
                    OnTarget = onTarget
                };
            }
        }
    }
}