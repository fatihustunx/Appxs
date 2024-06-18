using App.Appxs.eSecurities.Usings;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

using System;

namespace App.xContexts.xConfigs
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users").HasKey(c => c.Id);

            builder.Property(c => c.Id).HasColumnName("Id").IsRequired();

            builder.Property(c => c.Email).HasColumnName("Email").HasMaxLength(77).IsRequired();

            builder.HasIndex(indexExpression: b => b.Email, name: "UK_Users_Email").IsUnique();

            byte[] passwordSalt, passwordHash;
            HashingHelper.CreatePasswordHash("Passwords", out passwordSalt, out passwordHash);

            builder.HasData(new User
            {
                Id = 1,
                FirstName = "Fatih",
                LastName = "Üstün",
                Email = "Admin",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Status = true
            });
        }
    }

    public class OperationClaimConfiguration : IEntityTypeConfiguration<OperationClaim>
    {
        public void Configure(EntityTypeBuilder<OperationClaim> builder)
        {
            builder.ToTable("OperationClaims").HasKey(c => c.Id);

            builder.Property(c => c.Id).HasColumnName("Id").IsRequired();

            builder.Property(c => c.Name).HasColumnName("Name").HasMaxLength(77).IsRequired();

            builder.HasIndex(indexExpression: b => b.Name, name: "UK_Users_Name").IsUnique();

            builder.HasData(new OperationClaim { Id = 1, Name = "Admin" });
            builder.HasData(new OperationClaim { Id = 2, Name = "User" });
        }
    }

    public class UserOperationClaimConfiguration : IEntityTypeConfiguration<UserOperationClaim>
    {
        public void Configure(EntityTypeBuilder<UserOperationClaim> builder)
        {
            builder.ToTable("UserOperationClaims").HasKey(c => c.Id);

            builder.Property(c => c.Id).HasColumnName("Id").IsRequired();
            builder.Property(c => c.UserId).HasColumnName("UserId").IsRequired();
            builder.Property(c => c.OperationClaimId).HasColumnName("OperationClaimId").IsRequired();

            builder.HasData(new UserOperationClaim { Id = 1, UserId = 1, OperationClaimId = 1 });
        }
    }
}