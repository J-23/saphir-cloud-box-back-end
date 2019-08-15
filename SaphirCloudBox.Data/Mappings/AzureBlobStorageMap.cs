using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Data.Mappings
{
    public class AzureBlobStorageMap : IEntityTypeConfiguration<AzureBlobStorage>
    {
        public void Configure(EntityTypeBuilder<AzureBlobStorage> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
