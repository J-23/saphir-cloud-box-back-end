using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SaphirCloudBox.Data
{
    public class SaphirCloudBoxDataContextFactory : IDesignTimeDbContextFactory<SaphirCloudBoxDataContext>
    {
        public SaphirCloudBoxDataContext CreateDbContext(string[] args)
        {
            var builder = new ConfigurationBuilder()
                      .SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configuration = builder.Build();

            var config = new SaphirCloudBoxConnectionConfiguration(configuration, "DefaultConnection");
            return new SaphirCloudBoxDataContext(config);
        }
    }
}
