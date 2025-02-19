using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddAuthentication("OAuth")
				.AddJwtBearer("OAuth", config =>
				{
					var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);
					var key = new SymmetricSecurityKey(secretBytes);

					// Send token in URL instead of header
					config.Events = new JwtBearerEvents()
					{
						OnMessageReceived = context =>
						{
							if (context.Request.Query.ContainsKey("access_token"))
							{
								context.Token = context.Request.Query["access_token"];
							}
							return Task.CompletedTask;
						}
					};

					config.TokenValidationParameters = new TokenValidationParameters()
					{
						ValidIssuer = Constants.Issuer,
						ValidAudience = Constants.Audience,
						IssuerSigningKey = key
					};
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
