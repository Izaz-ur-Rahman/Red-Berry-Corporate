using Microsoft.EntityFrameworkCore;
//using RedBerryCorporate.Api.Models;
using RedBerryCorporate.Models;

namespace RedBerryCorporate.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<BlueprintSubmission> BlueprintSubmissions { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<TblEmployee> TblEmployees { get; set; }
    }
}