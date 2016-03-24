using System;

namespace Server.Spells
{
    public class FlySpell : Spell
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Gargoyle Flight", null, -1, 9002);
        private bool m_Stop;
        public FlySpell(Mobile caster)
            : base(caster, null, m_Info)
        {
        }

        public override bool ClearHandsOnCast
        {
            get
            {
                return false;
            }
        }
        public override bool RevealOnCast
        {
            get
            {
                return false;
            }
        }
        public override double CastDelayFastScalar
        {
            get
            {
                return 0;
            }
        }
        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(.25);
            }
        }
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

        public void Stop()
        {
            this.m_Stop = true;
            this.Disturb(DisturbType.Hurt, false, false);
        }

        public override bool CheckDisturb(DisturbType type, bool checkFirst, bool resistable)
        {
            if (type == DisturbType.EquipRequest || type == DisturbType.UseRequest/* || type == DisturbType.Hurt*/)
                return false;

            return true;
        }

        public override void DoHurtFizzle()
        {
        }

        public override void DoFizzle()
        {
        }

        public override void OnDisturb(DisturbType type, bool message)
        {
            if (message && !this.m_Stop)
                this.Caster.SendLocalizedMessage(1113192); // You have been disrupted while attempting to fly!
        }

        public override void OnCast()
        {
            this.Caster.Flying = false;
            BuffInfo.RemoveBuff(this.Caster, BuffIcon.Fly);

			if (Factions.Sigil.ExistsOn(Caster))
			{
				Caster.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
				return;
			}
			else if (!Caster.CanBeginAction(typeof(Seventh.PolymorphSpell)))
			{
				Caster.SendLocalizedMessage(1061628); // You can't do that while polymorphed.
				return;
			}
			else if (Ninjitsu.AnimalForm.UnderTransformation(Caster) ||
				Mystic.StoneFormSpell.IsEffected(Caster) ||
				TransformationSpellHelper.UnderTransformation(Caster))
			{
				Caster.SendLocalizedMessage(1061091); // You cannot cast that spell in this form.
				return;
			}

            this.Caster.Animate(60, 10, 1, true, false, 0);
            this.Caster.SendLocalizedMessage(1112567); // You are flying.
            this.Caster.Flying = true;
            BuffInfo.AddBuff(this.Caster, new BuffInfo(BuffIcon.Fly, 1112567));
            this.FinishSequence();
        }
    }
}