
using ServiceStack.DataAnnotations;

namespace Site
{
	public partial class Employer
	{
		[AutoIncrement]
		public int Id { get; set; }
		public string CompanyName { get; set; }
	};
}
