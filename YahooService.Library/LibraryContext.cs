using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace YahooService.Library
{
    public class LibraryContext
    {
        public static void Initialize(IConfigurationSection configSection)
        {
            LibraryContext.Config = new Configuration(configSection);
        }

        public static Configuration Config { get; set; }
        
        public class Configuration
        {
            private Configuration()
            {
            }
            public Configuration(IConfigurationSection serviceSettingsConfigurationSection)
            {
                serviceSettingsConfigurationSection.Bind(this);
            }

            public string StockBaseURL { get; set; }
            public ServiceOperation Stock { get; set; }
            public ServiceOperation StockQuote { get; set; }
            public ServiceOperation StockDaily { get; set; }
            public ServiceOperation StockPeriodic { get; set; }

            public class ServiceOperation
            {
                public string Parameters {get;set;}
                public string Query {get;set;}
            }
        }
    }
}
