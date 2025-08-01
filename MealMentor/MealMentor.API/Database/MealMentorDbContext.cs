using MealMentor.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace MealMentor.API.Database;

public partial class MealMentorDbContext : DbContext
{
    public static MealMentorDbContext Instance
    {
        get
        {
            if (instance == null)
                instance = new();
            return instance;
        }
    }
    private static MealMentorDbContext instance;
    public MealMentorDbContext()
    {
    }

    public MealMentorDbContext(DbContextOptions<MealMentorDbContext> options)
         : base(options)
    {
        try
        {
            var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
            if (databaseCreator != null)
            {
                if (!databaseCreator.CanConnect()) databaseCreator.Create();
                if (!databaseCreator.HasTables()) databaseCreator.CreateTables();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

    => optionsBuilder.UseSqlServer(GetConnectionString());

    private string GetConnectionString()
    {

        IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();
        var str = configuration["ConnectionStrings:DefaultConnectionString"] ?? Environment.GetEnvironmentVariable("ConnectionStrings_DefaultConnection");
        return str;
    }

    public virtual DbSet<Friendship> Friendships { get; set; }

    public virtual DbSet<Ingredient> Ingredients { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<PlanDate> PlanDates { get; set; }

    public virtual DbSet<PlanDateDetail> PlanDateDetails { get; set; }

    public virtual DbSet<Recipe> Recipes { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Friendship>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Friendsh__3214EC0768BB3D59");

            entity.ToTable("Friendship");

            entity.Property(e => e.ReceiverId).HasMaxLength(50);
            entity.Property(e => e.RequestDate).HasColumnType("datetime");
            entity.Property(e => e.ResponseDate).HasColumnType("datetime");
            entity.Property(e => e.SenderId).HasMaxLength(50);

            entity.HasOne(d => d.Receiver).WithMany(p => p.FriendshipReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .HasConstraintName("FK__Friendshi__Recei__398D8EEE");

            entity.HasOne(d => d.Sender).WithMany(p => p.FriendshipSenders)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("FK__Friendshi__Sende__38996AB5");
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Ingredie__3214EC075C5397B0");

            entity.ToTable("Ingredient");

            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Food).HasMaxLength(50);
            entity.Property(e => e.FoodId).HasMaxLength(50);
            entity.Property(e => e.FoodMatch).HasMaxLength(50);
            entity.Property(e => e.Measure).HasMaxLength(30);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.TranslatedName).HasMaxLength(100);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Order__C3905BCFABED6853");

            entity.ToTable("Order");

            entity.Property(e => e.CreatedDateAt).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UserId).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Order__UserId__267ABA7A");
        });

        modelBuilder.Entity<PlanDate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PlanDate__3214EC07E08A02DF");

            entity.ToTable("PlanDate");

            entity.Property(e => e.Accessibility).HasMaxLength(50);
            entity.Property(e => e.CreateDateTime).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(255);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.PlanDates)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__PlanDate__Create__32E0915F");
        });

        modelBuilder.Entity<PlanDateDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PlanDate__3214EC07BA26145A");

            entity.ToTable("PlanDateDetail");

            entity.Property(e => e.PlanTime).HasColumnType("datetime");
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.Plan).WithMany(p => p.PlanDateDetails)
                .HasForeignKey(d => d.PlanId)
                .HasConstraintName("FK__PlanDateD__PlanI__35BCFE0A");
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Recipe__3214EC0750A13412");

            entity.ToTable("Recipe");

            entity.Property(e => e.Accessibility).HasMaxLength(50);
            entity.Property(e => e.CreateDateTime).HasColumnType("datetime");
            entity.Property(e => e.CreatedBy).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.TranslatedName).HasMaxLength(100);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Recipes)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Recipe__LikeQuan__300424B4");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK__RefreshT__658FEEEA16671CA7");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpirationDate).HasColumnType("datetime");
            entity.Property(e => e.Token).HasMaxLength(256);
            entity.Property(e => e.UpdatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RefreshTo__UserI__3F466844");
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subscrip__3214EC076D52EB22");

            entity.ToTable("Subscription");

            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UserId).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.Subscriptions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Subscript__UserI__2B3F6F97");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC076D6039CE");

            entity.ToTable("User");

            entity.Property(e => e.Id).HasMaxLength(50);
            entity.Property(e => e.BirthDate).HasColumnType("datetime");
            entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.Username).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
