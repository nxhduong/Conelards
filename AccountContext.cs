using Microsoft.EntityFrameworkCore;

public class AccountContext : DbContext
{
    public DbSet<AccountModel> Users { get; set; }

    public string DbPath { get; }

    public AccountContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "ColardsDb.sqlite");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}