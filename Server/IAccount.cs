#region Header
// **********
// ServUO - IAccount.cs
// **********
#endregion

#region References
using System;
#endregion

namespace Server.Accounting
{
	public interface IAccount : IComparable<IAccount>
	{
		string Username { get; set; }
		AccessLevel AccessLevel { get; set; }

		int Length { get; }
		int Limit { get; }
		int Count { get; }
		Mobile this[int index] { get; set; }

		void Delete();
		void SetPassword(string password);
		bool CheckPassword(string password);
	}
}