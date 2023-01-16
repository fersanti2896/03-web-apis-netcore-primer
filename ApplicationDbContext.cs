using AutoresAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoresAPI {
    public class ApplicationDbContext : DbContext {
        public ApplicationDbContext(DbContextOptions options) : base(options) {
        }

        public DbSet<Autor> Autores { get; set; }
    }
}
