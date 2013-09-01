
using System.Collections.Generic;

namespace Site
{
	public class Assignment
	{
		public int? InternId { get; set; }
		public string InternName { get; set; }
		public int? EmployerId { get; set; }
		public string Organization { get; set; }
		public List<EmployerInfo> Employers { get; set; }
	};
}
