using System;
using Server.Engines.Craft;

namespace Server.Items
{
    public class Scorp : BaseTool
    {
        [Constructable]
        public Scorp()
            : base(0x10E7)
        {
            this.Weight = 1.0;
        }

        [Constructable]
        public Scorp(int uses)
            : base(uses, 0x10E7)
        {
            this.Weight = 1.0;
        }

        public Scorp(Serial serial)
            : base(serial)
        {
        }

        public override CraftSystem CraftSystem
        {
            get
            {
                return DefCarpentry.CraftSystem;
            }
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