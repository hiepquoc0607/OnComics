using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OnComics.Infrastructure.Entities;

namespace OnComics.Infrastructure.Persistence;

public partial class OnComicsDatabaseContext : DbContext
{
    public OnComicsDatabaseContext()
    {
    }

    public OnComicsDatabaseContext(DbContextOptions<OnComicsDatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Attachment> Attachments { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Chapter> Chapters { get; set; }

    public virtual DbSet<Chaptersource> Chaptersources { get; set; }

    public virtual DbSet<Comic> Comics { get; set; }

    public virtual DbSet<Comiccategory> Comiccategories { get; set; }

    public virtual DbSet<Comicrating> Comicratings { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<History> Histories { get; set; }

    public virtual DbSet<Interaction> Interactions { get; set; }

    public virtual DbSet<Interactiontype> Interactiontypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        IConfiguration configuration = builder.Build();

        optionsBuilder.UseMySQL(configuration.GetConnectionString("DefaultConnection")!);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("account");

            entity.HasIndex(e => e.Email, "Email").IsUnique();

            entity.Property(e => e.Id).HasColumnType("binary(16)");
            entity.Property(e => e.Dob).HasColumnType("date");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Fcmtoken)
                .HasColumnType("text")
                .HasColumnName("FCMToken");
            entity.Property(e => e.Fullname).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.ImgUrl).HasColumnType("text");
            entity.Property(e => e.PasswordHash).HasColumnType("text");
            entity.Property(e => e.RefreshExpireTime).HasColumnType("datetime");
            entity.Property(e => e.RefreshToken).HasColumnType("text");
            entity.Property(e => e.Role).HasMaxLength(10);
            entity.Property(e => e.Status).HasMaxLength(10);
        });

        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("attachment");

            entity.HasIndex(e => e.CommentId, "CommentId");

            entity.Property(e => e.Id).HasColumnType("binary(16)");
            entity.Property(e => e.CommentId).HasColumnType("binary(16)");
            entity.Property(e => e.StorageUrl).HasColumnType("text");

            entity.HasOne(d => d.Comment).WithMany(p => p.Attachments)
                .HasForeignKey(d => d.CommentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("attachment_ibfk_1");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("category");

            entity.HasIndex(e => e.Name, "Name").IsUnique();

            entity.Property(e => e.Id).HasColumnType("binary(16)");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(10);
        });

        modelBuilder.Entity<Chapter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("chapter");

            entity.HasIndex(e => new { e.ComicId, e.ChapNo }, "ComicId").IsUnique();

            entity.Property(e => e.Id).HasColumnType("binary(16)");
            entity.Property(e => e.ComicId).HasColumnType("binary(16)");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.ReleaseTime).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(10);

            entity.HasOne(d => d.Comic).WithMany(p => p.Chapters)
                .HasForeignKey(d => d.ComicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("chapter_ibfk_1");
        });

        modelBuilder.Entity<Chaptersource>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("chaptersource");

            entity.HasIndex(e => e.ChapterId, "ChapterId");

            entity.Property(e => e.Id).HasColumnType("binary(16)");
            entity.Property(e => e.ChapterId).HasColumnType("binary(16)");
            entity.Property(e => e.SrcUrl).HasColumnType("text");
            entity.Property(e => e.ViewUrl).HasColumnType("text");

            entity.HasOne(d => d.Chapter).WithMany(p => p.Chaptersources)
                .HasForeignKey(d => d.ChapterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("chaptersource_ibfk_1");
        });

        modelBuilder.Entity<Comic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("comic");

            entity.Property(e => e.Id).HasColumnType("binary(16)");
            entity.Property(e => e.Author).HasMaxLength(100);
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Rating).HasPrecision(3, 1);
            entity.Property(e => e.ReleaseDate).HasColumnType("date");
            entity.Property(e => e.Status).HasMaxLength(10);
            entity.Property(e => e.ThumbnailUrl).HasColumnType("text");
            entity.Property(e => e.UpdateTime).HasColumnType("datetime");
        });

        modelBuilder.Entity<Comiccategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("comiccategory");

            entity.HasIndex(e => e.CategoryId, "CategoryId");

            entity.HasIndex(e => new { e.ComicId, e.CategoryId }, "ComicId").IsUnique();

            entity.Property(e => e.Id).HasColumnType("binary(16)");
            entity.Property(e => e.CategoryId).HasColumnType("binary(16)");
            entity.Property(e => e.ComicId).HasColumnType("binary(16)");

            entity.HasOne(d => d.Category).WithMany(p => p.Comiccategories)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comiccategory_ibfk_2");

            entity.HasOne(d => d.Comic).WithMany(p => p.Comiccategories)
                .HasForeignKey(d => d.ComicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comiccategory_ibfk_1");
        });

        modelBuilder.Entity<Comicrating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("comicrating");

            entity.HasIndex(e => new { e.AccountId, e.ComicId }, "AccountId").IsUnique();

            entity.HasIndex(e => e.ComicId, "ComicId");

            entity.Property(e => e.Id).HasColumnType("binary(16)");
            entity.Property(e => e.AccountId).HasColumnType("binary(16)");
            entity.Property(e => e.ComicId).HasColumnType("binary(16)");
            entity.Property(e => e.Rating).HasPrecision(2, 1);

            entity.HasOne(d => d.Account).WithMany(p => p.Comicratings)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comicrating_ibfk_1");

            entity.HasOne(d => d.Comic).WithMany(p => p.Comicratings)
                .HasForeignKey(d => d.ComicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comicrating_ibfk_2");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("comment");

            entity.HasIndex(e => e.AccountId, "AccountId");

            entity.HasIndex(e => e.ComicId, "ComicId");

            entity.HasIndex(e => e.MainCmtId, "MainCmtId");

            entity.Property(e => e.Id).HasColumnType("binary(16)");
            entity.Property(e => e.AccountId).HasColumnType("binary(16)");
            entity.Property(e => e.CmtTime).HasColumnType("datetime");
            entity.Property(e => e.ComicId).HasColumnType("binary(16)");
            entity.Property(e => e.Content).HasColumnType("text");
            entity.Property(e => e.MainCmtId).HasColumnType("binary(16)");

            entity.HasOne(d => d.Account).WithMany(p => p.Comments)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comment_ibfk_1");

            entity.HasOne(d => d.Comic).WithMany(p => p.Comments)
                .HasForeignKey(d => d.ComicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comment_ibfk_2");

            entity.HasOne(d => d.MainCmt).WithMany(p => p.InverseMainCmt)
                .HasForeignKey(d => d.MainCmtId)
                .HasConstraintName("comment_ibfk_3");
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("favorite");

            entity.HasIndex(e => new { e.AccountId, e.ComicId }, "AccountId").IsUnique();

            entity.HasIndex(e => e.ComicId, "ComicId");

            entity.Property(e => e.Id).HasColumnType("binary(16)");
            entity.Property(e => e.AccountId).HasColumnType("binary(16)");
            entity.Property(e => e.ComicId).HasColumnType("binary(16)");

            entity.HasOne(d => d.Account).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("favorite_ibfk_1");

            entity.HasOne(d => d.Comic).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.ComicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("favorite_ibfk_2");
        });

        modelBuilder.Entity<History>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("history");

            entity.HasIndex(e => new { e.AccountId, e.ChapterId }, "AccountId").IsUnique();

            entity.HasIndex(e => e.ChapterId, "ChapterId");

            entity.Property(e => e.Id).HasColumnType("binary(16)");
            entity.Property(e => e.AccountId).HasColumnType("binary(16)");
            entity.Property(e => e.ChapterId).HasColumnType("binary(16)");
            entity.Property(e => e.ReadTime).HasColumnType("datetime");

            entity.HasOne(d => d.Account).WithMany(p => p.Histories)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("history_ibfk_1");

            entity.HasOne(d => d.Chapter).WithMany(p => p.Histories)
                .HasForeignKey(d => d.ChapterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("history_ibfk_2");
        });

        modelBuilder.Entity<Interaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("interaction");

            entity.HasIndex(e => new { e.AccountId, e.CommentId }, "AccountId").IsUnique();

            entity.HasIndex(e => e.CommentId, "CommentId");

            entity.HasIndex(e => e.TypeId, "TypeId");

            entity.Property(e => e.Id).HasColumnType("binary(16)");
            entity.Property(e => e.AccountId).HasColumnType("binary(16)");
            entity.Property(e => e.CommentId).HasColumnType("binary(16)");
            entity.Property(e => e.ReactTime).HasColumnType("datetime");
            entity.Property(e => e.TypeId).HasColumnType("binary(16)");

            entity.HasOne(d => d.Account).WithMany(p => p.Interactions)
                .HasForeignKey(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("interaction_ibfk_1");

            entity.HasOne(d => d.Comment).WithMany(p => p.Interactions)
                .HasForeignKey(d => d.CommentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("interaction_ibfk_2");

            entity.HasOne(d => d.Type).WithMany(p => p.Interactions)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("interaction_ibfk_3");
        });

        modelBuilder.Entity<Interactiontype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("interactiontype");

            entity.HasIndex(e => e.Name, "Name").IsUnique();

            entity.Property(e => e.Id).HasColumnType("binary(16)");
            entity.Property(e => e.ImgUrl).HasColumnType("text");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(10);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
