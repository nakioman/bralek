using Castle.MicroKernel.Facilities;
using System.Data.Entity;
using Treenks.Bralek.Common.Model;
using Treenks.Bralek.Web.Migrations;

namespace Treenks.Bralek.Web.Plumbing
{
    public class DatabaseInitializationFacility : AbstractFacility
    {
        protected override void Init()
        {
            Database.SetInitializer<BralekDbContext>(null);

            // forcing the application of the migrations so the users table is modified before
            // the code below tries to create it. 
            using (var context = new BralekDbContext("DefaultConnection"))
            {
                var migrations = new MigrateDatabaseToLatestVersion<BralekDbContext, Configuration>();
                migrations.InitializeDatabase(context);
            }

            
        }
    }
}