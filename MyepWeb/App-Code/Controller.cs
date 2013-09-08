
using System;
using System.Web.Mvc;
using ServiceStack.OrmLite;

namespace Site
{
	[Authorize]
	public class AppController : Controller
	{
		private readonly InternsRepository _internsRepo;
		private readonly EmployersRepository _employersRepo;
		private readonly UserRepository _userRepository;

		public AppController()
		{
			_internsRepo = Ioc.Get<InternsRepository>();
			_employersRepo = Ioc.Get<EmployersRepository>();
			_userRepository = Ioc.Get<UserRepository>();
		}

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult Employers()
		{
			var model = _employersRepo.Query(null);
			return View(model);
		}

		public ActionResult Employer(int? id)
		{
			var model = _employersRepo.Get(id);
			ViewBag.Interns = _internsRepo.Query(null, model.Id);
			return View(model);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Employer(Employer model)
		{
			_employersRepo.Save(model);
			return RedirectToAction("Employers");
		}

		public ActionResult Interns()
		{
			var model = _internsRepo.Query(null, null);
			return View(model);
		}

		public ActionResult Intern(int? id)
		{
			var model = _internsRepo.Get(id);
			return View(model);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Intern(Intern model)
		{
			_internsRepo.Save(model);
			return RedirectToAction("Interns");
		}

		public ActionResult Assign(int? id)
		{
			var model = _internsRepo.GetAssignment(id);
			return View("Assign", model);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Assign(int id, int employerId)
		{
			_internsRepo.Assign(id, employerId);
			return RedirectToAction("Intern", new{ id });
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Unassign(int? id, int? internId)
		{
			_internsRepo.Assign(internId ?? id ?? 0, null);

			if (internId.HasValue)
				return RedirectToAction("Employer", new { id });
			else if (id.HasValue)
				return RedirectToAction("Intern", new { id });
			return Content("Can't assign");
		}

		public ActionResult Users()
		{
			var model = _userRepository.Query(null);
			return View(model);
		}

		public new ActionResult User(int? id)
		{
			var model = _userRepository.Get(id);
			return View(model);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public new ActionResult User(User model)
		{
			_userRepository.Save(model);
			return RedirectToAction("Users");
		}

		[AllowAnonymous]
		public ActionResult Login()
		{
			var user = new User();
			return View(user);
		}

		[AllowAnonymous, AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Login(User user)
		{
			if (!_userRepository.Login(user.Email, user.Password, true))
			{
				throw new Exception("Invalid login");
			}
			return RedirectToAction("Index");
		}

		public ActionResult Logout()
		{
			_userRepository.Logout();
			return Redirect(Request.UrlReferrer == null ? "/" : Request.UrlReferrer.ToString());
		}

		public ActionResult Reset()
		{
			using (var db = Ioc.Get<IDbConnectionFactory>().OpenDbConnection())
			{
				db.DropAndCreateTable<Employer>();
				db.DropAndCreateTable<Intern>();
				//db.InsertAll(SeedRockstars);
			}
			return Content("Database Reset", "text/plain");
		}

	};
}
