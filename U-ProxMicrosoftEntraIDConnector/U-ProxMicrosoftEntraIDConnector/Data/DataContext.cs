using Microsoft.EntityFrameworkCore;
using U_ProxMicrosoftEntraIDConnector.Data.Entities;

namespace U_ProxMicrosoftEntraIDConnector.Data
{
    public class DataContext : DbContext
    {
        public DbSet<UserEntity> Users => Set<UserEntity>();
        public DbSet<SettingsEntity> Settings => Set<SettingsEntity>();
        public DataContext()
        {
            Database.EnsureCreated();
        }

        /*public DataContext(DbContextOptions<DataContext> options)
        : base(options)
        {
            //Database.EnsureCreated();
        }*/
    
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=app.db");
        }
    
        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }*/
    }
}
