using System;
using System.Threading;
using Zakupowo.Services;

namespace Zakupowo;

public class ExchangeRateBackgroundTask
{
    private readonly Timer _timer;
    private readonly ExchangeRateService _exchangeRateService;

    public ExchangeRateBackgroundTask(ExchangeRateService exchangeRateService)
    {
        _exchangeRateService = exchangeRateService;
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
    }

    private async void DoWork(object state)
    {
        try
        {
            await _exchangeRateService.UpdateExchangeRatesAsync();
        }
        catch (Exception ex)
        {
            // Loguj wyjątki, jeśli używasz systemu logowania
            Console.WriteLine("Błąd w tle: " + ex.Message);
        }
    }

    public void Stop()
    {
        _timer.Dispose();
    }
}

