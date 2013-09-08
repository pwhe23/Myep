
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Security;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;

namespace Site
{
	[Alias("Users")]
	public class User 
	{
		[AutoIncrement]
		public int Id { get; set; }
		[Required, Index(Unique = true)]
		public string Email { get; set; }
		[Required]
		public string Password { get; set; }
		public string ResetCode { get; set; }
	};

	public class UserInfo
	{
		public int UserId { get; set; }
		public string Email { get; set; }
	}

	public class UserRepository
	{
		readonly IDbConnectionFactory _db;

		public UserRepository(IDbConnectionFactory db)
		{
			_db = db;
		}

		public List<UserInfo> Query(int? userId)
		{
			var where = new List<string> { "1=1" };
			if (userId != null)
			{
				where.Add("[Id]=" + userId.Value);
			}

			using (var db = _db.OpenDbConnection())
			{
				return db.Select<UserInfo>(@"
					SELECT [Users].Id AS UserId, Email
					FROM [Users]
					WHERE (" + string.Join(") AND (", where) + ")"
				);
			}
		}

		public User Get(string email)
		{
			using (var db = _db.OpenDbConnection())
			{
				return db.SingleOrDefault<User>("SELECT * FROM [Users] WHERE [Email]={0}", email);
			}
		}

		public User Get(int? id)
		{
			using (var db = _db.OpenDbConnection())
			{
				return db.GetByIdOrDefault<User>(id) ?? new User();
			}
		}

		public void Save(User model)
		{
			using (var db = _db.OpenDbConnection())
			{
				db.Save(model);
				model.Id = (int)db.GetLastInsertId();
			}
		}

		public bool Login(string email, string password, bool remember)
		{
			if (Membership.ValidateUser(email, password))
			{
				FormsAuthentication.SetAuthCookie(email, remember);
				return true;
			}
			return false;
		}

		public void Logout()
		{
			HttpContext.Current.Session.Abandon();
			FormsAuthentication.SignOut();
		}
	};
}
