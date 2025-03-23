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
using Server.Mobiles;
using Server.Targeting;
#endregion

namespace VitaNex.Items
{
	public class ThrowableCat : BaseThrowableAtMobile<Mobile>
	{
		[Constructable]
		public ThrowableCat()
			: this(1)
		{ }

		[Constructable]
		public ThrowableCat(int amount)
			: base(8475, amount)
		{
			Name = "Snowball";

			Weight = 2.0;
			Stackable = true;

			Consumable = true;
			AllowCombat = false;
			TargetFlags = TargetFlags.None;

			Delivery = ThrowableAtMobileDelivery.None;
			DismountUser = false;

			ThrowSound = 105;
			ImpactSound = 676;

			EffectID = ItemID;
			EffectHue = Hue;

			ThrowRecovery = TimeSpan.Zero;
		}

		public ThrowableCat(Serial serial)
			: base(serial)
		{ }

		protected override void OnThrownAt(Mobile from, Mobile target)
		{
			if (from == null || from.Deleted || target == null || target.Deleted)
			{
				return;
			}

			var cat = new Cat
			{
				Name = "Snowball"
			};

			var pmTarget = target as PlayerMobile;

			if (pmTarget != null)
			{
				if (pmTarget.Followers + cat.ControlSlots <= pmTarget.FollowersMax)
				{
					cat.Controlled = true;
					cat.ControlMaster = pmTarget;
					cat.ControlTarget = pmTarget;
					cat.ControlOrder = OrderType.Follow;
				}
			}

			cat.MoveToWorld(target.Location, target.Map);

			base.OnThrownAt(from, target);
		}

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