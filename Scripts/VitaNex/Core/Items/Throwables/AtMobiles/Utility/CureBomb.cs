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
	public class ThrowableCureBomb : BaseThrowableAtMobile<Mobile>
	{
		[CommandProperty(AccessLevel.GameMaster)]
		public int ExplosionRange { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public override string Usage
		{
			get => !String.IsNullOrWhiteSpace(base.Usage)
					? base.Usage
					: String.Format(
						"Cause an explosion at the target which will attempt to cure all allies within {0:#,0} paces.",
						ExplosionRange);
			set => base.Usage = value;
		}

		[Constructable]
		public ThrowableCureBomb()
			: this(1)
		{ }

		[Constructable]
		public ThrowableCureBomb(int amount)
			: base(Utility.RandomList(8790, 8791), amount)
		{
			Name = "Curing Bobomb";
			Usage = String.Empty;
			Hue = 74;
			Weight = 10.0;
			Stackable = true;

			Delivery = ThrowableAtMobileDelivery.None;

			ThrowSound = 1491;
			ImpactSound = Utility.RandomList(776, 777);

			EffectID = ItemID;
			EffectHue = Hue;

			ThrowRecovery = TimeSpan.FromSeconds(60.0);

			ExplosionRange = 5;

			RequiredSkillValue = 100.0;
		}

		public ThrowableCureBomb(Serial serial)
			: base(serial)
		{ }

		protected override void OnThrownAt(Mobile from, Mobile target)
		{
			if (from == null || from.Deleted || target == null || target.Deleted)
			{
				return;
			}

			var fx = new PoisonExplodeEffect(target, target.Map, ExplosionRange)
			{
				EffectHandler = e =>
				{
					if (e.ProcessIndex != 0)
					{
						ExplosionCure(e);
					}
				}
			};

			fx.Send();

			base.OnThrownAt(from, target);
		}

		public virtual void ExplosionCure(EffectInfo info)
		{
			Effects.PlaySound(info.Source.Location, info.Map, ImpactSound);

			var targets = info.Source.Location.GetMobilesInRange(info.Map, 0);

			foreach (var m in targets.Where(m => m != null && !m.Deleted && User.CanBeBeneficial(m, false, false)))
			{
				m.PlaySound(ImpactSound);
				m.CurePoison(User);
			}

			targets.Free(true);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
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
				case 0:
					ExplosionRange = reader.ReadInt();
					break;
			}
		}
	}
}