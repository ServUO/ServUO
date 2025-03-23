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

using Server;
using Server.Items;
using Server.Network;
#endregion

namespace VitaNex.Items
{
	public class BoundsPreviewTile : Static
	{
		public override bool IsVirtualItem => true;
		public override bool DisplayLootType => false;
		public override bool DisplayWeight => false;
		public override double DefaultWeight => 0;
		public override bool Decays => true;
		public override TimeSpan DecayTime => TimeSpan.FromMinutes(10.0);

		public BoundsPreviewTile(string name, int hue)
			: base(9272, 1)
		{
			Name = name;
			Hue = hue;
			Movable = false;
		}

		public BoundsPreviewTile(Serial serial)
			: base(serial)
		{ }

		public override void SendInfoTo(NetState state, bool sendOplPacket)
		{
			if (state == null || state.Mobile == null || state.Mobile.AccessLevel >= AccessLevel.Counselor)
			{
				base.SendInfoTo(state, sendOplPacket);
			}
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			list.Add(String.Format("({0}, {1}, {2})", X, Y, Z));
		}
		
		public override void OnAosSingleClick(Mobile from)
		{
			OnSingleClick(from);
		}

		public override void OnSingleClick(Mobile from)
		{
			base.OnSingleClick(from);

			LabelTo(from, String.Format("{0} ({1}, {2}, {3})", this.ResolveName(from.GetLanguage()), X, Y, Z));
		}

		public override void Serialize(GenericWriter writer)
		{ }

		public override void Deserialize(GenericReader reader)
		{
			Delete();
		}
	}
}
