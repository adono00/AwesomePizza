using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AwesomePizza.Repository.Data;

// serve solo ai comandi "dotnet ef" (creazione migration): non apre nessuna connessione
public class PizzaDbContextFactory : IDesignTimeDbContextFactory<PizzaDbContext>
{
    public PizzaDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<PizzaDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=AwesomePizza;Trusted_Connection=True;TrustServerCertificate=True")
            .Options;

        return new PizzaDbContext(options);
    }
}
