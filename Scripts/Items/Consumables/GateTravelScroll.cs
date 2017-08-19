using System;
using Server.Multis;

namespace Server.Items
{
    public class GateTravelScroll : SpellScroll
    {
        [Constructable]
        public GateTravelScroll()
            : this(1)
        {
        }

        [Constructable]
        public GateTravelScroll(int amount)
            : base(51, 0x1F60, amount)
        {
        }

        public override bool DropToWorld(Mobile m, Point3D p)
        {
            if (m.Map == null || m.Map == Map.Internal)
                return false;

            IPooledEnumerable eable = m.Map.GetItemsInRange(p, 0);

            foreach (Item item in eable)
            {
                if (item is HouseTeleporterTile)
                {
                    var tile = item as HouseTeleporterTile;

                    if (tile.UsesCharges)
                    {
                        if (tile.Charges >= HouseTeleporterTile.MaxCharges)
                        {
                            m.SendLocalizedMessage(1115126); // The House Teleporter cannot be charged any further.
                        }
                        else
                        {
                            int left = HouseTeleporterTile.MaxCharges - tile.Charges;
                            int scrollsNeeded = Math.Max(1, left / 5);

                            if (Amount <= scrollsNeeded)
                            {
                                tile.Charges = Math.Min(HouseTeleporterTile.MaxCharges, tile.Charges + (Amount * 5));
                                Delete();
                            }
                            else
                            {
                                Amount -= scrollsNeeded;
                                tile.Charges = HouseTeleporterTile.MaxCharges;
                            }

                            m.SendLocalizedMessage(1115127); // The Gate Travel scroll crumbles to dust as it strengthens the House Teleporter.
                        }

                        eable.Free();
                        return false;
                    }
                }
            }

            eable.Free();

            return base.DropToWorld(m, p);
        }

        public GateTravelScroll(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}