using System;
using Server.Items;

namespace Server.Engines.ConPVP
{
    public enum DuelTeleporterType
    {
        Squares = 6095,
        Buds = 6104,
        Flowers = 6113,
        Spikes = 6122,
        Arrows = 6140,
        Links = 6149
    }

    public class DuelTeleporterAddon : BaseAddon
    {
        [Constructable]
        public DuelTeleporterAddon()
            : this(DuelTeleporterType.Squares)
        {
        }

        [Constructable]
        public DuelTeleporterAddon(DuelTeleporterType type)
        {
            int itemID = (int)type;

            this.AddComponent(new AddonComponent(itemID + 0), -1, -1, 5);
            this.AddComponent(new AddonComponent(itemID + 1), -1, 0, 5);
            this.AddComponent(new AddonComponent(itemID + 2), 0, -1, 5);
            this.AddComponent(new AddonComponent(itemID + 3), -1, +1, 5);
            this.AddComponent(new AddonComponent(itemID + 4), 0, 0, 5);
            this.AddComponent(new AddonComponent(itemID + 5), +1, -1, 5);
            this.AddComponent(new AddonComponent(itemID + 6), 0, +1, 5);
            this.AddComponent(new AddonComponent(itemID + 7), +1, 0, 5);
            this.AddComponent(new AddonComponent(itemID + 8), +1, +1, 5);

            this.AddComponent(new AddonComponent(0x759), -2, -2, 0);
            this.AddComponent(new AddonComponent(0x75A), +2, +2, 0);
            this.AddComponent(new AddonComponent(0x75B), -2, +2, 0);
            this.AddComponent(new AddonComponent(0x75C), +2, -2, 0);

            this.AddComponent(new AddonComponent(0x751), -1, +2, 0);
            this.AddComponent(new AddonComponent(0x751), 0, +2, 0);
            this.AddComponent(new AddonComponent(0x751), +1, +2, 0);

            this.AddComponent(new AddonComponent(0x752), +2, -1, 0);
            this.AddComponent(new AddonComponent(0x752), +2, 0, 0);
            this.AddComponent(new AddonComponent(0x752), +2, +1, 0);

            this.AddComponent(new AddonComponent(0x753), -1, -2, 0);
            this.AddComponent(new AddonComponent(0x753), 0, -2, 0);
            this.AddComponent(new AddonComponent(0x753), +1, -2, 0);

            this.AddComponent(new AddonComponent(0x754), -2, -1, 0);
            this.AddComponent(new AddonComponent(0x754), -2, 0, 0);
            this.AddComponent(new AddonComponent(0x754), -2, +1, 0);
        }

        public DuelTeleporterAddon(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DuelTeleporterType Type
        {
            get
            {
                if (this.Components.Count > 0)
                    return (DuelTeleporterType)(((Item)this.Components[0]).ItemID);

                return DuelTeleporterType.Squares;
            }
            set
            {
                for (int i = 0; i < this.Components.Count && i < 9; ++i)
                    ((Item)this.Components[i]).ItemID = i + (int)value;
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