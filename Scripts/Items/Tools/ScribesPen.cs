using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [FlipableAttribute(0x0FBF, 0x0FC0)]
    public class ScribesPen : BaseTool
    {
		public override CraftSystem CraftSystem { get { return DefInscription.CraftSystem; } }
        public override int LabelNumber { get { return 1044168; } }// scribe's pen
		
        [Constructable]
        public ScribesPen()
            : base(0x0FBF)
        {
        }

        [Constructable]
        public ScribesPen(int uses)
            : base(uses, 0x0FBF)
        {
            Weight = 1.0;
        }

        public ScribesPen(Serial serial)
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