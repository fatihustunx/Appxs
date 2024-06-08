using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Features.Entities.Configurations
{
    public class AppEntityConfiguration : IEntityTypeConfiguration<AppEntity>
    {
        public void Configure(EntityTypeBuilder<AppEntity> builder)
        {
            builder.ToTable("AppEntities").HasKey(c => c.Id);

            builder.Property(c => c.Id).HasColumnName("Id").IsRequired();

            builder.Property(c => c.Name).HasColumnName("Name").IsRequired();

            builder.HasIndex(indexExpression: b => b.Name, name: "UK_AppEntites_Name").IsUnique();
        }
    }
}
