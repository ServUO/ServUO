using System;
using Server.Factions;

namespace Server.Ethics.Evil
{
    public sealed class EvilEthic : Ethic
    {
        public EvilEthic()
        {
            this.m_Definition = new EthicDefinition(
                0x455,
                "Evil", "(Evil)",
                "I am evil incarnate",
                new Power[]
                {
                    new UnholySense(),
                    new UnholyItem(),
                    new SummonFamiliar(),
                    new VileBlade(),
                    new BlightPower(),
                    new UnholyShield(),
                    new UnholySteed(),
                    new UnholyWord()
                });
        }

        public override bool IsEligible(Mobile mob)
        {
            Faction fac = Faction.Find(mob);

            return (fac is Minax || fac is Shadowlords);
        }
    }
}