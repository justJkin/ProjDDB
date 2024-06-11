using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using financialApp.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=FinancialDatabaseApp;Trusted_Connection=True;TrustServerCertificate=True;");

        return new DataContext(optionsBuilder.Options);
    }
}
