using System;

namespace Server.Items
{
    public class ScatteredCrystals : PeerlessKey
    {
        [Constructable]
        public ScatteredCrystals()
            : base(0x2248)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 1;
            this.Hue = 0x47E;
        }

        public ScatteredCrystals(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074264;
            }
        }// scattered crystals
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