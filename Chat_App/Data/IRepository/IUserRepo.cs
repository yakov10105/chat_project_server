using Chat_App.Models;
using System.Collections.Generic;
using System;


namespace Chat_App.Data
{
	public interface IUserRepo
	{
		bool SaveChanges();

		IEnumerable<User> GetAllUsers();
		User GetUserById(int id);
		User GetUserByUserName(string userName);
		void CreateUser(User user);
		void UpdateUser(User user);
		void DeleteUser(User user);
	}
}

