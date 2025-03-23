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
using System.Text;

using Server;
using Server.Accounting;
using Server.Misc;

using VitaNex.IO;
#endregion

namespace VitaNex.Modules.AutoDonate
{
	[PropertyObject]
	public sealed class DonationTransaction : IEquatable<DonationTransaction>, IComparable<DonationTransaction>
	{
		private static void Protect(Item item)
		{
			var flags = ScriptCompiler.FindTypeByFullName("Server.Items.ItemFlags");

			if (flags != null)
			{
				flags.InvokeMethod("SetStealable", item, false);
			}
		}

		[CommandProperty(AutoDonate.Access, true)]
		public string ID { get; private set; }

		[CommandProperty(AutoDonate.Access, true)]
		public IAccount Account { get; private set; }

		[CommandProperty(AutoDonate.Access, true)]
		public bool Deleted { get; private set; }

		[CommandProperty(AutoDonate.Access, true)]
		public int Version { get; set; }

		[CommandProperty(AutoDonate.Access, true)]
		public TimeStamp Time { get; set; }

		[CommandProperty(AutoDonate.Access, true)]
		public string Email { get; set; }

		[CommandProperty(AutoDonate.Access, true)]
		public double Total { get; set; }

		[CommandProperty(AutoDonate.Access, true)]
		public long Bonus { get; set; }

		[CommandProperty(AutoDonate.Access)]
		public long Credit { get; set; }

		[CommandProperty(AutoDonate.Access)]
		public string Notes { get; set; }

		[CommandProperty(AutoDonate.Access)]
		public string Extra { get; set; }

		[CommandProperty(AutoDonate.Access)]
		public TimeStamp DeliveryTime { get; set; }

		[CommandProperty(AutoDonate.Access)]
		public string DeliveredTo { get; set; }

		private TransactionState _State;

		[CommandProperty(AutoDonate.Access)]
		public TransactionState State
		{
			get => _State;
			set
			{
				if (_State == value)
				{
					return;
				}

				var old = _State;

				_State = value;

				OnStateChanged(old);
			}
		}

		public TransactionState InternalState { get => State; set => _State = value; }

		[CommandProperty(AutoDonate.Access)]
		public bool Hidden => (IsClaimed || IsVoided) && !AutoDonate.CMOptions.ShowHistory;

		[CommandProperty(AutoDonate.Access)]
		public long CreditTotal => Credit + Bonus;

		public bool IsClaimed => State == TransactionState.Claimed;
		public bool IsPending => State == TransactionState.Pending;
		public bool IsProcessed => State == TransactionState.Processed;
		public bool IsVoided => State == TransactionState.Voided;

		public string FullPath => String.Format("{0}|{1}|{2}", Time.Value.Year, Time.Value.GetMonth(), ID);

		public DonationTransaction(
			string id,
			IAccount account,
			string email,
			double total,
			long credit,
			string notes,
			string extra)
		{
			Version = 0;
			Time = TimeStamp.Now;

			ID = id;

			Account = account;
			Email = email;
			Total = total;

			Credit = credit;
			Bonus = 0;

			Notes = notes;
			Extra = extra;

			_State = TransactionState.Pending;
		}

		public DonationTransaction(GenericReader reader)
		{
			Deserialize(reader);
		}

		private void OnStateChanged(TransactionState oldState)
		{
			DonationEvents.InvokeStateChanged(this, oldState);
		}

		public bool Void()
		{
			if (State == TransactionState.Voided)
			{
				return true;
			}

			if ((State = TransactionState.Voided) == TransactionState.Voided)
			{
				++Version;

				DonationEvents.InvokeTransVoided(this);

				LogToFile();

				return true;
			}

			return false;
		}

		public bool Process()
		{
			if (State == TransactionState.Processed)
			{
				return true;
			}

			if (State != TransactionState.Pending)
			{
				return false;
			}

			if ((State = TransactionState.Processed) == TransactionState.Processed)
			{
				++Version;

				DonationEvents.InvokeTransProcessed(this);

				LogToFile();

				return true;
			}

			return false;
		}

		public bool Claim(Mobile m)
		{
			if (State == TransactionState.Claimed)
			{
				return true;
			}

			if (State != TransactionState.Processed)
			{
				return false;
			}

			if ((State = TransactionState.Claimed) == TransactionState.Claimed)
			{
				Deliver(m);
				DeliveredTo = m.RawName;
				DeliveryTime = TimeStamp.Now;

				Extra += "{SHARD: " + ServerList.ServerName + "}";

				++Version;

				DonationEvents.InvokeTransClaimed(this, m);

				LogToFile();

				return true;
			}

			return false;
		}

		public long GetCredit(DonationProfile dp, out long credit, out long bonus)
		{
			return GetCredit(dp, false, out credit, out bonus);
		}

		private long GetCredit(DonationProfile dp, bool delivering, out long credit, out long bonus)
		{
			if (!delivering && State != TransactionState.Processed)
			{
				return (credit = Credit) + (bonus = Bonus);
			}

			var total = DonationEvents.InvokeTransExchange(this, dp);

			if (AutoDonate.CMOptions.CreditBonus > 0)
			{
				total += (long)Math.Floor(Credit * AutoDonate.CMOptions.CreditBonus);
			}

			bonus = Math.Max(0, total - Credit);
			credit = Math.Max(0, total - bonus);

			return total;
		}

		private void Deliver(Mobile m)
		{
			if (m == null || m.Account == null)
			{
				return;
			}

			if (Account != m.Account)
			{
				SetAccount(m.Account);
			}

			var dp = AutoDonate.EnsureProfile(Account);

			if (dp == null)
			{
				return;
			}

			var total = GetCredit(dp, true, out var credit, out var bonus);

			Credit = credit;
			Bonus = bonus;

			var bag = DonationEvents.InvokeTransPack(this, dp);

			if (bag == null || bag.Deleted)
			{
				dp.Credit += total;
				return;
			}

			Protect(bag);

			while (credit > 0)
			{
				var cur = AutoDonate.CMOptions.CurrencyType.CreateInstance();

				if (cur == null)
				{
					bag.Delete();
					break;
				}

				Protect(cur);

				if (cur.Stackable)
				{
					cur.Amount = (int)Math.Min(credit, 60000);
				}

				credit -= cur.Amount;

				bag.DropItem(cur);
			}

			if (bag.Deleted)
			{
				dp.Credit += total;
				return;
			}

			while (bonus > 0)
			{
				var cur = AutoDonate.CMOptions.CurrencyType.CreateInstance();

				if (cur == null)
				{
					bag.Delete();
					break;
				}

				Protect(cur);

				cur.Name = String.Format("{0} [Bonus]", cur.ResolveName(m));

				if (cur.Stackable)
				{
					cur.Amount = (int)Math.Min(bonus, 60000);
				}

				bonus -= cur.Amount;

				bag.DropItem(cur);
			}

			if (bag.Deleted || bag.GiveTo(m, GiveFlags.PackBankDelete) == GiveFlags.Delete)
			{
				dp.Credit += total;
			}
		}

		public void SetAccount(IAccount acc)
		{
			if (Account == acc)
			{
				return;
			}

			++Version;

			Extra += "{ACCOUNT CHANGED FROM '" + Account + "' TO '" + acc + "'}";

			var profile = AutoDonate.FindProfile(Account);

			if (profile != null)
			{
				profile.Remove(this);
			}

			Account = acc;

			profile = AutoDonate.EnsureProfile(Account);

			if (profile != null)
			{
				profile.Add(this);
			}

			LogToFile();
		}

		public void Delete()
		{
			if (Deleted)
			{
				return;
			}

			Deleted = true;

			AutoDonate.Transactions.Remove(ID);

			DonationEvents.InvokeTransactionDeleted(this);

			DeliveredTo = null;
			SetAccount(null);
		}

		public override int GetHashCode()
		{
			return ID.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is DonationTransaction && Equals((DonationTransaction)obj);
		}

		public bool Equals(DonationTransaction other)
		{
			return other != null && String.Equals(ID, other.ID);
		}

		public int CompareTo(DonationTransaction other)
		{
			var res = 0;

			if (this.CompareNull(other, ref res))
			{
				return res;
			}

			return TimeStamp.Compare(Time, other.Time);
		}

		public override string ToString()
		{
			return ID;
		}

		public void LogToFile()
		{
			var file = IOUtility.EnsureFile(VitaNexCore.LogsDirectory + "/Donations/" + ID + ".log");

			var sb = new StringBuilder();

			sb.AppendLine();
			sb.AppendLine(new string('*', 80));
			sb.AppendLine();
			sb.AppendLine("{0}:		{1}", file.Exists ? "UPDATED" : "CREATED", DateTime.Now);
			sb.AppendLine();
			sb.AppendLine("ID:			{0}", ID);
			sb.AppendLine("State:		{0}", State);
			sb.AppendLine("Time:		{0}", Time.Value);
			sb.AppendLine("Version:		{0:#,0}", Version);
			sb.AppendLine();
			sb.AppendLine("Account:		{0}", Account);
			sb.AppendLine("Email:		{0}", Email);
			sb.AppendLine();
			sb.AppendLine("Total:		{0}{1} {2}", AutoDonate.CMOptions.MoneySymbol, Total, AutoDonate.CMOptions.MoneyAbbr);
			sb.AppendLine("Credit:		{0:#,0} {1}", Credit, AutoDonate.CMOptions.CurrencyName);
			sb.AppendLine("Bonus:		{0:#,0} {1}", Bonus, AutoDonate.CMOptions.CurrencyName);

			if (DeliveredTo != null)
			{
				sb.AppendLine();
				sb.AppendLine("Delivered:	{0}", DeliveryTime.Value);
				sb.AppendLine("Recipient:	{0}", DeliveredTo);
			}

			if (!String.IsNullOrWhiteSpace(Notes))
			{
				sb.AppendLine();
				sb.AppendLine("Notes:");
				sb.AppendLine(Notes);
			}

			if (!String.IsNullOrWhiteSpace(Extra))
			{
				sb.AppendLine();
				sb.AppendLine("Extra:");
				sb.AppendLine(Extra);
			}

			sb.Log(file);
		}

		public void Serialize(GenericWriter writer)
		{
			var version = writer.SetVersion(3);

			switch (version)
			{
				case 3:
					writer.Write(Deleted);
					goto case 2;
				case 2:
				case 1:
					writer.Write(Bonus);
					goto case 0;
				case 0:
				{
					writer.Write(ID);
					writer.WriteFlag(_State);
					writer.Write(Account);
					writer.Write(Email);
					writer.Write(Total);
					writer.Write(Credit);
					writer.Write(Time);
					writer.Write(Version);

					writer.Write(Notes);
					writer.Write(Extra);

					writer.Write(DeliveredTo);
					writer.Write(DeliveryTime);
				}
				break;
			}
		}

		public void Deserialize(GenericReader reader)
		{
			var version = reader.GetVersion();

			switch (version)
			{
				case 3:
					Deleted = reader.ReadBool();
					goto case 2;
				case 2:
				case 1:
					Bonus = reader.ReadLong();
					goto case 0;
				case 0:
				{
					ID = reader.ReadString();
					_State = reader.ReadFlag<TransactionState>();
					Account = reader.ReadAccount();
					Email = reader.ReadString();
					Total = reader.ReadDouble();
					Credit = reader.ReadLong();

					Time = version > 0 ? reader.ReadTimeStamp() : reader.ReadDouble();

					Version = reader.ReadInt();

					if (version < 1)
					{
						reader.ReadInt(); // InternalVersion
					}

					Notes = reader.ReadString();
					Extra = reader.ReadString();

					if (version > 1)
					{
						DeliveredTo = reader.ReadString();
						DeliveryTime = reader.ReadTimeStamp();
					}
					else if (version > 0)
					{
						var m = reader.ReadMobile();

						DeliveredTo = m != null ? m.RawName : null;
						DeliveryTime = reader.ReadTimeStamp();
					}
					else
					{
						reader.ReadMobile(); // DeliverFrom

						var m = reader.ReadMobile();

						DeliveredTo = m != null ? m.RawName : null;
						DeliveryTime = reader.ReadDouble();
					}
				}
				break;
			}
		}
	}
}