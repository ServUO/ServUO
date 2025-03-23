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
using Server.Network;
using Server.Targeting;
#endregion

namespace VitaNex.Items
{
	public class ThrowableAxe : BaseThrowableAtMobile<Mobile>
	{
		[CommandProperty(AccessLevel.GameMaster)]
		public bool InstantKillForced { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public int InstantKillChance { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool InstantKillHead { get; set; }

		[Constructable]
		public ThrowableAxe()
			: this(1)
		{ }

		[Constructable]
		public ThrowableAxe(int amount)
			: base(0x255D, amount)
		{
			InstantKillForced = false;
			InstantKillChance = 1;

			Name = "Throwing Axe";
			Usage = "When thrown, has a slight chance to decapitate the target.";
			Token = "BOOM! Head Shot!";

			Weight = 10.0;
			Stackable = true;
			Consumable = true;
			AllowCombat = true;

			TargetFlags = TargetFlags.Harmful;

			Delivery = ThrowableAtMobileDelivery.MoveToWorld;
			DismountUser = true;

			Damages = true;
			DamageMin = 30;
			DamageMax = 50;

			ThrowSound = 513;
			ImpactSound = 1310;

			EffectID = ItemID;
			EffectHue = Hue;

			ThrowRecovery = TimeSpan.FromMinutes(2.0);

			RequiredSkill = SkillName.Tactics;
			RequiredSkillValue = 50.0;
		}

		public ThrowableAxe(Serial serial)
			: base(serial)
		{ }

		protected virtual SeveredHead CreateHead(Mobile target)
		{
			return !InstantKillHead || target == null || !target.Body.IsHuman || target.Blessed ? null : new SeveredHead(target);
		}

		protected override void OnThrownAt(Mobile m, Mobile target)
		{
			if (m != null && target != null && (InstantKillForced || Utility.RandomDouble() < InstantKillChance / 100.0))
			{
				SeveredHead.Decapitate(m, target, CreateHead);
				m.PublicOverheadMessage(MessageType.Yell, 37, true, "BOOM! Head Shot!");
			}

			base.OnThrownAt(m, target);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(2);

			switch (version)
			{
				case 2:
				case 1:
					writer.Write(InstantKillHead);
					goto case 0;
				case 0:
				{
					writer.Write(InstantKillForced);
					writer.Write(InstantKillChance);
				}
				break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.GetVersion();

			switch (version)
			{
				case 2:
				case 1:
					InstantKillHead = reader.ReadBool();
					goto case 0;
				case 0:
				{
					InstantKillForced = reader.ReadBool();
					InstantKillChance = version < 2 ? (int)reader.ReadDouble() : reader.ReadInt();
				}
				break;
			}
		}
	}
}