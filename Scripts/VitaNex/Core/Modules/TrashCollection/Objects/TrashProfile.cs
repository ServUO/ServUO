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
using System.Text;

using Server;
using Server.Mobiles;
#endregion

namespace VitaNex.Modules.TrashCollection
{
	public sealed class TrashProfileEntry
	{
		public TrashProfileEntry(DateTime when, Mobile source, Item trashed, int tokens)
		{
			TrashedTime = when;

			if (source != null)
			{
				SourceSerial = source.Serial;
				SourceName = source.RawName;
			}
			else
			{
				SourceSerial = Serial.Zero;
				SourceName = String.Empty;
			}

			if (trashed != null)
			{
				TrashedSerial = trashed.Serial;
				TrashedType = trashed.GetType().Name;
				TrashedName = trashed.ResolveName();
			}
			else
			{
				TrashedSerial = Serial.Zero;
				TrashedType = String.Empty;
				TrashedName = String.Empty;
			}

			TokenAmount = tokens;
		}

		public TrashProfileEntry(GenericReader reader)
		{
			Deserialize(reader);
		}

		public Serial SourceSerial { get; private set; }
		public string SourceName { get; private set; }

		public Serial TrashedSerial { get; private set; }
		public string TrashedType { get; private set; }
		public string TrashedName { get; private set; }
		public DateTime TrashedTime { get; private set; }

		public int TokenAmount { get; private set; }

		public string ToHtmlString(bool technical = false)
		{
			var html = new StringBuilder();

			if (technical)
			{
				html.AppendLine(
					String.Format(
						"[{0}]: '{1}' ({2}: {3}) trashed by '{4}' ({5}) for {6} tokens.",
						TrashedTime.ToSimpleString(TrashCollection.CMOptions.DateFormat),
						TrashedName,
						TrashedSerial,
						TrashedType,
						SourceName,
						SourceSerial,
						TokenAmount > 0 ? TokenAmount.ToString("#,#") : "0"));
			}
			else
			{
				html.AppendLine(
					String.Format(
						"[{0}]: '{1}' trashed by '{2}' for {3} tokens.",
						TrashedTime.ToSimpleString(TrashCollection.CMOptions.DateFormat),
						TrashedName,
						SourceName,
						TokenAmount > 0 ? TokenAmount.ToString("#,#") : "0"));
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
					writer.Write(SourceSerial);
					writer.Write(SourceName);

					writer.Write(TrashedSerial);
					writer.Write(TrashedType);
					writer.Write(TrashedName);
					writer.Write(TrashedTime);

					writer.Write(TokenAmount);
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
					SourceSerial = reader.ReadSerial();
					SourceName = reader.ReadString();

					TrashedSerial = reader.ReadSerial();
					TrashedType = reader.ReadString();
					TrashedName = reader.ReadString();
					TrashedTime = reader.ReadDateTime();

					TokenAmount = reader.ReadInt();
				}
				break;
			}
		}
	}

	public sealed class TrashProfile
	{
		public static event Action<TrashProfile, TrashToken> TokensReceived;

		public bool Deleted { get; private set; }
		public Mobile Owner { get; private set; }

		public Dictionary<TimeStamp, List<TrashProfileEntry>> History { get; private set; }

		public TrashProfile(Mobile owner)
		{
			Owner = owner;
			History = new Dictionary<TimeStamp, List<TrashProfileEntry>>();
		}

		public TrashProfile(GenericReader reader)
		{
			Deserialize(reader);
		}

		private static TimeStamp GenerateKey(ref DateTime when)
		{
			return (when = new DateTime(when.Year, when.Month, when.Day)).ToTimeStamp();
		}

		private static int InternalHistorySort(TrashProfileEntry a, TrashProfileEntry b)
		{
			if (a == null && b == null)
			{
				return 0;
			}

			if (a == null)
			{
				return 1;
			}

			if (b == null)
			{
				return -1;
			}

			if (a.TrashedTime < b.TrashedTime)
			{
				return 1;
			}

			if (a.TrashedTime > b.TrashedTime)
			{
				return -1;
			}

			return 0;
		}

		public bool TransferTokens(Mobile source, Item trashed, int amount, bool message = true)
		{
			if (Deleted || Owner == null || Owner.Deleted)
			{
				return false;
			}

			if (source == null)
			{
				source = Owner;
			}

			var limitReached = false;
			var total = GetTokenTotal(DateTime.UtcNow);

			if (TrashCollection.CMOptions.DailyLimit > 0)
			{
				if (total >= TrashCollection.CMOptions.DailyLimit)
				{
					limitReached = true;
					amount = 0;
				}
				else if (total + amount > TrashCollection.CMOptions.DailyLimit)
				{
					limitReached = true;
					amount = (total + amount) - TrashCollection.CMOptions.DailyLimit;
				}
			}

			if (amount > 0)
			{
				var token = new TrashToken(amount);
				var name = token.ResolveName(Owner.GetLanguage());

				if (Owner.Backpack.TryDropItem(Owner, token, true))
				{
					if (message)
					{
						Owner.SendMessage(
							0x55,
							"{0}{1}{2} {3} been placed in your backpack.",
							!name.StartsWith("a") && amount == 1 ? "a " : String.Empty,
							name,
							!name.EndsWith("s") && amount > 1 ? "s" : String.Empty,
							amount > 1 ? "have" : "has");
					}
				}
				else if (Owner.BankBox.TryDropItem(Owner, token, true))
				{
					if (message)
					{
						Owner.SendMessage(
							0x55,
							"{0}{1}{2} {3} been placed in your bank.",
							!name.StartsWith("a") && amount == 1 ? "a " : String.Empty,
							name,
							!name.EndsWith("s") && amount > 1 ? "s" : String.Empty,
							amount > 1 ? "have" : "has");
					}
				}
				else
				{
					if (Owner.NetState == null && Owner is PlayerMobile)
					{
						token.MoveToWorld(Owner.LogoutLocation, Owner.LogoutMap);
					}
					else
					{
						token.MoveToWorld(Owner.Location, Owner.Map);
					}

					if (message)
					{
						Owner.SendMessage(
							0x55,
							"{0}{1}{2} {3} been placed at your feet.",
							!name.StartsWith("a") && amount == 1 ? "a " : String.Empty,
							name,
							!name.EndsWith("s") && amount > 1 ? "s" : String.Empty,
							amount > 1 ? "have" : "has");
					}
				}

				if (TokensReceived != null)
				{
					TokensReceived.Invoke(this, token);
				}
			}
			else
			{
				if (message)
				{
					Owner.SendMessage(0x22, "You didn't receive any tokens this time.");
				}

				return false;
			}

			if (limitReached && message)
			{
				Owner.SendMessage(
					0x22,
					"You have reached your daily trash token limit of {0:#,0}.",
					TrashCollection.CMOptions.DailyLimit);
			}

			RegisterTokens(source, trashed, amount);
			return true;
		}

		public void RegisterTokens(Mobile source, Item trashed, int amount)
		{
			if (source == null || trashed == null || amount <= 0)
			{
				return;
			}

			var when = DateTime.UtcNow;
			var key = GenerateKey(ref when);

			if (!History.ContainsKey(key))
			{
				History.Add(key, new List<TrashProfileEntry>());
			}

			History[key].Add(new TrashProfileEntry(when, source, trashed, amount));
		}

		public void ClearHistory(DateTime when)
		{
			var key = GenerateKey(ref when);

			if (!History.ContainsKey(key))
			{
				return;
			}

			History[key].Clear();
			History.Remove(key);
		}

		public void ClearHistory()
		{
			History.Values.ForEach(l => l.Clear());
			History.Clear();
		}

		public List<TrashProfileEntry> GetHistory(DateTime when, int limit = 0)
		{
			return GetHistory(GenerateKey(ref when), limit);
		}

		public List<TrashProfileEntry> GetHistory(TimeStamp key, int limit = 0)
		{
			var list = History.ContainsKey(key) ? History[key].ToList() : new List<TrashProfileEntry>();

			if (limit > 0 && list.Count > limit)
			{
				list.RemoveRange(limit, list.Count - limit);
			}

			list.Sort(InternalHistorySort);
			return list;
		}

		public List<TrashProfileEntry> GetHistory(int limit = 0)
		{
			var list = History.SelectMany(kv => kv.Value).ToList();

			if (limit > 0 && list.Count > limit)
			{
				list.RemoveRange(limit, list.Count - limit);
			}

			list.Sort(InternalHistorySort);

			return list;
		}

		public int GetTokenTotal(DateTime when)
		{
			return GetHistory(when).Sum(e => e.TokenAmount);
		}

		public int GetTokenTotal()
		{
			return GetHistory().Sum(e => e.TokenAmount);
		}

		public void Delete()
		{
			if (Deleted)
			{
				return;
			}

			Deleted = true;
			ClearHistory();

			this.Remove();
		}

		public string ToHtmlString(Mobile viewer = null)
		{
			var html = new StringBuilder();

			html.AppendLine(String.Format("Trash Collection Profile for <big>{0}</big>", Owner.RawName));
			html.AppendLine();

			var totalToday = GetTokenTotal(DateTime.UtcNow);
			var totalAllTime = GetTokenTotal();
			var limitToday = TrashCollection.CMOptions.DailyLimit;

			html.AppendLine(
				limitToday > 0
					? String.Format("Tokens Collected Today: <big>{0:#,0}</big>/<big>{1:#,0}</big>", totalToday, limitToday)
					: String.Format("Tokens Collected Today: <big>{0:#,0}</big>", totalToday));

			html.AppendLine(String.Format("Tokens Collected Total: <big>{0:#,0}</big>", totalAllTime));

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
								var v = r1.ReadBlockList(r2 => new TrashProfileEntry(r2));
								return new KeyValuePair<TimeStamp, List<TrashProfileEntry>>(k, v);
							});
					}
				}
				break;
			}
		}
	}
}