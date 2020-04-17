using System;

namespace Server.Items
{
    [Flipable(0x315C, 0x315D)]
    public class DreadFlute : BaseInstrument
    {
        public override bool IsArtifact => true;
        [Constructable]
        public DreadFlute()
            : base(0x315C, 0x58B, 0x58C)// TODO check sounds
        {
            Weight = 1.0;
            ReplenishesCharges = true;
            Hue = 0x4F2;
        }

        public DreadFlute(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075089;// Dread Flute
        public override int InitMinUses => 700;
        public override int InitMaxUses => 700;
        public override TimeSpan ChargeReplenishRate => TimeSpan.FromMinutes(15.0);
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