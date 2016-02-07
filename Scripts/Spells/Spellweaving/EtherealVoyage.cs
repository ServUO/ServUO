using System;

namespace Server.Spells.Spellweaving
{
    public class EtherealVoyageSpell : ArcaneForm
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Ethereal Voyage", "Orlavdra",
            -1);
        public EtherealVoyageSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(3.5);
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 24.0;
            }
        }
        public override int RequiredMana
        {
            get
            {
                return 32;
            }
        }
        public override int Body
        {
            get
            {
                return 0x302;
            }
        }
        public override int Hue
        {
            get
            {
                return 0x48F;
            }
        }
        public static void Initialize()
        {
            EventSink.AggressiveAction += new AggressiveActionEventHandler(delegate(AggressiveActionEventArgs e)
            {
                if (TransformationSpellHelper.UnderTransformation(e.Aggressor, typeof(EtherealVoyageSpell)))
                {
                    TransformationSpellHelper.RemoveContext(e.Aggressor, true);
                }
            });
        }

        public override bool CheckCast()
        {
            if (TransformationSpellHelper.UnderTransformation(this.Caster, typeof(EtherealVoyageSpell)))
            {
                this.Caster.SendLocalizedMessage(501775); // This spell is already in effect.
            }
            else if (!this.Caster.CanBeginAction(typeof(EtherealVoyageSpell)))
            {
                this.Caster.SendLocalizedMessage(1075124); // You must wait before casting that spell again.
            }
            else if (this.Caster.Combatant != null)
            {
                this.Caster.SendLocalizedMessage(1072586); // You cannot cast Ethereal Voyage while you are in combat.
            }
            else
            {
                return base.CheckCast();
            }

            return false;
        }

        public override void DoEffect(Mobile m)
        {
            m.PlaySound(0x5C8);
            m.SendLocalizedMessage(1074770); // You are now under the effects of Ethereal Voyage.

            double skill = this.Caster.Skills.Spellweaving.Value;

            TimeSpan duration = TimeSpan.FromSeconds(12 + (int)(skill / 24) + (this.FocusLevel * 2));

            Timer.DelayCall<Mobile>(duration, new TimerStateCallback<Mobile>(RemoveEffect), this.Caster);

            this.Caster.BeginAction(typeof(EtherealVoyageSpell));	//Cannot cast this spell for another 5 minutes(300sec) after effect removed.

            BuffInfo.AddBuff(this.Caster, new BuffInfo(BuffIcon.EtherealVoyage, 1031613, 1075805, duration, this.Caster));
        }

        public override void RemoveEffect(Mobile m)
        {
            m.SendLocalizedMessage(1074771); // You are no longer under the effects of Ethereal Voyage.

            TransformationSpellHelper.RemoveContext(m, true);

            Timer.DelayCall(TimeSpan.FromMinutes(5), delegate
            {
                m.EndAction(typeof(EtherealVoyageSpell));
            });

            BuffInfo.RemoveBuff(m, BuffIcon.EtherealVoyage);
        }
    }
}