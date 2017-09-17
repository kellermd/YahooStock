using System;
using StockData.Library.DataContext;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace StockData.Library
{
    public class LibraryContext
    {
        public static void Initialize(IConfigurationSection configSection)
        {
            LibraryContext.Config = new Configuration(configSection);

            using (var db = new StockDataContext())
            {
                if (LibraryContext.Config.Logging)
                {
                    var serviceProvider = db.GetInfrastructure<IServiceProvider>();
                    var loggerFactory = (ILoggerFactory)serviceProvider.GetService(typeof(ILoggerFactory));
                    loggerFactory.AddProvider(new DatabaseLoggerProvider());
                }
            }
        }

        public static Configuration Config { get; set; }

        public static void ConfigureOptions(DbContextOptionsBuilder builder)
        {

        }

        public class Configuration
        {
            private Configuration()
            {
            }
            public Configuration(IConfigurationSection configSection)
            {
                configSection.Bind(this);
            }

            public string StockDatabaseConnectionString { get; set; }
            public bool Logging {get;set;}
        }
    }
}

