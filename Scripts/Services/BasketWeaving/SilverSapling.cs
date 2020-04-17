using Server.ContextMenus;
using Server.Mobiles;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class SilverSapling : Item
    {
        public override int LabelNumber => 1113052;  // The Silver Sapling

        [Constructable]
        public SilverSapling()
            : base(0x0CE3)
        {
            Hue = 1150;
            Movable = false;
        }

        public SilverSapling(Serial serial)
            : base(serial)
        {
        }

        public override bool HandlesOnMovement => true;

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (Parent == null && Utility.InRange(Location, m.Location, 1) && !Utility.InRange(Location, oldLocation, 1))
                Ankhs.Resurrect(m, this);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            Ankhs.GetContextMenuEntries(from, this, list);
        }

        public override void OnDoubleClickDead(Mobile m)
        {
            Ankhs.Resurrect(m, this);
        }

        public override void OnDoubleClick(Mobile m)
        {
            PlayerMobile pm = m as PlayerMobile;

            if (pm != null && pm.Backpack != null)
            {
                if (pm.SSNextSeed > DateTime.UtcNow)
                {
                    pm.SendLocalizedMessage(1113042); // You must wait a full day before receiving another Seed of the Silver Sapling
                }
                else
                {
                    pm.SendLocalizedMessage(1113043); // The Silver Sapling pulses with light, and a shining seed appears in your hands.
                    pm.SSNextSeed = DateTime.UtcNow + TimeSpan.FromDays(1.0);
                    pm.PlaceInBackpack(new SilverSaplingSeed());
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class SilverSaplingSeed : Item
    {
        public override int LabelNumber => 1113053;  // a seed of the Silver Sapling

        [Constructable]
        public SilverSaplingSeed()
            : base(0x0DCF)
        {
            Hue = 1150;
            Stackable = true;
        }

        public SilverSaplingSeed(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
            PlayerMobile pm = m as PlayerMobile;

            if (pm == null)
                return;

            if (pm.Region.IsPartOf("Abyss"))
            {
                pm.SendLocalizedMessage(1113056, "", 0x3C); // The seed disappears into the earth and for a brief moment you see a vision of a small sapling growing before you. Should you perish in your adventures in the Abyss, you shall be restored to this place with your possessions.
                Consume();
                pm.SSSeedLocation = pm.Location;
                pm.SSSeedMap = pm.Map;
                pm.SSSeedExpire = DateTime.UtcNow + TimeSpan.FromDays(1.0);
            }
            else
            {
                pm.SendLocalizedMessage(1113055, "", 0x23); // The seed of the Silver Sapling can only be planted within the Stygian Abyss...
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
