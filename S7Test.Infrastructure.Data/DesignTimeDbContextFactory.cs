using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace S7Test.Infrastructure.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AppDbContext>();
            var conStr = "Server=MYPC\\SERVER;Database=S7Test;User ID=sa;Password=pass;Trusted_Connection=False;Encrypt=False;TrustServerCertificate=False;MultipleActiveResultSets=True;";
            builder.UseSqlServer(conStr);
            return new AppDbContext(builder.Options);
        }
    }
}
