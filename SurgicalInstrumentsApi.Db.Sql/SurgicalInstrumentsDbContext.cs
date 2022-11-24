using Microsoft.EntityFrameworkCore;
using SurgicalInstrumentsApi.Db.Sql.Model;
using System.Data.Common;

namespace SurgicalInstrumentsApi.Db.Sql
{
    public class SurgicalInstrumentsDbContext : DbContext
    {
        private static readonly string connectionString = "Data Source=ZENIT\\SQLEXPRESS2019;Initial Catalog=SurgicalInstrumentsApi;Integrated Security=True";
        public SurgicalInstrumentsDbContext() : base(GetOptions(connectionString))
        {
        }

        private static DbContextOptions GetOptions(string connectionString)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder(), connectionString).Options;
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Subcategory> Subcategories { get; set; }
        public virtual DbSet<Instrument> Instruments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<Subcategory>().ToTable("Subcategory");
            modelBuilder.Entity<Instrument>().ToTable("Instrument");
        }
    }
}
