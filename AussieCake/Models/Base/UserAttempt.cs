using System;

namespace AussieCake.Models
{
	public abstract class UserAttempt : BaseModel
	{
		public int IdUser { get; set; }
		public bool IsCorrect { get; set; }
		public DateTime	When { get; set; }
	}
}
