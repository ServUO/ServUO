using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [Flipable(0x1EB8, 0x1EB9)]
    public class TinkerTools : BaseTool
    {
        [Constructable]
        public TinkerTools()
            : base(0x1EB8)
        {
            this.Weight = 1.0;
        }

        [Constructable]
        public TinkerTools(int uses)
            : base(uses, 0x1EB8)
        {
            this.Weight = 1.0;
        }

        public TinkerTools(Serial serial)
            : base(serial)
        {
        }

        public override CraftSystem CraftSystem
        {
            get
            {
                return DefTinkering.CraftSystem;
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

    public class TinkersTools : BaseTool
    {
        [Constructable]
        public TinkersTools()
            : base(0x1EBC)
        {
            this.Weight = 1.0;
        }

        [Constructable]
        public TinkersTools(int uses)
            : base(uses, 0x1EBC)
        {
            this.Weight = 1.0;
        }

        public TinkersTools(Serial serial)
            : base(serial)
        {
        }

        public override CraftSystem CraftSystem
        {
            get
            {
                return DefTinkering.CraftSystem;
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