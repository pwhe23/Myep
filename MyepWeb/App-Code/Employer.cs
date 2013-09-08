
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ServiceStack.DataAnnotations;

namespace Site
{
	/// <summary> Employer model </summary>
	public partial class Employer
	{
		[AutoIncrement]
		public int Id { get; set; }
		[Required]
		public string Organization { get; set; }
		public string ContactTitle { get; set; }
		public string ContactFirstName { get; set; }
		public string ContactLastName { get; set; }
		public string Address { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Zip { get; set; }
		public string Phone { get; set; }
		public string Phone2 { get; set; }
		public string Email { get; set; }
		public int? Positions { get; set; }
		public DateTime? AgreementDate { get; set; }

		public override string ToString()
		{
			return Organization;
		}

		public static Employer Get(int? id)
		{
			var repo = Ioc.Get<EmployersRepository>();
			return repo.Get(id);
		}
	};

	public class EmployerInfo
	{
		public int EmployerId { get; set; }
		public string Organization { get; set; }
		public string ContactName { get; set; }
		public int Available { get; set; }
		public int Filled { get; set; }
	};

	/// <summary> Persist Employers to the database </summary>
	public class EmployersRepository : Repository
	{
		public List<EmployerInfo> Query(int? employerId)
		{
			var where = new List<string> { "1=1" };
			if (employerId != null)
			{
				where.Add("[Id]=" + employerId.Value);
			}

			return base.List<EmployerInfo>(@"
				SELECT Employer.Id AS EmployerId, Organization, ContactFirstName+' '+ContactLastName AS ContactName, ISNULL(Positions,0) AS Available,
					(SELECT COUNT(*) FROM Intern WHERE EmployerId=Employer.Id) AS Filled
				FROM Employer
				WHERE (" + string.Join(") AND (", where) + ")"
			);
		}

		public Employer Get(int? id)
		{
			return base.SingleOrDefault<Employer>(id) ?? new Employer();
		}

		public void Save(Employer model)
		{
			model.Id = base.Save(model);
		}
	};
}
