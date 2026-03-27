using System;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Data;

// IdentityDbContext will create the AspNetUsers table in the database on our behalf.
public class AppDbContext(DbContextOptions options) : IdentityDbContext<AppUser>(options)
{
    // public DbSet<AppUser> Users { get; set; }
    // Each DbSet = one table, define the rows in Entities(<Member>)
    public DbSet<Member> Members { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<MemberLike> Likes { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Connection> Connections { get; set; }

    // adjusting time to UTC time
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // base = the base class of AppDbContext, which is DbContext
        base.OnModelCreating(modelBuilder);

        // HasQueryFilter: only return approved photos in queries
        modelBuilder.Entity<Photo>().HasQueryFilter(x => x.IsApproved);

        modelBuilder.Entity<IdentityRole>()
        .HasData(
            new IdentityRole { Id = "member-id", Name = "Member", NormalizedName = "MEMBER", ConcurrencyStamp = "member-static" },
            new IdentityRole { Id = "moderator-id", Name = "Moderator", NormalizedName = "MODERATOR", ConcurrencyStamp = "moderator-static" },
            new IdentityRole { Id = "admin-id", Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "admin-static" }
        );

        modelBuilder.Entity<Message>().HasOne(x => x.Recipient).WithMany(m => m.MessagesReceived).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Message>().HasOne(x => x.Sender).WithMany(m => m.MessagesSent).OnDelete(DeleteBehavior.Restrict);

        // setting the primary key of the MemberLike entity to be a composite key consisting of SourceMemberId and TargetMemberId
        modelBuilder.Entity<MemberLike>()
            .HasKey(x => new { x.SourceMemberId, x.TargetMemberId });

        // one source member can have many liked members; the foreign key in the memberlike table is SourceMemberId; 
        // should we delete the source member and that's going to cascade into the member like table
        // .OnDelete(DeleteBehavior.Cascade): 用於定義關聯數據的刪除行為
        modelBuilder.Entity<MemberLike>().HasOne(s => s.SourceMember).WithMany(t => t.LikedMembers).HasForeignKey(s => s.SourceMemberId).OnDelete(DeleteBehavior.Cascade);

        // .OnDelete(DeleteBehavior.NoAction): operation will fail when deleting an entity if dependent entities exist.
        modelBuilder.Entity<MemberLike>().HasOne(s => s.TargetMember).WithMany(t => t.LikedByMembers).HasForeignKey(s => s.TargetMemberId).OnDelete(DeleteBehavior.NoAction);

        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
        );

        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? v.Value.ToUniversalTime() : null,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null
        );

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(nullableDateTimeConverter);
                }
            }
        }
    }
}
