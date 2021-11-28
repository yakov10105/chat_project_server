using Chat_App.Models;
using System.Collections.Generic;
using System;


namespace Chat_App.Data
{
	public interface IUserRepo
	{
		IEnumerable<User> GetAllUsers();
		User GetUserById(int id);
		User GetUserByUserName(string userName);
		int GetUserIdByUserName(string userName);
		void CreateUser(User user);
		void UpdateUser(User user);
		void DeleteUser(User user);
		void UpdateIsOnline(int id, bool online);
		IEnumerable<User> GetOnlineUsers();
		void AddCoins(User user, int coinsAmount);
		void RemoveCoins(User user, int coinsAmount);
		bool IsEnoughCoinsForGame(User user, int coinsAmount);
	}
}

