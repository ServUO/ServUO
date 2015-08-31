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
        #region Gold Account
        bool DepositCurrency(double amount);
        bool DepositGold(int amount);
        bool DepositPlat(int amount);

        bool WithdrawCurrency(double amount);
        bool WithdrawGold(int amount);
        bool WithdrawPlat(int amount);

        void GetGoldBalance(out int gold, out double totalGold);
        void GetPlatBalance(out int plat, out double totalPlat);
        void GetBalance(out int gold, out double totalGold, out int plat, out double totalPlat);

	    #endregion
		void Delete();
		void SetPassword(string password);
		bool CheckPassword(string password);
	}
}