
using Microsoft.EntityFrameworkCore;

namespace BlogManagementApp.Models;

public partial class BlogManagementSystemContext : DbContext
{
    public BlogManagementSystemContext()
    {
    }

    public BlogManagementSystemContext(DbContextOptions<BlogManagementSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BlogPost> BlogPosts { get; set; }

    public virtual DbSet<UserList> UserLists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("name=dbConn");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BlogPost>(entity =>
        {
            entity.HasKey(e => e.Bid).HasName("PK__BlogPost__C6DE0CC184F6C2A5");

            entity.ToTable("BlogPost");

            entity.Property(e => e.Bid)
                .ValueGeneratedNever()
                .HasColumnName("BId");

            entity.HasOne(d => d.CancelUser).WithMany(p => p.BlogPostCancelUsers)
                .HasForeignKey(d => d.CancelUserId)
                .HasConstraintName("FK__BlogPost__Cancel__5070F446");

            entity.HasOne(d => d.UploadUser).WithMany(p => p.BlogPostUploadUsers)
                .HasForeignKey(d => d.UploadUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__BlogPost__Upload__4F7CD00D");
        });

        modelBuilder.Entity<UserList>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__UserList__1788CC4CF13633A6");

            entity.ToTable("UserList");

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.CurrentAddress).HasMaxLength(40);
            entity.Property(e => e.EmailAddress).HasMaxLength(50);
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.UserRole).HasMaxLength(40);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
