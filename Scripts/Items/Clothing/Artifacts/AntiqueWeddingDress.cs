using System;

namespace Server.Items
{
    public class AntiqueWeddingDress : PlainDress
    {
        [Constructable]
        public AntiqueWeddingDress()
        {
			this.Hue = 2961; // Hue probably not exact
			this.Name = ("An Antique Wedding Dress");
        }

        public AntiqueWeddingDress(Serial serial)
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