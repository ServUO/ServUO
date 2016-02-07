using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Multis
{
    public class MageCamp : BaseCamp
    {
        [Constructable]
        public MageCamp()
            : base(0x1F5)
        {
        }

        public MageCamp(Serial serial)
            : base(serial)
        {
        }

        public override void AddComponents()
        {
            BaseDoor west, east;

            this.AddItem(west = new LightWoodGate(DoorFacing.WestCW), -4, 4, 7);
            this.AddItem(east = new LightWoodGate(DoorFacing.EastCCW), -3, 4, 7);

            west.Link = east;
            east.Link = west;

            this.AddItem(new Sign(SignType.Mage, SignFacing.West), -5, 5, -4);

            this.AddMobile(new Mage(), 4, -4, 3, 7);
            this.AddMobile(new Mage(), 5, 4, -2, 0);
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