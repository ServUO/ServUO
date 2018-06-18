using System;
using Server.Gumps;

namespace Server.Items
{
    public class HoodedBritanniaRobe : BaseOuterTorso
    {
        public override int LabelNumber { get { return 1125155; } } // Hooded Britannia Robe

        [Constructable]
        public HoodedBritanniaRobe(int id)
            : base(id)
        {
            LootType = LootType.Blessed;
        }

        public HoodedBritanniaRobe(Serial serial)
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
