using Server.Mobiles;
using Server.Network;
using Server.Spells.Mysticism;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;
using Server.Spells.Seventh;

using System;

namespace Server.Spells
{
	public class FlySpell : Spell
	{
		private static readonly SpellInfo m_Info = new SpellInfo("Gargoyle Flight", null, -1, 9002);

		public static bool CheckFlyingAllowed(Mobile mob, bool message)
		{
			if (mob.Region != null && !mob.Region.AllowFlying(mob))
			{
				mob.SendMessage("You may not fly here.");
				return false;
			}

			return true;
		}

		public override bool ClearHandsOnCast => false;
		public override bool RevealOnCast => false;
		public override double CastDelayFastScalar => 0;
		public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(2.0);
		public override bool CheckNextSpellTime => false;

		public FlySpell(Mobile caster)
			: base(caster, null, m_Info)
		{ }

		public override void SayMantra()
		{ }

		public override TimeSpan GetCastRecovery()
		{
			return TimeSpan.Zero;
		}

		public override int GetMana()
		{
			return 0;
		}

		public override bool ConsumeReagents()
		{
			return true;
		}

		public override bool CheckFizzle()
		{
			return true;
		}

		public override bool CheckDisturb(DisturbType type, bool checkFirst, bool resistable)
		{
			if (Caster.Flying)
				return false;

			if (type == DisturbType.EquipRequest || type == DisturbType.UseRequest)
				return false;

			return true;
		}

		public override void OnDisturb(DisturbType type, bool message)
		{
			if (Caster.Flying)
				return;

			if (message)
				Caster.SendLocalizedMessage(1113192); // You have been disrupted while attempting to fly!
		}

		public override bool CheckCast()
		{
			if (Caster.Flying)
				return true;

			if (!CheckFlyingAllowed(Caster, true))
				return false;

			if (!BaseMount.CheckMountAllowed(Caster, true, true))
				return false;

			if (!Caster.Alive)
			{
				Caster.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1113082); // You may not fly while dead.
				return false;
			}

			if (!Caster.CanBeginAction(typeof(PolymorphSpell)))
			{
				Caster.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1112453); // You can't fly in your current form!
				return false;
			}

			if (AnimalForm.UnderTransformation(Caster) || StoneFormSpell.IsEffected(Caster) || (Caster.IsBodyMod && !Caster.Body.IsHuman))
			{
				Caster.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1112453); // You can't fly in your current form!
				return false;
			}

			if (TransformationSpellHelper.UnderTransformation(Caster) && !TransformationSpellHelper.UnderTransformation(Caster, typeof(VampiricEmbraceSpell)))
			{
				Caster.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1112453); // You can't fly in your current form!
				return false;
			}

			return true;
		}

		public override void OnCast()
		{
			FinishSequence();

			Caster.Flying = !Caster.Flying;
		}
	}
}
