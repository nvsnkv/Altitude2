using Microsoft.EntityFrameworkCore;

namespace NV.Altitude2.ApiServer.Models
{
    public class MeasurementsContext : DbContext
    {
        public MeasurementsContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<DbMeasurement> Measurements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasSequence<long>("MeasurementNumbers")
                .StartsAt(1)
                .IncrementsBy(1);

            modelBuilder.Entity<DbMeasurement>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<DbMeasurement>()
                .Property(m => m.Id)
                .HasDefaultValueSql("NEXT VALUE FOR MeasurementNumbers");
        }
    }
}