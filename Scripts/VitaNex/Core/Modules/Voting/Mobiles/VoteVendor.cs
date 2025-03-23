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
using Server;

using VitaNex.Items;
using VitaNex.Mobiles;
#endregion

namespace VitaNex.Modules.Voting
{
	public class VoteVendor : AdvancedVendor
	{
		[Constructable]
		public VoteVendor()
			: base("the vote registrar", typeof(VoteToken), "Vote Tokens", "VT")
		{ }

		public VoteVendor(Serial serial)
			: base(serial)
		{ }

		protected override void InitBuyInfo()
		{
			AddStock<ThrowableStinkBomb>(2);
			AddStock<ThrowableCat>(2);
			AddStock<ThrowableRock>(2);

			AddStock<StrobeLantern>(100);

			AddStock<BroadcastScroll>(1);
			AddStock<BroadcastScroll_3Uses>(3);
			AddStock<BroadcastScroll_5Uses>(5);
			AddStock<BroadcastScroll_10Uses>(10);
			AddStock<BroadcastScroll_30Uses>(30);
			AddStock<BroadcastScroll_50Uses>(50);
			AddStock<BroadcastScroll_100Uses>(100);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.GetVersion();

			switch (version)
			{
				case 0:
					break;
			}
		}
	}
}