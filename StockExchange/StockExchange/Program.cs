using System;
using System.Linq;
using System.Net;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace StockExchange
{
    internal class Program
    {
        const string tickers = "AAPL,GOOG,GOOGL,YHOO,TSLA,INTC,AMZN,BIDU,ORCL,MSFT,ORCL,ATVI,NVDA,LNKD,NFLX,A,AZZ,SHLM,ADES,PIH,SAFT,SANM,SASR,FLWS,FCCY,SRCE,VNET";
        private static void Main()
        {
            var timer = new Timer(Callback, "Some state", TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
            Thread.Sleep(Timeout.Infinite); // Wait a bit over 4 seconds.
            //timer.Change(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
            //Thread.Sleep(Timeout.Infinite);
            /*timer.Change(-1, -1);*/ // Stop the timer from running.
        }
        private static void Callback(object state)
        {
            Console.Clear();
            string json;

            using (var web = new WebClient())
            {
                var url = $"http://finance.google.com/finance/info?client=ig&q=NASDAQ%3A{tickers}";
                json = web.DownloadString(url);
            }

            //Google adds a comment before the json for some unknown reason, so we need to remove it
            json = json.Replace("//", "");

            var v = JArray.Parse(json);

            foreach (var i in v)
            {
                var ticker = i.SelectToken("t");
                var price = (decimal)i.SelectToken("l");
                var lastTime = i.SelectToken("lt");
                var change = i.SelectToken("c");
                if (change.Value<string>() == "")
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{ticker} : {price} : {lastTime} : 0.00");
                }
                if (change.Value<string>() != "" && (decimal)change < 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{ticker} : {price} : {lastTime} : {change}");
                }
                if (change.Value<string>() != "" && (decimal)change > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{ticker} : {price} : {lastTime} : {change}");
                }

            }
        }
    }
}