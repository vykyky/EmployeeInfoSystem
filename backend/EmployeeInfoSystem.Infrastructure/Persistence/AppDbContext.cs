using EmployeeInfoSystem.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeInfoSystem.Infrastructure.Persistence
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<News> News => Set<News>();
        public DbSet<User> Users => Set<User>();
        public DbSet<EmployeeProfile> EmployeeProfiles => Set<EmployeeProfile>();
        public DbSet<Ppe> Ppes => Set<Ppe>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<Request> Requests => Set<Request>();
        public DbSet<RequestType> RequestTypes => Set<RequestType>();
        public DbSet<RecipientGroup> RecipientGroups => Set<RecipientGroup>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

            modelBuilder.Entity<News>(entity =>
            {
                entity.ToTable("news");

                entity.Property(x => x.Id).HasColumnName("id");
                entity.Property(x => x.Title).HasColumnName("title");
                entity.Property(x => x.Body).HasColumnName("body");
                entity.Property(x => x.ImagePath).HasColumnName("image_path");
                entity.Property(x => x.AuthorId).HasColumnName("author_id");
                entity.Property(x => x.CreatedAt).HasColumnName("created_at");

                entity.HasOne(x => x.Author)
                  .WithMany()
                  .HasForeignKey(x => x.AuthorId)
                  .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.Property(x => x.Id).HasColumnName("id");
                entity.Property(x => x.Tabn).HasColumnName("tabn");
                entity.Property(x => x.PasswordHash).HasColumnName("password_hash");
                entity.Property(x => x.Role).HasColumnName("role");
                entity.Property(x => x.Phone).HasColumnName("phone");      // добавил
                entity.Property(x => x.Email).HasColumnName("email");      // добавил
                entity.Property(x => x.PushToken).HasColumnName("push_token");
                entity.Property(x => x.CreatedAt).HasColumnName("created_at");
                entity.Property(x => x.LastLoginAt).HasColumnName("last_login_at");

                entity.HasOne(u => u.EmployeeProfile)
                      .WithOne()
                      .HasForeignKey<EmployeeProfile>(e => e.Tabn)
                      .HasPrincipalKey<User>(u => u.Tabn);
            });

            modelBuilder.Entity<EmployeeProfile>(entity =>
            {
                entity.ToTable("employeecache");
                entity.Property(x => x.Id).HasColumnName("id");
                entity.Property(x => x.Tabn).HasColumnName("tabn");
                entity.Property(x => x.Fio).HasColumnName("fio");
                entity.Property(x => x.BornDate).HasColumnName("born_date");
                entity.Property(x => x.HireDate).HasColumnName("hire_date");
                entity.Property(x => x.Department).HasColumnName("department");
                entity.Property(x => x.Position).HasColumnName("post");
                entity.Property(x => x.Phone).HasColumnName("phone");
                entity.Property(x => x.Email).HasColumnName("email");
                entity.Property(x => x.ClothesSize).HasColumnName("size_clothes");
                entity.Property(x => x.WinterClothesSize).HasColumnName("size_clothes_winter");
                entity.Property(x => x.ShoesSize).HasColumnName("size_shoes");
                entity.Property(x => x.WinterShoesSize).HasColumnName("size_shoes_winter");
                entity.Property(x => x.Height).HasColumnName("height");
                entity.Property(x => x.SyncedAt).HasColumnName("synced_at");
            });

            modelBuilder.Entity<Ppe>(entity =>
            {
                entity.ToTable("ppecache");
                entity.Property(x => x.Id).HasColumnName("id");
                entity.Property(x => x.Tabn).HasColumnName("tabn");
                entity.Property(x => x.GroupName).HasColumnName("group_name");
                entity.Property(x => x.ItemName).HasColumnName("item_name");
                entity.Property(x => x.GiveDate).HasColumnName("give_date");
                entity.Property(x => x.EndDate).HasColumnName("end_date");
                entity.Property(x => x.WearPeriod).HasColumnName("wear_period");
                entity.Property(x => x.Quantity).HasColumnName("quantity");
                entity.Property(x => x.SyncedAt).HasColumnName("synced_at");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("notifications");
                entity.Property(x => x.Id).HasColumnName("id");
                entity.Property(x => x.RecipientId).HasColumnName("recipient_id");
                entity.Property(x => x.SenderId).HasColumnName("sender_id");
                entity.Property(x => x.Title).HasColumnName("title");
                entity.Property(x => x.Body).HasColumnName("body");
                entity.Property(x => x.IsRead).HasColumnName("is_read");
                entity.Property(x => x.CreatedAt).HasColumnName("created_at");

                entity.Property(x => x.RequestId).HasColumnName("request_id");

                entity.HasOne(x => x.Recipient)
                      .WithMany()
                      .HasForeignKey(x => x.RecipientId)
                      .OnDelete(DeleteBehavior.NoAction);

                // Связываем свойство Sender с внешним ключом SenderId
                entity.HasOne(x => x.Sender)
                      .WithMany()
                      .HasForeignKey(x => x.SenderId)
                      .OnDelete(DeleteBehavior.NoAction);

                // Дополнительно: связываем свойство Request с внешним ключом RequestId
                entity.HasOne(x => x.Request)
                      .WithMany()
                      .HasForeignKey(x => x.RequestId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Request>(entity =>
            {
                entity.ToTable("requests");
                entity.Property(x => x.Id).HasColumnName("id");
                entity.Property(x => x.EmployeeId).HasColumnName("employee_id");
                entity.Property(x => x.RequestTypeId).HasColumnName("request_type_id");
                entity.Property(x => x.Comment).HasColumnName("comment");
                entity.Property(x => x.NewValue).HasColumnName("new_value");
                entity.Property(x => x.Status).HasColumnName("status");
                entity.Property(x => x.ManagerId).HasColumnName("manager_id");
                entity.Property(x => x.ResolutionComment).HasColumnName("resolution_comment");
                entity.Property(x => x.CreatedAt).HasColumnName("created_at");
                entity.Property(x => x.ResolvedAt).HasColumnName("resolved_at");

                // Связи (Foreign Keys)
                entity.HasOne(x => x.Employee)
                      .WithMany()
                      .HasForeignKey(x => x.EmployeeId)
                      .OnDelete(DeleteBehavior.NoAction);

                // Связываем свойство Manager с внешним ключом ManagerId
                entity.HasOne(x => x.Manager)
                      .WithMany()
                      .HasForeignKey(x => x.ManagerId)
                      .OnDelete(DeleteBehavior.NoAction);

                // Связываем свойство RequestType с внешним ключом RequestTypeId
                entity.HasOne(x => x.RequestType)
                      .WithMany()
                      .HasForeignKey(x => x.RequestTypeId)
                      .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<RequestType>(entity =>
            {
                entity.ToTable("requesttypes");
                entity.Property(x => x.Id).HasColumnName("id");
                entity.Property(x => x.Name).HasColumnName("name");
                entity.Property(x => x.IsActive).HasColumnName("is_active");
                entity.Property(x => x.Code).HasColumnName("code");
                entity.Property(x => x.IsSystem).HasColumnName("is_system");
            });

            

            modelBuilder.Entity<RecipientGroup>(entity =>
            {
                entity.ToTable("recipient_groups"); // имя таблицы в БД
                entity.Property(x => x.Id).HasColumnName("id");
                entity.Property(x => x.Name).HasColumnName("name");
                entity.Property(x => x.Department).HasColumnName("department");
                entity.Property(x => x.Role).HasColumnName("role");
                entity.Property(x => x.CreatedAt).HasColumnName("created_at");
            });
        }
    }
}
