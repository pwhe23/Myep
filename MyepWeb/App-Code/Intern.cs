
using System;
using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

namespace Site
{
	/// <summary> Intern model </summary>
	public partial class Intern
	{
		[AutoIncrement]
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Address { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Zip { get; set; }
		public string Phone { get; set; }
		public string ReferringAgency { get; set; }
		public string School { get; set; }
		public string Ssn { get; set; }
		public DateTime? Dob { get; set; }
		public string Gender { get; set; }
		public string Email { get; set; }
		public string ParentsEmail { get; set; }
		public string BackgroundCheck { get; set; }
		public string ShirtSize { get; set; }
		public bool? DS { get; set; }
		public string CDD { get; set; }
		public string Training { get; set; }
		public string Signature { get; set; }
		public string Comments { get; set; }
		public DateTime? IneligibleLetterDate { get; set; }

		[ForeignKey(typeof(Employer), OnDelete = "CASCADE", OnUpdate = "CASCADE")]
		public int? EmployerId { get; set; }

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

		public List<Intern> Query()
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
}
