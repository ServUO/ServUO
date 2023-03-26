#region References
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml;

using Server.Misc;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
#endregion

namespace Server.Accounting
{
	[PropertyObject]
	public partial class Account : IAccount
	{
		public static TimeSpan YoungDuration { get => Accounts.YoungDuration; set => Accounts.YoungDuration = value; }
		public static TimeSpan InactiveDuration { get => Accounts.InactiveDuration; set => Accounts.InactiveDuration = value; }
		public static TimeSpan EmptyInactiveDuration { get => Accounts.EmptyInactiveDuration; set => Accounts.EmptyInactiveDuration = value; }

		public static void GetAccountInfo(IAccount a, out AccessLevel accessLevel, out bool online)
		{
			accessLevel = a.AccessLevel;
			online = false;

			for (var i = 0; i < a.Length; i++)
			{
				var check = a[i];

				if (check == null)
				{
					continue;
				}

				if (check.AccessLevel > accessLevel)
				{
					accessLevel = check.AccessLevel;
				}

				if (check.NetState != null)
				{
					online = true;
				}
			}
		}

		protected Mobile[] m_Mobiles = new Mobile[7];

		/// <summary>
		///     Object detailing information about the hardware of the last person to log into this account
		/// </summary>
		[CommandProperty(AccessLevel.Administrator)]
		public HardwareInfo HardwareInfo { get; set; }

		/// <summary>
		///     List of IP addresses for restricted access. '*' wildcard supported. If the array contains zero entries, all IP
		///     addresses are allowed.
		/// </summary>
		public string[] IPRestrictions { get; set; } = Array.Empty<string>();

		/// <summary>
		///     List of IP addresses which have successfully logged into this account.
		/// </summary>
		public IPAddress[] LoginIPs { get; set; } = Array.Empty<IPAddress>();

		protected List<IAccountComment> m_Comments;

		/// <summary>
		///     List of account comments. Type of contained objects is IAccountComment.
		/// </summary>
		public List<IAccountComment> Comments => m_Comments ?? (m_Comments = new List<IAccountComment>());

		protected List<IAccountTag> m_Tags;

		/// <summary>
		///     List of account tags. Type of contained objects is IAccountTag.
		/// </summary>
		public List<IAccountTag> Tags => m_Tags ?? (m_Tags = new List<IAccountTag>());

		/// <summary>
		///     Account password. Plain text. Case sensitive validation. May be null.
		/// </summary>
		public string PlainPassword { get; set; }

		/// <summary>
		///     Account password. Hashed with MD5. May be null.
		/// </summary>
		public string MD5Password { get; set; }

		/// <summary>
		///     Account username and password hashed with SHA1. May be null.
		/// </summary>
		public string SHA1Password { get; set; }

		/// <summary>
		///     Account username and password hashed with SHA512. May be null.
		/// </summary>
		public string SHA512Password { get; set; }

		/// <summary>
		///     Internal bitfield of account flags. Consider using direct access properties (Banned, Young), or GetFlag/SetFlag
		///     methods
		/// </summary>
		public int Flags { get; set; }

		/// <summary>
		///		Gets a flag indicating if this account is deleted.
		/// </summary>
		[CommandProperty(AccessLevel.Administrator, true)]
		public bool Deleted { get => GetFlag(31); private set => SetFlag(31, value); }

		/// <summary>
		///     Gets or sets a flag indiciating if this account is banned.
		/// </summary>
		[CommandProperty(AccessLevel.Administrator)]
		public bool Banned
		{
			get
			{
				if (!GetFlag(0))
				{
					return false;
				}


				if (!GetBanTags(out var banTime, out var banDuration) || banDuration == TimeSpan.MaxValue || DateTime.UtcNow < (banTime + banDuration))
				{
					return true;
				}

				SetUnspecifiedBan(null); // clear
				SetFlag(0, false);

				return false;
			}
			set => SetFlag(0, value);
		}

		protected Timer m_YoungTimer;

		/// <summary>
		///     Gets or sets a flag indicating if the characters created on this account will have the young status.
		/// </summary>
		[CommandProperty(AccessLevel.Administrator)]
		public bool Young
		{
			get => !GetFlag(1);
			set
			{
				SetFlag(1, !value);

				if (m_YoungTimer == null)
				{
					return;
				}

				m_YoungTimer.Stop();
				m_YoungTimer = null;
			}
		}

		/// <summary>
		///     The date and time of when this account was created.
		/// </summary>
		[CommandProperty(AccessLevel.Administrator, true)]
		public DateTime Created { get; set; } = DateTime.UtcNow;

		[CommandProperty(AccessLevel.Administrator)]
		public TimeSpan Age => DateTime.UtcNow - Created;

		/// <summary>
		///     Gets or sets the date and time when this account was last accessed.
		/// </summary>
		[CommandProperty(AccessLevel.Administrator)]
		public DateTime LastLogin { get; set; } = DateTime.UtcNow;

		/// <summary>
		///     An account is considered inactive based upon LastLogin and InactiveDuration.  If the account is empty, it is based
		///     upon EmptyInactiveDuration
		/// </summary>
		[CommandProperty(AccessLevel.Administrator)]
		public bool Inactive
		{
			get
			{
				if (AccessLevel >= AccessLevel.Counselor)
				{
					return false;
				}

				var inactiveLength = DateTime.UtcNow - LastLogin;

				return inactiveLength > (Count == 0 ? EmptyInactiveDuration : InactiveDuration);
			}
		}

		protected TimeSpan m_TotalGameTime;

		/// <summary>
		///     Gets the total game time of this account, also considering the game time of characters
		///     that have been deleted.
		/// </summary>
		[CommandProperty(AccessLevel.Administrator, true)]
		public TimeSpan TotalGameTime
		{
			get
			{
				for (var i = 0; i < m_Mobiles.Length; i++)
				{
					if (m_Mobiles[i] != null && m_Mobiles[i].NetState != null)
					{
						return m_TotalGameTime + m_Mobiles[i].NetState.ConnectedFor;
					}
				}

				return m_TotalGameTime;
			}
			protected set => m_TotalGameTime = value;
		}

		/// <summary>
		///     Account username. Case insensitive validation.
		/// </summary>
		[CommandProperty(AccessLevel.Administrator, true)]
		public string Username { get; private set; }

		/// <summary>
		///     Account email address. Case insensitive validation.
		/// </summary>
		[CommandProperty(AccessLevel.Administrator)]
		public string Email { get; set; }

		/// <summary>
		///     Initial AccessLevel for new characters created on this account.
		/// </summary>
		[CommandProperty(AccessLevel.Administrator, AccessLevel.Owner)]
		public AccessLevel AccessLevel { get; set; }

		/// <summary>
		///     Gets the current number of characters on this account.
		/// </summary>
		[CommandProperty(AccessLevel.Administrator)]
		public int Count
		{
			get
			{
				var count = 0;

				for (var i = 0; i < Length; ++i)
				{
					if (this[i]?.Deleted == false)
					{
						++count;
					}
				}

				return count;
			}
		}

		/// <summary>
		///     Gets the maximum amount of characters allowed to be created on this account. Values other than 1, 5, 6, or 7 are
		///     not supported by the client.
		/// </summary>
		[CommandProperty(AccessLevel.Administrator)]
		public int Limit => Siege.SiegeShard ? Siege.CharacterSlots : 7;

		/// <summary>
		///     Gets the maxmimum amount of characters that this account can hold.
		/// </summary>
		[CommandProperty(AccessLevel.Administrator)]
		public int Length => m_Mobiles.Length;

		/// <summary>
		///     Gets or sets the character at a specified index for this account.
		///     Out of bound index values are handled; null returned for get, ignored for set.
		/// </summary>
		public Mobile this[int index]
		{
			get
			{
				if (index < 0 || index >= m_Mobiles.Length)
				{
					return null;
				}

				var m = m_Mobiles[index];

				if (m == null || !m.Deleted)
				{
					return m;
				}

				m.Account = null;
				m_Mobiles[index] = null;
				return null;
			}
			set
			{
				if (index < 0 || index >= m_Mobiles.Length)
				{
					return;
				}

				if (m_Mobiles[index] != null)
				{
					m_Mobiles[index].Account = null;
				}

				m_Mobiles[index] = value;

				if (m_Mobiles[index] != null)
				{
					m_Mobiles[index].Account = this;
				}
			}
		}

		public Account(string username, string password)
		{
			Username = username;

			SetPassword(password);

			Accounts.Add(this);
		}

		public Account(XmlElement node)
		{
			if (!Load(node))
			{
				Delete();
			}
		}

		public Account(GenericReader reader)
		{
			if (!Load(reader))
			{
				Delete();
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			for (var i = 0; i < Length; ++i)
			{
				var m = this[i];

				if (m != null)
				{
					yield return m;
				}
			}
		}

		IEnumerator<Mobile> IEnumerable<Mobile>.GetEnumerator()
		{
			for (var i = 0; i < Length; ++i)
			{
				var m = this[i];

				if (m != null)
				{
					yield return m;
				}
			}
		}

		/// <summary>
		///     Deletes the account, all characters of the account, and all houses of those characters
		/// </summary>
		public void Delete()
		{
			if (Deleted)
			{
				return;
			}

			Accounts.Remove(Username);

			if (LoginIPs.Length != 0 && AccountHandler.IPTable.ContainsKey(LoginIPs[0]))
			{
				--AccountHandler.IPTable[LoginIPs[0]];
			}

			OnDelete();

			Deleted = true;

			OnAfterDelete();
		}

		protected virtual void OnDelete()
		{
			for (var i = 0; i < Length; ++i)
			{
				var m = this[i];

				if (m == null)
				{
					continue;
				}

				var list = BaseHouse.GetHouses(m);

				foreach (var h in list)
				{
					h.Delete();
				}

				ColUtility.Free(list);

				m.Delete();

				m.Account = null;
				m_Mobiles[i] = null;
			}
		}

		protected virtual void OnAfterDelete()
		{ }

		public virtual void SetPassword(string plainPassword)
		{
			PlainPassword = MD5Password = SHA1Password = SHA512Password = null;

			var prot = AccountHandler.ProtectPasswords;

			var salt = prot >= PasswordProtection.SHA1 ? Username : String.Empty;

			var pass = Hash(salt + plainPassword, AccountHandler.ProtectPasswords);

			switch (prot)
			{
				case PasswordProtection.None: PlainPassword = pass; break;
				case PasswordProtection.MD5: MD5Password = pass; break;
				case PasswordProtection.SHA1: SHA1Password = pass; break;
				case PasswordProtection.SHA512: SHA512Password = pass; break;
			}
		}

		public virtual string GetPassword()
		{
			switch (AccountHandler.ProtectPasswords)
			{
				case PasswordProtection.None: return PlainPassword;
				case PasswordProtection.MD5: return MD5Password;
				case PasswordProtection.SHA1: return SHA1Password;
				case PasswordProtection.SHA512: return SHA512Password;
			}

			return null;
		}

		public virtual bool CheckPassword(string plainPassword)
		{
			var ok = false;
			var curProt = PasswordProtection.None;

			if (PlainPassword != null)
			{
				ok = PlainPassword == Hash(plainPassword, curProt = PasswordProtection.None);
			}
			else if (MD5Password != null)
			{
				ok = MD5Password == Hash(plainPassword, curProt = PasswordProtection.MD5);
			}
			else if (SHA1Password != null)
			{
				ok = SHA1Password == Hash(Username + plainPassword, curProt = PasswordProtection.SHA1);
			}
			else if (SHA512Password != null)
			{
				ok = SHA512Password == Hash(Username + plainPassword, curProt = PasswordProtection.SHA512);
			}

			if (ok && curProt != AccountHandler.ProtectPasswords)
			{
				SetPassword(plainPassword);
			}

			return ok;
		}

		public int CompareTo(IAccount other)
		{
			if (other == null)
			{
				return 1;
			}

			GetAccountInfo(this, out var aLevel, out var aOnline);
			GetAccountInfo(other, out var bLevel, out var bOnline);

			if (aOnline && !bOnline)
			{
				return -1;
			}

			if (bOnline && !aOnline)
			{
				return 1;
			}

			if (aLevel > bLevel)
			{
				return -1;
			}

			if (aLevel < bLevel)
			{
				return 1;
			}

			return String.Compare(Username, other.Username, StringComparison.Ordinal);
		}

		public int CompareTo(object obj)
		{
			return CompareTo(obj as IAccount);
		}

		/// <summary>
		///     Gets the value of a specific flag in the Flags bitfield.
		/// </summary>
		/// <param name="index">The zero-based flag index.</param>
		public bool GetFlag(int index)
		{
			return (Flags & (1 << index)) != 0;
		}

		/// <summary>
		///     Sets the value of a specific flag in the Flags bitfield.
		/// </summary>
		/// <param name="index">The zero-based flag index.</param>
		/// <param name="value">The value to set.</param>
		public void SetFlag(int index, bool value)
		{
			if (value)
			{
				Flags |= (1 << index);
			}
			else
			{
				Flags &= ~(1 << index);
			}
		}

		/// <summary>
		///     Adds a new comment to this account.
		/// </summary>
		/// <param name="author">New comment author.</param>
		/// <param name="content">New comment content.</param>
		public virtual void AddComment(string author, string content)
		{
			Comments.Add(new AccountComment(author, content));
		}

		/// <summary>
		///     Adds a new tag to this account. This method does not check for duplicate names.
		/// </summary>
		/// <param name="name">New tag name.</param>
		/// <param name="value">New tag value.</param>
		public virtual void AddTag(string name, string value)
		{
			Tags.Add(new AccountTag(name, value));
		}

		/// <summary>
		///     Removes all tags with the specified name from this account.
		/// </summary>
		/// <param name="name">Tag name to remove.</param>
		public void RemoveTag(string name)
		{
			for (var i = Tags.Count - 1; i >= 0; --i)
			{
				if (i >= Tags.Count)
				{
					continue;
				}

				var tag = Tags[i];

				if (tag.Name == name)
				{
					Tags.RemoveAt(i);
				}
			}
		}

		/// <summary>
		///     Modifies an existing tag or adds a new tag if no tag exists.
		/// </summary>
		/// <param name="name">Tag name.</param>
		/// <param name="value">Tag value.</param>
		public void SetTag(string name, string value)
		{
			var tag = Tags.Find(t => t.Name == name);

			if (tag != null)
			{
				tag.Value = value;
			}
			else
			{
				AddTag(name, value);
			}
		}

		/// <summary>
		///     Gets the value of a tag -or- null if there are no tags with the specified name.
		/// </summary>
		/// <param name="name">Name of the desired tag value.</param>
		public string GetTag(string name)
		{
			return Tags.Find(t => t.Name == name)?.Value;
		}

		public virtual void SetUnspecifiedBan(Mobile from)
		{
			SetBanTags(from, DateTime.MinValue, TimeSpan.Zero);
		}

		public virtual void SetBanTags(Mobile from, DateTime banTime, TimeSpan banDuration)
		{
			if (from == null)
			{
				RemoveTag("BanDealer");
			}
			else
			{
				SetTag("BanDealer", from.ToString());
			}

			if (banTime == DateTime.MinValue)
			{
				RemoveTag("BanTime");
			}
			else
			{
				SetTag("BanTime", XmlConvert.ToString(banTime, XmlDateTimeSerializationMode.Utc));
			}

			if (banDuration == TimeSpan.Zero)
			{
				RemoveTag("BanDuration");
			}
			else
			{
				SetTag("BanDuration", banDuration.ToString());
			}
		}

		public virtual bool GetBanTags(out DateTime banTime, out TimeSpan banDuration)
		{
			var tagTime = GetTag("BanTime");
			var tagDuration = GetTag("BanDuration");

			banTime = tagTime != null ? Utility.GetXMLDateTime(tagTime, DateTime.MinValue) : DateTime.MinValue;

			if (tagDuration == "Infinite")
			{
				banDuration = TimeSpan.MaxValue;
			}
			else if (tagDuration != null)
			{
				banDuration = Utility.ToTimeSpan(tagDuration);
			}
			else
			{
				banDuration = TimeSpan.Zero;
			}

			return banTime != DateTime.MinValue && banDuration != TimeSpan.Zero;
		}

		public virtual void RemoveYoungStatus(int message)
		{
			Young = false;

			foreach (var m in m_Mobiles.OfType<PlayerMobile>().Where(m => m.Young))
			{
				m.Young = false;

				if (m.NetState == null)
				{
					continue;
				}

				if (message > 0)
				{
					m.SendLocalizedMessage(message);
				}

				// You are no longer considered a young player of Ultima Online, and are no longer subject to the limitations and benefits of being in that caste.
				m.SendLocalizedMessage(1019039);
			}
		}

		public virtual void CheckYoung()
		{
			if (TotalGameTime >= YoungDuration)
			{
				// You are old enough to be considered an adult, and have outgrown your status as a young player!
				RemoveYoungStatus(1019038);
			}
		}

		/// <summary>
		///     Checks if a specific NetState is allowed access to this account.
		/// </summary>
		/// <param name="ns">NetState instance to check.</param>
		/// <returns>True if allowed, false if not.</returns>
		public virtual bool HasAccess(NetState ns)
		{
			return ns != null && HasAccess(ns.Address);
		}

		public virtual bool HasAccess(IPAddress ipAddress)
		{
			var level = AccountHandler.LockdownLevel;

			if (level >= AccessLevel.Counselor)
			{
				var hasAccess = false;

				if (AccessLevel >= level)
				{
					hasAccess = true;
				}
				else
				{
					for (var i = 0; !hasAccess && i < Length; ++i)
					{
						var m = this[i];

						if (m != null && m.AccessLevel >= level)
						{
							hasAccess = true;
						}
					}
				}

				if (!hasAccess)
				{
					return false;
				}
			}

			var accessAllowed = IPRestrictions.Length == 0 || IPLimiter.IsExempt(ipAddress);

			for (var i = 0; !accessAllowed && i < IPRestrictions.Length; ++i)
			{
				accessAllowed = Utility.IPMatch(IPRestrictions[i], ipAddress);
			}

			return accessAllowed;
		}

		/// <summary>
		///     Records the IP address of 'ns' in its 'LoginIPs' list.
		/// </summary>
		/// <param name="ns">NetState instance to record.</param>
		public virtual void LogAccess(NetState ns)
		{
			if (ns != null)
			{
				LogAccess(ns.Address);
			}
		}

		public virtual void LogAccess(IPAddress ipAddress)
		{
			if (IPLimiter.IsExempt(ipAddress))
			{
				return;
			}

			if (LoginIPs.Length == 0)
			{
				if (AccountHandler.IPTable.ContainsKey(ipAddress))
				{
					AccountHandler.IPTable[ipAddress]++;
				}
				else
				{
					AccountHandler.IPTable[ipAddress] = 1;
				}
			}

			var contains = false;

			for (var i = 0; !contains && i < LoginIPs.Length; ++i)
			{
				contains = LoginIPs[i].Equals(ipAddress);
			}

			if (contains)
			{
				return;
			}

			var old = LoginIPs;
			LoginIPs = new IPAddress[old.Length + 1];

			for (var i = 0; i < old.Length; ++i)
			{
				LoginIPs[i] = old[i];
			}

			LoginIPs[old.Length] = ipAddress;
		}

		/// <summary>
		///     Checks if a specific NetState is allowed access to this account. If true, the NetState IPAddress is added to the
		///     address list.
		/// </summary>
		/// <param name="ns">NetState instance to check.</param>
		/// <returns>True if allowed, false if not.</returns>
		public virtual bool CheckAccess(NetState ns)
		{
			return ns != null && CheckAccess(ns.Address);
		}

		public virtual bool CheckAccess(IPAddress ipAddress)
		{
			var hasAccess = HasAccess(ipAddress);

			if (hasAccess)
			{
				LogAccess(ipAddress);
			}

			return hasAccess;
		}

		public override string ToString()
		{
			return Username;
		}

		private class YoungTimer : Timer
		{
			private readonly Account _Account;

			public YoungTimer(Account account)
				: base(TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(1.0))
			{
				_Account = account;

				Priority = TimerPriority.FiveSeconds;
			}

			protected override void OnTick()
			{
				_Account.CheckYoung();
			}
		}

		#region Gold Account

		/// <summary>
		///     This amount specifies the value at which point Gold turns to Platinum.
		///     By default, when 1,000,000,000 Gold is accumulated, it will transform
		///     into 1 Platinum.
		/// </summary>
		public static int CurrencyThreshold => AccountGold.CurrencyThreshold;

		/// <summary>
		///     This amount represents the total amount of currency owned by the player.
		///     It is cumulative of both Gold and Platinum, the absolute total amount of
		///     Gold owned by the player can be found by multiplying this value by the
		///     CurrencyThreshold value.
		/// </summary>
		[CommandProperty(AccessLevel.Administrator, true)]
		public double TotalCurrency { get; private set; }

		/// <summary>
		///     This amount represents the current amount of Gold owned by the player.
		///     The value does not include the value of Platinum and ranges from
		///     0 to 999,999,999 by default.
		/// </summary>
		[CommandProperty(AccessLevel.Administrator)]
		public int TotalGold => (int)Math.Floor((TotalCurrency - Math.Truncate(TotalCurrency)) * Math.Max(1.0, CurrencyThreshold));

		/// <summary>
		///     This amount represents the current amount of Platinum owned by the player.
		///     The value does not include the value of Gold and ranges from
		///     0 to 2,147,483,647 by default.
		///     One Platinum represents the value of CurrencyThreshold in Gold.
		/// </summary>
		[CommandProperty(AccessLevel.Administrator)]
		public int TotalPlat => (int)Math.Truncate(TotalCurrency);

		public void SetCurrency(double amount)
		{
			if (Double.IsNaN(amount) || Double.IsInfinity(amount))
			{
				return;
			}

			if (amount <= 0)
			{
				return;
			}

			var oldAmount = TotalCurrency;
			var balance = Math.Max(0, amount);

			if (Double.IsNaN(balance) || Double.IsInfinity(balance))
			{
				return;
			}

			TotalCurrency = balance;

			if (oldAmount != balance)
			{
				EventSink.InvokeAccountCurrencyChange(new AccountCurrencyChangeEventArgs(this, oldAmount, balance));
			}
		}

		public void SetGold(int amount)
		{
			SetCurrency(Math.Truncate(TotalCurrency) + (amount / Math.Max(1.0, CurrencyThreshold)));
		}

		public void SetPlat(int amount)
		{
			SetCurrency(amount + (TotalCurrency - Math.Truncate(TotalCurrency)));
		}

		/// <summary>
		///     Attempts to deposit the given amount of Gold and Platinum into this account.
		/// </summary>
		/// <param name="amount">Amount to deposit.</param>
		/// <returns>True if successful, false if amount given is less than or equal to zero.</returns>
		public bool DepositCurrency(double amount)
		{
			if (Double.IsNaN(amount) || Double.IsInfinity(amount))
			{
				return false;
			}

			if (amount <= 0)
			{
				return false;
			}

			var oldAmount = TotalCurrency;
			var balance = Math.Max(0, oldAmount + amount);

			if (Double.IsNaN(balance) || Double.IsInfinity(balance))
			{
				return false;
			}

			TotalCurrency = balance;

			if (oldAmount != balance)
			{
				EventSink.InvokeAccountCurrencyChange(new AccountCurrencyChangeEventArgs(this, oldAmount, balance));
			}

			return true;
		}

		/// <summary>
		///     Attempts to deposit the given amount of Gold into this account.
		///     If the given amount is greater than the CurrencyThreshold,
		///     Platinum will be deposited to offset the difference.
		/// </summary>
		/// <param name="amount">Amount to deposit.</param>
		/// <returns>True if successful, false if amount given is less than or equal to zero.</returns>
		public bool DepositGold(int amount)
		{
			return DepositCurrency(amount / Math.Max(1.0, CurrencyThreshold));
		}

		/// <summary>
		///     Attempts to deposit the given amount of Gold into this account.
		///     If the given amount is greater than the CurrencyThreshold,
		///     Platinum will be deposited to offset the difference.
		/// </summary>
		/// <param name="amount">Amount to deposit.</param>
		/// <returns>True if successful, false if amount given is less than or equal to zero.</returns>
		public bool DepositGold(long amount)
		{
			return DepositCurrency(amount / Math.Max(1.0, CurrencyThreshold));
		}

		/// <summary>
		///     Attempts to deposit the given amount of Platinum into this account.
		/// </summary>
		/// <param name="amount">Amount to deposit.</param>
		/// <returns>True if successful, false if amount given is less than or equal to zero.</returns>
		public bool DepositPlat(int amount)
		{
			return DepositCurrency(amount);
		}

		/// <summary>
		///     Attempts to deposit the given amount of Platinum into this account.
		/// </summary>
		/// <param name="amount">Amount to deposit.</param>
		/// <returns>True if successful, false if amount given is less than or equal to zero.</returns>
		public bool DepositPlat(long amount)
		{
			return DepositCurrency(amount);
		}

		/// <summary>
		///     Attempts to withdraw the given amount of Platinum and Gold from this account.
		/// </summary>
		/// <param name="amount">Amount to withdraw.</param>
		/// <returns>True if successful, false if balance was too low, or the amount given is less than zero.</returns>
		public bool WithdrawCurrency(double amount)
		{
			if (Double.IsNaN(amount) || Double.IsInfinity(amount))
			{
				return false;
			}

			if (amount < 0 || amount > TotalCurrency)
			{
				return false;
			}

			var oldAmount = TotalCurrency;
			var balance = Math.Max(0, oldAmount - amount);

			if (Double.IsNaN(balance) || Double.IsInfinity(balance))
			{
				return false;
			}

			TotalCurrency = balance;

			if (oldAmount != balance)
			{
				EventSink.InvokeAccountCurrencyChange(new AccountCurrencyChangeEventArgs(this, oldAmount, balance));
			}

			return true;
		}

		/// <summary>
		///     Attempts to withdraw the given amount of Gold from this account.
		///     If the given amount is greater than the CurrencyThreshold,
		///     Platinum will be withdrawn to offset the difference.
		/// </summary>
		/// <param name="amount">Amount to withdraw.</param>
		/// <returns>True if successful, false if balance was too low, or the amount given is less than zero.</returns>
		public bool WithdrawGold(int amount)
		{
			return WithdrawCurrency(amount / Math.Max(1.0, CurrencyThreshold));
		}

		/// <summary>
		///     Attempts to withdraw the given amount of Gold from this account.
		///     If the given amount is greater than the CurrencyThreshold,
		///     Platinum will be withdrawn to offset the difference.
		/// </summary>
		/// <param name="amount">Amount to withdraw.</param>
		/// <returns>True if successful, false if balance was too low, or the amount given is less than zero.</returns>
		public bool WithdrawGold(long amount)
		{
			return WithdrawCurrency(amount / Math.Max(1.0, CurrencyThreshold));
		}

		/// <summary>
		///     Attempts to withdraw the given amount of Platinum from this account.
		/// </summary>
		/// <param name="amount">Amount to withdraw.</param>
		/// <returns>True if successful, false if balance was too low, or the amount given is less than zero.</returns>
		public bool WithdrawPlat(int amount)
		{
			return WithdrawCurrency(amount);
		}

		/// <summary>
		///     Attempts to withdraw the given amount of Platinum from this account.
		/// </summary>
		/// <param name="amount">Amount to withdraw.</param>
		/// <returns>True if successful, false if balance was too low, or the amount given is less than zero.</returns>
		public bool WithdrawPlat(long amount)
		{
			return WithdrawCurrency(amount);
		}

		/// <summary>
		///     Gets the total balance of Gold for this account.
		/// </summary>
		/// <param name="gold">Gold value, Platinum exclusive</param>
		/// <param name="totalGold">Gold value, Platinum inclusive</param>
		public void GetGoldBalance(out int gold, out double totalGold)
		{
			gold = TotalGold;
			totalGold = TotalCurrency * Math.Max(1.0, CurrencyThreshold);
		}

		/// <summary>
		///     Gets the total balance of Gold for this account.
		/// </summary>
		/// <param name="gold">Gold value, Platinum exclusive</param>
		/// <param name="totalGold">Gold value, Platinum inclusive</param>
		public void GetGoldBalance(out long gold, out double totalGold)
		{
			gold = TotalGold;
			totalGold = TotalCurrency * Math.Max(1.0, CurrencyThreshold);
		}

		/// <summary>
		///     Gets the total balance of Platinum for this account.
		/// </summary>
		/// <param name="plat">Platinum value, Gold exclusive</param>
		/// <param name="totalPlat">Platinum value, Gold inclusive</param>
		public void GetPlatBalance(out int plat, out double totalPlat)
		{
			plat = TotalPlat;
			totalPlat = TotalCurrency;
		}

		/// <summary>
		///     Gets the total balance of Platinum for this account.
		/// </summary>
		/// <param name="plat">Platinum value, Gold exclusive</param>
		/// <param name="totalPlat">Platinum value, Gold inclusive</param>
		public void GetPlatBalance(out long plat, out double totalPlat)
		{
			plat = TotalPlat;
			totalPlat = TotalCurrency;
		}

		/// <summary>
		///     Gets the total balance of Gold and Platinum for this account.
		/// </summary>
		/// <param name="gold">Gold value, Platinum exclusive</param>
		/// <param name="totalGold">Gold value, Platinum inclusive</param>
		/// <param name="plat">Platinum value, Gold exclusive</param>
		/// <param name="totalPlat">Platinum value, Gold inclusive</param>
		public void GetBalance(out int gold, out double totalGold, out int plat, out double totalPlat)
		{
			GetGoldBalance(out gold, out totalGold);
			GetPlatBalance(out plat, out totalPlat);
		}

		/// <summary>
		///     Gets the total balance of Gold and Platinum for this account.
		/// </summary>
		/// <param name="gold">Gold value, Platinum exclusive</param>
		/// <param name="totalGold">Gold value, Platinum inclusive</param>
		/// <param name="plat">Platinum value, Gold exclusive</param>
		/// <param name="totalPlat">Platinum value, Gold inclusive</param>
		public void GetBalance(out long gold, out double totalGold, out long plat, out double totalPlat)
		{
			GetGoldBalance(out gold, out totalGold);
			GetPlatBalance(out plat, out totalPlat);
		}

		public bool HasGoldBalance(double amount)
		{
			GetGoldBalance(out int _, out var totalGold);

			return amount <= totalGold;
		}

		public bool HasPlatBalance(double amount)
		{
			GetPlatBalance(out int _, out var totalPlat);

			return amount <= totalPlat;
		}
		#endregion

		#region Secure Account

		[ConfigProperty("Accounts.MaxSecureAmount")]
		public static int MaxSecureAmount { get => Config.Get("Accounts.MaxSecureAmount", 100000000); set => Config.Set("Accounts.MaxSecureAmount", value); }

		public Dictionary<Mobile, int> SecureAccounts { get; private set; }

		public int GetSecureBalance(Mobile m)
		{
			if (m == null || m.Deleted || m.Account != this)
			{
				return 0;
			}

			if (SecureAccounts != null && SecureAccounts.TryGetValue(m, out var balance))
			{
				return balance;
			}

			return 0;
		}

		public void SetSecureBalance(Mobile m, int amount)
		{
			if (m == null || m.Deleted || m.Account != this)
			{
				return;
			}

			var oldAmount = 0;

			if (SecureAccounts != null)
			{
				SecureAccounts.TryGetValue(m, out oldAmount);
			}
			else if (amount <= 0)
			{
				return;
			}

			var balance = Math.Max(0, Math.Min(MaxSecureAmount, amount));

			if (balance > 0)
			{
				if (SecureAccounts == null)
				{
					SecureAccounts = new Dictionary<Mobile, int>();
				}

				SecureAccounts[m] = balance;
			}
			else
			{
				if (SecureAccounts == null || !SecureAccounts.Remove(m))
				{
					return;
				}

				if (SecureAccounts.Count == 0)
				{
					SecureAccounts = null;
				}
			}

			if (oldAmount != balance)
			{
				EventSink.InvokeAccountSecureChange(new AccountSecureChangeEventArgs(this, m, oldAmount, balance));
			}
		}

		public bool HasSecureBalance(Mobile m, int amount)
		{
			if (m == null || m.Deleted || m.Account != this)
			{
				return false;
			}

			if (SecureAccounts == null || !SecureAccounts.TryGetValue(m, out var balance))
			{
				return false;
			}

			return balance >= amount;
		}

		public bool DepositSecure(Mobile m, int amount)
		{
			if (m == null || m.Deleted || m.Account != this || amount < 0)
			{
				return false;
			}

			var oldAmount = 0;

			if (SecureAccounts != null)
			{
				SecureAccounts.TryGetValue(m, out oldAmount);
			}

			var balance = oldAmount + amount;

			if (balance > MaxSecureAmount)
			{
				return false;
			}

			if (balance > 0)
			{
				if (SecureAccounts == null)
				{
					SecureAccounts = new Dictionary<Mobile, int>();
				}

				SecureAccounts[m] = balance;
			}
			else
			{
				if (SecureAccounts == null || !SecureAccounts.Remove(m))
				{
					return false;
				}

				if (SecureAccounts.Count == 0)
				{
					SecureAccounts = null;
				}
			}

			if (oldAmount != balance)
			{
				EventSink.InvokeAccountSecureChange(new AccountSecureChangeEventArgs(this, m, oldAmount, balance));
			}

			return true;
		}

		public bool WithdrawSecure(Mobile m, int amount)
		{
			if (m == null || m.Deleted || m.Account != this || amount < 0)
			{
				return false;
			}

			if (SecureAccounts == null || !SecureAccounts.TryGetValue(m, out var oldAmount))
			{
				return false;
			}

			if (oldAmount < amount)
			{
				return false;
			}

			var balance = oldAmount - amount;

			if (balance <= 0)
			{
				SecureAccounts.Remove(m);

				if (SecureAccounts.Count == 0)
				{
					SecureAccounts = null;
				}
			}
			else
			{
				SecureAccounts[m] = balance;
			}

			if (oldAmount != balance)
			{
				EventSink.InvokeAccountSecureChange(new AccountSecureChangeEventArgs(this, m, oldAmount, balance));
			}

			return true;
		}

		#endregion

		#region IStoreAccount

		[CommandProperty(AccessLevel.Administrator, true)]
		public int Sovereigns { get; private set; }

		public void SetSovereigns(int amount)
		{
			Sovereigns = Math.Max(0, amount);
		}

		public bool DepositSovereigns(int amount)
		{
			if (amount <= 0)
			{
				return false;
			}

			Sovereigns += amount;
			return true;
		}

		public bool WithdrawSovereigns(int amount)
		{
			if (amount <= 0)
			{
				return true;
			}

			if (amount > Sovereigns)
			{
				return false;
			}

			Sovereigns -= amount;
			return true;
		}

		public bool HasSovereigns(int amount)
		{
			return Sovereigns >= amount;
		}
		
		#endregion
	}
}
