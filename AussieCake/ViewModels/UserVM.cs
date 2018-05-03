using AussieCake.Controllers;
using AussieCake.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AussieCake.ViewModels
{
	public class UserVM
	{
		public int Id { get; private set; }

		public List<string> Logins { get; set; }
		public string Password { get; set; }

		public UserVM(List<string> logins, string password)
		{
			Logins = logins;
			Password = password;
		}

		public UserVM(User user)
		{
			Id = user.Id;
			Logins = user.Logins.Split(';').ToList();
			Password = user.Password;
		}

		public bool IsPasswordCorrect(string password)
		{
			if (string.IsNullOrEmpty(Password))
				return true;

			if (Password == password)
				return true;

			return false;
		}

		public DateTime GetLastLogin()
		{
      return DBController.CollocationAttempts.Max(x => x.When);
    }

  }
}
