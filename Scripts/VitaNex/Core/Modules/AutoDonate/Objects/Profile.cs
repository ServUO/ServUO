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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Accounting;

using VitaNex.Crypto;
#endregion

namespace VitaNex.Modules.AutoDonate
{
	public sealed class DonationProfile : IEnumerable<DonationTransaction>, IEquatable<DonationProfile>
	{
		public delegate double TierCalculation(DonationProfile p, int tier, double factor);

		public static TierCalculation ComputeNextTier = (p, t, f) => (t + 1) * f;

		[CommandProperty(AutoDonate.Access, true)]
		public CryptoHashCode UID { get; private set; }

		[CommandProperty(AutoDonate.Access, true)]
		public IAccount Account { get; private set; }

		public Dictionary<string, DonationTransaction> Transactions { get; private set; }

		public DonationTransaction this[string id] => Transactions.GetValue(id);

		public IEnumerable<DonationTransaction> Pending => Find(TransactionState.Pending);
		public IEnumerable<DonationTransaction> Processed => Find(TransactionState.Processed);
		public IEnumerable<DonationTransaction> Claimed => Find(TransactionState.Claimed);
		public IEnumerable<DonationTransaction> Voided => Find(TransactionState.Voided);

		public IEnumerable<DonationTransaction> Visible => Transactions.Values.Where(t => !t.Hidden);

		[CommandProperty(AutoDonate.Access)]
		public long TotalCredit => Claimed.Aggregate(0L, (c, t) => c + t.CreditTotal);

		[CommandProperty(AutoDonate.Access)]
		public double TotalValue => Claimed.Aggregate(0.0, (c, t) => c + t.Total);

		[CommandProperty(AutoDonate.Access)]
		public int Tier
		{
			get
			{
				var tier = 0;

				if (AutoDonate.CMOptions.TierFactor <= 0.0)
				{
					return tier;
				}

				double total = TotalValue, factor = AutoDonate.CMOptions.TierFactor, req;

				while (total > 0)
				{
					req = ComputeNextTier(this, tier, factor);

					if (req <= 0 || total < req)
					{
						break;
					}

					total -= req;

					++tier;
				}

				return tier;
			}
		}

		[CommandProperty(AutoDonate.Access)]
		public long Credit { get; set; }

		public DonationProfile(IAccount account)
		{
			Transactions = new Dictionary<string, DonationTransaction>();

			Account = account;

			UID = new CryptoHashCode(CryptoHashType.MD5, Account.Username);
		}

		public DonationProfile(GenericReader reader)
		{
			Deserialize(reader);
		}

		public IEnumerable<DonationTransaction> Find(TransactionState state)
		{
			return Transactions.Values.Where(trans => trans != null && trans.State == state);
		}

		public DonationTransaction Find(string id)
		{
			return Transactions.GetValue(id);
		}

		public bool Contains(DonationTransaction trans)
		{
			return Transactions.ContainsKey(trans.ID);
		}

		public void Add(DonationTransaction trans)
		{
			if (trans != null)
			{
				AutoDonate.Transactions[trans.ID] = Transactions[trans.ID] = trans;
			}
		}

		public bool Remove(DonationTransaction trans)
		{
			return Transactions.Remove(trans.ID);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Transactions.Values.GetEnumerator();
		}

		public IEnumerator<DonationTransaction> GetEnumerator()
		{
			return Transactions.Values.GetEnumerator();
		}

		public override int GetHashCode()
		{
			return UID.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is DonationProfile && Equals((DonationProfile)obj);
		}

		public bool Equals(DonationProfile other)
		{
			return !ReferenceEquals(other, null) && (ReferenceEquals(other, this) || UID.Equals(other.UID));
		}

		public override string ToString()
		{
			return UID.ToString();
		}

		public void Serialize(GenericWriter writer)
		{
			var version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
				case 0:
				{
					writer.Write(Account);
					writer.Write(Credit);

					writer.WriteDictionary(
						Transactions,
						(w, k, v) =>
						{
							if (v == null)
							{
								w.Write(false);
							}
							else
							{
								w.Write(true);

								if (version > 0)
								{
									w.Write(v.ID);
								}
								else
								{
									v.Serialize(w);
								}
							}
						});
				}
				break;
			}
		}

		public void Deserialize(GenericReader reader)
		{
			var version = reader.GetVersion();

			switch (version)
			{
				case 1:
				case 0:
				{
					Account = reader.ReadAccount();
					Credit = reader.ReadLong();

					Transactions = reader.ReadDictionary(
						r =>
						{
							string k = null;
							DonationTransaction v = null;

							if (r.ReadBool())
							{
								if (version > 0)
								{
									k = r.ReadString();
									v = AutoDonate.Transactions.GetValue(k);
								}
								else
								{
									v = new DonationTransaction(r);
									k = v.ID;

									AutoDonate.Transactions[k] = v;
								}
							}

							return new KeyValuePair<string, DonationTransaction>(k, v);
						},
						Transactions);

					if (version < 1) // Gifts
					{
						reader.ReadDictionary(
							() =>
							{
								var k = reader.ReadString();
								var v = reader.ReadString();
								return new KeyValuePair<string, string>(k, v);
							});
					}
				}
				break;
			}
		}

		public static bool operator ==(DonationProfile l, DonationProfile r)
		{
			return ReferenceEquals(l, null) ? ReferenceEquals(r, null) : l.Equals(r);
		}

		public static bool operator !=(DonationProfile l, DonationProfile r)
		{
			return ReferenceEquals(l, null) ? !ReferenceEquals(r, null) : !l.Equals(r);
		}
	}
}