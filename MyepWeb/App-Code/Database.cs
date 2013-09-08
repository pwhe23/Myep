
using System.Collections.Generic;
using Funq;
using ServiceStack.OrmLite;

namespace Site
{
	public class Database
	{
		public static void Configure(Container container)
		{
			container.RegisterAutoWired<EmployersRepository>().ReusedWithin(ReuseScope.None);
			container.RegisterAutoWired<InternsRepository>().ReusedWithin(ReuseScope.None);
			container.RegisterAutoWired<UserRepository>().ReusedWithin(ReuseScope.None);
		}

		public static void Create(Container container)
		{
			using (var db = container.Resolve<IDbConnectionFactory>().OpenDbConnection())
			{
				db.AlterTable<Employer>();
				db.AlterTable<Intern>();
				db.AlterTable<User>();
			}
		}
	};

	public abstract class Repository
	{
		private readonly IDbConnectionFactory _db;

		protected Repository()
		{
			_db = Ioc.Get<IDbConnectionFactory>();
		}

		public List<T> List<T>(string sql)
		{
			using (var db = _db.OpenDbConnection())
			{
				return db.Select<T>(sql);
			}
		}

		public T SingleOrDefault<T>(int? id)
		{
			using (var db = _db.OpenDbConnection())
			{
				return db.GetByIdOrDefault<T>(id);
			}
		}

		public T SingleOrDefault<T>(string sql, params object[] args)
		{
			using (var db = _db.OpenDbConnection())
			{
				return db.SingleOrDefault<T>(sql.SqlFormat(args));
			}
		}

		public int Save<T>(T model) where T:new()
		{
			using (var db = _db.OpenDbConnection())
			{
				db.Save(model);
				return (int)db.GetLastInsertId();
			}
		}
	};
}
