using System;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Data;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<AppUser> Users { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<MemberLike> Likes { get; set; }

    // adjusting time to UTC time
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // base = the base class of AppDbContext, which is DbContext
        base.OnModelCreating(modelBuilder);

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

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }
            }
        }
    }
}
