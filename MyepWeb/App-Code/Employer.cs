﻿
using System;
using System.Collections.Generic;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

namespace Site
{
	/// <summary> Employer model </summary>
	public partial class Employer
	{
		[AutoIncrement]
		public int Id { get; set; }
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

		public List<Employer> Query()
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
}