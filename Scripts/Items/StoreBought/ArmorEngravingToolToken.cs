using System;
using Server.Gumps;

namespace Server.Items
{
    public class ArmorEngravingToolToken : PromotionalToken
    {
        public override TextDefinition ItemName { get { return 1080547; } } // Armor Engraving Tool
        public override TextDefinition ItemReceiveMessage { get { return 1072223; } } // An item has been placed in your backpack.
        public override TextDefinition ItemGumpName { get { return 1071163; } } // <center>Armor Engraving Tool</center>

        public override bool PlaceInBank { get { return false; } }

        [Constructable]
        public ArmorEngravingToolToken()
        {
        }

        public override Item CreateItemFor(Mobile from)
        {
            return new ArmorEngravingTool();
        }

        public ArmorEngravingToolToken(Serial serial)
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
