using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace StockData.Library.DataContext
{
    public class StockDataContext : DbContext
    {
        public DbSet<Stock> Stock { get; set; }
        public DbSet<StockQuote> StockQuote { get; set; }
        public DbSet<StockPeriodic> StockPeriodic { get; set; }
        public DbSet<StockDaily> StockDaily { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = LibraryContext.Config.StockDatabaseConnectionString;
            Console.WriteLine(string.Format("Using Sqlite database.  Connection:{0}", connectionString));
            optionsBuilder.UseSqlite(connectionString, (sqliteOptionsBuilder)=>{
            });
            optionsBuilder.EnableSensitiveDataLogging(false);
        }
    }

    // ------------------------------------------------------------------------

    public interface IStockData
    {

    }

    public class Stock : IStockData
    {
        public int StockID { get; set; }
        public string Symbol { get; set; }

        /// <summary>
        /// From Currency
        /// </summary>
        public string CurrencyCode { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// From StockExchange
        /// </summary>
        public string StockExchangeCode { get; set; }

        public void Copy(Stock stock)
        {
            this.Symbol = stock.Symbol;
            this.Name = stock.Name;
            this.StockExchangeCode = stock.StockExchangeCode;
            this.CurrencyCode = stock.CurrencyCode;
        }
    }

    // ------------------------------------------------------------------------

    public class StockPeriodic : IStockData
    {
        public int StockPeriodicID { get; set; }
        public Stock Stock { get; set; }

        public DateTime BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? DividendPayDate { get; set; }
        public DateTime? ExDividendDate { get; set; }

        /// <summary>
        /// From DividendShare
        /// </summary>
        public decimal? DividendPerShare { get; set; }

        /// <summary>
        /// EarningShare
        /// </summary>
        public decimal? EarningsPerShare { get; set; }

        public decimal? EBITDA { get; set; }

        public int? MarketCapitalization { get; set; }

        /// <summary>
        /// From EPSEstimateCurrentYear
        /// </summary>
        public decimal? EPSEstimateCurrentYear { get; set; }

        /// <summary>
        /// From EPSEstimateNextYear
        /// </summary>
        public decimal? EPSEstimateNextYear { get; set; }

        /// <summary>
        /// From EPSEstimateNextQuarter
        /// </summary>
        public decimal? EPSEstimateNextQuarter { get; set; }

        public decimal? OneYearTargetPrice { get; set; }

        public bool IsValuesEqual(StockPeriodic stockPeriodic)
        {
            bool isEqual = this.DividendPayDate.Equals(stockPeriodic.DividendPayDate) &&
                           this.ExDividendDate.Equals(stockPeriodic.ExDividendDate) &&
                           this.MarketCapitalization.Equals(stockPeriodic.MarketCapitalization) &&
                           this.OneYearTargetPrice.Equals(stockPeriodic.OneYearTargetPrice) &&
                           this.DividendPerShare.Equals(stockPeriodic.DividendPerShare) &&
                           this.EarningsPerShare.Equals(stockPeriodic.EarningsPerShare) &&
                           this.EBITDA.Equals(stockPeriodic.EBITDA) &&
                           this.EPSEstimateCurrentYear.Equals(stockPeriodic.EPSEstimateCurrentYear) &&
                           this.EPSEstimateNextQuarter.Equals(stockPeriodic.EPSEstimateNextQuarter) &&
                           this.EPSEstimateNextYear.Equals(stockPeriodic.EPSEstimateNextYear);
            return isEqual;
        }
    }

    // ------------------------------------------------------------------------

    public class StockDaily : IStockData
    {
        public int StockDailyID { get; set; }
        public Stock Stock { get; set; }
        public DateTime DailyDate { get; set; }

        public int AverageDailyVolume { get; set; }
        public decimal FiftyDayMovingAverage { get; set; }
        public decimal TwoHundredDayMovingAverage { get; set; }
        public decimal Open { get; set; }
        public decimal PreviousClose { get; set; }
        public decimal BookValue { get; set; }
        public decimal DaysLow { get; set; }
        public decimal DaysHigh { get; set; }
        public decimal YearLow { get; set; }
        public decimal YearHigh { get; set; }

        public decimal DividendYield { get; set; }

        public void Copy(StockDaily stockDaily)
        {
            this.AverageDailyVolume = stockDaily.AverageDailyVolume;
            this.BookValue = stockDaily.BookValue;
            this.DailyDate = stockDaily.DailyDate;
            this.DaysHigh = stockDaily.DaysHigh;
            this.DaysLow = stockDaily.DaysLow;
            this.DividendYield = stockDaily.DividendYield;
            this.FiftyDayMovingAverage = stockDaily.FiftyDayMovingAverage;
            this.Open = stockDaily.Open;
            this.PreviousClose = stockDaily.PreviousClose;
            this.Stock = stockDaily.Stock;
            this.TwoHundredDayMovingAverage = stockDaily.TwoHundredDayMovingAverage;
            this.YearHigh = stockDaily.YearHigh;
            this.YearLow = stockDaily.YearLow;
        }
    }

    // ------------------------------------------------------------------------

    public class StockQuote : IStockData
    {
        public int StockQuoteID { get; set; }

        public Stock Stock { get; set; }

        public decimal ? Ask { get; set; }
        public decimal ? Bid { get; set; }
        
        /// <summary>
        /// From LastTradeTime
        /// </summary>
        public DateTime? TradeDateTime { get; set; }

        /// <summary>
        /// From LastTradePriceOnly
        /// </summary>
        public decimal ? TradePrice { get; set; }
        public int? Volume { get; set; }
    }
}
