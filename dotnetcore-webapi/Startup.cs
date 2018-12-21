using dotnetcore.webapi.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace dotnetcore.webapi
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<DockerDbContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:DefaultConnection"]));

			services
			.AddIdentity<ApplicationUser, IdentityRole>()
			.AddEntityFrameworkStores<DockerDbContext>()
			.AddDefaultTokenProviders();

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			// Password policy
			services.Configure<IdentityOptions>(options =>
			{
				options.Password.RequireDigit = false;
				options.Password.RequiredLength = 4;
				options.Password.RequireLowercase = false;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = false;
			});

			services.AddCors();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				// app.UseHsts();
			}

			app.UseCors(config => config
				.AllowAnyOrigin()
				.AllowAnyHeader()
				.AllowAnyMethod());

			// app.UseHttpsRedirection();
			app.UseMvc();
		}
	}
}
