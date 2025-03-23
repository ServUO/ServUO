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
using Server.Targeting;
#endregion

namespace VitaNex.Items
{
	public abstract class ThrowableFurniture : BaseThrowableAtMobile<Mobile>, IDyable
	{
		public ThrowableFurniture(int itemID)
			: base(itemID, 1)
		{
			Weight = 10.0;

			AllowCombat = true;
			AllowDeadUser = false;
			Consumable = true;
			ClearHands = true;
			DismountUser = true;

			Damages = false;
			Heals = false;

			ThrowRange = 8;

			ThrowSound = 1491;
			ImpactSound = 1335;

			RequiredSkillValue = 0.0;

			TargetFlags = TargetFlags.None;

			Delivery = ThrowableAtMobileDelivery.None;

			ThrowRecovery = TimeSpan.Zero;
		}

		public ThrowableFurniture(Serial serial)
			: base(serial)
		{ }

		public virtual bool Dye(Mobile m, DyeTub sender)
		{
			return m != null && sender is FurnitureDyeTub;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}
}