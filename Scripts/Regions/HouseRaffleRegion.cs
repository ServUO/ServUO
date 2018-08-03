using System;
using System.Collections.Generic;
using Server.Items;
using Server.Spells.Sixth;
using Server.Targeting;

namespace Server.Regions
{
    public class HouseRaffleRegion : BaseRegion
    {
        private readonly HouseRaffleStone m_Stone;

        [CommandProperty(AccessLevel.GameMaster)]
        public HouseRaffleStone Stone { get { return m_Stone; } }

        public HouseRaffleRegion(HouseRaffleStone stone)
            : base(null, stone.PlotFacet, Region.DefaultPriority, stone.PlotBounds)
        {
            m_Stone = stone;
        }

        public override bool AllowHousing(Mobile from, Point3D p)
        {
            if (Stone == null)
                return false;

            if (Stone.IsExpired)
                return true;

            if (Stone.Deed == null)
                return false;

            Container pack = from.Backpack;

            if (pack != null && this.ContainsDeed(pack))
                return true;

            BankBox bank = from.FindBankNoCreate();

            if (bank != null && this.ContainsDeed(bank))
                return true;

            return false;
        }

        public override bool OnTarget(Mobile m, Target t, object o)
        {
            if (m.Spell != null && m.Spell is MarkSpell && m.IsPlayer())
            {
                m.SendLocalizedMessage(501800); // You cannot mark an object at that location.
                return false;
            }

            return base.OnTarget(m, t, o);
        }

        private bool ContainsDeed(Container cont)
        {
            List<HouseRaffleDeed> deeds = cont.FindItemsByType<HouseRaffleDeed>();

            for (int i = 0; i < deeds.Count; ++i)
            {
                if (deeds[i] == Stone.Deed)
                    return true;
            }

            return false;
        }
    }
}