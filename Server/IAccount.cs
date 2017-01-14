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
		/// This amount specifies the value at which point Gold turns to Platinum.
		/// By default, when 1,000,000,000 Gold is accumulated, it will transform
		/// into 1 Platinum.
		/// !!! WARNING !!!
		/// The client is designed to perceive the currency threashold at 1,000,000,000
		/// if you change this, it may cause unexpected results when using secure trading.
		/// </summary>
		public static int CurrencyThreshold = 1000000000;

		/// <summary>
		/// Enables or Disables automatic conversion of Gold and Checks to Bank Currency
		/// when they are added to a bank box container.
		/// </summary>
		public static bool ConvertOnBank = true;

		/// <summary>
		/// Enables or Disables automatic conversion of Gold and Checks to Bank Currency
		/// when they are added to a secure trade container.
		/// </summary>
		public static bool ConvertOnTrade = false;
	}

	public interface IGoldAccount
	{
		/// <summary>
		/// This amount represents the total amount of currency owned by the player.
		/// It is cumulative of both Gold and Platinum, the absolute total amount of
		/// Gold owned by the player can be found by multiplying this value by the
		/// CurrencyThreshold value.
		/// </summary>
		[CommandProperty(AccessLevel.Administrator)]
		double TotalCurrency { get; }

		/// <summary>
		/// This amount represents the current amount of Gold owned by the player.
		/// The value does not include the value of Platinum and ranges from
		/// 0 to 999,999,999 by default.
		/// </summary>
		[CommandProperty(AccessLevel.Administrator)]
		int TotalGold { get; }

		/// <summary>
		/// This amount represents the current amount of Platinum owned by the player.
		/// The value does not include the value of Gold and ranges from
		/// 0 to 2,147,483,647 by default.
		/// One Platinum represents the value of CurrencyThreshold in Gold.
		/// </summary>
		[CommandProperty(AccessLevel.Administrator)]
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
		/// Attempts to deposit the given amount of Gold into this account.
		/// If the given amount is greater than the CurrencyThreshold, 
		/// Platinum will be deposited to offset the difference.
		/// </summary>
		/// <param name="amount">Amount to deposit.</param>
		/// <returns>True if successful, false if amount given is less than or equal to zero.</returns>
		bool DepositGold(long amount);

		/// <summary>
		/// Attempts to deposit the given amount of Platinum into this account.
		/// </summary>
		/// <param name="amount">Amount to deposit.</param>
		/// <returns>True if successful, false if amount given is less than or equal to zero.</returns>
		bool DepositPlat(int amount);

		/// <summary>
		/// Attempts to deposit the given amount of Platinum into this account.
		/// </summary>
		/// <param name="amount">Amount to deposit.</param>
		/// <returns>True if successful, false if amount given is less than or equal to zero.</returns>
		bool DepositPlat(long amount);

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
		/// Attempts to withdraw the given amount of Gold from this account.
		/// If the given amount is greater than the CurrencyThreshold, 
		/// Platinum will be withdrawn to offset the difference.
		/// </summary>
		/// <param name="amount">Amount to withdraw.</param>
		/// <returns>True if successful, false if balance was too low.</returns>
		bool WithdrawGold(long amount);

		/// <summary>
		/// Attempts to withdraw the given amount of Platinum from this account.
		/// </summary>
		/// <param name="amount">Amount to withdraw.</param>
		/// <returns>True if successful, false if balance was too low.</returns>
		bool WithdrawPlat(int amount);

		/// <summary>
		/// Attempts to withdraw the given amount of Platinum from this account.
		/// </summary>
		/// <param name="amount">Amount to withdraw.</param>
		/// <returns>True if successful, false if balance was too low.</returns>
		bool WithdrawPlat(long amount);

		/// <summary>
		/// Gets the total balance of Gold for this account.
		/// </summary>
		/// <param name="gold">Gold value, Platinum exclusive</param>
		/// <param name="totalGold">Gold value, Platinum inclusive</param>
		void GetGoldBalance(out int gold, out double totalGold);

		/// <summary>
		/// Gets the total balance of Gold for this account.
		/// </summary>
		/// <param name="gold">Gold value, Platinum exclusive</param>
		/// <param name="totalGold">Gold value, Platinum inclusive</param>
		void GetGoldBalance(out long gold, out double totalGold);

		/// <summary>
		/// Gets the total balance of Platinum for this account.
		/// </summary>
		/// <param name="plat">Platinum value, Gold exclusive</param>
		/// <param name="totalPlat">Platinum value, Gold inclusive</param>
		void GetPlatBalance(out int plat, out double totalPlat);

		/// <summary>
		/// Gets the total balance of Platinum for this account.
		/// </summary>
		/// <param name="plat">Platinum value, Gold exclusive</param>
		/// <param name="totalPlat">Platinum value, Gold inclusive</param>
		void GetPlatBalance(out long plat, out double totalPlat);

		/// <summary>
		/// Gets the total balance of Gold and Platinum for this account.
		/// </summary>
		/// <param name="gold">Gold value, Platinum exclusive</param>
		/// <param name="totalGold">Gold value, Platinum inclusive</param>
		/// <param name="plat">Platinum value, Gold exclusive</param>
		/// <param name="totalPlat">Platinum value, Gold inclusive</param>
		void GetBalance(out int gold, out double totalGold, out int plat, out double totalPlat);

		/// <summary>
		/// Gets the total balance of Gold and Platinum for this account.
		/// </summary>
		/// <param name="gold">Gold value, Platinum exclusive</param>
		/// <param name="totalGold">Gold value, Platinum inclusive</param>
		/// <param name="plat">Platinum value, Gold exclusive</param>
		/// <param name="totalPlat">Platinum value, Gold inclusive</param>
		void GetBalance(out long gold, out double totalGold, out long plat, out double totalPlat);
	}

	public interface IAccount : IGoldAccount, IComparable<IAccount>
	{
		[CommandProperty(AccessLevel.Administrator, true)]
		string Username { get; set; }

		[CommandProperty(AccessLevel.Administrator, true)]
		string Email { get; set; }

		[CommandProperty(AccessLevel.Administrator, AccessLevel.Owner)]
		AccessLevel AccessLevel { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		int Length { get; }

		[CommandProperty(AccessLevel.Administrator)]
		int Limit { get; }

		[CommandProperty(AccessLevel.Administrator)]
		int Count { get; }

		Mobile this[int index] { get; set; }

		void Delete();
		void SetPassword(string password);
		bool CheckPassword(string password);
	}
}