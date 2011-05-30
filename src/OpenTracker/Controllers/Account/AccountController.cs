using System.Threading;
using System.Web.Mvc;
using System.Web.Routing;
using OpenTracker.Core;
using OpenTracker.Core.Account;
using OpenTracker.Core.Common;
using OpenTracker.Models.Account;

namespace OpenTracker.Controllers.Account
{
	public class AccountController : BaseController
	{
		public IFormsAuthenticationService AuthenticationService { get; set; }
		public AccountService AccountService { get; set; }

		protected override void Initialize(RequestContext requestContext)
		{
			if (AuthenticationService == null) { AuthenticationService = new FormsAuthenticationService(); }
			if (AccountService == null) { AccountService = new AccountService(); }

			base.Initialize(requestContext);
		}

		[AuthorizeUser]
		public ActionResult Index()
		{
			return View();
		}

		//
		// GET: /Account/
		//
		public ActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Login(LoginModel loginModel, string returnUrl)
		{
			if (ModelState.IsValid)
			{
				/*
				var profiler = GetProfiler();
				using (profiler.Step("bcrypt match password"))
				{
				}
				*/
				if (AccountService.ValidateUser(loginModel.Username, loginModel.Password))
				{
					AuthenticationService.SignIn(loginModel.Username, loginModel.RememberMe);

					if (Url.IsLocalUrl(returnUrl))
						return Redirect(returnUrl);

					return RedirectToAction("Index", "Account");
				}
				ModelState.AddModelError("", "The user name or password provided is incorrect.");
			}
			// if we got this far, something failed, redisplay form
			return View(loginModel);
		}

		// 
		// URL: /Account/LogOff
		// 
		public ActionResult LogOff()
		{
			AuthenticationService.SignOut();

			return RedirectToAction("Index", "Account");
		}


		// 
		// URL: /Account/Register
		// 
		public ActionResult Register()
		{
			ViewBag.PasswordLength = 6;
			return View();
		}

		[HttpPost]
		public ActionResult Register(RegisterModel registerModel)
		{
			if (!ModelState.IsValid)
				return View();

			var profiler = GetProfiler();

			string bcryptHashed;
			using (profiler.Step("bcrypt password"))
				bcryptHashed = BCrypt.HashPassword(registerModel.Password, BCrypt.GenerateSalt(10));

			using (profiler.Step("register user"))
			{
				var createStatus = AccountService.CreateUser(
					registerModel.Username,
					bcryptHashed,
					registerModel.Email
				);

				if (createStatus == AccountService.AccountCreateStatus.Success)
				{
					// AuthenticationService.SignIn(registerModel.Username, false /* createPersistentCookie */);
					return RedirectToAction("Login", "Account");
				}
				else
				{
					ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
				}
			}
			return View(registerModel);
		}

	}


	public abstract class BaseController : Controller
	{
		public MiniProfiler GetProfiler()
		{
			// does a profiler already exist for this request?
			var profiler = HttpContext.GetProfiler();
			if (profiler != null) return profiler;

			// might want to decide here (or maybe inside the action) whether you want
			// to profile this request - for example, using an "IsSystemAdmin" flag against
			// the user, or similar; this could also all be done in action filters, but this
			// is simple and practical; just return null for most users. For our test, we'll
			// profiler only for local requests (seems reasonable)
			if (Request.IsLocal)
			{
				profiler = new MiniProfiler(Request.Url.OriginalString);
				HttpContext.SetProfiler(profiler);
			}
			return profiler;
		}

		protected override void OnResultExecuting(ResultExecutingContext filterContext)
		{
			// note  that the capturing will usually be terminated by the view anyway
			filterContext.HttpContext.GetProfiler().Step("OnResultExecuting");
			base.OnResultExecuting(filterContext);
		}
	}


}
