using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Server.Accounting;

namespace Server.Engines.UOStore
{
	public static class PromoCodes
	{
		public static readonly string FilePath = Path.Combine(Core.BaseDirectory, "Saves", "Misc", "PromoCodes.bin");

		public static Dictionary<string, PromoCodeHandler> Codes { get; } = new Dictionary<string, PromoCodeHandler>(StringComparer.InvariantCultureIgnoreCase);

		public static Dictionary<string, HashSet<IAccount>> Redeemed { get; } = new Dictionary<string, HashSet<IAccount>>(StringComparer.InvariantCultureIgnoreCase);

		public static void Configure()
		{
			// TODO: Admin UI
			// CommandSystem.Register("Promos", AccessLevel.Administrator, e => OpenEditor(e.Mobile as PlayerMobile));

			EventSink.WorldSave += OnSave;
			EventSink.WorldLoad += OnLoad;

#if DEBUG
			Register("TEST-1234", TestCodeRedeem);
#endif
		}

#if DEBUG
		private static bool TestCodeRedeem(Mobile user, string code)
		{
			if (user.AccessLevel > AccessLevel.Counselor)
			{
				user.SendMessage($"The promo code test has passed for '{code}'");

				return true;
			}

			return false;
		}
#endif

		#region Register API

		public static void Unregister(string code, bool clearRedeemed)
		{
			Codes.Remove(code);

			if (clearRedeemed)
			{
				ClearRedeemed(code);
			}
		}

		public static void Register(string code, PromoCodeRedeemer redeemer)
		{
			Register(code, 0, DateTime.MinValue, DateTime.MaxValue, redeemer);
		}

		public static void Register(string code, int limited, PromoCodeRedeemer redeemer)
		{
			Register(code, limited, DateTime.MinValue, DateTime.MaxValue, redeemer);
		}

		public static void Register(string code, int endYear, int endMonth, int endDay, PromoCodeRedeemer redeemer)
		{
			Register(code, 0, endYear, endMonth, endDay, redeemer);
		}

		public static void Register(string code, DateTime ends, PromoCodeRedeemer redeemer)
		{
			Register(code, 0, DateTime.MinValue, ends, redeemer);
		}

		public static void Register(string code, int limited, int endYear, int endMonth, int endDay, PromoCodeRedeemer redeemer)
		{
			Register(code, limited, DateTime.MinValue, new DateTime(endYear, endMonth, endDay, 23, 59, 59, 999, DateTimeKind.Utc), redeemer);
		}

		public static void Register(string code, int beginYear, int beginMonth, int beginDay, int endYear, int endMonth, int endDay, PromoCodeRedeemer redeemer)
		{
			Register(code, 0, beginYear, beginMonth, beginDay, endYear, endMonth, endDay, redeemer);
		}

		public static void Register(string code, DateTime begins, DateTime ends, PromoCodeRedeemer redeemer)
		{
			Register(code, 0, begins, ends, redeemer);
		}

		public static void Register(string code, int limited, int beginYear, int beginMonth, int beginDay, int endYear, int endMonth, int endDay, PromoCodeRedeemer redeemer)
		{
			Register(code, limited, new DateTime(beginYear, beginMonth, beginDay, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(endYear, endMonth, endDay, 23, 59, 59, 999, DateTimeKind.Utc), redeemer);
		}

		public static void Register(string code, int limited, DateTime begins, DateTime ends, PromoCodeRedeemer redeemer)
		{
			Codes[code] = new PromoCodeHandler(limited, begins, ends, redeemer);
		}

		#endregion

		#region Redeem API

		public static void ClearRedeemed(string code)
		{
			if (Redeemed.TryGetValue(code, out var users))
			{
				users.Clear();
				users.TrimExcess();

				Redeemed.Remove(code);
			}
		}

		public static bool TryRedeem(Mobile m, string code)
		{
			if (m == null || m.Deleted || m.Account == null)
			{
				return false;
			}

			if (m.Account.Age.TotalDays < 1)
			{
				m.SendMessage("Your account must be at least one day old to redeem promo codes!");

				return false;
			}

			if (String.IsNullOrWhiteSpace(code))
			{
				m.SendMessage("The promo code you provided is invalid.");

				return false;
			}

			code = code.Trim();

			if (!Codes.TryGetValue(code, out var handler))
			{
				m.SendMessage("The promo code you provided is invalid.");

				return false;
			}

			if (handler == null || DateTime.UtcNow < handler.Begins)
			{
				m.SendMessage("The promo code you provided could not be processed at this time.");

				return false;
			}

			if (!Redeemed.TryGetValue(code, out var users) || users == null)
			{
				Redeemed[code] = users = new HashSet<IAccount>();
			}

			if (users.Contains(m.Account) || users.Any(a => a.LoginIPs.Any(ip => Array.IndexOf(m.Account.LoginIPs, ip) != -1)))
			{
				m.SendMessage("You have already redeemed this promo code.");

				return false;
			}

			if (handler.Limited > 0 && users.Count >= handler.Limited)
			{
				m.SendMessage("The promo code you provded has reached its claim limit.");

				return false;
			}

			if (handler.Expires > DateTime.MinValue && DateTime.UtcNow > handler.Expires)
			{
				m.SendMessage("The promo code you provded has expired.");

				return false;
			}

			if (!users.Add(m.Account))
			{
				m.SendMessage("You have already redeemed this promo code.");

				return false;
			}

			if (!handler.Redeemer(m, code))
			{
				m.SendMessage("The promo code you provided could not be processed at this time.");

				return false;
			}

			return true;
		}

		#endregion

		#region Persistence API

		public static void OnSave(WorldSaveEventArgs e)
		{
			Persistence.Serialize(FilePath, Serialize);
		}

		public static void OnLoad()
		{
			Persistence.Deserialize(FilePath, Deserialize);
		}

		private static void Serialize(GenericWriter writer)
		{
			writer.Write(0);

			writer.Write(Codes.Count);

			foreach (var kvp in Redeemed)
			{
				writer.Write(kvp.Key);

				writer.Write(kvp.Value.Count);

				foreach (var a in kvp.Value)
				{
					writer.Write(a.Username);
				}
			}
		}

		private static void Deserialize(GenericReader reader)
		{
			reader.ReadInt();

			var count = reader.ReadInt();

			for (var i = 0; i < count; i++)
			{
				var code = reader.ReadString();

				var ac = reader.ReadInt();

				var list = new HashSet<IAccount>(ac);

				while (--ac >= 0)
				{
					var u = reader.ReadString();

					if (u != null)
					{
						var a = Accounts.GetAccount(u);

						if (a != null)
						{
							list.Add(a);
						}
					}
				}

				if (code != null && list.Count > 0 && Codes.ContainsKey(code))
				{
					Redeemed[code] = list;
				}
			}
		}

		#endregion
	}

	public delegate bool PromoCodeRedeemer(Mobile user, string code);

	[PropertyObject]
	public class PromoCodeHandler
	{
		[CommandProperty(AccessLevel.Administrator)]
		public DateTime Begins { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public DateTime Expires { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public int Limited { get; set; }

		public PromoCodeRedeemer Redeemer { get; set; }

		public PromoCodeHandler(int limited, DateTime begins, DateTime ends, PromoCodeRedeemer redeemer)
		{
			if (begins > ends)
			{
				Begins = ends;
				Expires = begins;
			}
			else
			{
				Begins = begins;
				Expires = ends;
			}

			Limited = limited;
			Redeemer = redeemer;
		}
	}
}
