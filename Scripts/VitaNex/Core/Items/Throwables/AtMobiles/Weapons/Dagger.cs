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
	public class ThrowableTrainingDagger : ThrowableDagger
	{
		private PollTimer TrainingTimer { get; set; }
		private Point3D TrainingLocation { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public double SkillGainChance { get; set; }

		[Constructable]
		public ThrowableTrainingDagger()
			: this(1)
		{ }

		[Constructable]
		public ThrowableTrainingDagger(int amount)
			: base(amount)
		{
			SkillGainChance = 25.0;

			Name = "Throwing Training Dagger";
			Usage = "Throw at a target to train your Throwing skill.";

			Hue = 2020;
			Layer = Layer.OneHanded;
			Weight = 1.0;
			Stackable = true;

			TargetFlags = TargetFlags.None;

			Consumable = false;
			Delivery = ThrowableAtMobileDelivery.None;
			DismountUser = false;

			Damages = false;
			DamageMin = 0;
			DamageMax = 0;

			ThrowRecovery = TimeSpan.Zero;

			RequiredSkillValue = 0;
		}

		public ThrowableTrainingDagger(Serial serial)
			: base(serial)
		{ }

		protected override void OnThrownAt(Mobile from, Mobile target)
		{
			base.OnThrownAt(from, target);

			if (Utility.RandomDouble() * 100 <= SkillGainChance)
			{
				from.Skills[RequiredSkill].IncreaseBase(Utility.RandomDouble());
			}

			CheckTraining(from, target);
		}

		public virtual void CheckTraining(Mobile from, Mobile target)
		{
			if (from == null || from.Deleted || target == null || target.Deleted)
			{
				if (TrainingTimer != null)
				{
					TrainingTimer.Stop();
					TrainingTimer = null;
				}

				return;
			}

			if (TrainingTimer == null)
			{
				TrainingLocation = from.Location;

				TrainingTimer = PollTimer.CreateInstance(
					TimeSpan.FromSeconds(1.0),
					() =>
					{
						if (from.Location != TrainingLocation)
						{
							TrainingTimer.Stop();
							TrainingTimer = null;
							return;
						}

						BeginThrow(from, target);
					});
			}
			else if (from.Location != TrainingLocation)
			{
				TrainingTimer.Stop();
				TrainingTimer = null;
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					writer.Write(SkillGainChance);
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
					SkillGainChance = reader.ReadDouble();
					break;
			}
		}
	}

	[Flipable(3921, 3922)]
	public class ThrowableDagger : BaseThrowableAtMobile<Mobile>
	{
		[Constructable]
		public ThrowableDagger()
			: this(1)
		{ }

		[Constructable]
		public ThrowableDagger(int amount)
			: base(Utility.RandomList(3921, 3922), amount)
		{
			Name = "Throwing Dagger";

			Layer = Layer.OneHanded;
			Weight = 1.0;
			Stackable = true;

			TargetFlags = TargetFlags.Harmful;

			Delivery = ThrowableAtMobileDelivery.AddToPack;
			DismountUser = false;

			Damages = true;
			DamageMin = 20;
			DamageMax = 40;

			ThrowSound = 1492;
			ImpactSound = 903;

			EffectID = ItemID;
			EffectHue = Hue;

			ThrowRecovery = TimeSpan.FromSeconds(60.0);
			RequiredSkillValue = 60.0;
		}

		public ThrowableDagger(Serial serial)
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