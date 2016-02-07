using System;
using Server.Mobiles;

namespace Server.Spells.Eighth
{
    public class AirElementalSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Air Elemental", "Kal Vas Xen Hur",
            269,
            9010,
            false,
            Reagent.Bloodmoss,
            Reagent.MandrakeRoot,
            Reagent.SpidersSilk);
        public AirElementalSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Eighth;
            }
        }
        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            if ((this.Caster.Followers + 2) > this.Caster.FollowersMax)
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
                TimeSpan duration = TimeSpan.FromSeconds((2 * this.Caster.Skills.Magery.Fixed) / 5);

                if (Core.AOS)
                    SpellHelper.Summon(new SummonedAirElemental(), this.Caster, 0x217, duration, false, false);
                else
                    SpellHelper.Summon(new AirElemental(), this.Caster, 0x217, duration, false, false);
            }

            this.FinishSequence();
        }
    }
}