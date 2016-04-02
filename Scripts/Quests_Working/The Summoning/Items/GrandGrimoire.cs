using System;

namespace Server.Engines.Quests.Doom
{
    public class GrandGrimoire : Item
    {
        [Constructable]
        public GrandGrimoire()
            : base(0xEFA)
        {
            this.Weight = 1.0;
            this.Hue = 0x835;
            this.Layer = Layer.OneHanded;
            this.LootType = LootType.Blessed;
        }

        public GrandGrimoire(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1060801;
            }
        }// The Grand Grimoire
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