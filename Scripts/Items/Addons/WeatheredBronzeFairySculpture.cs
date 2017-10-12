using System;
using Server;

namespace Server.Items
{
    [Flipable(0x9D09, 0x9D0A)]
    public class WeatheredBronzeFairySculptureComponent : AddonComponent
    {
        public override int LabelNumber { get { return 1156883; } } // weathered bronze fairy sculpture

        public WeatheredBronzeFairySculptureComponent()
            : base(0x9D09)
        {
        }

        public WeatheredBronzeFairySculptureComponent(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }

    public class WeatheredBronzeFairySculptureAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new WeatheredBronzeFairySculptureDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public WeatheredBronzeFairySculptureAddon()
        {
            AddComponent(new WeatheredBronzeFairySculptureComponent(), 0, 0, 0);
        }

        public WeatheredBronzeFairySculptureAddon(Serial serial)
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

    public class WeatheredBronzeFairySculptureDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new WeatheredBronzeFairySculptureAddon(); } }
        public override int LabelNumber { get { return 1156883; } } // weathered bronze fairy sculpture

        [Constructable]
        public WeatheredBronzeFairySculptureDeed()
        {
        }

        public WeatheredBronzeFairySculptureDeed(Serial serial)
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