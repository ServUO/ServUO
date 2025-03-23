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
using Server.Targeting;
#endregion

namespace VitaNex.Items
{
	public class ThrowableRock : BaseThrowableAtMobile<Mobile>
	{
		[Constructable]
		public ThrowableRock()
			: this(1)
		{ }

		[Constructable]
		public ThrowableRock(int amount)
			: base(0x11B6, amount)
		{
			Name = "Gigantic Boulder";
			Usage = "Pass The Rock!";
			Token = "Your Strength Has Increased By Over 9000!";

			Weight = 1.0;
			Stackable = true;

			AllowCombat = false;
			TargetFlags = TargetFlags.None;

			Delivery = ThrowableAtMobileDelivery.AddToPack;
			DismountUser = false;

			ThrowSound = 543;
			ImpactSound = 1613;

			EffectID = ItemID;
			EffectHue = Hue;

			ThrowRecovery = TimeSpan.Zero;
		}

		public ThrowableRock(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{ }
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
				{ }
				break;
			}
		}
	}
}