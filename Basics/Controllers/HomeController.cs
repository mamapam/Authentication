using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Basics.Controllers
{
	public class HomeController : Controller
	{
		// MAY NOT WANT TO INJECT THIS HERE BUT PERHAPS ONLY AT FUNCTION LEVEL
		//private readonly IAuthorizationService _authorizationService;

		//public HomeController(IAuthorizationService authorizationService)
		//{
		//	_authorizationService = authorizationService;
		//}

		public IActionResult Index()
		{
			return View();
		}

		// Authorize attribute used to guard an action
		[Authorize]
		public IActionResult Secret()
		{
			return View();
		}

		[Authorize(Policy = "Claim.DoB")]
		public IActionResult SecretPolicy()
		{
			return View("Secret");
		}

		[Authorize(Roles = "Admin")]
		public IActionResult SecretRole()
		{
			return View("Secret");
		}

		public IActionResult Authenticate()
		{
			// Create a user

			var grandmaClaims = new List<Claim>()
			{
				new Claim(ClaimTypes.Name, "Bob"),
				new Claim(ClaimTypes.Email, "Bob@fmail.com"),
				new Claim(ClaimTypes.DateOfBirth, "11/11/2000"),
				new Claim(ClaimTypes.Role, "Admin"),
				new Claim("Grandma.Says", "Very nice boi"),
			};

			var licenseClaims = new List<Claim>()
			{
				new Claim(ClaimTypes.Name, "Bob K Foo"),
				new Claim("DrivingLicense", "A+"),
			};

			var grandmaIdentity = new ClaimsIdentity(grandmaClaims, "Grandma Identity");
			var licenseIdentity = new ClaimsIdentity(licenseClaims, "Government");

			var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity, licenseIdentity });

			HttpContext.SignInAsync(userPrincipal);

			return RedirectToAction("Index");
		}

		// This way makes injecting local to the function
		public async Task<IActionResult> DoStuff(
			[FromServices] IAuthorizationService authorizationService)
		{
			// we are doing stuff here

			// below code allows us to do authorization strategically in certain parts of code

			var builder = new AuthorizationPolicyBuilder("Schema");
			var customPolicy = builder.RequireClaim("Hello").Build();

			var authResult = await authorizationService.AuthorizeAsync(User, "Claim.DoB", customPolicy);

			if (authResult.Succeeded)
			{
				return View("Index");
			}

			return View("Index");
		}

	}
}
