using Domains;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure
{
    public class ContextData : DbContext
{
    public ContextData(DbContextOptions<ContextData> options)
        : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            TaskerMapping(modelBuilder);
        }

        public DbSet<TaskEvent> TaskEvents { get; set; }
        protected ModelBuilder TaskerMapping(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TaskEvent>(b =>
            {
                b.HasKey(t => t.Id);
                b.ToTable("Taskers");

                b.Property(t => t.Id)
                    .ValueGeneratedOnAdd()
                    .UseIdentityColumn(increment: 1);

                b.Property(t => t.Title)
                     .HasMaxLength(100)
                     .IsRequired();

                // b.Property(t => t.DateEvent)
                //      .HasDefaultValue(DateTime.Now)
                //      .IsRequired();

                b.Property(t => t.Status)
                .HasDefaultValue(EventsStatus.PENDENTE)
                    .IsRequired();
            });

            return modelBuilder;
        }
    }
}
