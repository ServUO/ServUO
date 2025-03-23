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
#endregion

namespace VitaNex.Items
{
	public interface IMagicFoodMod
	{ }

	public interface IMagicFood
	{
		TimeSpan BuffDuration { get; set; }
		int IconID { get; set; }
		int EffectID { get; set; }
		int SoundID { get; set; }

		bool Eat(Mobile m);
		bool ApplyBuff(Mobile m);
		bool RemoveBuff(Mobile m);
	}

	public abstract class MagicFood : Food, IMagicFood
	{
		private Timer DelayTimer { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual TimeSpan BuffDuration { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int IconID { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int EffectID { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int SoundID { get; set; }

		public MagicFood(int itemID)
			: this(itemID, 1)
		{ }

		public MagicFood(int itemID, int amount)
			: base(amount, itemID)
		{
			BuffDuration = TimeSpan.FromSeconds(300.0);
		}

		public MagicFood(Serial serial)
			: base(serial)
		{ }

		public override sealed bool Eat(Mobile m)
		{
			if (m == null || m.Deleted)
			{
				return false;
			}

			if (CheckHunger(m))
			{
				m.PlaySound(Utility.Random(0x3A, 3));

				if (m.Body.IsHuman && !m.Mounted)
				{
					m.Animate(34, 5, 1, true, false, 0);
				}

				if (Poison != null)
				{
					m.ApplyPoison(Poisoner, Poison);
				}

				OnEaten(m);
				Consume();
				return true;
			}

			return false;
		}

		public abstract bool ApplyBuff(Mobile m);
		public abstract bool RemoveBuff(Mobile m);

		public override bool CheckHunger(Mobile m)
		{
			return FillFactor <= 0 || FillHunger(m, FillFactor);
		}

		protected virtual void OnEaten(Mobile m)
		{
			if (m != null && !m.Deleted)
			{
				DoApplyBuff(m);
			}
		}

		protected virtual void DoApplyBuff(Mobile m)
		{
			if (m == null || m.Deleted || !ApplyBuff(m))
			{
				return;
			}

			if (SoundID > 0)
			{
				m.PlaySound(SoundID);
			}

			if (EffectID > 0)
			{
				m.FixedParticles(EffectID, 10, 15, 5018, EffectLayer.Waist);
			}

			if (IconID > 0)
			{
				m.Send(new AddBuffPacket(m, new BuffInfo((BuffIcon)IconID, LabelNumber)));
			}

			if (DelayTimer != null && DelayTimer.Running)
			{
				DelayTimer.Stop();
			}

			DelayTimer = Timer.DelayCall(BuffDuration, DoRemoveBuff, m);
		}

		protected virtual void DoRemoveBuff(Mobile m)
		{
			if (m == null || m.Deleted || !RemoveBuff(m))
			{
				return;
			}

			if (IconID > 0)
			{
				m.Send(new RemoveBuffPacket(m, (BuffIcon)IconID));
			}

			if (DelayTimer != null && DelayTimer.Running)
			{
				DelayTimer.Stop();
			}

			DelayTimer = null;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.Write(BuffDuration);
					writer.Write(IconID);
					writer.Write(EffectID);
					writer.Write(SoundID);
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
				case 0:
				{
					BuffDuration = reader.ReadTimeSpan();
					IconID = reader.ReadInt();
					EffectID = reader.ReadInt();
					SoundID = reader.ReadInt();
				}
				break;
			}
		}
	}
}