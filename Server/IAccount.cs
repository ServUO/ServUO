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
	public static class AccountGold
	{
		public static bool Enabled = false;

		/// <summary>
		///     This amount specifies the value at which point Gold turns to Platinum.
		///     By default, when 1,000,000,000 Gold is accumulated, it will transform
		///     into 1 Platinum.
		/// </summary>
		public static int CurrencyThreshold = 1000000000;
	}

	public interface IGoldAccount
	{
		/// <summary>
		/// This amount represents the total amount of currency owned by the player.
		/// It is cumulative of both Gold and Platinum, the absolute total amount of
		/// Gold owned by the player can be found by multiplying this value by the
		/// CurrencyThreshold value.
		/// </summary>
		double TotalCurrency { get; }

		/// <summary>
		/// This amount represents the current amount of Gold owned by the player.
		/// The value does not include the value of Platinum and ranges from
		/// 0 to 999,999,999 by default.
		/// </summary>
		int TotalGold { get; }

		/// <summary>
		/// This amount represents the current amount of Platinum owned by the player.
		/// The value does not include the value of Gold and ranges from
		/// 0 to 2,147,483,647 by default.
		/// One Platinum represents the value of CurrencyThreshold in Gold.
		/// </summary>
		int TotalPlat { get; }

		/// <summary>
		/// Attempts to deposit the given amount of Gold and Platinum into this account.
		/// </summary>
		/// <param name="amount">Amount to deposit.</param>
		/// <returns>True if successful, false if amount given is less than or equal to zero.</returns>
		bool DepositCurrency(double amount);

		/// <summary>
		/// Attempts to deposit the given amount of Gold into this account.
		/// If the given amount is greater than the CurrencyThreshold, 
		/// Platinum will be deposited to offset the difference.
		/// </summary>
		/// <param name="amount">Amount to deposit.</param>
		/// <returns>True if successful, false if amount given is less than or equal to zero.</returns>
		bool DepositGold(int amount);

		/// <summary>
		/// Attempts to deposit the given amount of Platinum into this account.
		/// </summary>
		/// <param name="amount">Amount to deposit.</param>
		/// <returns>True if successful, false if amount given is less than or equal to zero.</returns>
		bool DepositPlat(int amount);

		/// <summary>
		/// Attempts to withdraw the given amount of Platinum and Gold from this account.
		/// </summary>
		/// <param name="amount">Amount to withdraw.</param>
		/// <returns>True if successful, false if balance was too low.</returns>
		bool WithdrawCurrency(double amount);

		/// <summary>
		/// Attempts to withdraw the given amount of Gold from this account.
		/// If the given amount is greater than the CurrencyThreshold, 
		/// Platinum will be withdrawn to offset the difference.
		/// </summary>
		/// <param name="amount">Amount to withdraw.</param>
		/// <returns>True if successful, false if balance was too low.</returns>
		bool WithdrawGold(int amount);

		/// <summary>
		/// Attempts to withdraw the given amount of Platinum from this account.
		/// </summary>
		/// <param name="amount">Amount to withdraw.</param>
		/// <returns>True if successful, false if balance was too low.</returns>
		bool WithdrawPlat(int amount);

		/// <summary>
		/// Gets the total balance of Gold for this account.
		/// </summary>
		/// <param name="gold">Gold value, Platinum exclusive</param>
		/// <param name="totalGold">Gold value, Platinum inclusive</param>
		void GetGoldBalance(out int gold, out double totalGold);

		/// <summary>
		/// Gets the total balance of Platinum for this account.
		/// </summary>
		/// <param name="plat">Platinum value, Gold exclusive</param>
		/// <param name="totalPlat">Platinum value, Gold inclusive</param>
		void GetPlatBalance(out int plat, out double totalPlat);

		/// <summary>
		/// Gets the total balance of Gold and Platinum for this account.
		/// </summary>
		/// <param name="gold">Gold value, Platinum exclusive</param>
		/// <param name="totalGold">Gold value, Platinum inclusive</param>
		/// <param name="plat">Platinum value, Gold exclusive</param>
		/// <param name="totalPlat">Platinum value, Gold inclusive</param>
		void GetBalance(out int gold, out double totalGold, out int plat, out double totalPlat);
	}

	public interface IAccount : IGoldAccount, IComparable<IAccount>
	{
		string Username { get; set; }
		string Email { get; set; }
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