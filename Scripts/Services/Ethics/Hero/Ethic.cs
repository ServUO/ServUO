using System;
using Server.Factions;

namespace Server.Ethics.Hero
{
    public sealed class HeroEthic : Ethic
    {
        public HeroEthic()
        {
            this.m_Definition = new EthicDefinition(
                0x482,
                "Hero", "(Hero)",
                "I will defend the virtues",
                new Power[]
                {
                    new HolySense(),
                    new HolyItem(),
                    new SummonFamiliar(),
                    new HolyBlade(),
                    new Bless(),
                    new HolyShield(),
                    new HolySteed(),
                    new HolyWord()
                });
        }

        public override bool IsEligible(Mobile mob)
        {
            if (mob.Murderer)
                return false;

            Faction fac = Faction.Find(mob);

            return (fac is TrueBritannians || fac is CouncilOfMages);
        }
    }
}