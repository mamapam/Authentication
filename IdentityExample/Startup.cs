using IdentityExample.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityExample
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			// This makes AppDbContext available to be accessible anywhere in app, Dependency Injection
			services.AddDbContext<AppDbContext>(config =>
			{
				config.UseInMemoryDatabase("Memory");
			});

			// AddIdentity registers the services
			services.AddIdentity<IdentityUser, IdentityRole>(config =>
			{
				config.Password.RequiredLength = 4;
				config.Password.RequireDigit = false;
				config.Password.RequireNonAlphanumeric = false;
				config.Password.RequireUppercase = false;
			})
				.AddEntityFrameworkStores<AppDbContext>()
				.AddDefaultTokenProviders();

			services.ConfigureApplicationCookie(config =>
			{
				config.Cookie.Name = "Identity";
				config.LoginPath = "/Home/Login";
			});

			services.AddControllersWithViews();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			// Looks at route being accessed and decides which endpoint to use
			app.UseRouting();

			// Who are you?
			app.UseAuthentication();

			// This must be after routing so it can actually authorize against route
			// Are you allowed?
			app.UseAuthorization();


			// Maps to the endpoint that routing invokes
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapDefaultControllerRoute();
			});
		}
	}
}
