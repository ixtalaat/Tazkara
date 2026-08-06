using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Tazkara.Infrastructure.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // Read connection string from environment or use default
            var connectionString = Environment.GetEnvironmentVariable("DefaultConnection") 
                ?? "Server=db62608.public.databaseasp.net; Database=db62608; User Id=db62608; Password=9z#Tt?G47p+S; Encrypt=False; MultipleActiveResultSets=True;";

            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
