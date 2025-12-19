using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal;

namespace Infrastructure.DBContext
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Campus> Campus { get; set; }
        public virtual DbSet<ItemCategory> ItemCategories { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Claim> Claims { get; set; }
        public virtual DbSet<VerificationLog> VerificationLogs { get; set; }
        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<ReturnRecord> ReturnRecords { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        
        // Lookup tables
        public virtual DbSet<UserRoleLookup> UserRoles { get; set; }
        public virtual DbSet<ItemStatusLookup> ItemStatuses { get; set; }
        public virtual DbSet<ItemTypeLookup> ItemTypes { get; set; }
        public virtual DbSet<ClaimStatusLookup> ClaimStatuses { get; set; }
        public virtual DbSet<AppointmentStatusLookup> AppointmentStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            ConfigureColumnNames(modelBuilder);

            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Campus>().ToTable("campus");
            modelBuilder.Entity<ItemCategory>().ToTable("item_category");
            modelBuilder.Entity<Item>().ToTable("item");
            modelBuilder.Entity<Claim>().ToTable("claim");
            modelBuilder.Entity<VerificationLog>().ToTable("verification_log");
            modelBuilder.Entity<Appointment>().ToTable("appointment");
            modelBuilder.Entity<ReturnRecord>().ToTable("return_record");
            modelBuilder.Entity<Notification>().ToTable("notification");
            
            // Lookup tables
            modelBuilder.Entity<UserRoleLookup>().ToTable("user_roles");
            modelBuilder.Entity<ItemStatusLookup>().ToTable("item_statuses");
            modelBuilder.Entity<ItemTypeLookup>().ToTable("item_types");
            modelBuilder.Entity<ClaimStatusLookup>().ToTable("claim_statuses");
            modelBuilder.Entity<AppointmentStatusLookup>().ToTable("appointment_statuses");

            // User → UserRole
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany()
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // User → Campus
            modelBuilder.Entity<User>()
                .HasOne(u => u.Campus)
                .WithMany()
                .HasForeignKey(u => u.CampusId)
                .OnDelete(DeleteBehavior.SetNull);
            // Item → ItemType
            modelBuilder.Entity<Item>()
                .HasOne(i => i.ItemType)
                .WithMany()
                .HasForeignKey(i => i.ItemTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Item → ItemStatus
            modelBuilder.Entity<Item>()
                .HasOne(i => i.Status)
                .WithMany()
                .HasForeignKey(i => i.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Item → Category
            modelBuilder.Entity<Item>()
                .HasOne(i => i.Category)
                .WithMany()
                .HasForeignKey(i => i.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
            
            // Item → LostBy (User)
            modelBuilder.Entity<Item>()
                .HasOne(i => i.LostByStudent)
                .WithMany()
                .HasForeignKey(i => i.LostByStudentId)
                .OnDelete(DeleteBehavior.SetNull);
            
            // Item → FoundBy (User)
            modelBuilder.Entity<Item>()
                .HasOne(i => i.FoundByUser)
                .WithMany()
                .HasForeignKey(i => i.FoundByUserId)
                .OnDelete(DeleteBehavior.SetNull);
            
            // Item → Campus
            modelBuilder.Entity<Item>()
                .HasOne(i => i.Campus)
                .WithMany()
                .HasForeignKey(i => i.CampusId)
                .OnDelete(DeleteBehavior.Cascade);
            // Claim → Item
            modelBuilder.Entity<Claim>()
                .HasOne(c => c.Item)
                .WithMany()
                .HasForeignKey(c => c.ItemId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Claim → User (student)
            modelBuilder.Entity<Claim>()
                .HasOne(c => c.Student)
                .WithMany()
                .HasForeignKey(c => c.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Claim → ClaimStatus
            modelBuilder.Entity<Claim>()
                .HasOne(c => c.Status)
                .WithMany()
                .HasForeignKey(c => c.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
            // VerificationLog → Item
            modelBuilder.Entity<VerificationLog>()
                .HasOne<Item>()
                .WithMany()
                .HasForeignKey(v => v.ItemId)
                .OnDelete(DeleteBehavior.Cascade);
            // VerificationLog → Claim (optional)
            modelBuilder.Entity<VerificationLog>()
                .HasOne<Claim>()
                .WithMany()
                .HasForeignKey(v => v.ClaimId)
                .OnDelete(DeleteBehavior.SetNull);
            // Appointment → Item
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Item)
                .WithMany()
                .HasForeignKey(a => a.ItemId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Appointment → Staff
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Staff)
                .WithMany()
                .HasForeignKey(a => a.StaffId)
                .OnDelete(DeleteBehavior.SetNull);
            
            // Appointment → Student
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Student)
                .WithMany()
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Appointment → AppointmentStatus
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Status)
                .WithMany()
                .HasForeignKey(a => a.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
            // ReturnRecord → Item
            modelBuilder.Entity<ReturnRecord>()
                .HasOne<Item>()
                .WithMany()
                .HasForeignKey(r => r.ItemId)
                .OnDelete(DeleteBehavior.Cascade);
            // ReturnRecord → Staff
            modelBuilder.Entity<ReturnRecord>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(r => r.StaffId)
                .OnDelete(DeleteBehavior.NoAction);
            // ReturnRecord → Student
            modelBuilder.Entity<ReturnRecord>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.NoAction);
            // Notification → User
            modelBuilder.Entity<Notification>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigureColumnNames(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Fullname).HasColumnName("full_name");
                entity.Property(e => e.Email).HasColumnName("email");
                entity.Property(e => e.Phone).HasColumnName("phone");
                entity.Property(e => e.RoleId).HasColumnName("role_id");
                entity.Property(e => e.CampusId).HasColumnName("campus_id");
                entity.Property(e => e.CreateAt).HasColumnName("created_at");
                entity.Property(e => e.UpdateAt).HasColumnName("updated_at");
                entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            });

            modelBuilder.Entity<Campus>(entity =>
            {
                entity.HasKey(e => e.CampusId);
                entity.Property(e => e.CampusId).HasColumnName("campus_id");
                entity.Property(e => e.CampusName).HasColumnName("campus_name");
                entity.Property(e => e.CampusAddress).HasColumnName("address");
                entity.Property(e => e.CampusPhone).HasColumnName("phone");
                entity.Property(e => e.CreateAt).HasColumnName("created_at");
            });

            modelBuilder.Entity<ItemCategory>(entity =>
            {
                entity.HasKey(e => e.CategoryId);
                entity.Property(e => e.CategoryId).HasColumnName("category_id");
                entity.Property(e => e.CategoryName).HasColumnName("category_name");
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(e => e.ItemId);
                entity.Property(e => e.ItemId).HasColumnName("item_id");
                entity.Property(e => e.Title).HasColumnName("title");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.ImageUrl).HasColumnName("image_url");
                entity.Property(e => e.ItemTypeId).HasColumnName("item_type_id");
                entity.Property(e => e.StatusId).HasColumnName("status_id");
                entity.Property(e => e.CategoryId).HasColumnName("category_id");
                entity.Property(e => e.LostByStudentId).HasColumnName("lost_by_student_id");
                entity.Property(e => e.FoundByUserId).HasColumnName("found_by_user_id");
                entity.Property(e => e.CampusId).HasColumnName("campus_id");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<Claim>(entity =>
            {
                entity.HasKey(e => e.ClaimId);
                entity.Property(e => e.ClaimId).HasColumnName("claim_id");
                entity.Property(e => e.ItemId).HasColumnName("item_id");
                entity.Property(e => e.StudentId).HasColumnName("student_id");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.EvidenceImageUrl).HasColumnName("evidence_image_url");
                entity.Property(e => e.StatusId).HasColumnName("status_id");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            });

            modelBuilder.Entity<VerificationLog>(entity =>
            {
                entity.HasKey(e => e.VerificationId);
                entity.Property(e => e.VerificationId).HasColumnName("verification_id");
                entity.Property(e => e.ItemId).HasColumnName("item_id");
                entity.Property(e => e.ClaimId).HasColumnName("claim_id");
                entity.Property(e => e.VerifiedByUserId).HasColumnName("verified_by_user_id");
                entity.Property(e => e.VerificationType).HasColumnName("verification_type");
                entity.Property(e => e.Notes).HasColumnName("notes");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            });

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.AppointmentId);
                entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
                entity.Property(e => e.ItemId).HasColumnName("item_id");
                entity.Property(e => e.StaffId).HasColumnName("staff_id");
                entity.Property(e => e.StudentId).HasColumnName("student_id");
                entity.Property(e => e.Date).HasColumnName("date").HasColumnType("date");
                entity.Property(e => e.Time).HasColumnName("time").HasColumnType("time without time zone");

                entity.Property(e => e.StatusId).HasColumnName("status_id");
            });

            modelBuilder.Entity<ReturnRecord>(entity =>
            {
                entity.HasKey(e => e.ReturnId);
                entity.Property(e => e.ReturnId).HasColumnName("return_id");
                entity.Property(e => e.ItemId).HasColumnName("item_id");
                entity.Property(e => e.StaffId).HasColumnName("staff_id");
                entity.Property(e => e.StudentId).HasColumnName("student_id");
                entity.Property(e => e.ReturnTime).HasColumnName("return_time");
                entity.Property(e => e.SignatureUrl).HasColumnName("signature_url");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.NotificationId);
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Title).HasColumnName("title");
                entity.Property(e => e.Message).HasColumnName("message");
                entity.Property(e => e.IsRead).HasColumnName("is_read");
                entity.Property(e => e.CreateAt).HasColumnName("created_at");
            });
            
            // Lookup tables
            modelBuilder.Entity<UserRoleLookup>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.RoleName).HasColumnName("role_name");
            });
            
            modelBuilder.Entity<ItemStatusLookup>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.StatusName).HasColumnName("status_name");
            });
            
            modelBuilder.Entity<ItemTypeLookup>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.TypeName).HasColumnName("type_name");
            });
            
            modelBuilder.Entity<ClaimStatusLookup>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.StatusName).HasColumnName("status_name");
            });
            
            modelBuilder.Entity<AppointmentStatusLookup>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.StatusName).HasColumnName("status_name");
            });
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }
    }
}
