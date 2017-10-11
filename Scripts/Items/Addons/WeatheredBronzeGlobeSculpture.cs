using System;
using Server;

namespace Server.Items
{
    public class WeatheredBronzeGlobeSculptureAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new WeatheredBronzeGlobeSculptureDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public WeatheredBronzeGlobeSculptureAddon()
        {
            AddComponent(new AddonComponent(0x9CEF), 0, 0, 0);
        }

        public WeatheredBronzeGlobeSculptureAddon(Serial serial)
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

    public class WeatheredBronzeGlobeSculptureDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new WeatheredBronzeGlobeSculptureAddon(); } }
        public override int LabelNumber { get { return 1156881; } } // weathered bronze globe sculpture

        [Constructable]
        public WeatheredBronzeGlobeSculptureDeed()
        {
        }

        public WeatheredBronzeGlobeSculptureDeed(Serial serial)
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