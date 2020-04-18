using Server.Multis;

namespace Server.Items
{
    public class PumpkinDeed : ShipCannonDeed
    {
        public override CannonPower CannonType => CannonPower.Pumpkin;
        public override int LabelNumber => 1159232;  // Pumpkin Cannon

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
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class PumpkinCannon : BaseShipCannon
    {
        public override int LabelNumber => 1023691;  // cannon

        public override int Range => 10;
        public override ShipCannonDeed GetDeed => new PumpkinDeed();
        public override CannonPower Power => CannonPower.Pumpkin;

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
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
