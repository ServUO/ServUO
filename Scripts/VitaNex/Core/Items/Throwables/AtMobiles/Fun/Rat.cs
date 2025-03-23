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
	public class ThrowableRat : BaseThrowableAtMobile<Mobile>
	{
		[Constructable]
		public ThrowableRat()
			: this(1)
		{ }

		[Constructable]
		public ThrowableRat(int amount)
			: base(8483, amount)
		{
			Name = "Reeking Rat";
			Weight = 1.0;
			Token = "Mickey's Ugly Cousin";

			Consumable = true;
			AllowCombat = false;
			TargetFlags = TargetFlags.None;

			Delivery = ThrowableAtMobileDelivery.None;
			DismountUser = false;

			ThrowSound = 206;
			ImpactSound = 204;

			EffectID = ItemID;
			EffectHue = Hue;

			ThrowRecovery = TimeSpan.Zero;
		}

		public ThrowableRat(Serial serial)
			: base(serial)
		{ }

		protected override void OnThrownAt(Mobile from, Mobile target)
		{
			if (from == null || from.Deleted || target == null || target.Deleted)
			{
				return;
			}

			var rat = new Rat
			{
				Name = "Reeking Rat"
			};

			var pmTarget = target as PlayerMobile;

			if (pmTarget != null)
			{
				if (pmTarget.Followers + rat.ControlSlots <= pmTarget.FollowersMax)
				{
					rat.Controlled = true;
					rat.ControlMaster = pmTarget;
					rat.ControlTarget = pmTarget;
					rat.ControlOrder = OrderType.Follow;
				}
			}

			rat.MoveToWorld(target.Location, target.Map);

			base.OnThrownAt(from, target);
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

			var version = reader.ReadInt();

			switch (version)
			{
				case 0:
					break;
			}
		}
	}
}