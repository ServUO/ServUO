using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [FlipableAttribute(0x0FBF, 0x0FC0)]
    public class MapmakersPen : BaseTool
    {
        [Constructable]
        public MapmakersPen()
            : base(0x0FBF)
        {
            this.Weight = 1.0;
        }

        [Constructable]
        public MapmakersPen(int uses)
            : base(uses, 0x0FBF)
        {
            this.Weight = 1.0;
        }

        public MapmakersPen(Serial serial)
            : base(serial)
        {
        }

        public override CraftSystem CraftSystem
        {
            get
            {
                return DefCartography.CraftSystem;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1044167;
            }
        }// mapmaker's pen
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.Weight == 2.0)
                this.Weight = 1.0;
        }
    }
}