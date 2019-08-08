using Anthill.Common.Data;
using Microsoft.Extensions.Configuration;
using SaphirCloudBox.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Data
{
    public class SaphirCloudBoxConnectionConfiguration : AbstractConnectionConfiguration, ISaphirCloudBoxConnectionConfiguration
    {
        public SaphirCloudBoxConnectionConfiguration(IConfigurationRoot configurationRoot, string connectionName) 
            : base(configurationRoot, connectionName)
        {
        }
    }
}
