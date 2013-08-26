
using System.Collections.Generic;
using ServiceStack.Common.Web;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace Site
{
	/// <summary> Intern model </summary>
	public partial class Intern
	{
		[AutoIncrement]
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }

		[Ignore]
		public string FullName
		{
			get { return FirstName + " " + LastName; }
		}
	};

	/// <summary> Persist Interns to the database </summary>
	public class InternsRepository
	{
		private readonly IDbConnectionFactory _db;

		public InternsRepository(IDbConnectionFactory db)
		{
			_db = db;
		}

		public int Count()
		{
			using (var db = _db.OpenDbConnection())
			{
				return db.Scalar<int>("SELECT COUNT(*) FROM [Intern]");
			}
		}

		public List<Intern> Query(int? id)
		{
			using (var db = _db.OpenDbConnection())
			{
				return db.Select<Intern>();
			}
		}

		public void Save(Intern model)
		{
			using (var db = _db.OpenDbConnection())
			{
				db.Save(model);
				model.Id = (int)db.GetLastInsertId();
			}
		}

		public Intern Get(int? id)
		{
			using (var db = _db.OpenDbConnection())
			{
				return db.GetByIdOrDefault<Intern>(id) ?? new Intern();
			}
		}
	};

	/// <summary> Query for a list of Interns </summary>
	[Route("/interns", "GET")]
	public class InternsQuery : IReturn<List<Intern>>
	{
		public int? Id { get; set; }
	};

	[Route("/interns/{Id}", "GET")]
	public class ViewIntern : IReturn<Intern>
	{
		public int? Id { get; set; }
	};

	[Route("/interns/{Id}", "POST")]
	public partial class Intern { };

	/// <summary> Interns rest-service endpoint </summary>
	[ClientCanSwapTemplates]
	public class InternsService : Service
	{
		private readonly InternsRepository _repo;

		public InternsService()
		{
			_repo = TryResolve<InternsRepository>();
		}

		[DefaultView("Interns")]
		public List<Intern> Get(InternsQuery request)
		{
			return _repo.Query(request.Id);
		}

		[DefaultView("Intern")]
		public Intern Get(ViewIntern request)
		{
			var model = _repo.Get(request.Id);
			return model;
		}

		public HttpResult Post(Intern model)
		{
			_repo.Save(model);
			return this.Created(model, "/interns/" + model.Id);
		}
	};
}
