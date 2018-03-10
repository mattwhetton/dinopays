using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using dinopays.web.Starling.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace dinopays.web.Starling
{
    public class StarlingClient : IStarlingClient
    {
        private readonly HttpClient _httpClient;

        public StarlingClient()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api-sandbox.starlingbank.com/api/v1/"),
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue("Bearer", 
                                                                  "xxx")
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

                return JsonConvert.DeserializeObject<EmbedResponse<TransactionsResponse>>(transactions,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        Converters = { new StringEnumConverter() }
                    })._embedded;
            }
        }
    }
}