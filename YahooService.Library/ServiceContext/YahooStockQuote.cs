using Newtonsoft.Json;
using StockData.Library.DataContext;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Diagnostics;
using System.Net;

namespace YahooService.Library.ServiceContext
{
    public class YahooFinanceResponse 
    {
        public Query query { get; set; }

        public class Query
        {
            public int count { get; set; }
            public string created { get; set; }
            public string lang { get; set; }
            public Results results { get; set; }
        }

        public class Results
        {
            public List<Quote> quote { get; set; }

            public class Quote
            {
                public string symbol { get; set; }
                public string Ask { get; set; }
                public string AverageDailyVolume { get; set; }
                public string Bid { get; set; }
                //public object AskRealtime { get; set; }
                //public object BidRealtime { get; set; }
                public string BookValue { get; set; }
                public string Change_PercentChange { get; set; }
                public string Change { get; set; }
                public object Commission { get; set; }
                public string Currency { get; set; }
                //public object ChangeRealtime { get; set; }
                //public object AfterHoursChangeRealtime { get; set; }
                public string DividendShare { get; set; }
                public string LastTradeDate { get; set; }
                public object TradeDate { get; set; }
                public string EarningsShare { get; set; }
                public object ErrorIndicationreturnedforsymbolchangedinvalid { get; set; }
                public string EPSEstimateCurrentYear { get; set; }
                public string EPSEstimateNextYear { get; set; }
                public string EPSEstimateNextQuarter { get; set; }
                public string DaysLow { get; set; }
                public string DaysHigh { get; set; }
                public string YearLow { get; set; }
                public string YearHigh { get; set; }
                public object HoldingsGainPercent { get; set; }
                public object AnnualizedGain { get; set; }
                public object HoldingsGain { get; set; }
                //public object HoldingsGainPercentRealtime { get; set; }
                //public object HoldingsGainRealtime { get; set; }
                public object MoreInfo { get; set; }
                //public object OrderBookRealtime { get; set; }
                public string MarketCapitalization { get; set; }
                //public object MarketCapRealtime { get; set; }
                public string EBITDA { get; set; }
                public string ChangeFromYearLow { get; set; }
                public string PercentChangeFromYearLow { get; set; }
                //public object LastTradeRealtimeWithTime { get; set; }
                //public object ChangePercentRealtime { get; set; }
                public string ChangeFromYearHigh { get; set; }
                public string PercebtChangeFromYearHigh { get; set; }
                public string LastTradeWithTime { get; set; }
                public string LastTradePriceOnly { get; set; }
                public object HighLimit { get; set; }
                public object LowLimit { get; set; }
                public string DaysRange { get; set; }
                //public object DaysRangeRealtime { get; set; }
                public string FiftydayMovingAverage { get; set; }
                public string TwoHundreddayMovingAverage { get; set; }
                public string ChangeFromTwoHundreddayMovingAverage { get; set; }
                public string PercentChangeFromTwoHundreddayMovingAverage { get; set; }
                public string ChangeFromFiftydayMovingAverage { get; set; }
                public string PercentChangeFromFiftydayMovingAverage { get; set; }
                public string Name { get; set; }
                public object Notes { get; set; }
                public string Open { get; set; }
                public string PreviousClose { get; set; }
                public object PricePaid { get; set; }
                public string ChangeinPercent { get; set; }
                public string PriceSales { get; set; }
                public string PriceBook { get; set; }
                public string ExDividendDate { get; set; }
                public string PERatio { get; set; }
                public string DividendPayDate { get; set; }
                //public object PERatioRealtime { get; set; }
                public string PEGRatio { get; set; }
                public string PriceEPSEstimateCurrentYear { get; set; }
                public string PriceEPSEstimateNextYear { get; set; }
                public string Symbol { get; set; }
                public object SharesOwned { get; set; }
                public string ShortRatio { get; set; }
                public string LastTradeTime { get; set; }
                public object TickerTrend { get; set; }
                public string OneyrTargetPrice { get; set; }
                public string Volume { get; set; }
                public object HoldingsValue { get; set; }
                //public object HoldingsValueRealtime { get; set; }
                public string YearRange { get; set; }
                public object DaysValueChange { get; set; }
                //public object DaysValueChangeRealtime { get; set; }
                public string StockExchange { get; set; }
                public string DividendYield { get; set; }
                public string PercentChange { get; set; }
            }
        }
    }

    // ------------------------------------------------------------------------

    public class YahooFinanceErrorResponse
    {
        public Error error { get; set; }

        public class Error
        {
            public string lang { get; set; }
            public string description { get; set; }
        }
    }

    // ------------------------------------------------------------------------

    public class Service
    {
        public static List<StockQuote> GetStockQuoteList(List<Stock> stocks)
        {
            return GetQuery<StockQuote>(LibraryContext.Config.StockQuote, stocks, 
                                        (stockQuote, serviceQuote)=>{
                                            StockDatabaseYahooServiceAdapter.CopyFromServiceQuote(stockQuote, stocks, serviceQuote);
                                        });
        }

        public static List<StockDaily> GetStockDailyList(List<Stock> stocks)
        {
            return GetQuery<StockDaily>(LibraryContext.Config.StockDaily, stocks, 
                                        (stockDaily, serviceQuote)=>{
                                            StockDatabaseYahooServiceAdapter.CopyFromServiceQuote(stockDaily, stocks, serviceQuote);
                                        });
        }

        public static List<Stock> GetStockList(List<Stock> stocks)
        {
            return GetQuery<Stock>(LibraryContext.Config.Stock, stocks, 
                                        (stock, serviceQuote)=>{
                                            StockDatabaseYahooServiceAdapter.CopyFromServiceQuote(stock, stocks, serviceQuote);
                                        });
        }

        public static List<StockPeriodic> GetPeriodic(List<Stock> stocks)
        {
            return GetQuery<StockPeriodic>(LibraryContext.Config.StockPeriodic, stocks, 
                                        (stockPeriodic, serviceQuote)=>{
                                            StockDatabaseYahooServiceAdapter.CopyFromServiceQuote(stockPeriodic, stocks, serviceQuote);
                                        });
        }

        public static List<T> GetQuery<T>(LibraryContext.Configuration.ServiceOperation serviceOperationConfig, 
                                          List<Stock> stocks, 
                                          Action<T, YahooFinanceResponse.Results.Quote> assignAction) where T : IStockData,new()
        {
            string parametersTemplate = serviceOperationConfig.Parameters; 
            string queryTemplate = serviceOperationConfig.Query;
            var returnValue = new List<T>();
            string stocksList = string.Empty;
            stocks.ForEach((stock) => stocksList += ("'" + stock.Symbol + "',"));
            stocksList = (stocksList.Length > 1) ? stocksList.Substring(0, stocksList.Length - 1) : stocksList; // trim trailing comma

            if (stocksList == string.Empty)
            {
                // Nothing to query
            }
            else
            {
                var query = System.Web.HttpUtility.UrlEncode(string.Format(queryTemplate, stocksList));
                string arguments = string.Format(parametersTemplate, query);
                string url = string.Format("{0}{1}", LibraryContext.Config.StockBaseURL, arguments);
                //Console.Write("URL: "); Debug.WriteLine(url);
                using (var httpClient = new HttpClient())
                {
                    var response = httpClient.GetAsync(url).Result;
                    if (response.StatusCode ==  HttpStatusCode.OK)
                    {
                        var json = response.Content.ReadAsStringAsync().Result;
                        var dataResponse = JsonConvert.DeserializeObject<YahooFinanceResponse>(json);
                        foreach (var quoteItem in dataResponse.query.results.quote)
                        {
                            var item = new T();
                            assignAction(item, quoteItem);
                            returnValue.Add(item);         
                        }
                    }
                    else
                    {
                        var json = response.Content.ReadAsStringAsync().Result;
                        var errorResponse = JsonConvert.DeserializeObject<YahooFinanceErrorResponse>(json);
                        throw new ApplicationException(string.Format("Failed to get successful response from Yahoo Finance service. Description:{0}",
                                                       errorResponse == null ? "Unknown cause." : errorResponse.error.description));
                    }
                }
            }
            return returnValue;
        }
    }

    public class StockDatabaseYahooServiceAdapter
    {
        public static void CopyFromServiceQuote(StockDaily stockDaily, List<Stock> stocks, YahooFinanceResponse.Results.Quote quote)
        {
            stockDaily.Stock = stocks.Find(x=>x.Symbol == quote.Symbol);
            stockDaily.DailyDate = DateTime.Today;
            stockDaily.AverageDailyVolume = int.TryParse(quote.AverageDailyVolume, out int myInt) ? myInt : 0;
            stockDaily.BookValue = decimal.TryParse(quote.BookValue, out decimal myDecimal) ? myDecimal : 0M;
            stockDaily.DaysHigh = decimal.TryParse(quote.DaysHigh, out myDecimal) ? myDecimal : 0M;
            stockDaily.DaysLow = decimal.TryParse(quote.DaysLow, out myDecimal) ? myDecimal : 0M;
            stockDaily.DividendYield = decimal.TryParse(quote.DividendYield, out myDecimal) ? myDecimal : 0M;
            stockDaily.FiftyDayMovingAverage = decimal.TryParse(quote.FiftydayMovingAverage, out myDecimal) ? myDecimal : 0M;
            stockDaily.Open = decimal.TryParse(quote.Open, out myDecimal) ? myDecimal : 0M;
            stockDaily.PreviousClose = decimal.TryParse(quote.PreviousClose, out myDecimal) ? myDecimal : 0M;
            stockDaily.TwoHundredDayMovingAverage = decimal.TryParse(quote.TwoHundreddayMovingAverage, out myDecimal) ? myDecimal : 0M;
            stockDaily.YearHigh = decimal.TryParse(quote.YearHigh, out myDecimal) ? myDecimal : 0M;
            stockDaily.YearLow = decimal.TryParse(quote.YearLow, out myDecimal) ? myDecimal : 0M;
        }

        public static void CopyFromServiceQuote(StockPeriodic stockPeriodic, List<Stock> stocks, YahooFinanceResponse.Results.Quote quote)
        {
            stockPeriodic.Stock = stocks.Find(x=>x.Symbol == quote.Symbol);
            stockPeriodic.BeginDate = DateTime.Today;
            stockPeriodic.EndDate = null;
            stockPeriodic.DividendPayDate = DateTime.TryParse(quote.DividendPayDate, out DateTime dividendPayDate) ? dividendPayDate : (DateTime?)null;
            stockPeriodic.DividendPerShare = decimal.TryParse(quote.DividendShare, out decimal dividendPerShare) ? dividendPerShare : (decimal?)null;
            stockPeriodic.EarningsPerShare = decimal.TryParse(quote.EarningsShare, out decimal earningsPerShare) ? earningsPerShare : (decimal?)null;
            stockPeriodic.EBITDA = decimal.TryParse(quote.EBITDA, out decimal ebitda) ? ebitda : (decimal?)null;
            stockPeriodic.EPSEstimateCurrentYear = decimal.TryParse(quote.EPSEstimateCurrentYear, out decimal epsEstimateCurrentYear) ? epsEstimateCurrentYear : (decimal?)null;
            stockPeriodic.EPSEstimateNextQuarter = decimal.TryParse(quote.EPSEstimateNextQuarter, out decimal epsEstimateNextQuarter) ? epsEstimateNextQuarter : (decimal?)null;
            stockPeriodic.EPSEstimateNextYear = decimal.TryParse(quote.EPSEstimateNextYear, out decimal epsEstimateNextYear) ? epsEstimateNextYear : (decimal?)null;
            stockPeriodic.ExDividendDate = DateTime.TryParse(quote.ExDividendDate, out DateTime exDividendDate) ? exDividendDate : (DateTime?)null;
            stockPeriodic.MarketCapitalization = int.TryParse(quote.MarketCapitalization, out int marketCapitalization) ? marketCapitalization : (int?)null;
            stockPeriodic.OneYearTargetPrice = decimal.TryParse(quote.OneyrTargetPrice, out decimal oneYearTargetPrice) ? oneYearTargetPrice : (decimal?)null;             
        }

        public static void CopyFromServiceQuote(StockQuote stockQuote, List<Stock> stocks, YahooFinanceResponse.Results.Quote quote)
        {
            var lastTradeDateTimeString = (string.IsNullOrEmpty(quote.LastTradeDate) ? string.Empty : quote.LastTradeDate) +
                                          (string.IsNullOrEmpty(quote.LastTradeTime) ? string.Empty : quote.LastTradeTime);
            stockQuote.Stock = stocks.Find(x=>x.Symbol == quote.Symbol);
            stockQuote.Ask = decimal.TryParse(quote.Ask, out decimal ask) ? ask : (decimal?)null;
            stockQuote.Bid = decimal.TryParse(quote.Bid, out decimal bid) ? bid : (decimal?)null;
            stockQuote.TradeDateTime = DateTime.TryParse(lastTradeDateTimeString, out DateTime tradeDateTime) ? tradeDateTime : (DateTime?)null;
            stockQuote.TradePrice = decimal.TryParse(quote.LastTradePriceOnly, out decimal tradePrice) ? tradePrice : (decimal?)null;
            stockQuote.Volume = int.TryParse(quote.Volume, out int volume) ? volume : (int?)null;
        }

        public static void CopyFromServiceQuote(Stock stock, List<Stock> stocks, YahooFinanceResponse.Results.Quote quote)
        {
            stock.CurrencyCode = quote.Currency;
            stock.Name = quote.Name;
            stock.StockExchangeCode = quote.StockExchange;
            stock.Symbol = quote.Symbol;
        }
    }    
}