#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Server;
using Server.Accounting;
using Server.Mobiles;
#endregion

namespace VitaNex.Modules.Voting
{
	public sealed class VoteProfileEntry
	{
		public DateTime VoteTime { get; set; }
		public IVoteSite VoteSite { get; set; }
		public int TokenAmount { get; set; }

		public VoteProfileEntry(DateTime when, IVoteSite site, int tokens)
		{
			VoteTime = when;
			VoteSite = site;
			TokenAmount = tokens;
		}

		public VoteProfileEntry(GenericReader reader)
		{
			Deserialize(reader);
		}

		public string ToHtmlString(bool technical = false)
		{
			var html = new StringBuilder();

			if (technical)
			{
				html.AppendLine(
					String.Format(
						"[{0}]: Vote at '{1}' ({2}) for {3} tokens.",
						VoteTime.ToSimpleString(Voting.CMOptions.DateFormat),
						VoteSite.Name,
						VoteSite.Link,
						TokenAmount.ToString("#,0")));
			}
			else
			{
				html.AppendLine(
					String.Format(
						"[{0}]: Vote at '{1}' for {2} tokens.",
						VoteTime.ToSimpleString(Voting.CMOptions.DateFormat),
						VoteSite.Name,
						TokenAmount.ToString("#,0")));
			}

			return html.ToString();
		}

		public void Serialize(GenericWriter writer)
		{
			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.Write(VoteTime);
					writer.Write(VoteSite.UID);
				}
				break;
			}
		}

		public void Deserialize(GenericReader reader)
		{
			var version = reader.GetVersion();

			switch (version)
			{
				case 0:
				{
					VoteTime = reader.ReadDateTime();
					VoteSite = Voting.FindSite(reader.ReadInt());
				}
				break;
			}
		}
	}

	public sealed class VoteProfile
	{
		public bool Deleted { get; private set; }
		public PlayerMobile Owner { get; private set; }

		private static readonly IPAddress[] _DefaultIPs = new IPAddress[0];

		public IPAddress[] LoginIPs => Owner != null && Owner.Account is Account ? ((Account)Owner.Account).LoginIPs : _DefaultIPs;

		public Dictionary<TimeStamp, List<VoteProfileEntry>> History { get; private set; }

		public VoteProfile(PlayerMobile owner)
		{
			Owner = owner;
			History = new Dictionary<TimeStamp, List<VoteProfileEntry>>();
		}

		public VoteProfile(GenericReader reader)
		{
			Deserialize(reader);
		}

		private static TimeStamp GenerateKey(ref DateTime when)
		{
			return (when = new DateTime(when.Year, when.Month, when.Day)).ToTimeStamp();
		}

		public bool TransferTokens(IVoteSite site, int amount, bool message = true)
		{
			if (site == null || Deleted || Owner == null || Owner.Deleted)
			{
				return false;
			}

			var limitReached = false;
			var total = GetTokenTotal(DateTime.UtcNow);

			if (Voting.CMOptions.DailyLimit > 0)
			{
				if (total >= Voting.CMOptions.DailyLimit)
				{
					limitReached = true;
					amount = 0;
				}
				else if ((total + amount) > Voting.CMOptions.DailyLimit)
				{
					limitReached = true;
					amount = (total + amount) - Voting.CMOptions.DailyLimit;
				}
			}

			if (amount > 0)
			{
				new VoteToken(amount).GiveTo(Owner);
			}

			if (limitReached && message)
			{
				Owner.SendMessage(0x22, "You have reached your daily token limit of {0:#,0}.", Voting.CMOptions.DailyLimit);
			}

			RegisterTokens(site, amount);
			return true;
		}

		public void RegisterTokens(IVoteSite site, int amount)
		{
			if (site == null)
			{
				return;
			}

			var now = DateTime.UtcNow;
			var when = now;
			var key = GenerateKey(ref when);

			if (!History.ContainsKey(key))
			{
				History.Add(key, new List<VoteProfileEntry>());
			}

			var e = GetHistory(now).FirstOrDefault(s => s.VoteSite == site);

			if (e == null)
			{
				History[key].Add(new VoteProfileEntry(now, site, amount));
			}
			else
			{
				e.VoteTime = now;
				e.TokenAmount += amount;
			}
		}

		public void ClearHistory(DateTime when)
		{
			var key = GenerateKey(ref when);

			if (!History.ContainsKey(key))
			{
				return;
			}

			History[key].Free(true);
			History.Remove(key);
		}

		public void ClearHistory()
		{
			History.Values.Free(true);
			History.Clear();
		}

		public IEnumerable<VoteProfileEntry> GetHistory(DateTime when, int limit = 0)
		{
			return GetHistory(GenerateKey(ref when), limit);
		}

		public IEnumerable<VoteProfileEntry> GetHistory(TimeStamp key, int limit = 0)
		{
			var list = History.GetValue(key);

			if (list == null || list.Count <= 0)
			{
				return Enumerable.Empty<VoteProfileEntry>();
			}

			IEnumerable<VoteProfileEntry> ie = list.OrderByDescending(e => e.VoteTime);

			if (limit > 0)
			{
				ie = ie.Take(limit);
			}

			return ie;
		}

		public IEnumerable<VoteProfileEntry> GetHistory(int limit = 0)
		{
			IEnumerable<VoteProfileEntry> ie = History.SelectMany(kv => kv.Value).OrderByDescending(e => e.VoteTime);

			if (limit > 0)
			{
				ie = ie.Take(limit);
			}

			return ie;
		}

		public int GetTokenTotal(DateTime when)
		{
			var val = 0;

			var h = History.GetValue(GenerateKey(ref when));

			if (h != null)
			{
				val = h.Aggregate(val, (c, e) => c + e.TokenAmount);
			}

			return val;
		}

		public int GetTokenTotal()
		{
			return History.SelectMany(kv => kv.Value).Aggregate(0, (c, e) => c + e.TokenAmount);
		}

		public void Delete()
		{
			if (Deleted)
			{
				return;
			}

			ClearHistory();

			Deleted = true;

			this.Remove();
		}

		public string ToHtmlString(Mobile viewer = null)
		{
			var html = new StringBuilder();

			html.AppendLine(String.Format("Vote Profile for <big>{0}</big>", Owner.RawName));
			html.AppendLine();

			var totalToday = GetTokenTotal(DateTime.UtcNow);
			var totalAllTime = GetTokenTotal();
			var limitToday = Voting.CMOptions.DailyLimit;

			if (limitToday <= 0)
			{
				html.AppendLine(String.Format("Tokens Collected Today: <big>{0}</big>", totalToday.ToString("#,0")));
			}
			else
			{
				html.AppendLine(
					String.Format(
						"Tokens Collected Today: <big>{0}</big>/<big>{1}</big>",
						totalToday.ToString("#,0"),
						limitToday.ToString("#,0")));
			}

			html.AppendLine(String.Format("Tokens Collected Total: <big>{0}</big>", totalAllTime.ToString("#,0")));

			return html.ToString();
		}

		public void Serialize(GenericWriter writer)
		{
			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.Write(Owner);
					writer.Write(Deleted);

					if (!Deleted)
					{
						writer.WriteBlockDictionary(
							History,
							(w1, k, v) =>
							{
								w1.Write(k.Stamp);
								w1.WriteBlockList(v, (w2, e) => e.Serialize(w2));
							});
					}
				}
				break;
			}
		}

		public void Deserialize(GenericReader reader)
		{
			var version = reader.GetVersion();

			switch (version)
			{
				case 0:
				{
					Owner = reader.ReadMobile<PlayerMobile>();
					Deleted = reader.ReadBool();

					if (!Deleted)
					{
						History = reader.ReadBlockDictionary(
							r1 =>
							{
								TimeStamp k = r1.ReadDouble();
								var v = r1.ReadBlockList(r2 => new VoteProfileEntry(r2));

								return new KeyValuePair<TimeStamp, List<VoteProfileEntry>>(k, v);
							});
					}
				}
				break;
			}

			if (History != null)
			{
				foreach (var h in History.Values)
				{
					h.Prune(true, e => e.VoteSite);
				}
			}
		}
	}
}