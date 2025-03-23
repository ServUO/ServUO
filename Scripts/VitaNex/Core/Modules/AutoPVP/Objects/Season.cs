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

namespace VitaNex.Modules.AutoPvP
{
	public class PvPSeason : IComparable<PvPSeason>, IEquatable<PvPSeason>
	{
		public PvPSeason(int number)
		{
			Number = number;

			Winners = new Dictionary<PlayerMobile, List<Item>>();
			Losers = new Dictionary<PlayerMobile, List<Item>>();
		}

		public PvPSeason(GenericReader reader)
		{
			Deserialize(reader);
		}

		public int Number { get; private set; }
		public DateTime? Started { get; private set; }
		public DateTime? Ended { get; private set; }

		public Dictionary<PlayerMobile, List<Item>> Winners { get; private set; }
		public Dictionary<PlayerMobile, List<Item>> Losers { get; private set; }

		public bool Active => (this == AutoPvP.CurrentSeason);

		public virtual int CompareTo(PvPSeason a)
		{
			if (a == null)
			{
				return -1;
			}

			if (Number > a.Number)
			{
				return 1;
			}

			if (Number < a.Number)
			{
				return -1;
			}

			return 0;
		}

		public virtual bool Equals(PvPSeason a)
		{
			return a != null && (ReferenceEquals(this, a) || Number == a.Number);
		}

		public override int GetHashCode()
		{
			return Number;
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public virtual void Start()
		{
			Started = DateTime.UtcNow;
		}

		public virtual void End()
		{
			Ended = DateTime.UtcNow;
		}

		public virtual void Sync()
		{
			if (Active && Started == null)
			{
				Start();
			}
			else if (!Active && Ended == null)
			{
				if (Started == null)
				{
					Start();
				}

				End();
				AutoPvP.SeasonChanged(this);
			}
		}

		public virtual void Serialize(GenericWriter writer)
		{
			var version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
				{
					writer.WriteBlockDictionary(
						Winners,
						(w, k, v) =>
						{
							w.Write(k);
							w.WriteItemList(v, true);
						});

					writer.WriteBlockDictionary(
						Losers,
						(w, k, v) =>
						{
							w.Write(k);
							w.WriteItemList(v, true);
						});
				}
				goto case 0;
				case 0:
				{
					writer.Write(Number);

					if (version < 1)
					{
						writer.WriteMobileList(Winners.Keys.ToList(), true);
						writer.WriteMobileList(Losers.Keys.ToList(), true);
					}

					if (Started != null)
					{
						writer.Write(true);
						writer.Write(Started.Value);
					}
					else
					{
						writer.Write(false);
					}

					if (Ended != null)
					{
						writer.Write(true);
						writer.Write(Ended.Value);
					}
					else
					{
						writer.Write(false);
					}
				}
				break;
			}
		}

		public virtual void Deserialize(GenericReader reader)
		{
			var version = reader.GetVersion();

			switch (version)
			{
				case 1:
				{
					Winners = reader.ReadBlockDictionary(
						r =>
						{
							var k = r.ReadMobile<PlayerMobile>();
							var v = r.ReadStrongItemList();
							return new KeyValuePair<PlayerMobile, List<Item>>(k, v);
						});

					Losers = reader.ReadBlockDictionary(
						r =>
						{
							var k = r.ReadMobile<PlayerMobile>();
							var v = r.ReadStrongItemList();
							return new KeyValuePair<PlayerMobile, List<Item>>(k, v);
						});
				}
				goto case 0;
				case 0:
				{
					Number = reader.ReadInt();

					if (version < 1)
					{
						var winners = reader.ReadStrongMobileList<PlayerMobile>();
						Winners = new Dictionary<PlayerMobile, List<Item>>(winners.Count);
						winners.ForEach(m => Winners.Add(m, new List<Item>()));

						var losers = reader.ReadStrongMobileList<PlayerMobile>();
						Losers = new Dictionary<PlayerMobile, List<Item>>(losers.Count);
						losers.ForEach(m => Losers.Add(m, new List<Item>()));
					}

					if (reader.ReadBool())
					{
						Started = reader.ReadDateTime();
					}

					if (reader.ReadBool())
					{
						Ended = reader.ReadDateTime();
					}
				}
				break;
			}
		}

		public override string ToString()
		{
			return String.Format("Season: {0}", Number);
		}

		public string ToHtmlString(Mobile viewer = null, bool big = true)
		{
			var sb = new StringBuilder();

			sb.AppendLine(ToString());
			sb.AppendLine();

			if (Started != null)
			{
				sb.AppendLine("Started: {0}", Started);
			}

			if (Ended != null)
			{
				sb.AppendLine("Ended: {0}", Ended);
			}

			if (Winners.Count > 0)
			{
				sb.AppendLine("Winners");
				sb.AppendLine();

				Winners.Keys.For(
					(i, p) => sb.AppendLine("{0}: {1}", (Numeral)(i + 1), p.Name.WrapUOHtmlColor(viewer.GetNotorietyColor(p))));
			}

			sb.AppendLine();

			return big ? String.Format("<big>{0}</big>", sb) : sb.ToString();
		}
	}
}