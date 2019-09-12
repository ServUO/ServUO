using Server;
using System;
using Server.Multis;

namespace Server.Items
{
    public class PumpkinDeed : ShipCannonDeed
    {
        public override CannonPower CannonType { get { return CannonPower.Pumpkin; } }
        public override int LabelNumber { get { return 1159232; } } // Pumpkin Cannon

        [Constructable]
        public PumpkinDeed()
        {
            Hue = 1192;
        }

        public PumpkinDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class PumpkinCannon : BaseShipCannon
    {
        public override int LabelNumber { get { return 1023691; } } // cannon

        public override int Range { get { return 10; } }
        public override ShipCannonDeed GetDeed { get { return new PumpkinDeed(); } }
        public override CannonPower Power { get { return CannonPower.Pumpkin; } }

        public PumpkinCannon(BaseGalleon g)
            : base(g)
        {
        }

        public PumpkinCannon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
