using AussieCake.ViewModels;
using System;

namespace AussieCake.Models
{
	public class User : BaseModel
	{
		public string Logins { get; set; }
		public string Password { get; set; } // it's a concatenate list (windows user)

		public User(string logins, string password)
		{
			Logins = logins;
			Password = password;
		}

		public User(int id, string logins, string password) : this(logins, password)
		{
			Id = id;
		}

    public User(UserVM viewModel)
    {
      Id = viewModel.Id;
      Logins = String.Join(";", viewModel.Logins.ToArray());
      Password = viewModel.Password;
    }
	}
}
