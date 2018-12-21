using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetcore.webapi.Data
{
	public class DbSeeder
	{
		public static void Initialize(IServiceProvider serviceProvider)
		{
			var initializer = new DbSeeder();

			var context = serviceProvider
				.GetRequiredService<DockerDbContext>();

			var userManager = serviceProvider
				.GetRequiredService<UserManager<ApplicationUser>>();

			var roleManager = serviceProvider
				.GetRequiredService<RoleManager<IdentityRole>>();

			CreateUserAndRoles(context, userManager, roleManager).Wait();
		}

		private static async Task CreateUserAndRoles(
		DockerDbContext context, UserManager<ApplicationUser> manager, RoleManager<IdentityRole> roleManager)
		{
			context.Database.EnsureCreated();
			if (context.Users.Any())
			{
				return;
			}

			await roleManager.CreateAsync(new IdentityRole("admin"));
			var adminUser = new ApplicationUser
			{
				UserName = "admin",
				FirstName = "Admin",
				LastName = "User",
				Email = "admin@admin.com",
				SecurityStamp = Guid.NewGuid().ToString(),
			};
			await manager.CreateAsync(adminUser, "admin");
			await manager.AddToRoleAsync(adminUser, "admin");
		}
	}
}
