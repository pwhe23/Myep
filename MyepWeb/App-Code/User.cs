
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Security;
using ServiceStack.DataAnnotations;

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

	public class UserRepository : Repository
	{
		public List<UserInfo> Query(int? userId)
		{
			var where = new List<string> { "1=1" };
			if (userId != null)
			{
				where.Add("[Id]=" + userId.Value);
			}

			return base.List<UserInfo>(@"
				SELECT [Users].Id AS UserId, Email
				FROM [Users]
				WHERE (" + string.Join(") AND (", where) + ")"
			);
		}

		public User Get(string email)
		{
			return base.SingleOrDefault<User>("SELECT * FROM [Users] WHERE [Email]={0}", email);
		}

		public User Get(int? id)
		{
			return base.SingleOrDefault<User>(id) ?? new User();
		}

		public void Save(User model)
		{
			model.Id = base.Save(model);
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
