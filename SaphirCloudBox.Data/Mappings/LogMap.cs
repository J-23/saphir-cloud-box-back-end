using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaphirCloudBox.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Data.Mappings
{
    public class LogMap : IEntityTypeConfiguration<Log>
    {
        public void Configure(EntityTypeBuilder<Log> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.LogType).IsRequired();

            builder.Property(x => x.Text).HasMaxLength(255).IsRequired();
        }
    }
}
