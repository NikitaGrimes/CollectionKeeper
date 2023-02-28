using CollectionKeeper.Entities;
using Microsoft.AspNetCore.Identity;

namespace CollectionKeeper.Data
{
    public class ApplicationSeeder
    {
        private readonly ApplicationContext _context;

        public ApplicationSeeder(ApplicationContext context, 
            UserManager<CollectionUser> userManager, 
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
        }

        public async Task Seed(UserManager<CollectionUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context.Database.EnsureCreated();
            if (!_context.Topics.Any())
            {
                List<Topic> topics = new List<Topic>
                {
                    new Topic() { Name = "Books" },
                    new Topic() { Name = "Cards" },
                    new Topic() { Name = "Drinks" },
                    new Topic() { Name = "Paintings" },
                    new Topic() { Name = "Entertainment" },
                    new Topic() { Name = "Hobby" },
                    new Topic() { Name = "Other" }
                };
                _context.Topics.AddRange(topics);
                _context.SaveChanges();
            }

            if (!_context.Roles.Any())
            {
                var result = await roleManager.CreateAsync(new IdentityRole("admin"));
                if (result.Succeeded)
                {
                    CollectionUser admin = new CollectionUser()
                    {
                        UserName = "admin",
                        Email = "admin@gmail.com"
                    };
                    var result1 = await userManager.CreateAsync(admin, "p@ssw0rd!");
                    if (result1.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "admin");
                    }
                }
            }
        }
    }
}
