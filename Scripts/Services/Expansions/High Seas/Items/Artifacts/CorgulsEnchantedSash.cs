using Server;
using System;

namespace Server.Items
{
    public class CorgulsEnchantedSash : BodySash
    {
        public override int LabelNumber { get { return 1149781; } }

        [Constructable]
        public CorgulsEnchantedSash()
        {
            Attributes.BonusStam = 1;
            Attributes.DefendChance = 5;
        }

        public CorgulsEnchantedSash(Serial serial)
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