using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anthill.Common.Data
{
    public abstract class AbstractConnectionConfiguration
    {
        /// <summary>
        /// Stores the connection name from config file.
        /// </summary>
        protected readonly string _connectionName;
        private IConfigurationRoot _configurationRoot;

        public AbstractConnectionConfiguration(IConfigurationRoot configurationRoot, string connectionName)
        {
            _connectionName = connectionName;
            _configurationRoot = configurationRoot;
        }

        /// <summary>
        /// Returns a connection string.
        /// The implementer should either return a connection string from a configuration file or it could be a mock object if is used for testing.
        /// </summary>
        public string GetConnectionString()
        {
            return _configurationRoot.GetConnectionString(_connectionName);
        }
    }
}
