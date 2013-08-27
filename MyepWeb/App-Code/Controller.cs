
using System.Web.Mvc;
using ServiceStack.OrmLite;

namespace Site
{
	public class AppController : Controller
	{
		private readonly InternsRepository _internsRepo;
		private readonly EmployersRepository _employersRepo;

		public AppController()
		{
			_internsRepo = Ioc.Get<InternsRepository>();
			_employersRepo = Ioc.Get<EmployersRepository>();
		}

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult Employers()
		{
			var model = _employersRepo.Query();
			return View(model);
		}

		public ActionResult Employer(int? id)
		{
			var model = _employersRepo.Get(id);
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
			var model = _internsRepo.Query();
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
