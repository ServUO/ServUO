using System;
using Server.Mobiles;

namespace Server.Spells.Mystic
{
    public class RisingColossusSpell : MysticSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Rising Colossus", "Kal Vas Xen Corp Ylem",
            230,
            9022,
            Reagent.DaemonBone,
            Reagent.DragonBlood,
            Reagent.FertileDirt,
            Reagent.Nightshade);
        public RisingColossusSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        // When this spell is invoked, a weapon is conjured and animated. This weapon attacks nearby foes. 
        // Shame you cannot target a weapon/armor and animate it Diablo II's Summon Steel Golem style, it would be retardly simple too, just equip the mobile with the item and mark it unmovable.
        public override int RequiredMana
        {
            get
            {
                return 50;
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 83.0;
            }
        }
        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            if (this.Caster.Followers + 5 > this.Caster.FollowersMax)
            {
                this.Caster.SendLocalizedMessage(1049645); // You have too many followers to summon that creature.
                return false;
            }

            return true;
        }

        public override void OnCast()
        {
            if (this.CheckSequence())
            {
                TimeSpan duration = TimeSpan.FromSeconds((2 * this.Caster.Skills[SkillName.Imbuing].Fixed) / 5);
                SpellHelper.Summon(new RisingColossus(), this.Caster, 0x216, duration, false, false);
            }

            this.FinishSequence();
        }
    }
}