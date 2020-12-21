using Basics.AuthorizationRequirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

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

			services.AddAuthorization(config =>
			{
				// This is by default what happens under the hood with authorization.
				//var defaultAuthBuilder = new AuthorizationPolicyBuilder();
				//var defaultAuthPolicy = defaultAuthBuilder
				//	.RequireAuthenticatedUser()
				//	.RequireClaim(ClaimTypes.DateOfBirth)
				//	.Build();


				//config.DefaultPolicy = defaultAuthPolicy;

				//config.AddPolicy("Claim.DoB", policyBuilder =>
				//{
				//	policyBuilder.RequireClaim(ClaimTypes.DateOfBirth)
				//});

				config.AddPolicy("Claim.DoB", policyBuilder =>
				{
					// same line as the next one, but with newly created extension method
					//policyBuilder.AddRequirements(new CustomRequireClaim(ClaimTypes.DateOfBirth));
					policyBuilder.RequireCustomClaim(ClaimTypes.DateOfBirth);
				});
			});

			services.AddScoped<IAuthorizationHandler, CustomRequireClaimHandler>();

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
