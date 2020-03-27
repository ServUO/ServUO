using System;
using Server.Items;

namespace Server.Spells.Necromancy
{
    public abstract class NecromancerSpell : Spell
    {
        public NecromancerSpell(Mobile caster, Item scroll, SpellInfo info)
            : base(caster, scroll, info)
        {
        }

        public abstract double RequiredSkill { get; }
        public abstract int RequiredMana { get; }
        public override SkillName CastSkill
        {
            get
            {
                return SkillName.Necromancy;
            }
        }
        public override SkillName DamageSkill
        {
            get
            {
                return SkillName.SpiritSpeak;
            }
        }

        public override bool ClearHandsOnCast
        {
            get
            {
                return false;
            }
        }

        public override int ComputeKarmaAward()
        {
            //TODO: Verify this formula being that Necro spells don't HAVE a circle.
            //int karma = -(70 + (10 * (int)Circle));
            int karma = -(40 + (int)(10 * (this.CastDelayBase.TotalSeconds / this.CastDelaySecondsPerTick)));

            karma += AOS.Scale(karma, AosAttributes.GetValue(this.Caster, AosAttribute.IncreasedKarmaLoss));

            return karma;
        }

        public override void GetCastSkills(out double min, out double max)
        {
            min = this.RequiredSkill;
            max = this.Scroll != null ? min : this.RequiredSkill + 40.0;
        }

        public override bool ConsumeReagents()
        {
            if (base.ConsumeReagents())
                return true;

            if (ArcaneGem.ConsumeCharges(this.Caster, 1))
                return true;

            return false;
        }

        public override int GetMana()
        {
            return this.RequiredMana;
        }
    }
}
