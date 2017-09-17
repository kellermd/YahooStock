using Microsoft.Extensions.Configuration;
using StockData.Library.DataContext;
using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace YahooStockReader
{
    class Program
    {
        static Configuration Config { get; set; }

        class Configuration
        {
            private Configuration()
            {
            }
            public Configuration(IConfigurationSection configurationSection)
            {
                configurationSection.Bind(this);
            }

            public EmailConfiguration Email { get; set; }

            public class EmailConfiguration
            {
                public string Host { get; set; }
                public int Port { get; set; }
                public bool EnableSsl { get; set; }
                public bool UseDefaultCredentials { get; set; }
                public CredentialType Credential { get; set; }
                public EmailUser Sender { get; set; }
                public List<EmailUser> Recipients { get; set; }
                public string Subject { get; set; }

                public class CredentialType
                {
                    public string Username { get; set; }
                    public string Password { get; set; }
                }

                public class EmailUser
                {
                    public string Name { get; set; }
                    public string Address { get; set; }
                }
            }
        }

        static void UpdateStockDaily()
        {
            using (var db = new StockDataContext())
            {
                var stocks = (from stock in db.Stock select stock).ToList();

                try
                {
                    var stockDailyList = YahooService.Library.ServiceContext.Service.GetStockDailyList(stocks);
                    if (stockDailyList != null)
                    {
                        foreach (var stockDaily in stockDailyList)
                        {
                            var existingStockDaily = (from sd in db.StockDaily
                                                      where sd.Stock == stockDaily.Stock &&
                                                            sd.DailyDate == stockDaily.DailyDate
                                                      select sd).FirstOrDefault();
                            if (existingStockDaily == null)
                            {
                                db.StockDaily.Add(stockDaily);
                            }
                            else
                            {
                                if (existingStockDaily.Equals(stockDaily))
                                {
                                }
                                else
                                {
                                    stockDaily.Copy(existingStockDaily);
                                }
                            }
                        }
                        int recordsAffected = db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    LogWriteLine(string.Format("Failed to process 'StockDaily'.  Exception:{0}", ex.Message));
                }
           }
        }

        static void TestStockQuote()
        {
            using (var db = new StockDataContext())
            {
                var stocks = db.Stock.ToList();

                try
                {
                    var stockQuotes = YahooService.Library.ServiceContext.Service.GetStockQuoteList(stocks);
                    foreach (var stockQuote in stockQuotes)
                    {
                        var existingStockQuote = (from squote in db.StockQuote
                                                  where squote.Stock == stockQuote.Stock &&
                                                        squote.TradeDateTime == stockQuote.TradeDateTime
                                                  select squote).FirstOrDefault();
                        if (existingStockQuote == null)
                        {
                            db.StockQuote.Add(stockQuote);
                        }
                    }
                    int recordsAffected = db.SaveChanges();
                }
                catch (Exception ex)
                {
                    LogWriteLine(string.Format("Failed to process 'StockQuote'.  Exception:{0}", ex.Message));
                    throw ex;
                }
            }
        }

        static void UpdateStock()
        {
            using (var db = new StockDataContext())
            {
                var stocks = db.Stock.ToList();
                try
                {
                    var stockList = YahooService.Library.ServiceContext.Service.GetStockList(stocks);
                    if (stockList != null)
                    {
                        foreach (var stock in stockList)
                        {
                            var existingStock = (from squote in db.Stock 
                                                 where squote.Symbol == stock.Symbol
                                                 select squote).FirstOrDefault();
                            if (existingStock == null)
                            {
                                db.Stock.Add(stock);
                            }
                            else
                            {
                                existingStock.Copy(stock);
                                    //db.Stock.Update(theStock);
                            }
                        }
                        int recordsAffected = db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    LogWriteLine(string.Format("Failed to process 'Stock'.  Exception:{0}", ex.Message));
                    throw ex;
                }
            }
        }

        static void UpdateStockPeriodic()
        {
            using (var db = new StockDataContext())
            {
                var stocks = db.Stock.ToList();
                try
                {
                    var stockPeriodList = YahooService.Library.ServiceContext.Service.GetPeriodic(stocks);
                    if (stockPeriodList != null)
                    {
                        foreach (var stockPeriodic in stockPeriodList)
                        {
                            var existingStockPeriodic = (from speriodic in db.StockPeriodic
                                                         where speriodic.Stock == stockPeriodic.Stock &&
                                                               speriodic.EndDate == null
                                                         select speriodic).FirstOrDefault();
                            if (existingStockPeriodic == null)
                            {
                                db.StockPeriodic.Add(stockPeriodic);
                            }
                            else
                            {
                                if (existingStockPeriodic.IsValuesEqual(stockPeriodic))
                                {
                                    // Nothing to update
                                }
                                else
                                {
                                    existingStockPeriodic.EndDate = stockPeriodic.BeginDate.AddDays(-1);
                                    db.StockPeriodic.Add(stockPeriodic);
                                }
                            }
                        }
                        int recordsAffected = db.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    LogWriteLine(string.Format("Failed to process 'StockPeriod'.  Exception:{0}", ex.Message));
                    throw ex;
                }
            }
        }

        /***
        static void TestDB()
        {
            using (var db = new StockDataContext())
            {
                var stockList = db.Stock.Where(stock => stock.Symbol == "A").ToList();
                string[] stockSymbols = { "A", "B" };
                var existingStocks = db.Stock.Where(stock => stockSymbols.Contains(stock.Symbol)).ToList();

                var newStocks = new Stock[]
                {
                    new Stock() { Symbol = "A", Name = "AT&T", CurrencyCode = "USD", StockExchangeCode = "NYQ" },
                    new Stock() { Symbol = "B", Name = "Bayer", CurrencyCode = "USD", StockExchangeCode = "NYQ" },
                };
                foreach (var newStock in newStocks)
                {
                    var existingStock = (existingStocks.Find(stock => stock.Symbol == newStock.Symbol));
                    if (existingStock == null)
                    {
                        db.Stock.Add(newStock);
                        existingStock = newStock;
                    }
                    else
                    {
                        existingStock.Copy(newStock);
                    }
                    var stockQuote = new StockQuote()
                    {
                        Stock = existingStock,
                        Ask = (decimal)1.00,
                        Bid = 1.00M,
                        Volume = 1000,
                        TradePrice = 1.01M,
                        TradeDateTime = DateTime.Now,
                    };
                    db.StockQuote.Add(stockQuote);
                }

                var count = db.SaveChanges();
                LogWriteLine("{0} records saved to database", count);


                LogWriteLine();
                LogWriteLine("All Stock in database:");
                foreach (var stock in db.Stock)
                {
                    LogWriteLine("{0} - {1}", stock.Symbol, stock.Name);
                }
            }
        }
        ***/

        // --------------------------------------------------------------------

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json");
            return builder.Build();
        }

        static void Initialize(IConfigurationSection configurationSection)
        {
            Program.Config = new Configuration(configurationSection);
        }

        private static StringBuilder s_LogStringBuilder = new StringBuilder();
        static void LogWriteLine(string message)
        {
            Console.WriteLine(message);
            s_LogStringBuilder.AppendLine(message);
        }

        static void Main(string[] args)
        {
            LogWriteLine("Yahoo Stock Reader!");

            // ----------------------------------------------------------------
            // Configuration and library initialization
            {
                IConfiguration configuration = GetConfiguration();
                StockData.Library.LibraryContext.Initialize(configuration.GetSection("StockData.Library"));
                YahooService.Library.LibraryContext.Initialize(configuration.GetSection("YahooService.Library"));
                Program.Initialize(configuration.GetSection("YahooStockReader"));
            }
            // ----------------------------------------------------------------

            bool successAll = true;
            var actions = new List<Action>{UpdateStock, UpdateStockDaily, UpdateStockPeriodic};
            const int  k_maxRetries = 10;

            foreach (var action in actions)
            {
                bool success = false;
                int retry = 0;
                do
                {
                    try
                    {
                        LogWriteLine(string.Format("Running action using method '{0}'", action.Method.Name));
                        action();
                        success = true;
                    }
                    catch (Exception)
                    {
                        LogWriteLine("Failure ... delaying for 10 seconds.");
                        var waitUntil = DateTime.Now.AddSeconds(10);
                        while (waitUntil > DateTime.Now)
                        {
                            Thread.Sleep(500);
                        }
                        retry++;
                    }
                }
                while (success == false && retry < k_maxRetries);

                if (retry >= k_maxRetries)
                {
                    LogWriteLine("The last action to be attempted was retried the maximum number of times.  No other action will be attempted and the program ends now.");
                    successAll = false;
                    break;
                }

                LogWriteLine(string.Format("Action using method '{0}' completed with {1} retries", action.Method.Name, retry));
            } // end-for

            if (successAll)
            {
                LogWriteLine("All actions were successfully completed.");
            }

            SendLogByEmail();
        }

        private static void SendLogByEmail()
        {
            var emailConfig = Program.Config.Email;

            using (SmtpClient smtp = new SmtpClient()
            {
                Host = emailConfig.Host,
                Port = emailConfig.Port,
                EnableSsl = emailConfig.EnableSsl,
                UseDefaultCredentials = emailConfig.UseDefaultCredentials,
                Credentials = new NetworkCredential(emailConfig.Credential.Username, emailConfig.Credential.Password),
            })
            {
                var fromAddress = new MailAddress(emailConfig.Sender.Address, emailConfig.Sender.Name);
                var firstRecipient = new MailAddress(emailConfig.Recipients[0].Address, emailConfig.Recipients[0].Name);
                var content = s_LogStringBuilder.ToString();
                using (var msg = new MailMessage(fromAddress, firstRecipient))
                {
                    msg.Subject = emailConfig.Subject;
                    msg.Body    = content;
                    for (int i = 1; i < emailConfig.Recipients.Count; i++)
                    {
                        var recipient = emailConfig.Recipients[i];
                        msg.To.Add(new MailAddress(recipient.Address, recipient.Name));
                    }

                    try
                    {
                        smtp.Send(msg);
                    }
                    catch (Exception)
                    {
                        // TODO: Handle the exception
                    }
                }
            }
        }
    }
}
