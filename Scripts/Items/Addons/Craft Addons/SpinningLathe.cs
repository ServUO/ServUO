using System;
using Server;
using Server.Engines.Craft;
using System.Collections.Generic;

namespace Server.Items
{
    public class SpinningLathe : CraftAddon
    {
        public override BaseAddonDeed Deed { get { return new SpinningLatheDeed(Tools.Count > 0 ? Tools[0].UsesRemaining : 0); } }
        public override CraftSystem CraftSystem { get { return DefCarpentry.CraftSystem; } }

        [Constructable]
        public SpinningLathe(bool south, int uses)
        {
            if (south)
            {
                AddCraftComponent(new AddonToolComponent(CraftSystem, 39962, 39963, 1156369, uses, this), 0, 0, 0);
                AddComponent(new ToolDropComponent(40006, 1024014), -1, 0, 0);
            }
            else
            {
                AddCraftComponent(new AddonToolComponent(CraftSystem, 39972, 39973, 1156369, uses, this), 0, 0, 0);
                AddComponent(new ToolDropComponent(40007, 1024014), 0, 1, 0);
            }
        }

        public SpinningLathe(Serial serial)
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

    public class SpinningLatheDeed : CraftAddonDeed
    {
        public override int LabelNumber { get { return 1156369; } } // spinning lathe
        public override BaseAddon Addon { get { return new SpinningLathe(_South, UsesRemaining); } }

        private bool _South;

        [Constructable]
        public SpinningLatheDeed() : this(0)
        {
        }

        [Constructable]
        public SpinningLatheDeed(int uses) : base(uses)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.SendGump(new SouthEastGump(s =>
                {
                    _South = s;
                    base.OnDoubleClick(from);
                }));
            }
        }

        public SpinningLatheDeed(Serial serial)
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