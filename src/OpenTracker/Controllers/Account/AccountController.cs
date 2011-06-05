using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using OpenTracker.Core;
using OpenTracker.Core.Account;
using OpenTracker.Core.Common;
using OpenTracker.Models.Account;

namespace OpenTracker.Controllers.Account
{
	public class AccountController : Controller
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
		// GET: /Account/Login
		//
		public ActionResult Login(string message)
		{
			if (User.Identity.IsAuthenticated)
				return RedirectToAction("Index");

			switch (message)
			{
				case "registersuccess":
					ViewBag.Notification = "window.history.pushState('', '', '/account/login');\nshowSuccess('Please check your inbox for activation link.');";
					break;
				case "activationfail":
					ViewBag.Notification = "window.history.pushState('', '', '/account/login');\nshowError('Invalid activation code.');";
					break;
				case "activationsuccess":
					ViewBag.Notification = "window.history.pushState('', '', '/account/login');\nshowSuccess('Your account has successfully been activated.');";
					break;
				case "activateexist":
					ViewBag.Notification = "window.history.pushState('', '', '/account/login');\n showError('Your account has already been activated.');";
					break;
			}

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
				var validateUser = AccountService.ValidateUser(loginModel.Username, loginModel.Password);
				if (validateUser > 0)
				{
					AuthenticationService.SignIn(loginModel.Username, loginModel.RememberMe, validateUser);

					if (Url.IsLocalUrl(returnUrl))
						return Redirect(returnUrl);

					return RedirectToAction("Index", "Account");
				}
				ModelState.AddModelError("", "The user name or password provided is incorrect.");
				ViewBag.Notification = "showError('The username or password provided is incorrect..');";
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
		// URL: /Account/LogOut
		// 
		public ActionResult LogOut()
		{
			return RedirectToAction("LogOff", "Account");
		}


		// 
		// URL: /Account/Register
		// 
		public ActionResult Register()
		{
			if (User.Identity.IsAuthenticated)
				return RedirectToAction("Index");

			return View();
		}

		[HttpPost]
		public ActionResult Register(RegisterModel registerModel)
		{
			if (!ModelState.IsValid)
				return View();

			var bcryptHashed = BCrypt.HashPassword(registerModel.Password, BCrypt.GenerateSalt(10));

			var createStatus = AccountService.CreateUser(
				registerModel.Username,
				bcryptHashed,
				registerModel.Email
			);

			if (createStatus == AccountService.AccountCreateStatus.Success)
			{
				// AuthenticationService.SignIn(registerModel.Username, false /* createPersistentCookie */);
				// return RedirectToAction("Login", "Account", new { registered = "true" });

				return RedirectToAction("Login", "Account", new { message = "registersuccess" });
			}
			else
			{
				ViewBag.Notification = string.Format("showError('{0}');", AccountValidation.ErrorCodeToString(createStatus));
				ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
			}
			return View(registerModel);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hash"></param>
		/// <returns></returns>
		public ActionResult Activate(string hash)
		{
			using (var context = new OpenTrackerDbContext())
			{
				var checkActivation = (from u in context.users
										where u.activatesecret == hash
										select u).Take(1).FirstOrDefault();
				if (checkActivation == null)
					return RedirectToAction("Login", "Account", new { message = "activationfail" });
				if (checkActivation.activated == 1)
					return RedirectToAction("Login", "Account", new { message = "activateexist" });

				checkActivation.activated = 1;
				checkActivation.@class = 1;
				checkActivation.uploaded = TrackerSettings.DEFAULT_UPLOADED_VALUE;
				context.SaveChanges();

				return RedirectToAction("Login", "Account", new { message = "activationsuccess" });
			}
		}


	}




}
