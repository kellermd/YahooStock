using Microsoft.Extensions.Configuration;
using System.IO;

namespace YahooStockReader
{
    public class Configuration
    {
        private IConfiguration _configuration = null;

        private static Configuration s_configuration = null;
        public static Configuration Instance
        {
            get
            {
                if (s_configuration == null)
                {
                    s_configuration = new Configuration();
                }
                return s_configuration;
            }
        }

        private Configuration()
        {
            this.ConnectionStrings = new ConnectionStringType(_configuration);
        }

        public ConnectionStringType ConnectionStrings { get; private set; }

        public class ConnectionStringType
        {
            private ConnectionStringType()
            {
            }
            public ConnectionStringType(IConfiguration configuration)
            {
                configuration.GetSection("ConnectionStrings").Bind(this);
            }

            public string StockDatabase { get; set; }
        }
    }
}
