using Microsoft.EntityFrameworkCore;

#nullable disable

namespace PersonalAPI.Data
{
    public partial class PersonalAppContext : DbContext
    {
        public PersonalAppContext(DbContextOptions<PersonalAppContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Todo> Todos { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_CI_AS");

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
