using Microsoft.EntityFrameworkCore;

namespace Conelards.Database;

public class AccountDbContext : DbContext
{
    public DbSet<AccountDbModel> Users { get; set; }

    public string DbPath { get; }

    public AccountDbContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "ConelardsDb.sqlite");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}