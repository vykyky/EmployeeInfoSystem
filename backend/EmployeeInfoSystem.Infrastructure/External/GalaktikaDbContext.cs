using EmployeeInfoSystem.Infrastructure.External.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EmployeeInfoSystem.Infrastructure.External
{
    public class GalaktikaDbContext : DbContext
    {
        public GalaktikaDbContext(DbContextOptions<GalaktikaDbContext> options) : base(options)
        {
        }

        public DbSet<Person> Persons => Set<Person>();
        public DbSet<Lschet> Lschet => Set<Lschet>();
        public DbSet<PersCard> PersCard => Set<PersCard>();
        public DbSet<PersSfo> PersSfo => Set<PersSfo>();
        public DbSet<GroupSfo> GroupSfo => Set<GroupSfo>();
        public DbSet<KatMbp> KatMbp => Set<KatMbp>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Конвертер DateTime → UTC
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                            v => v.ToUniversalTime(),
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                        ));
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(new ValueConverter<DateTime?, DateTime?>(
                            v => v.HasValue ? v.Value.ToUniversalTime() : v,
                            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v
                        ));
                    }
                }
            }

            modelBuilder.Entity<Person>(e =>
            {
                e.ToTable("persons");
                e.HasKey(x => x.Nrec);
                e.Property(x => x.Nrec).HasColumnName("nrec");
                e.Property(x => x.Fio).HasColumnName("fio");
                e.Property(x => x.BornDate).HasColumnName("borndate");
                e.Property(x => x.Phone).HasColumnName("phone");
                e.Property(x => x.Email).HasColumnName("email");
            });

            modelBuilder.Entity<Lschet>(e =>
            {
                e.ToTable("lschet");
                e.HasKey(x => x.Nrec);
                e.Property(x => x.Nrec).HasColumnName("nrec");
                e.Property(x => x.Tabn).HasColumnName("tabn");
                e.Property(x => x.TPerson).HasColumnName("tperson");
                e.Property(x => x.DatPos).HasColumnName("datpos");
                e.Property(x => x.Tarif).HasColumnName("tarif");
            });

            modelBuilder.Entity<PersCard>(e =>
            {
                e.ToTable("perscard");
                e.HasKey(x => x.Nrec);
                e.Property(x => x.Nrec).HasColumnName("nrec");
                e.Property(x => x.CLschet).HasColumnName("clschet");
                e.Property(x => x.Sizes).HasColumnName("sizes");
            });

            modelBuilder.Entity<PersSfo>(e =>
            {
                e.ToTable("perssfo");
                e.HasKey(x => x.Nrec);
                e.Property(x => x.Nrec).HasColumnName("nrec");
                e.Property(x => x.CPerscard).HasColumnName("cperscard");
                e.Property(x => x.CGrupSfo).HasColumnName("cgroupsfo");
                e.Property(x => x.CKatMbp).HasColumnName("ckatmbp");
                e.Property(x => x.CurKol).HasColumnName("curkol");
                e.Property(x => x.Spisdate).HasColumnName("spisdate");
                e.Property(x => x.GiveDate).HasColumnName("givedate");
                e.Property(x => x.EndDate).HasColumnName("enddate");
                e.Property(x => x.Srok).HasColumnName("srok");
            });

            modelBuilder.Entity<GroupSfo>(e =>
            {
                e.ToTable("groupsfo");
                e.HasKey(x => x.Nrec);
                e.Property(x => x.Nrec).HasColumnName("nrec");
                e.Property(x => x.Name).HasColumnName("name");
                e.Property(x => x.Kod).HasColumnName("kod");
                e.Property(x => x.Positions).HasColumnName("positions");
            });

            modelBuilder.Entity<KatMbp>(e =>
            {
                e.ToTable("katmbp");
                e.HasKey(x => x.Nrec);
                e.Property(x => x.Nrec).HasColumnName("nrec");
                e.Property(x => x.Name).HasColumnName("name");
            });
        }
    }
}