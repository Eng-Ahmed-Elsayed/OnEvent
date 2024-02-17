using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.StaticClasses;

namespace DataAccess.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole
                {
                    Name = RoleNames.User,
                    NormalizedName = "USER",
                },
                new IdentityRole
                {
                    Name = RoleNames.Admin,
                    NormalizedName = "ADMINI"
                }
                ,
                new IdentityRole
                {
                    Name = RoleNames.Moderator,
                    NormalizedName = "MODERATOR"
                }
                );
        }
    }
}
