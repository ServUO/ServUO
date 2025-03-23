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
using System.Drawing;

using Server;
using Server.Items;
using Server.Mobiles;

using VitaNex.Network;
#endregion

namespace VitaNex.TimeBoosts
{
	[Flipable(4173, 4174)]
	public class TimeBoostToken : Item
	{
		private ITimeBoost _Boost;

		[CommandProperty(AccessLevel.Counselor, true)]
		public ITimeBoost Boost
		{
			get => _Boost ?? (_Boost = TimeBoosts.MinValue);
			private set
			{
				if (_Boost == value)
				{
					return;
				}

				_Boost = value ?? TimeBoosts.MinValue;
				InvalidateProperties();
			}
		}

		[CommandProperty(AccessLevel.Administrator)]
		public TimeSpan Value { get => Boost.Value; set => Boost = TimeBoosts.Find(value); }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public override int Hue
		{
			get
			{
				if (World.Saving)
				{
					return base.Hue;
				}

				var hue = base.Hue;

				return hue > 0 ? hue : Boost.Hue;
			}
			set => base.Hue = value;
		}

		public override string DefaultName => Boost.Name;

		[Constructable(AccessLevel.Administrator)]
		public TimeBoostToken()
			: this(TimeBoosts.RandomValue, 1)
		{ }

		[Constructable(AccessLevel.Administrator)]
		public TimeBoostToken(string time)
			: this(time, 1)
		{ }

		[Constructable(AccessLevel.Administrator)]
		public TimeBoostToken(string time, int amount)
			: this(TimeSpan.Parse(time), amount)
		{ }

		public TimeBoostToken(TimeSpan time)
			: this(TimeBoosts.Find(time))
		{ }

		public TimeBoostToken(TimeSpan time, int amount)
			: this(TimeBoosts.Find(time), amount)
		{ }

		public TimeBoostToken(ITimeBoost boost)
			: this(boost, 1)
		{ }

		public TimeBoostToken(ITimeBoost boost, int amount)
			: base(Utility.RandomList(4173, 4174))
		{
			Boost = boost;

			Stackable = true;
			Amount = Math.Max(1, Math.Min(60000, amount));

			LootType = LootType.Blessed;
		}

		public TimeBoostToken(Serial serial)
			: base(serial)
		{ }

		public override bool StackWith(Mobile m, Item dropped, bool playSound)
		{
			if (!(dropped is TimeBoostToken) || ((TimeBoostToken)dropped).Boost != Boost)
			{
				return false;
			}

			return base.StackWith(m, dropped, playSound);
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			new ExtendedOPL(list)
			{
				{"Use: Credits {0:#,0} {1} to your account".WrapUOHtmlColor(Color.LawnGreen), Amount, Boost},
				"\"Time Boosts reduce the time required to do certain things\"".WrapUOHtmlColor(Color.Gold)
			}.Apply();
		}

		public override void OnDoubleClick(Mobile m)
		{
			if (!this.CheckDoubleClick(m, true, false, -1, true, false, false) || !(m is PlayerMobile))
			{
				return;
			}

			var user = (PlayerMobile)m;

			if (TimeBoosts.Credit(user, Boost, Amount))
			{
				TimeBoostsUI.Update(user);

				m.SendMessage(85, "{0:#,0} {1} has been credited to your account.", Amount, Boost);
				Delete();
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);

			writer.Write(Boost);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();

			Boost = reader.ReadTimeBoost();
		}
	}
}
