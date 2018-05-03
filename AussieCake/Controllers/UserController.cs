using AussieCake.Models;
using AussieCake.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AussieCake.Controllers
{
	public static class UserController
	{
		public static int ActiveId { get; set; }

    public static void Insert(UserVM user)
    {
      if (DBController.Users.Any(s => s.Logins.Contains(user.Logins.FirstOrDefault())))
        return;

      var model = new User(user);
      SqLiteHelper.InsertUser(model);
      DBController.LoadUsersViewModel();
    }

    public static void Update(UserVM user)
    {
      var model = new User(user);
      SqLiteHelper.UpdateUser(model);
      var oldVM = DBController.Users.FirstOrDefault(x => x.Id == user.Id);
      oldVM = user;
    }

    public static void Remove(UserVM user)
    {
      var model = new User(user);
      SqLiteHelper.RemoveUser(model);
      DBController.Users.Remove(user);
    }

    public static UserVM Get(int id)
    {
      return DBController.Users.FirstOrDefault(x => x.Id == id);
    }

  }
}
