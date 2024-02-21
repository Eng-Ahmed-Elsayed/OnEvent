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
                Id = "124878f6-6c0e-4760-877d-5b74d1f9f022",
                Name = RoleNames.User,
                NormalizedName = "USER",
            },
            new IdentityRole
            {
                Id = "289878f6-6c0e-4760-877d-5b74d1f9f022",
                Name = RoleNames.Moderator,
                NormalizedName = "MODERATOR"
            },
            new IdentityRole
            {
                Id = "318278f6-6c0e-4760-877d-5b74d1f9f022",
                Name = RoleNames.Admin,
                NormalizedName = "ADMINI"
            });
        }
    }
}
