using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.UI;
using Newtonsoft.Json;
using Zakupowo.Models;

namespace Zakupowo.Services;

public class ExchangeRateService
{
    private readonly string _nbpApiUrl = "https://api.nbp.pl/api/exchangerates/rates/A/";
    private ZakupowoDbContext db = new ZakupowoDbContext();

    public ExchangeRateService(ZakupowoDbContext dbContext)
    {
        db = dbContext;
    }

    public async Task UpdateExchangeRatesAsync()
    {
        var currencies = new[] { "USD", "EUR" };

        foreach (var currency in currencies)
        {
            var rate = await GetExchangeRateFromNbpAsync(currency);
            if (rate != null)
            {
                var existingRate = db.Currencies.FirstOrDefault(r => r.CurrencyCode == currency);
                if (existingRate != null)
                {
                    existingRate.ExchangeRate = rate.Value;
                    existingRate.LastUpdate = DateTime.UtcNow;
                }
                else
                {
                    db.Currencies.Add(new Currency()
                    {
                        CurrencyCode = currency,
                        ExchangeRate = rate.Value,
                        LastUpdate = DateTime.UtcNow
                    });
                }
            }
        }
        await db.SaveChangesAsync();
    }

    private async Task<decimal?> GetExchangeRateFromNbpAsync(string currencyCode)
    {
        using (var httpClient = new HttpClient())
        {
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var url = $"{_nbpApiUrl}{currencyCode}/?format=json";
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<NbpExchangeRateResponse>(json);
                return data?.Rates?.FirstOrDefault()?.Mid;
            }
        }
        return null;
    }
    public class NbpExchangeRateResponse
    {
        public string Table { get; set; }
        public string Currency { get; set; }
        public string Code { get; set; }
        public List<NbpRate> Rates { get; set; }
    }

    public class NbpRate
    {
        public string No { get; set; }
        public DateTime EffectiveDate { get; set; }
        public decimal Mid { get; set; }
    }
}