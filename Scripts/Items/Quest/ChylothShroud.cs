using System;

namespace Server.Engines.Quests.Doom
{
    public class ChylothShroud : Item
    {
        [Constructable]
        public ChylothShroud()
            : base(0x204E)
        {
            this.Hue = 0x846;
            this.Layer = Layer.OuterTorso;
        }

        public ChylothShroud(Serial serial)
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