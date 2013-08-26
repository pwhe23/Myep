
using System.Collections.Generic;
using ServiceStack.Common.Web;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Cors;

namespace Site
{
	/// <summary> Employer model </summary>
	public partial class Employer
	{
		[AutoIncrement]
		public int Id { get; set; }
		public string CompanyName { get; set; }
	};

	/// <summary> Persist Employers to the database </summary>
	public class EmployersRepository
	{
		private readonly IDbConnectionFactory _db;

		public EmployersRepository(IDbConnectionFactory db)
		{
			_db = db;
		}

		public int Count()
		{
			using (var db = _db.OpenDbConnection())
			{
				return db.Scalar<int>("SELECT COUNT(*) FROM [Employer]");
			}
		}

		public List<Employer> Query(int? id)
		{
			using (var db = _db.OpenDbConnection())
			{
				return db.Select<Employer>();
			}
		}

		public void Save(Employer model)
		{
			using (var db = _db.OpenDbConnection())
			{
				db.Save(model);
				model.Id = (int)db.GetLastInsertId();
			}
		}

		public Employer Get(int? id)
		{
			using (var db = _db.OpenDbConnection())
			{
				return db.GetByIdOrDefault<Employer>(id) ?? new Employer();
			}
		}
	};

	/// <summary> Query for a list of Employers </summary>
	[Route("/employers", "GET")]
	public class EmployersQuery : IReturn<List<Employer>>
	{
		public int? Id { get; set; }
	};

	[Route("/employers/{Id}", "GET")]
	public class ViewEmployer : IReturn<Employer>
	{
		public int? Id { get; set; }
	};

	[Route("/employers/{Id}", "POST")]
	public partial class Employer { };

	/// <summary> Employers rest-service endpoint </summary>
	[ClientCanSwapTemplates]
	public class EmployersService : Service
	{
		private readonly EmployersRepository _repo;

		public EmployersService()
		{
			_repo = TryResolve<EmployersRepository>();
		}

		[EnableCors]
		public void Options(EmployersQuery request) { }

		[DefaultView("Employers")]
		public List<Employer> Get(EmployersQuery request)
		{
			return _repo.Query(request.Id);
		}

		[DefaultView("Employer")]
		public Employer Get(ViewEmployer request)
		{
			return _repo.Get(request.Id);
		}

		public HttpResult Post(Employer model)
		{
			_repo.Save(model);
			return this.Created(model, "/employers");
		}
	};
}
