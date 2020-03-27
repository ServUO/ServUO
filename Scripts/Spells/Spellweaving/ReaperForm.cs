using System;
using Server.Network;

namespace Server.Spells.Spellweaving
{
    public class ReaperFormSpell : ArcaneForm
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Reaper Form", "Tarisstree", -1);
        public ReaperFormSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(2.5);
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
                return 34;
            }
        }
        public override int Body
        {
            get
            {
                return 0x11D;
            }
        }
        public override int FireResistOffset
        {
            get
            {
                return -25;
            }
        }
        public override int PhysResistOffset
        {
            get
            {
                return 5 + this.FocusLevel;
            }
        }
        public override int ColdResistOffset
        {
            get
            {
                return 5 + this.FocusLevel;
            }
        }
        public override int PoisResistOffset
        {
            get
            {
                return 5 + this.FocusLevel;
            }
        }
        public override int NrgyResistOffset
        {
            get
            {
                return 5 + this.FocusLevel;
            }
        }
        public virtual int SwingSpeedBonus
        {
            get
            {
                return 10 + this.FocusLevel;
            }
        }
        public virtual int SpellDamageBonus
        {
            get
            {
                return 10 + this.FocusLevel;
            }
        }

        public override void DoEffect(Mobile m)
        {
            m.PlaySound(0x1BA);

            BuffInfo.AddBuff(Caster, new BuffInfo(BuffIcon.ReaperForm, 1071034, 1153781, "10\t10\t5\t5\t5\t5\t25"));
        }

        public override void RemoveEffect(Mobile m)
        {
            BuffInfo.RemoveBuff(m, BuffIcon.ReaperForm);
        }
    }
}
