using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Web.Security;
using System.Text;

namespace testapp.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modify { get; set; }
        [ForeignKey("UserId")]
        public virtual ICollection<GroupInUsers> GroupInUsers { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public static ApplicationUser GetCurrentUser(string userName)
        {
            using (var context = new ApplicationDbContext())
            {
                var currentUser = context.Users.FirstOrDefaultAsync(f => f.UserName == userName).Result;
                return currentUser;
            }
        }

        public string GetFullName(ApplicationUser applicationUser)
        {
            return applicationUser.FirstName + " " + applicationUser.LastName;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<ApplicationDbContext>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Product>()
              .ToTable("Products");
            modelBuilder.Entity<Permission>()
              .ToTable("Permissions");
            modelBuilder.Entity<Group>()
              .ToTable("Groups");
            modelBuilder.Entity<GroupInUsers>()
              .ToTable("GroupInUsers");
            modelBuilder.Entity<ApplicationUser>()
              .ToTable("Users");
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupInUsers> GroupInUsers { get; set; }
    }
}