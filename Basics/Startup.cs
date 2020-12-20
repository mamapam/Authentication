using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Basics
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddAuthentication("CookieAuth")
				.AddCookie("CookieAuth", config =>
				{
					config.Cookie.Name = "Grandmas.Cookie";
					config.LoginPath = "/Home/Authenticate";
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
