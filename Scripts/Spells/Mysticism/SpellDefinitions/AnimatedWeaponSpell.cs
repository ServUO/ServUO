using System;
using Server.Mobiles;

namespace Server.Spells.Mystic
{
    public class AnimatedWeaponSpell : MysticSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Animated Weapon", "In Jux Por Ylem",
            230,
            9022,
            Reagent.BlackPearl,
            Reagent.MandrakeRoot,
            Reagent.Nightshade,
            Reagent.Bone);
        public AnimatedWeaponSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        // When this spell is invoked, a weapon is conjured and animated. This weapon attacks nearby foes. 
        // Shame you cannot target a weapon/armor and animate it Diablo II's Summon Steel Golem style, it would be retardly simple too, just equip the mobile with the item and mark it unmovable.
        public override int RequiredMana
        {
            get
            {
                return 11;
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 33.0;
            }
        }
        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            if (this.Caster.Followers + 4 > this.Caster.FollowersMax)
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
                SpellHelper.Summon(new AnimatedWeapon(), this.Caster, 0x216, duration, false, false);
            }

            this.FinishSequence();
        }
    }
}