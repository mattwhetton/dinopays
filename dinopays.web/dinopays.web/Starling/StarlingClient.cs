using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using dinopays.web.Options;
using dinopays.web.Starling.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace dinopays.web.Starling
{
    public class StarlingClient : IStarlingClient
    {
        private readonly HttpClient _httpClient;

        public StarlingClient(IOptions<StarlingOptions> options)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api-sandbox.starlingbank.com/api/v1/"),
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue("Bearer", options.Value.StarlingAccessToken)
                }
            };
        }

        public async Task<TransactionsResponse> GetTransactions(DateTimeOffset from,
                                                                DateTimeOffset to,
                                                                CancellationToken cancel)
        {
            var fromParam = from.ToUniversalTime().ToString("yyyy-MM-dd");
            var toParam = to.ToUniversalTime().ToString("yyyy-MM-dd");
            using (var response = await _httpClient.GetAsync($"transactions?from={fromParam}&to={toParam}", cancel))
            {
                var transactions = await response.Content.ReadAsStringAsync();

                return Deserialize<EmbedResponse<TransactionsResponse>>(transactions)._embedded;
            }
        }

        public async Task<SpendingCategory> GetMastercardTransactionCategory(Guid id, CancellationToken cancel)
        {
            using (var response = await _httpClient.GetAsync($"transactions/mastercard/{id}", cancel))
            {
                var transaction = await response.Content.ReadAsStringAsync();

                return Deserialize<CategorisedTransaction>(transaction).SpendingCategory;
            }
        }

        public async Task<SpendingCategory> GetDirectDebitTransactionCategory(Guid id, CancellationToken cancel)
        {
            using (var response = await _httpClient.GetAsync($"transactions/direct-debit/{id}", cancel))
            {
                var transaction = await response.Content.ReadAsStringAsync();

                return Deserialize<CategorisedTransaction>(transaction).SpendingCategory;
            }
        }

        private static T Deserialize<T>(string content)
        {
            return JsonConvert.DeserializeObject<T>(content,
                                                    new JsonSerializerSettings
                                                    {
                                                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                                                        Converters = {new StringEnumConverter()}
                                                    });
        }
    }
}