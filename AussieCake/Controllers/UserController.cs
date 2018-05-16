using AussieCake.Models;
using AussieCake.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace AussieCake.Controllers
{
	public class UserController : SqLiteHelper
	{
		public static List<UserVM> Users { get; private set; }
		public static int ActiveId { get; set; }

    public static void Insert(UserVM user)
    {
      if (Users.Any(s => s.Logins.Contains(user.Logins.FirstOrDefault())))
        return;

      var model = new User(user);
			Application.Current.Dispatcher.Invoke(() =>
			{
				InsertUser(model);
				LoadUsersViewModel();
			});
    }

    public static void Update(UserVM user)
    {
      var model = new User(user);
			Application.Current.Dispatcher.Invoke(() => UpdateUser(model));
      var oldVM = Users.FirstOrDefault(x => x.Id == user.Id);
      oldVM = user;
    }

    public static void Remove(UserVM user)
    {
      var model = new User(user);
			Application.Current.Dispatcher.Invoke(() => RemoveUser(model));
      Users.Remove(user);
    }

    public static UserVM Get(int id)
    {
      return Users.FirstOrDefault(x => x.Id == id);
    }

		public static void LoadUsersViewModel()
		{
			if (Users == null)
				Users = new List<UserVM>();

			foreach (var user in UsersDB)
			{
				var vm = new UserVM(user);
				if (!Users.Contains(vm))
					Users.Add(vm);
			}
		}

	}
}
