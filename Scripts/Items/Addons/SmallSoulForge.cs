using Server;
using System;

namespace Server.Items
{
    public class SmallSoulForge : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new SmallSoulForgeDeed(); } }
        public override bool RetainDeedHue { get { return true; } }

        [Constructable]
        public SmallSoulForge()
        {
            AddComponent(new ForgeComponent(17607), 0, 0, 0);
        }

        public SmallSoulForge(Serial serial)
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

    public class SmallSoulForgeDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new SmallSoulForge(); } }
        public override int LabelNumber { get { return 1149695; } } 

        [Constructable]
        public SmallSoulForgeDeed()
        {
        }

        public SmallSoulForgeDeed(Serial serial)
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