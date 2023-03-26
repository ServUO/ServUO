using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

using Server.Accounting;
using Server.Commands;
using Server.Engines.Help;
using Server.Network;
using Server.Regions;

namespace Server.Misc
{
	public enum PasswordProtection : sbyte
	{
		None,
		MD5,
		SHA1,
		SHA512
	}

	public class AccountHandler
	{
		[ConfigProperty("Accounts.ProtectPasswords")]
		public static PasswordProtection ProtectPasswords { get => Config.GetEnum("Accounts.ProtectPasswords", PasswordProtection.SHA512); set => Config.SetEnum("Accounts.ProtectPasswords", value); }

		[ConfigProperty("Accounts.AccountsPerIp")]
		public static int MaxAccountsPerIP { get => Config.Get("Accounts.AccountsPerIp", 1); set => Config.Set("Accounts.AccountsPerIp", value); }

		[ConfigProperty("Accounts.AutoCreateAccounts")]
		public static bool AutoAccountCreation { get => Config.Get("Accounts.AutoCreateAccounts", true); set => Config.Set("Accounts.AutoCreateAccounts", value); }

		[ConfigProperty("Accounts.RestrictDeletion")]
		public static bool RestrictDeletion { get => Config.Get("Accounts.RestrictDeletion", !TestCenter.Enabled); set => Config.Set("Accounts.RestrictDeletion", value); }

		[ConfigProperty("Accounts.DeleteDelay")]
		public static TimeSpan DeleteDelay { get => Config.Get("Accounts.DeleteDelay", TimeSpan.FromDays(7.0)); set => Config.Set("Accounts.DeleteDelay", value); }

		[ConfigProperty("Accounts.PasswordCommandEnabled")]
		public static bool PasswordCommandEnabled { get => Config.Get("Accounts.PasswordCommandEnabled", false); set => Config.Set("Accounts.PasswordCommandEnabled", value); }

		public static CityInfo[] StartingCities = new CityInfo[]
		{
			new CityInfo("New Haven",   "New Haven Bank",   1150168, 3503,  2574,   14),
			new CityInfo("Yew", "The Empath Abbey", 1075072, 633,   858,    0),
			new CityInfo("Minoc", "The Barnacle", 1075073, 2476,    413,    15),
			new CityInfo("Britain", "The Wayfarer's Inn",   1075074, 1602,  1591,   20),
			new CityInfo("Moonglow",    "The Scholars Inn", 1075075, 4408,  1168,   0),
			new CityInfo("Trinsic", "The Traveler's Inn",   1075076, 1845,  2745,   0),
			new CityInfo("Jhelom", "The Mercenary Inn", 1075078, 1374,  3826,   0),
			new CityInfo("Skara Brae",  "The Falconer's Inn",   1075079, 618,   2234,   0),
			new CityInfo("Vesper", "The Ironwood Inn",  1075080, 2771,  976,    0),
			new CityInfo("Royal City", "Royal City Inn", 1150169, 738, 3486, -19, Map.TerMur)
		};

		private static readonly CityInfo[] SiegeStartingCities = new CityInfo[]
		{
			new CityInfo("Britain", "The Wayfarer's Inn",   1075074, 1602,  1591,   20, Map.Felucca),
			new CityInfo("Royal City", "Royal City Inn", 1150169, 738, 3486, -19, Map.TerMur)
		};

		public static AccessLevel LockdownLevel { get; set; }

		private static readonly char[] m_ForbiddenChars = new char[]
		{
			'<', '>', ':', '"', '/', '\\', '|', '?', '*', ' '
		};

		private static Dictionary<IPAddress, int> m_IPTable;

		public static Dictionary<IPAddress, int> IPTable
		{
			get
			{
				if (m_IPTable == null)
				{
					m_IPTable = new Dictionary<IPAddress, int>();

					foreach (var a in Accounts.GetAccounts().OfType<Account>())
					{
						if (a.LoginIPs.Length > 0)
						{
							var ip = a.LoginIPs[0];

							if (m_IPTable.ContainsKey(ip))
								m_IPTable[ip]++;
							else
								m_IPTable[ip] = 1;
						}
					}
				}

				return m_IPTable;
			}
		}

		public static void Initialize()
		{
			EventSink.DeleteRequest += EventSink_DeleteRequest;
			EventSink.AccountLogin += EventSink_AccountLogin;
			EventSink.GameLogin += EventSink_GameLogin;

			CommandSystem.Register("Password", AccessLevel.Player, Password_OnCommand);
		}

		[Usage("Password <newPassword> <repeatPassword>")]
		[Description("Changes the password of the commanding players account. Requires the same C-class IP address as the account's creator.")]
		public static void Password_OnCommand(CommandEventArgs e)
		{
			if (!PasswordCommandEnabled)
				return;

			var from = e.Mobile;
			var acct = from.Account as Account;

			if (acct == null)
				return;

			var accessList = acct.LoginIPs;

			if (accessList.Length == 0)
				return;

			var ns = from.NetState;

			if (ns == null)
				return;

			if (e.Length == 0)
			{
				from.SendMessage("You must specify the new password.");
				return;
			}
			else if (e.Length == 1)
			{
				from.SendMessage("To prevent potential typing mistakes, you must type the password twice. Use the format:");
				from.SendMessage("Password \"(newPassword)\" \"(repeated)\"");
				return;
			}

			var pass = e.GetString(0);
			var pass2 = e.GetString(1);

			if (pass != pass2)
			{
				from.SendMessage("The passwords do not match.");
				return;
			}

			var isSafe = true;

			for (var i = 0; isSafe && i < pass.Length; ++i)
				isSafe = (pass[i] >= 0x20 && pass[i] < 0x7F);

			if (!isSafe)
			{
				from.SendMessage("That is not a valid password.");
				return;
			}

			try
			{
				var ipAddress = ns.Address;

				if (Utility.IPMatchClassC(accessList[0], ipAddress))
				{
					acct.SetPassword(pass);
					from.SendMessage("The password to your account has changed.");
				}
				else
				{
					var entry = PageQueue.GetEntry(from);

					if (entry != null)
					{
						if (entry.Message.StartsWith("[Automated: Change Password]"))
							from.SendMessage("You already have a password change request in the help system queue.");
						else
							from.SendMessage("Your IP address does not match that which created this account.");
					}
					else if (PageQueue.CheckAllowedToPage(from))
					{
						from.SendMessage("Your IP address does not match that which created this account.  A page has been entered into the help system on your behalf.");

						from.SendLocalizedMessage(501234, "", 0x35); /* The next available Counselor/Game Master will respond as soon as possible.
                        * Please check your Journal for messages every few minutes.
                        */

						PageQueue.Enqueue(new PageEntry(from, String.Format("[Automated: Change Password]<br>Desired password: {0}<br>Current IP address: {1}<br>Account IP address: {2}", pass, ipAddress, accessList[0]), PageType.Account));
					}
				}
			}
			catch (Exception ex)
			{
				Diagnostics.ExceptionLogging.LogException(ex);
			}
		}

		public static bool CanCreate(IPAddress ip)
		{
			if (!IPTable.ContainsKey(ip) || IPLimiter.IsExempt(ip))
				return true;

			return (IPTable[ip] < MaxAccountsPerIP);
		}

		public static void EventSink_AccountLogin(AccountLoginEventArgs e)
		{
			// If the login attempt has already been rejected by another event handler
			// then just return
			if (e.Accepted == false)
				return;

			if (!IPLimiter.SocketBlock && !IPLimiter.Verify(e.State.Address))
			{
				e.Accepted = false;
				e.RejectReason = ALRReason.InUse;

				Utility.PushColor(ConsoleColor.Red);
				Console.WriteLine("Login: {0}: Past IP limit threshold", e.State);
				Utility.PopColor();

				using (var op = new StreamWriter("ipLimits.log", true))
					op.WriteLine("{0}\tPast IP limit threshold\t{1}", e.State, DateTime.UtcNow);

				return;
			}

			var un = e.Username;
			var pw = e.Password;

			e.Accepted = false;
			var acct = Accounts.GetAccount(un);

			if (acct == null)
			{
				if (AutoAccountCreation && un.Trim().Length > 0) // To prevent someone from making an account of just '' or a bunch of meaningless spaces
				{
					e.State.Account = acct = CreateAccount(e.State, un, pw);
					e.Accepted = acct == null ? false : acct.CheckAccess(e.State);

					if (!e.Accepted)
						e.RejectReason = ALRReason.BadComm;
				}
				else
				{
					Utility.PushColor(ConsoleColor.Red);
					Console.WriteLine("Login: {0}: Invalid username '{1}'", e.State, un);
					Utility.PopColor();
					e.RejectReason = ALRReason.Invalid;
				}
			}
			else if (!acct.HasAccess(e.State))
			{
				Utility.PushColor(ConsoleColor.Red);
				Console.WriteLine("Login: {0}: Access denied for '{1}'", e.State, un);
				Utility.PopColor();
				e.RejectReason = (LockdownLevel >= AccessLevel.Counselor ? ALRReason.BadComm : ALRReason.BadPass);
			}
			else if (!acct.CheckPassword(pw))
			{
				Utility.PushColor(ConsoleColor.Red);
				Console.WriteLine("Login: {0}: Invalid password for '{1}'", e.State, un);
				Utility.PopColor();
				e.RejectReason = ALRReason.BadPass;
			}
			else if (acct.Banned)
			{
				Utility.PushColor(ConsoleColor.Red);
				Console.WriteLine("Login: {0}: Banned account '{1}'", e.State, un);
				Utility.PopColor();
				e.RejectReason = ALRReason.Blocked;
			}
			else
			{
				Utility.PushColor(ConsoleColor.Green);
				Console.WriteLine("Login: {0}: Valid credentials for '{1}'", e.State, un);
				Console.WriteLine("Client Type: {0}: {1}", e.State, e.State.IsEnhancedClient ? "Enhanced Client" : "Classic Client");
				Utility.PopColor();
				e.State.Account = acct;
				e.Accepted = true;

				acct.LogAccess(e.State);
			}

			if (!e.Accepted)
				AccountAttackLimiter.RegisterInvalidAccess(e.State);
		}

		public static void EventSink_GameLogin(GameLoginEventArgs e)
		{
			if (!IPLimiter.SocketBlock && !IPLimiter.Verify(e.State.Address))
			{
				e.Accepted = false;

				Utility.PushColor(ConsoleColor.Red);
				Console.WriteLine("Login: {0}: Past IP limit threshold", e.State);
				Utility.PopColor();

				using (var op = new StreamWriter("ipLimits.log", true))
					op.WriteLine("{0}\tPast IP limit threshold\t{1}", e.State, DateTime.UtcNow);

				return;
			}

			var un = e.Username;
			var pw = e.Password;

			var acct = Accounts.GetAccount(un) as Account;

			if (acct == null)
			{
				e.Accepted = false;
			}
			else if (!acct.HasAccess(e.State))
			{
				Utility.PushColor(ConsoleColor.Red);
				Console.WriteLine("Login: {0}: Access denied for '{1}'", e.State, un);
				Utility.PopColor();
				e.Accepted = false;
			}
			else if (!acct.CheckPassword(pw))
			{
				Utility.PushColor(ConsoleColor.Red);
				Console.WriteLine("Login: {0}: Invalid password for '{1}'", e.State, un);
				Utility.PopColor();
				e.Accepted = false;
			}
			else if (acct.Banned)
			{
				Utility.PushColor(ConsoleColor.Red);
				Console.WriteLine("Login: {0}: Banned account '{1}'", e.State, un);
				Utility.PopColor();
				e.Accepted = false;
			}
			else
			{
				acct.LogAccess(e.State);

				Utility.PushColor(ConsoleColor.Yellow);
				Console.WriteLine("Login: {0}: Account '{1}' at character list", e.State, un);
				Utility.PopColor();
				e.State.Account = acct;
				e.Accepted = true;

				if (Siege.SiegeShard)
				{
					e.CityInfo = SiegeStartingCities;
				}
				else
				{
					e.CityInfo = StartingCities;
				}
			}

			if (!e.Accepted)
				AccountAttackLimiter.RegisterInvalidAccess(e.State);
		}

		public static bool CheckAccount(Mobile mobCheck, Mobile accCheck)
		{
			if (accCheck != null)
			{
				var a = accCheck.Account;

				if (a != null)
				{
					for (var i = 0; i < a.Length; ++i)
					{
						if (a[i] == mobCheck)
							return true;
					}
				}
			}

			return false;
		}

		private static void EventSink_DeleteRequest(DeleteRequestEventArgs e)
		{
			var state = e.State;
			var index = e.Index;

			var acct = state.Account;

			if (acct == null)
			{
				state.Dispose();
			}
			else if (index < 0 || index >= acct.Length)
			{
				state.Send(new DeleteResult(DeleteResultType.BadRequest));
				state.Send(new CharacterListUpdate(acct));
			}
			else
			{
				var m = acct[index];

				if (m == null)
				{
					state.Send(new DeleteResult(DeleteResultType.CharNotExist));
					state.Send(new CharacterListUpdate(acct));
				}
				else if (m.NetState != null)
				{
					state.Send(new DeleteResult(DeleteResultType.CharBeingPlayed));
					state.Send(new CharacterListUpdate(acct));
				}
				else if (RestrictDeletion && DateTime.UtcNow < (m.CreationTime + DeleteDelay))
				{
					state.Send(new DeleteResult(DeleteResultType.CharTooYoung));
					state.Send(new CharacterListUpdate(acct));
				}
				else if (m.IsPlayer() && Region.Find(m.LogoutLocation, m.LogoutMap).GetRegion(typeof(Jail)) != null)    //Don't need to check current location, if netstate is null, they're logged out
				{
					state.Send(new DeleteResult(DeleteResultType.BadRequest));
					state.Send(new CharacterListUpdate(acct));
				}
				else
				{
					Utility.PushColor(ConsoleColor.Red);
					Console.WriteLine("Client: {0}: Deleting character {1} (0x{2:X})", state, index, m.Serial.Value);
					Utility.PopColor();

					acct.AddComment("System", String.Format("Character #{0} {1} deleted by {2}", index + 1, m, state));

					m.Delete();
					state.Send(new CharacterListUpdate(acct));
				}
			}
		}

		private static bool IsForbiddenChar(char c)
		{
			for (var i = 0; i < m_ForbiddenChars.Length; ++i)
				if (c == m_ForbiddenChars[i])
					return true;

			return false;
		}

		private static IAccount CreateAccount(NetState state, string un, string pw)
		{
			if (un.Length == 0 || pw.Length == 0)
				return null;

			var isSafe = !(un.StartsWith(" ") || un.EndsWith(" ") || un.EndsWith("."));

			for (var i = 0; isSafe && i < un.Length; ++i)
				isSafe = (un[i] >= 0x20 && un[i] < 0x7F && !IsForbiddenChar(un[i]));

			for (var i = 0; isSafe && i < pw.Length; ++i)
				isSafe = (pw[i] >= 0x20 && pw[i] < 0x7F);

			if (!isSafe)
				return null;

			if (!CanCreate(state.Address))
			{
				Utility.PushColor(ConsoleColor.DarkYellow);
				Console.WriteLine("Login: {0}: Account '{1}' not created, ip already has {2} account{3}.", state, un, MaxAccountsPerIP, MaxAccountsPerIP == 1 ? "" : "s");
				Utility.PopColor();
				return null;
			}

			Utility.PushColor(ConsoleColor.Green);
			Console.WriteLine("Login: {0}: Creating new account '{1}'", state, un);
			Utility.PopColor();

			return Accounts.Create(un, pw);
		}
	}
}
