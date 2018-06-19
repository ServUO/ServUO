#region Header
// **********
// ServUO - Account.cs
// **********
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

using Server.Commands;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Multis;
using Server.Network;
#endregion

namespace Server.Accounting
{
	[PropertyObject]
	public class Account : IAccount, IComparable, IComparable<Account>
	{
		public static readonly TimeSpan YoungDuration = TimeSpan.FromHours(40.0);
		public static readonly TimeSpan InactiveDuration = TimeSpan.FromDays(180.0);
		public static readonly TimeSpan EmptyInactiveDuration = TimeSpan.FromDays(30.0);

		private static MD5CryptoServiceProvider m_MD5HashProvider;
		private static SHA1CryptoServiceProvider m_SHA1HashProvider;
        private static SHA512CryptoServiceProvider m_SHA512HashProvider;
        private static byte[] m_HashBuffer;

		public static void Configure()
		{
			CommandSystem.Register("ConvertCurrency", AccessLevel.Owner, ConvertCurrency);
		}

		private static void ConvertCurrency(CommandEventArgs e)
		{
			e.Mobile.SendMessage(
				"Converting All Banked Gold from {0} to {1}.  Please wait...",
				AccountGold.Enabled ? "checks and coins" : "account treasury",
				AccountGold.Enabled ? "account treasury" : "checks and coins");

			NetState.Pause();

			double found = 0.0, converted = 0.0;

			try
			{
				BankBox box;
				List<Gold> gold;
				List<BankCheck> checks;
				long share = 0, shared;
				int diff;

				foreach (var a in Accounts.GetAccounts().OfType<Account>().Where(a => a.Count > 0))
				{
					try
					{
						if (!AccountGold.Enabled)
						{
							share = (int)Math.Truncate((a.TotalCurrency / a.Count) * CurrencyThreshold);
							found += a.TotalCurrency * CurrencyThreshold;
						}

						foreach (var m in a.m_Mobiles.Where(m => m != null))
						{
							box = m.FindBankNoCreate();

							if (box == null)
							{
								continue;
							}

							if (AccountGold.Enabled)
							{
								foreach (var o in checks = box.FindItemsByType<BankCheck>())
								{
									found += o.Worth;

									if (!a.DepositGold(o.Worth))
									{
										break;
									}

									converted += o.Worth;
									o.Delete();
								}

								checks.Clear();
								checks.TrimExcess();

								foreach (var o in gold = box.FindItemsByType<Gold>())
								{
									found += o.Amount;

									if (!a.DepositGold(o.Amount))
									{
										break;
									}

									converted += o.Amount;
									o.Delete();
								}

								gold.Clear();
								gold.TrimExcess();
							}
							else
							{
								shared = share;

								while (shared > 0)
								{
									if (shared > 60000)
									{
										diff = (int)Math.Min(10000000, shared);

										if (a.WithdrawGold(diff))
										{
											box.DropItem(new BankCheck(diff));
										}
										else
										{
											break;
										}
									}
									else
									{
										diff = (int)Math.Min(60000, shared);

										if (a.WithdrawGold(diff))
										{
											box.DropItem(new Gold(diff));
										}
										else
										{
											break;
										}
									}

									converted += diff;
									shared -= diff;
								}
							}

							box.UpdateTotals();
						}
					}
					catch
					{ }
				}
			}
			catch
			{ }

			NetState.Resume();

			e.Mobile.SendMessage("Operation complete: {0:#,0} of {1:#,0} Gold has been converted in total.", converted, found);
		}

		private readonly DateTime m_Created;
		private readonly Mobile[] m_Mobiles;

		private AccessLevel m_AccessLevel;
		private List<AccountComment> m_Comments;
		private List<AccountTag> m_Tags;
		private TimeSpan m_TotalGameTime;
		private Timer m_YoungTimer;

		public Account(string username, string password)
		{
			Username = username;

			SetPassword(password);

			m_AccessLevel = AccessLevel.Player;

			m_Created = LastLogin = DateTime.UtcNow;
			m_TotalGameTime = TimeSpan.Zero;

			m_Mobiles = new Mobile[7];

			IPRestrictions = new string[0];
			LoginIPs = new IPAddress[0];

			Accounts.Add(this);
		}

		public Account(XmlElement node)
		{
			Username = Utility.GetText(node["username"], "empty");

			var plainPassword = Utility.GetText(node["password"], null);
			var MD5Password = Utility.GetText(node["cryptPassword"], null);
			var SHA1Password = Utility.GetText(node["newCryptPassword"], null);
            var SHA512Password = Utility.GetText(node["newSecureCryptPassword"], null);

            switch (AccountHandler.ProtectPasswords)
			{
				case PasswordProtection.None:
				{
					if (plainPassword != null)
					{
						SetPassword(plainPassword);
					}
					else if (SHA512Password != null)
					{
						_SHA512Password = SHA512Password;
					}
                    else if (SHA1Password != null)
                    {
                        _SHA1Password = SHA1Password;
                    }
                    else if (MD5Password != null)
					{
						_MD5Password = MD5Password;
					}
					else
					{
						SetPassword("empty");
					}

					break;
				}
				case PasswordProtection.Crypt:
				{
					if (MD5Password != null)
					{
						_MD5Password = MD5Password;
					}
					else if (plainPassword != null)
					{
						SetPassword(plainPassword);
					}
					else if (SHA1Password != null)
					{
						_SHA1Password = SHA1Password;
					}
                    else if (SHA512Password != null)
                    {
                        _SHA512Password = SHA512Password;
                    }
                    else
					{
						SetPassword("empty");
					}

					break;
				}
                case PasswordProtection.NewCrypt:
                {
                    if (SHA1Password != null)
                    {
                    _SHA1Password = SHA1Password;
                    }
                    else if (plainPassword != null)
                    {
                        SetPassword(plainPassword);
                    }
                    else if (MD5Password != null)
                    {
                        _MD5Password = MD5Password;
                    }
                    else if (SHA512Password != null)
                    {
                        _SHA512Password = SHA512Password;
                    }
                    else
                    {
                        SetPassword("empty");
                    }

                    break;
                }
                default: // PasswordProtection.NewSecureCrypt
                {
                    if (SHA512Password != null)
                    {
                        _SHA512Password = SHA512Password;
                    }
					else if (plainPassword != null)
					{
						SetPassword(plainPassword);
					}
                    else if (SHA1Password != null)
					{
						_SHA1Password = SHA1Password;
					}
					else if (MD5Password != null)
					{
						_MD5Password = MD5Password;
					}
					else
					{
						SetPassword("empty");
					}

					break;
				}
			}

			Enum.TryParse(Utility.GetText(node["accessLevel"], "Player"), true, out m_AccessLevel);

			Flags = Utility.GetXMLInt32(Utility.GetText(node["flags"], "0"), 0);
			m_Created = Utility.GetXMLDateTime(Utility.GetText(node["created"], null), DateTime.UtcNow);
			LastLogin = Utility.GetXMLDateTime(Utility.GetText(node["lastLogin"], null), DateTime.UtcNow);

			TotalCurrency = Utility.GetXMLDouble(Utility.GetText(node["totalCurrency"], "0"), 0);
            Sovereigns = Utility.GetXMLInt32(Utility.GetText(node["sovereigns"], "0"), 0);

			m_Mobiles = LoadMobiles(node);
			m_Comments = LoadComments(node);
			m_Tags = LoadTags(node);
			LoginIPs = LoadAddressList(node);
			IPRestrictions = LoadAccessCheck(node);

			foreach (Mobile m in m_Mobiles.Where(m => m != null))
			{
				m.Account = this;
			}

			var totalGameTime = Utility.GetXMLTimeSpan(Utility.GetText(node["totalGameTime"], null), TimeSpan.Zero);

			if (totalGameTime == TimeSpan.Zero)
			{
				totalGameTime = m_Mobiles.OfType<PlayerMobile>().Aggregate(totalGameTime, (current, m) => current + m.GameTime);
			}

			m_TotalGameTime = totalGameTime;

			if (Young)
			{
				CheckYoung();
			}

            LoadSecureAccounts(node);

			Accounts.Add(this);
		}

        /// <summary>
        /// Deserializes a list of secure account balances, and converts it to a dictionary containing the account characters
        /// </summary>
        /// <param name="node"></param>
        public void LoadSecureAccounts(XmlElement node)
        {
            int[] list = new int[7];
            XmlElement chars = node["SecureAccounts"];

            if (chars != null)
            {
                foreach (XmlElement ele in chars.GetElementsByTagName("char"))
                {
                    try
                    {
                        int index = Utility.GetXMLInt32(Utility.GetAttribute(ele, "index", "0"), 0);
                        int balance = Utility.GetXMLInt32(Utility.GetText(ele, "0"), 0);

                        if (balance > 0 && index >= 0 && index < list.Length && index < m_Mobiles.Length)
                        {
                            if (SecureAccounts == null)
                                SecureAccounts = new Dictionary<Mobile, int>();

                            SecureAccounts[m_Mobiles[index]] = balance;
                        }
                    }
                    catch (Exception ex)
                    {
                        Utility.PushColor(ConsoleColor.Red);
                        Console.WriteLine("Writing Secure Account Exception: {0}", ex);
                        Utility.PopColor();
                    }
                }
            }
        }

		/// <summary>
		///     Object detailing information about the hardware of the last person to log into this account
		/// </summary>
		[CommandProperty(AccessLevel.Administrator)]
		public HardwareInfo HardwareInfo { get; set; }

		/// <summary>
		///     List of IP addresses for restricted access. '*' wildcard supported. If the array contains zero entries, all IP
		///     addresses are allowed.
		/// </summary>
		public string[] IPRestrictions { get; set; }

		/// <summary>
		///     List of IP addresses which have successfully logged into this account.
		/// </summary>
		public IPAddress[] LoginIPs { get; set; }

		/// <summary>
		///     List of account comments. Type of contained objects is AccountComment.
		/// </summary>
		public List<AccountComment> Comments
		{
			get { return m_Comments ?? (m_Comments = new List<AccountComment>()); }
		}

		/// <summary>
		///     List of account tags. Type of contained objects is AccountTag.
		/// </summary>
		public List<AccountTag> Tags
		{
			get { return m_Tags ?? (m_Tags = new List<AccountTag>()); }
		}

		/// <summary>
		///     Account password. Plain text. Case sensitive validation. May be null.
		/// </summary>
		public string PlainPassword { get; set; }

		/// <summary>
		///     Account password. Hashed with MD5. May be null.
		/// </summary>
		public string _MD5Password { get; set; }

		/// <summary>
		///     Account username and password hashed with SHA1. May be null.
		/// </summary>
		public string _SHA1Password { get; set; }

        /// <summary>
        ///     Account username and password hashed with SHA512. May be null.
        /// </summary>
        public string _SHA512Password { get; set; }

        /// <summary>
        ///     Internal bitfield of account flags. Consider using direct access properties (Banned, Young), or GetFlag/SetFlag
        ///     methods
        /// </summary>
        public int Flags { get; set; }

		/// <summary>
		///     Gets or sets a flag indiciating if this account is banned.
		/// </summary>
		[CommandProperty(AccessLevel.Administrator)]
		public bool Banned
		{
			get
			{
				var isBanned = GetFlag(0);

				if (!isBanned)
				{
					return false;
				}

				DateTime banTime;
				TimeSpan banDuration;

				if (!GetBanTags(out banTime, out banDuration) || banDuration == TimeSpan.MaxValue ||
					DateTime.UtcNow < (banTime + banDuration))
				{
					return true;
				}

				SetUnspecifiedBan(null); // clear
				Banned = false;
				return false;
			}
			set { SetFlag(0, value); }
		}

		/// <summary>
		///     Gets or sets a flag indicating if the characters created on this account will have the young status.
		/// </summary>
		[CommandProperty(AccessLevel.Administrator)]
		public bool Young
		{
			get { return !GetFlag(1); }
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
		[CommandProperty(AccessLevel.Administrator)]
		public DateTime Created { get { return m_Created; } }

		/// <summary>
		///     Gets or sets the date and time when this account was last accessed.
		/// </summary>
		[CommandProperty(AccessLevel.Administrator)]
		public DateTime LastLogin { get; set; }

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

		/// <summary>
		///     Gets the total game time of this account, also considering the game time of characters
		///     that have been deleted.
		/// </summary>
		[CommandProperty(AccessLevel.Administrator)]
		public TimeSpan TotalGameTime
		{
			get
			{
				foreach (var m in m_Mobiles.OfType<PlayerMobile>().Where(m => m.NetState != null))
				{
					return m_TotalGameTime + (DateTime.UtcNow - m.SessionStart);
				}

				return m_TotalGameTime;
			}
		}

		/// <summary>
		///     Account username. Case insensitive validation.
		/// </summary>
		[CommandProperty(AccessLevel.Administrator, true)]
		public string Username { get; set; }

		/// <summary>
		///     Account email address. Case insensitive validation.
		/// </summary>
		[CommandProperty(AccessLevel.Administrator, true)]
		public string Email { get; set; }

		/// <summary>
		///     Initial AccessLevel for new characters created on this account.
		/// </summary>
		[CommandProperty(AccessLevel.Administrator, AccessLevel.Owner)]
		public AccessLevel AccessLevel { get { return m_AccessLevel; } set { m_AccessLevel = value; } }

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
					if (this[i] != null)
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
		public int Limit { get { return (Siege.SiegeShard ? Siege.CharacterSlots : Core.SA ? 7 : Core.AOS ? 6 : 5); } }

		/// <summary>
		///     Gets the maxmimum amount of characters that this account can hold.
		/// </summary>
		[CommandProperty(AccessLevel.Administrator)]
		public int Length { get { return m_Mobiles.Length; } }

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

		/// <summary>
		///     Deletes the account, all characters of the account, and all houses of those characters
		/// </summary>
		public void Delete()
		{
			for (var i = 0; i < Length; ++i)
			{
				var m = this[i];

				if (m == null)
				{
					continue;
				}

				var list = BaseHouse.GetHouses(m);

				foreach (BaseHouse h in list)
				{
					h.Delete();
				}

				m.Delete();

				m.Account = null;
				m_Mobiles[i] = null;
			}

			if (LoginIPs.Length != 0 && AccountHandler.IPTable.ContainsKey(LoginIPs[0]))
			{
				--AccountHandler.IPTable[LoginIPs[0]];
			}

			Accounts.Remove(Username);
		}

		public void SetPassword(string plainPassword)
		{
			switch (AccountHandler.ProtectPasswords)
			{
				case PasswordProtection.None:
				{
					PlainPassword = plainPassword;
					_MD5Password = null;
					_SHA1Password = null;
					_SHA512Password = null;
				}
					break;
				case PasswordProtection.Crypt:
				{
					PlainPassword = null;
					_MD5Password = HashMD5(plainPassword);
					_SHA1Password = null;
					_SHA512Password = null;
				}
					break;
                case PasswordProtection.NewCrypt:
                {
                    PlainPassword = null;
                    _MD5Password = null;
                    _SHA1Password = HashSHA1(Username + plainPassword);
					_SHA512Password = null;
                }
                    break;
                default: // PasswordProtection.NewSecureCrypt
				{
					PlainPassword = null;
					_MD5Password = null;
					_SHA1Password = null;
                    _SHA512Password = HashSHA512(Username + plainPassword); 
				}
					break;
			}
		}

		public bool CheckPassword(string plainPassword)
		{
			bool ok;
			PasswordProtection curProt;

			if (PlainPassword != null)
			{
				ok = (PlainPassword == plainPassword);
				curProt = PasswordProtection.None;
			}
			else if (_MD5Password != null)
			{
				ok = (_MD5Password == HashMD5(plainPassword));
				curProt = PasswordProtection.Crypt;
			}
			else if (_SHA1Password != null)
            {
				ok = (_SHA1Password == HashSHA1(Username + plainPassword));
				curProt = PasswordProtection.NewCrypt;
			}
            else
            {
                ok = (_SHA512Password == HashSHA512(Username + plainPassword));
                curProt = PasswordProtection.NewSecureCrypt;
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

			return String.Compare(Username, other.Username, StringComparison.Ordinal);
		}

		public int CompareTo(object obj)
		{
			if (obj is Account)
			{
				return CompareTo((Account)obj);
			}

			throw new ArgumentException();
		}

		public int CompareTo(Account other)
		{
			if (other == null)
			{
				return 1;
			}

			return String.Compare(Username, other.Username, StringComparison.Ordinal);
		}

		public static string HashMD5(string phrase)
		{
			if (m_MD5HashProvider == null)
			{
				m_MD5HashProvider = new MD5CryptoServiceProvider();
			}

			if (m_HashBuffer == null)
			{
				m_HashBuffer = new byte[256];
			}

			var length = Encoding.ASCII.GetBytes(phrase, 0, phrase.Length > 256 ? 256 : phrase.Length, m_HashBuffer, 0);
			var hashed = m_MD5HashProvider.ComputeHash(m_HashBuffer, 0, length);

			return BitConverter.ToString(hashed);
		}

		public static string HashSHA1(string phrase)
		{
			if (m_SHA1HashProvider == null)
			{
				m_SHA1HashProvider = new SHA1CryptoServiceProvider();
			}

			if (m_HashBuffer == null)
			{
				m_HashBuffer = new byte[256];
			}

			var length = Encoding.ASCII.GetBytes(phrase, 0, phrase.Length > 256 ? 256 : phrase.Length, m_HashBuffer, 0);
			var hashed = m_SHA1HashProvider.ComputeHash(m_HashBuffer, 0, length);

			return BitConverter.ToString(hashed);
		}
        public static string HashSHA512(string phrase)
        {
            if (m_SHA512HashProvider == null)
            {
                m_SHA512HashProvider = new SHA512CryptoServiceProvider();
            }

            if (m_HashBuffer == null)
            {
                m_HashBuffer = new byte[256];
            }

            var length = Encoding.ASCII.GetBytes(phrase, 0, phrase.Length > 256 ? 256 : phrase.Length, m_HashBuffer, 0);
            var hashed = m_SHA512HashProvider.ComputeHash(m_HashBuffer, 0, length);

            return BitConverter.ToString(hashed);
        }

        public static void Initialize()
		{
			EventSink.Connected += EventSink_Connected;
			EventSink.Disconnected += EventSink_Disconnected;
			EventSink.Login += EventSink_Login;
		}

		/// <summary>
		///     Deserializes a list of string values from an xml element. Null values are not added to the list.
		/// </summary>
		/// <param name="node">The XmlElement from which to deserialize.</param>
		/// <returns>String list. Value will never be null.</returns>
		public static string[] LoadAccessCheck(XmlElement node)
		{
			string[] stringList;
			var accessCheck = node["accessCheck"];

			if (accessCheck != null)
			{
				stringList =
					accessCheck.GetElementsByTagName("ip")
							   .Cast<XmlElement>()
							   .Select(ip => Utility.GetText(ip, null))
							   .Where(text => text != null)
							   .ToArray();
			}
			else
			{
				stringList = new string[0];
			}

			return stringList;
		}

		/// <summary>
		///     Deserializes a list of IPAddress values from an xml element.
		/// </summary>
		/// <param name="node">The XmlElement from which to deserialize.</param>
		/// <returns>Address list. Value will never be null.</returns>
		public static IPAddress[] LoadAddressList(XmlElement node)
		{
			IPAddress[] list;
			var addressList = node["addressList"];

			if (addressList != null)
			{
				var count = Utility.GetXMLInt32(Utility.GetAttribute(addressList, "count", "0"), 0);

				list = new IPAddress[count];

				count = 0;

				foreach (XmlElement ip in addressList.GetElementsByTagName("ip").Cast<XmlElement>().Where(ip => count < list.Length))
				{
					IPAddress address;

					if (!IPAddress.TryParse(Utility.GetText(ip, null), out address))
					{
						continue;
					}

					list[count] = Utility.Intern(address);
					count++;
				}

				if (count == list.Length)
				{
					return list;
				}

				var old = list;
				list = new IPAddress[count];

				for (var i = 0; i < count && i < old.Length; ++i)
				{
					list[i] = old[i];
				}
			}
			else
			{
				list = new IPAddress[0];
			}

			return list;
		}

		/// <summary>
		///     Deserializes a list of Mobile instances from an xml element.
		/// </summary>
		/// <param name="node">The XmlElement instance from which to deserialize.</param>
		/// <returns>Mobile list. Value will never be null.</returns>
		public static Mobile[] LoadMobiles(XmlElement node)
		{
			var list = new Mobile[7];
			var chars = node["chars"];

			//int length = Accounts.GetInt32( Accounts.GetAttribute( chars, "length", "6" ), 6 );
			//list = new Mobile[length];
			//Above is legacy, no longer used

			if (chars == null)
			{
				return list;
			}

			foreach (XmlElement ele in chars.GetElementsByTagName("char"))
			{
				try
				{
					var index = Utility.GetXMLInt32(Utility.GetAttribute(ele, "index", "0"), 0);
					var serial = Utility.GetXMLInt32(Utility.GetText(ele, "0"), 0);

					if (index >= 0 && index < list.Length)
					{
						list[index] = World.FindMobile(serial);
					}
				}
				catch
				{ }
			}

			return list;
		}

		/// <summary>
		///     Deserializes a list of AccountComment instances from an xml element.
		/// </summary>
		/// <param name="node">The XmlElement from which to deserialize.</param>
		/// <returns>Comment list. Value will never be null.</returns>
		public static List<AccountComment> LoadComments(XmlElement node)
		{
			var comments = node["comments"];

			if (comments == null)
			{
				return null;
			}

			var list = new List<AccountComment>();

			foreach (XmlElement comment in comments.GetElementsByTagName("comment"))
			{
				try
				{
					list.Add(new AccountComment(comment));
				}
				catch
				{ }
			}

			return list;
		}

		/// <summary>
		///     Deserializes a list of AccountTag instances from an xml element.
		/// </summary>
		/// <param name="node">The XmlElement from which to deserialize.</param>
		/// <returns>Tag list. Value will never be null.</returns>
		public static List<AccountTag> LoadTags(XmlElement node)
		{
			var tags = node["tags"];

			if (tags == null)
			{
				return null;
			}

			var list = new List<AccountTag>();

			foreach (XmlElement tag in tags.GetElementsByTagName("tag"))
			{
				try
				{
					list.Add(new AccountTag(tag));
				}
				catch
				{ }
			}

			return list;
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
		///     Adds a new tag to this account. This method does not check for duplicate names.
		/// </summary>
		/// <param name="name">New tag name.</param>
		/// <param name="value">New tag value.</param>
		public void AddTag(string name, string value)
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
			foreach (var tag in Tags.Where(tag => tag.Name == name))
			{
				tag.Value = value;
				return;
			}

			AddTag(name, value);
		}

		/// <summary>
		///     Gets the value of a tag -or- null if there are no tags with the specified name.
		/// </summary>
		/// <param name="name">Name of the desired tag value.</param>
		public string GetTag(string name)
		{
			return Tags.Where(tag => tag.Name == name).Select(tag => tag.Value).FirstOrDefault();
		}

		public void SetUnspecifiedBan(Mobile from)
		{
			SetBanTags(from, DateTime.MinValue, TimeSpan.Zero);
		}

		public void SetBanTags(Mobile from, DateTime banTime, TimeSpan banDuration)
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

		public bool GetBanTags(out DateTime banTime, out TimeSpan banDuration)
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

		public void RemoveYoungStatus(int message)
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

				m.SendLocalizedMessage(1019039);
				// You are no longer considered a young player of Ultima Online, and are no longer subject to the limitations and benefits of being in that caste.
			}
		}

		public void CheckYoung()
		{
			if (TotalGameTime >= YoungDuration)
			{
				RemoveYoungStatus(1019038);
				// You are old enough to be considered an adult, and have outgrown your status as a young player!
			}
		}

		/// <summary>
		///     Checks if a specific NetState is allowed access to this account.
		/// </summary>
		/// <param name="ns">NetState instance to check.</param>
		/// <returns>True if allowed, false if not.</returns>
		public bool HasAccess(NetState ns)
		{
			return (ns != null && HasAccess(ns.Address));
		}

		public bool HasAccess(IPAddress ipAddress)
		{
			var level = AccountHandler.LockdownLevel;

			if (level >= AccessLevel.Counselor)
			{
				var hasAccess = false;

				if (m_AccessLevel >= level)
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
		public void LogAccess(NetState ns)
		{
			if (ns != null)
			{
				LogAccess(ns.Address);
			}
		}

		public void LogAccess(IPAddress ipAddress)
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
		public bool CheckAccess(NetState ns)
		{
			return (ns != null && CheckAccess(ns.Address));
		}

		public bool CheckAccess(IPAddress ipAddress)
		{
			var hasAccess = HasAccess(ipAddress);

			if (hasAccess)
			{
				LogAccess(ipAddress);
			}

			return hasAccess;
		}

		/// <summary>
		///     Serializes this Account instance to an XmlTextWriter.
		/// </summary>
		/// <param name="xml">The XmlTextWriter instance from which to serialize.</param>
		public void Save(XmlTextWriter xml)
		{
			xml.WriteStartElement("account");

			xml.WriteStartElement("username");
			xml.WriteString(Username);
			xml.WriteEndElement();

			if (PlainPassword != null)
			{
				xml.WriteStartElement("password");
				xml.WriteString(PlainPassword);
				xml.WriteEndElement();
			}

			if (_MD5Password != null)
			{
				xml.WriteStartElement("cryptPassword");
				xml.WriteString(_MD5Password);
				xml.WriteEndElement();
			}

			if (_SHA1Password != null)
			{
				xml.WriteStartElement("newCryptPassword");
				xml.WriteString(_SHA1Password);
				xml.WriteEndElement();
			}

            if (_SHA512Password != null)
            {
                xml.WriteStartElement("newSecureCryptPassword");
                xml.WriteString(_SHA512Password);
                xml.WriteEndElement();
            }

            if (m_AccessLevel >= AccessLevel.Counselor)
			{
				xml.WriteStartElement("accessLevel");
				xml.WriteString(m_AccessLevel.ToString());
				xml.WriteEndElement();
			}

			if (Flags != 0)
			{
				xml.WriteStartElement("flags");
				xml.WriteString(XmlConvert.ToString(Flags));
				xml.WriteEndElement();
			}

			xml.WriteStartElement("created");
			xml.WriteString(XmlConvert.ToString(m_Created, XmlDateTimeSerializationMode.Utc));
			xml.WriteEndElement();

			xml.WriteStartElement("lastLogin");
			xml.WriteString(XmlConvert.ToString(LastLogin, XmlDateTimeSerializationMode.Utc));
			xml.WriteEndElement();

			xml.WriteStartElement("totalGameTime");
			xml.WriteString(XmlConvert.ToString(TotalGameTime));
			xml.WriteEndElement();

			xml.WriteStartElement("chars");

			for (var i = 0; i < m_Mobiles.Length; ++i)
			{
				var m = m_Mobiles[i];

				if (m != null && !m.Deleted)
				{
					xml.WriteStartElement("char");
					xml.WriteAttributeString("index", i.ToString());
					xml.WriteString(m.Serial.Value.ToString());
					xml.WriteEndElement();
				}
			}

			xml.WriteEndElement();

			if (m_Comments != null && m_Comments.Count > 0)
			{
				xml.WriteStartElement("comments");

				foreach (AccountComment c in m_Comments)
				{
					c.Save(xml);
				}

				xml.WriteEndElement();
			}

			if (m_Tags != null && m_Tags.Count > 0)
			{
				xml.WriteStartElement("tags");

				foreach (AccountTag t in m_Tags)
				{
					t.Save(xml);
				}

				xml.WriteEndElement();
			}

			if (LoginIPs.Length > 0)
			{
				xml.WriteStartElement("addressList");

				xml.WriteAttributeString("count", LoginIPs.Length.ToString());

				foreach (IPAddress ip in LoginIPs)
				{
					xml.WriteStartElement("ip");
					xml.WriteString(ip.ToString());
					xml.WriteEndElement();
				}

				xml.WriteEndElement();
			}

			if (IPRestrictions.Length > 0)
			{
				xml.WriteStartElement("accessCheck");

				foreach (string ip in IPRestrictions)
				{
					xml.WriteStartElement("ip");
					xml.WriteString(ip);
					xml.WriteEndElement();
				}

				xml.WriteEndElement();
			}

			xml.WriteStartElement("totalCurrency");
			xml.WriteString(XmlConvert.ToString(TotalCurrency));
			xml.WriteEndElement();

            xml.WriteStartElement("sovereigns");
            xml.WriteString(XmlConvert.ToString(Sovereigns));
            xml.WriteEndElement();

            if (SecureAccounts != null)
            {
                xml.WriteStartElement("SecureAccounts");

                for (int i = 0; i < m_Mobiles.Length; ++i)
                {
                    Mobile m = m_Mobiles[i];
                    int balance = GetSecureAccountAmount(m);

                    if (m != null && !m.Deleted && balance > 0)
                    {
                        xml.WriteStartElement("char");
                        xml.WriteAttributeString("index", i.ToString());
                        xml.WriteString(balance.ToString());
                        xml.WriteEndElement();
                    }
                }

                xml.WriteEndElement();
            }

			xml.WriteEndElement();
		}

		public override string ToString()
		{
			return Username;
		}

		private static void EventSink_Connected(ConnectedEventArgs e)
		{
			var acc = e.Mobile.Account as Account;

			if (acc == null)
			{
				return;
			}

			if (!acc.Young || acc.m_YoungTimer != null)
			{
				return;
			}

			acc.m_YoungTimer = new YoungTimer(acc);
			acc.m_YoungTimer.Start();
		}

		private static void EventSink_Disconnected(DisconnectedEventArgs e)
		{
			var acc = e.Mobile.Account as Account;

			if (acc == null)
			{
				return;
			}

			if (acc.m_YoungTimer != null)
			{
				acc.m_YoungTimer.Stop();
				acc.m_YoungTimer = null;
			}

			var m = e.Mobile as PlayerMobile;

			if (m != null)
			{
				acc.m_TotalGameTime += DateTime.UtcNow - m.SessionStart;
			}
		}

		private static void EventSink_Login(LoginEventArgs e)
		{
			var m = e.Mobile as PlayerMobile;

			if (m == null)
			{
				return;
			}

			var acc = m.Account as Account;

			if (acc == null)
			{
				return;
			}

			if (!m.Young || !acc.Young)
			{
				return;
			}

			var ts = YoungDuration - acc.TotalGameTime;
			var hours = Math.Max((int)ts.TotalHours, 0);

			m.SendAsciiMessage(
				"You will enjoy the benefits and relatively safe status of a young player for {0} more hour{1}.",
				hours,
				hours != 1 ? "s" : "");
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
		public static int CurrencyThreshold
		{
			get { return AccountGold.CurrencyThreshold; }
			set { AccountGold.CurrencyThreshold = value; }
		}

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
		public int TotalGold
		{
			get { return (int)Math.Floor((TotalCurrency - Math.Truncate(TotalCurrency)) * Math.Max(1.0, CurrencyThreshold)); }
		}

		/// <summary>
		///     This amount represents the current amount of Platinum owned by the player.
		///     The value does not include the value of Gold and ranges from
		///     0 to 2,147,483,647 by default.
		///     One Platinum represents the value of CurrencyThreshold in Gold.
		/// </summary>
		[CommandProperty(AccessLevel.Administrator)]
		public int TotalPlat { get { return (int)Math.Truncate(TotalCurrency); } }

		/// <summary>
		///     Attempts to deposit the given amount of Gold and Platinum into this account.
		/// </summary>
		/// <param name="amount">Amount to deposit.</param>
		/// <returns>True if successful, false if amount given is less than or equal to zero.</returns>
		public bool DepositCurrency(double amount)
		{
			if (amount <= 0)
			{
				return false;
			}

			TotalCurrency += amount;
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
		/// <returns>True if successful, false if balance was too low.</returns>
		public bool WithdrawCurrency(double amount)
		{
			if (amount <= 0)
			{
				return true;
			}

			if (amount > TotalCurrency)
			{
				return false;
			}

			TotalCurrency -= amount;
			return true;
		}

		/// <summary>
		///     Attempts to withdraw the given amount of Gold from this account.
		///     If the given amount is greater than the CurrencyThreshold,
		///     Platinum will be withdrawn to offset the difference.
		/// </summary>
		/// <param name="amount">Amount to withdraw.</param>
		/// <returns>True if successful, false if balance was too low.</returns>
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
		/// <returns>True if successful, false if balance was too low.</returns>
		public bool WithdrawGold(long amount)
		{
			return WithdrawCurrency(amount / Math.Max(1.0, CurrencyThreshold));
		}

		/// <summary>
		///     Attempts to withdraw the given amount of Platinum from this account.
		/// </summary>
		/// <param name="amount">Amount to withdraw.</param>
		/// <returns>True if successful, false if balance was too low.</returns>
		public bool WithdrawPlat(int amount)
		{
			return WithdrawCurrency(amount);
		}

		/// <summary>
		///     Attempts to withdraw the given amount of Platinum from this account.
		/// </summary>
		/// <param name="amount">Amount to withdraw.</param>
		/// <returns>True if successful, false if balance was too low.</returns>
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

        #region Secure Account
        public Dictionary<Mobile, int> SecureAccounts;

        public static readonly int MaxSecureAmount = 100000000;

        public int GetSecureAccountAmount(Mobile m)
        {
            for(int i = 0; i < this.Length; i++)
            {
                Mobile mob = m_Mobiles[i];

                if (mob == null)
                    continue;

                if (mob == m)
                {
                    if (SecureAccounts != null && SecureAccounts.ContainsKey(m))
                        return SecureAccounts[m];
                }
            }

            return 0;
        }

        public bool DepositToSecure(Mobile m, int amount)
        {
            for (int i = 0; i < this.Length; i++)
            {
                Mobile mob = m_Mobiles[i];

                if (mob == null)
                    continue;

                if (mob == m)
                {
                    if (SecureAccounts == null)
                        SecureAccounts = new Dictionary<Mobile, int>();

                    if (!SecureAccounts.ContainsKey(m))
                        SecureAccounts[m] = Math.Min(MaxSecureAmount, amount);
                    else
                        SecureAccounts[m] = Math.Min(MaxSecureAmount, SecureAccounts[m] + amount);

                    return true;
                }
            }

            return false;
        }

        public bool WithdrawFromSecure(Mobile m, int amount)
        {
            for (int i = 0; i < this.Length; i++)
            {
                Mobile mob = m_Mobiles[i];

                if (mob == null)
                    continue;

                if (m == mob)
                {
                    if (SecureAccounts == null || !SecureAccounts.ContainsKey(m) || SecureAccounts[m] < amount)
                        return false;

                    SecureAccounts[m] -= amount;

                    return true;
                }
            }

            return false;
        }
        #endregion
        #endregion

        #region Sovereigns
        /// <summary>
        ///     Sovereigns which can be used at the shard owners disposal. On EA, they are used for curerncy with the Ultima Store
        /// </summary>
        [CommandProperty(AccessLevel.Administrator, true)]
        public int Sovereigns { get; private set; }

        public void SetSovereigns(int amount)
        {
            Sovereigns = amount;
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
        #endregion
    }
}
