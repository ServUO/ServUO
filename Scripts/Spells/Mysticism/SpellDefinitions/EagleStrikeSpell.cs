using System;
using Server.Targeting;

namespace Server.Spells.Mystic
{
    public class EagleStrikeSpell : MysticSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Eagle Strike", "Kal Por Xen",
            230,
            9022,
            Reagent.Bloodmoss,
            Reagent.Bone,
            Reagent.MandrakeRoot,
            Reagent.SpidersSilk);
        public EagleStrikeSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        // Hurls a magical boulder at the Target, dealing physical damage. 
        // This spell also has a chance to knockback and stun a player Target. 
        public override int RequiredMana
        {
            get
            {
                return 9;
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 20;
            }
        }
        public override void OnCast()
        {
            this.Caster.Target = new MysticSpellTarget(this, TargetFlags.Harmful);
        }

        public override void OnTarget(Object o)
        {
            Mobile target = o as Mobile;

            if (target == null)
            {
                return;
            }
            else if (this.CheckHSequence(target))
            {
                this.Caster.MovingEffect(target, 0x407A, 12, 1, false, false, 0, 1);
                this.Caster.PlaySound(0x64E);

                SpellHelper.Damage(this, target, (int)this.GetNewAosDamage(20, 1, 5, target), 0, 0, 0, 0, 100);
            }

            this.FinishSequence();
        }
    }
}
/*




*/