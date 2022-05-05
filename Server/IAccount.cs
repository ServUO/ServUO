#region References
using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;

using Server.Network;
#endregion

namespace Server.Accounting
{
	public static class AccountGold
	{
		[ConfigProperty("Accounts.VirtualGold")]
		public static bool Enabled { get => Config.Get("Accounts.VirtualGold", Core.TOL); set => Config.Set("Accounts.VirtualGold", value); }

		/// <summary>
		/// This amount specifies the value at which point Gold turns to Platinum.
		/// By default, when 1,000,000,000 Gold is accumulated, it will transform
		/// into 1 Platinum.
		/// !!! WARNING !!!
		/// The client is designed to perceive the currency threashold at 1,000,000,000
		/// if you change this, it may cause unexpected results when using secure trading.
		/// </summary>
		public const int CurrencyThreshold = 1000000000;

		/// <summary>
		/// Enables or Disables automatic conversion of Gold and Checks to Bank Currency
		/// when they are added to a bank box container.
		/// </summary>
		[ConfigProperty("Accounts.ConvertGoldOnBank")]
		public static bool ConvertOnBank { get => Config.Get("Accounts.ConvertGoldOnBank", true); set => Config.Set("Accounts.ConvertGoldOnBank", value); }

		/// <summary>
		/// Enables or Disables automatic conversion of Gold and Checks to Bank Currency
		/// when they are added to a secure trade container.
		/// </summary>
		[ConfigProperty("Accounts.ConvertGoldOnTrade")]
		public static bool ConvertOnTrade { get => Config.Get("Accounts.ConvertGoldOnTrade", true); set => Config.Set("Accounts.ConvertGoldOnTrade", value); }

		public static double GetGoldTotal(Mobile m)
		{
			if (m == null)
			{
				return 0;
			}

			return GetGoldTotal(m.Account);
		}

		public static double GetGoldTotal(IGoldAccount a)
		{
			if (a == null)
			{
				return 0;
			}

			a.GetGoldBalance(out int gold, out var totalGold);

			return totalGold;
		}

		public static double GetPlatTotal(Mobile m)
		{
			if (m == null)
			{
				return 0;
			}

			return GetPlatTotal(m.Account);
		}

		public static double GetPlatTotal(IGoldAccount a)
		{
			if (a == null)
			{
				return 0;
			}

			a.GetPlatBalance(out int plat, out var totalPlat);

			return totalPlat;
		}
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

		void SetCurrency(double amount);

		void SetGold(int amount);

		void SetPlat(int amount);

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

		bool HasGoldBalance(double amount);
		bool HasPlatBalance(double amount);
	}

	public interface ISecureAccount
	{
		int GetSecureBalance(Mobile m);
		void SetSecureBalance(Mobile m, int amount);

		bool HasSecureBalance(Mobile m, int amount);

		bool DepositSecure(Mobile m, int amount);
		bool WithdrawSecure(Mobile m, int amount);
	}

	public interface IStoreAccount
	{
		[CommandProperty(AccessLevel.Administrator, true)]
		int Sovereigns { get; }

		void SetSovereigns(int amount);

		bool HasSovereigns(int amount);

		bool DepositSovereigns(int amount);
		bool WithdrawSovereigns(int amount);
	}

	public interface IAccount : IGoldAccount, ISecureAccount, IStoreAccount, IComparable, IComparable<IAccount>, IEnumerable<Mobile>
	{
		[CommandProperty(AccessLevel.Administrator, true)]
		string Username { get; }

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

		[CommandProperty(AccessLevel.Administrator, true)]
		DateTime Created { get; set; }

		[CommandProperty(AccessLevel.Administrator, true)]
		DateTime LastLogin { get; set; }

		[CommandProperty(AccessLevel.Administrator, true)]
		IPAddress[] LoginIPs { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		TimeSpan Age { get; }

		[CommandProperty(AccessLevel.Administrator)]
		TimeSpan TotalGameTime { get; }

		[CommandProperty(AccessLevel.Administrator)]
		bool Banned { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		bool Young { get; set; }

		[CommandProperty(AccessLevel.Administrator, true)]
		bool Deleted { get; }

		Mobile this[int index] { get; set; }

		void Delete();

		string GetPassword();
		void SetPassword(string password);
		bool CheckPassword(string password);

		int Flags { get; set; }

		bool Inactive { get; }

		string[] IPRestrictions { get; set; }

		void CheckYoung();
		void RemoveYoungStatus(int message);

		bool GetFlag(int index);
		void SetFlag(int index, bool value);

		List<IAccountComment> Comments { get; }

		void AddComment(string author, string value);

		List<IAccountTag> Tags { get; }

		string GetTag(string name);
		void SetTag(string name, string value);
		void AddTag(string name, string value);
		void RemoveTag(string name);

		bool GetBanTags(out DateTime banTime, out TimeSpan banDuration);
		void SetBanTags(Mobile from, DateTime banTime, TimeSpan banDuration);
		void SetUnspecifiedBan(Mobile from);

		bool CheckAccess(IPAddress ipAddress);
		bool CheckAccess(NetState ns);
		bool HasAccess(IPAddress ipAddress);
		bool HasAccess(NetState ns);
		void LogAccess(IPAddress ipAddress);
		void LogAccess(NetState ns);

		bool Save(XmlElement xml);
		bool Load(XmlElement xml);

		bool Save(GenericWriter writer);
		bool Load(GenericReader reader);
	}

	public interface IAccountComment
	{
		DateTime LastModified { get; set; }

		string AddedBy { get; set; }
		string Content { get; set; }

		void Save(XmlElement xml);
		void Load(XmlElement xml);

		void Save(GenericWriter writer);
		void Load(GenericReader reader);
	}

	public interface IAccountTag
	{
		string Name { get; set; }
		string Value { get; set; }

		void Save(XmlElement xml);
		void Load(XmlElement xml);

		void Save(GenericWriter writer);
		void Load(GenericReader reader);
	}
}
