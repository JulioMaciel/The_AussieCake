using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AussieCake.Models
{
	public abstract class UserAttempt : BaseModel
	{
		public int IdUser { get; set; }
		public bool IsCorrect { get; set; }
		public DateTime	When { get; set; }
	}
}
