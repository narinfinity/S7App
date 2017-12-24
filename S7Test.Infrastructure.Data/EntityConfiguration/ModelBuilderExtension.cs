using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Models;
using S7Test.Core.Entity.App;
using S7Test.Core.Entity.Domain;

namespace S7Test.Infrastructure.Data.EntityConfiguration
{
    public static class ModelBuilderExtension
    {
        public static void AddEntityConfigurations(this ModelBuilder builder)
        {
            //ASP.NET Identity
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

            builder.Entity<User>(b =>
            {
                b.ToTable("Users");
                b.Property(e => e.Id).HasMaxLength(128).IsUnicode(false).IsRequired();
                b.Property(e => e.ConcurrencyStamp).HasMaxLength(128).IsUnicode(false).IsRequired(false);
                b.Property(e => e.FirstName).HasMaxLength(100).IsUnicode().IsRequired();
                b.Property(e => e.LastName).HasMaxLength(100).IsUnicode().IsRequired();
                b.Property(e => e.Email).HasMaxLength(100).IsUnicode(false).IsRequired();
                b.Property(e => e.NormalizedEmail).HasMaxLength(100).IsUnicode(false).IsRequired(false);
                b.Property(e => e.UserName).HasMaxLength(100).IsUnicode().IsRequired();
                b.Property(e => e.NormalizedUserName).HasMaxLength(256).IsUnicode();
                b.Property(e => e.PasswordHash).HasMaxLength(256).IsUnicode(false).IsRequired(false);
                b.Property(e => e.SecurityStamp).HasMaxLength(256).IsUnicode(false).IsRequired(false);
                b.Property(e => e.PhoneNumber).HasMaxLength(100).IsUnicode(false).IsRequired(false);

                b.HasKey(e => e.Id);
                b.HasIndex(e => e.NormalizedUserName).IsUnique().HasName("UserNameIndex");
                b.HasIndex(e => e.NormalizedEmail).HasName("EmailIndex");
            });
            builder.Entity<User>()
                .HasMany(e => e.Claims)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<User>()
                .HasMany(e => e.Logins)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<User>()
                .HasMany(e => e.Roles)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<IdentityRole<string>>(b =>
            {
                b.ToTable("Roles");

                b.Property(e => e.Id).HasMaxLength(128).IsUnicode(false).ValueGeneratedOnAdd();
                b.Property(e => e.ConcurrencyStamp).HasMaxLength(128).IsUnicode(false).IsConcurrencyToken();
                b.Property(e => e.Name).HasMaxLength(128).IsUnicode(false);
                b.Property(e => e.NormalizedName).HasMaxLength(128).IsUnicode(false);
                b.Property<string>("Discriminator").HasMaxLength(128).IsUnicode(false).IsRequired(false);

                b.HasKey(e => e.Id);
                b.HasIndex(e => e.NormalizedName).IsUnique().HasName("RoleNameIndex");
            });

            //Domain EntityConfiguration
            builder.Entity<Player>(b =>
            {
                b.ToTable("Players");

                b.Property(e => e.Id).ValueGeneratedOnAdd();
                b.Property(e => e.IsActive).IsRequired();
                b.Property(e => e.Name).HasMaxLength(1024).IsUnicode().IsRequired();
                b.Property(e => e.Picture).HasMaxLength(2048).IsUnicode(false).IsRequired(false);
                b.Property(e => e.Age).IsRequired();
                b.Property(e => e.YellowCards).IsRequired();
                b.Property(e => e.RedCards).IsRequired();
                b.Property(e => e.Goals).IsRequired();
                b.Property(e => e.Appearances).IsRequired();

                b.Property(e => e.Gender).HasMaxLength(4000).IsUnicode(false).IsRequired(false);

                b.HasKey(e => e.Id);
                b.HasOne(e => e.Team).WithMany(e => e.Players).HasForeignKey(e => e.TeamId);
            });
            builder.Entity<Team>(b =>
            {
                b.ToTable("Teams");

                b.Property(e => e.Id).ValueGeneratedOnAdd();
                b.Property(e => e.Name).HasMaxLength(128).IsUnicode().IsRequired();

                b.HasKey(e => e.Id);
            });



        }
        public static void AddOpenIddictEntityConfigurations(this ModelBuilder builder)
        {
            // Configure the TApplication entity.
            builder.Entity<OpenIddictApplication<string>>(b =>
           {
               b.ToTable("OpenIdApps");
               b.HasKey(e => e.Id);
               b.HasIndex(e => e.ClientId).IsUnique();

               b.Property(e => e.Id).HasMaxLength(128).IsUnicode(false).IsRequired();
               b.Property(e => e.ClientId).HasMaxLength(1024).IsUnicode(false).IsRequired();
               b.Property(e => e.ClientSecret).HasMaxLength(4096).IsUnicode(false).IsRequired(false);
               b.Property(e => e.DisplayName).HasMaxLength(1024).IsUnicode().IsRequired(false);
               b.Property(e => e.PostLogoutRedirectUris).HasMaxLength(4096).IsUnicode(false).IsRequired(false);
               b.Property(e => e.RedirectUris).HasMaxLength(4096).IsUnicode(false).IsRequired(false);

               b.Property(e => e.Type).HasMaxLength(128).IsUnicode(false).IsRequired();

               b.HasMany(e => e.Authorizations).WithOne(e => e.Application).HasForeignKey("ApplicationId").IsRequired(false);
               b.HasMany(e => e.Tokens).WithOne(e => e.Application).HasForeignKey("ApplicationId").IsRequired(false);

           });

            // Configure the TAuthorization entity.
            builder.Entity<OpenIddictAuthorization<string>>(b =>
            {
                b.ToTable("OpenIdAuthzs");
                b.HasKey(e => e.Id);

                b.Property(e => e.Id).HasMaxLength(128).IsUnicode(false).IsRequired();
                b.Property("ApplicationId").HasMaxLength(128).IsUnicode(false).IsRequired(false);
                b.Property(e => e.Scopes).HasMaxLength(4096).IsUnicode(false).IsRequired(false);
                b.Property(e => e.Status).HasMaxLength(128).IsUnicode(false).IsRequired();
                b.Property(e => e.Subject).HasMaxLength(128).IsUnicode(false).IsRequired();
                b.Property(e => e.Type).HasMaxLength(128).IsUnicode(false).IsRequired();

                b.HasMany(e => e.Tokens).WithOne(e => e.Authorization).HasForeignKey("AuthorizationId").IsRequired(false);
            });

            // Configure the TScope entity.
            builder.Entity<OpenIddictScope<string>>(b =>
            {
                b.ToTable("OpenIdScopes");
                b.HasKey(e => e.Id);

                b.Property(e => e.Id).HasMaxLength(128).IsUnicode(false).IsRequired();
                b.Property(e => e.Name).HasMaxLength(128).IsUnicode(false).IsRequired();
                b.Property(e => e.Description).HasMaxLength(128).IsUnicode(false).IsRequired(false);
            });

            // Configure the TToken entity.
            builder.Entity<OpenIddictToken<string>>(b =>
            {
                b.ToTable("OpenIdTokens");
                b.HasKey(e => e.Id);

                b.Property(e => e.Id).HasMaxLength(128).IsUnicode(false).IsRequired();
                b.Property("ApplicationId").HasMaxLength(128).IsUnicode(false).IsRequired(false);
                b.Property("AuthorizationId").HasMaxLength(128).IsUnicode(false).IsRequired(false);

                b.Property(e => e.Ciphertext).HasMaxLength(4096).IsUnicode(false).IsRequired(false);
                b.Property(e => e.Hash).HasMaxLength(1024).IsUnicode(false).IsRequired(false);
                b.Property(e => e.Subject).HasMaxLength(128).IsUnicode(false).IsRequired();
                b.Property(e => e.Status).HasMaxLength(128).IsUnicode(false).IsRequired(false);
                b.Property(e => e.Status).HasMaxLength(128).IsUnicode(false).IsRequired();

            });



        }

    }
}
