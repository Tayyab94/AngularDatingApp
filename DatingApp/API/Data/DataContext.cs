using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<AppUser> AppUsers {get;set;}

         public DbSet<Photo> Photos {get;set;}

         public DbSet<UserLike> UserLikes {get;set;}

        public DbSet<Message> Messages{get;set;}

         protected override void OnModelCreating(ModelBuilder builder)
         {
            base.OnModelCreating(builder);

            builder.Entity<UserLike>()
            .HasKey(s=> new {s.SourceUserId,s.LikeUserId});

            builder.Entity<UserLike>()
                .HasOne(s=>s.SorceUser)
                .WithMany(s=>s.LikedUser)
                .HasForeignKey(s=>s.SourceUserId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.Entity<UserLike>()
                .HasOne(s=>s.LikedUser)
                .WithMany(s=>s.LikedByUser)
                .HasForeignKey(s=>s.LikeUserId
                ).OnDelete(DeleteBehavior.NoAction);


            builder.Entity<Message>()
            .HasOne(s=>s.Receiver)
            .WithMany(ss=>ss.MessageReceived)
            .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
            .HasOne(s=>s.Sender)
            .WithMany(ss=>ss.MessageSent)
            .OnDelete(DeleteBehavior.Restrict);
         }
    }
}