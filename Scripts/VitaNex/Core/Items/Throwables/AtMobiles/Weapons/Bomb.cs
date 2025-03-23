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
using System.Linq;

using Server;
using Server.Items;

using VitaNex.FX;
#endregion

namespace VitaNex.Items
{
	[Flipable(8790, 8791)]
	public class ThrowableBomb : BaseThrowableAtMobile<Mobile>
	{
		[CommandProperty(AccessLevel.GameMaster)]
		public int ExplosionRange { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public override string Usage
		{
			get => !String.IsNullOrWhiteSpace(base.Usage)
					? base.Usage
					: String.Format(
						"Cause an explosion at the target which will deal {0:#,0} to {1:#,0} damage to all enemies within {2:#,0} paces.",
						DamageMin,
						DamageMax,
						ExplosionRange);
			set => base.Usage = value;
		}

		[Constructable]
		public ThrowableBomb()
			: this(1)
		{ }

		[Constructable]
		public ThrowableBomb(int amount)
			: base(Utility.RandomList(8790, 8791), amount)
		{
			Name = "Bobomb";
			Token = "A.C.M.E";
			Hue = 2104;
			Weight = 10.0;
			Stackable = true;

			Delivery = ThrowableAtMobileDelivery.None;

			Damages = true;
			DamageMin = 25;
			DamageMax = 50;

			ThrowSound = 1491;
			ImpactSound = Utility.RandomList(776, 777);

			EffectID = ItemID;
			EffectHue = Hue;

			ThrowRecovery = TimeSpan.FromSeconds(60.0);

			ExplosionRange = 5;

			RequiredSkillValue = 100.0;
		}

		public ThrowableBomb(Serial serial)
			: base(serial)
		{ }

		protected override void OnThrownAt(Mobile from, Mobile target)
		{
			if (from == null || from.Deleted || target == null || target.Deleted)
			{
				return;
			}

			var fx = new FireExplodeEffect(target, target.Map, ExplosionRange)
			{
				EffectHandler = e =>
				{
					if (e.ProcessIndex != 0)
					{
						ExplosionDamage(e);
					}
				}
			};

			fx.Send();

			base.OnThrownAt(from, target);
		}

		public virtual void ExplosionDamage(EffectInfo info)
		{
			Effects.PlaySound(info.Source.Location, info.Map, ImpactSound);

			foreach (var m in info.Source.Location.GetMobilesInRange(info.Map, 0)
								  .Where(m => m != null && !m.Deleted && User.CanBeHarmful(m, false, true)))
			{
				m.PlaySound(ImpactSound);
				m.Damage(Utility.RandomMinMax(DamageMin, DamageMax), User, true);
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
				case 0:
					writer.Write(ExplosionRange);
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.GetVersion();

			switch (version)
			{
				case 1:
				case 0:
				{
					ExplosionRange = reader.ReadInt();

					if (version < 1)
					{
						reader.ReadInt();
						reader.ReadInt();
					}
				}
				break;
			}
		}
	}
}