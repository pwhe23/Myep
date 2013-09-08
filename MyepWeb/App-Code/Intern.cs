
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

namespace Site
{
	/// <summary> Intern model </summary>
	public partial class Intern
	{
		[AutoIncrement]
		public int Id { get; set; }
		[Required]
		public string FirstName { get; set; }
		[Required]
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

		public override string ToString()
		{
			return FullName;
		}
	};

	public class InternInfo
	{
		public int InternId { get; set; }
		public string FullName { get; set; }
		public int? EmployerId { get; set; }
		public string Organization { get; set; }
	}

	/// <summary> Persist Interns to the database </summary>
	public class InternsRepository : Repository
	{
		public List<InternInfo> Query(int? internId, int? employerId)
		{
			var where = new List<string> {"1=1"};
			if (internId != null) where.Add("Intern.Id=" + internId);
			if (employerId.HasValue) where.Add("EmployerId=" + employerId);

			return base.List<InternInfo>(@"
				SELECT Intern.Id AS InternId, FirstName+' '+LastName AS FullName, EmployerId, Organization 
				FROM Intern
				LEFT JOIN Employer ON Intern.EmployerId = Employer.Id
				WHERE (" + string.Join(") AND (", where) + ")"
			);
		}

		public void Save(Intern model)
		{
			model.Id = base.Save(model);
		}

		public Assignment GetAssignment(int? internId)
		{
			var assignment = new Assignment();
			var repo = Ioc.Get<EmployersRepository>();
			var intern = Query(internId ?? 0, null).SingleOrDefault();
			var employer = repo.Query(assignment.EmployerId ?? 0).SingleOrDefault();

			if (intern != null)
			{
				assignment.InternName = intern.FullName;
				assignment.EmployerId = intern.EmployerId;
			}

			if (employer != null)
			{
				assignment.Organization = employer.Organization;
			}

			assignment.Employers = repo.Query(null);

			return assignment;
		}

		public Intern Get(int? id)
		{
			return base.SingleOrDefault<Intern>(id) ?? new Intern();
		}

		public void Assign(int internId, int? employerId)
		{
			var intern = Get(internId);
			intern.EmployerId = employerId;
			Save(intern);
		}
	};
}
