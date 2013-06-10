using System.Data.Entity.Infrastructure;

namespace Treenks.Bralek.Common.Model
{
    public class MigrationsContextFactory : IDbContextFactory<BralekDbContext>
    {
        public BralekDbContext Create()
        {
            return new BralekDbContext("DefaultConnection");
        }
    }
}