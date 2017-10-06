using Server.Network;
using System;

namespace Server.Items
{
    [Server.Engines.Craft.Anvil]
    public class AnvilComponent : AddonComponent
    {
        [Constructable]
        public AnvilComponent(int itemID)
            : base(itemID)
        {
        }

        public AnvilComponent(Serial serial)
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

    [Server.Engines.Craft.Forge]
    public class ForgeComponent : AddonComponent
    {
        [Constructable]
        public ForgeComponent(int itemID)
            : base(itemID)
        {
        }

        public ForgeComponent(Serial serial)
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

    public class LocalizedAddonComponent : AddonComponent
    {
        private int m_LabelNumber;
        [Constructable]
        public LocalizedAddonComponent(int itemID, int labelNumber)
            : base(itemID)
        {
            m_LabelNumber = labelNumber;
        }

        public LocalizedAddonComponent(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Number
        {
            get
            {
                return m_LabelNumber;
            }
            set
            {
                m_LabelNumber = value;
                InvalidateProperties();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return m_LabelNumber;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)m_LabelNumber);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        m_LabelNumber = reader.ReadInt();
                        break;
                    }
            }
        }
    }

    public class AddonComponent : Item, IChopable
    {
        public override bool ForceShowProperties { get { return Addon != null && Addon.ForceShowProperties; } }

        private static readonly LightEntry[] m_Entries = new LightEntry[]
        {
            new LightEntry(LightType.WestSmall, 1122, 1123, 1124, 1141, 1142, 1143, 1144, 1145, 1146, 2347, 2359, 2360, 2361, 2362, 2363, 2364, 2387, 2388, 2389, 2390, 2391, 2392),
            new LightEntry(LightType.NorthSmall, 1131, 1133, 1134, 1147, 1148, 1149, 1150, 1151, 1152, 2352, 2373, 2374, 2375, 2376, 2377, 2378, 2401, 2402, 2403, 2404, 2405, 2406),
            new LightEntry(LightType.Circle300, 6526, 6538, 6571),
            new LightEntry(LightType.Circle150, 5703, 6587)
        };
        private Point3D m_Offset;
        private BaseAddon m_Addon;
        [Constructable]
        public AddonComponent(int itemID)
            : base(itemID)
        {
            Movable = false;
            ApplyLightTo(this);
        }

        public AddonComponent(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseAddon Addon
        {
            get
            {
                return m_Addon;
            }
            set
            {
                m_Addon = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D Offset
        {
            get
            {
                return m_Offset;
            }
            set
            {
                m_Offset = value;
            }
        }
        [Hue, CommandProperty(AccessLevel.GameMaster)]
        public override int Hue
        {
            get
            {
                return base.Hue;
            }
            set
            {
                base.Hue = value;

                if (m_Addon != null && m_Addon.ShareHue)
                    m_Addon.Hue = value;
            }
        }
        public virtual bool NeedsWall
        {
            get
            {
                return false;
            }
        }
        public virtual Point3D WallPosition
        {
            get
            {
                return Point3D.Zero;
            }
        }
        public static void ApplyLightTo(Item item)
        {
            if ((item.ItemData.Flags & TileFlag.LightSource) == 0)
                return; // not a light source

            int itemID = item.ItemID;

            for (int i = 0; i < m_Entries.Length; ++i)
            {
                LightEntry entry = m_Entries[i];
                int[] toMatch = entry.m_ItemIDs;
                bool contains = false;

                for (int j = 0; !contains && j < toMatch.Length; ++j)
                    contains = (itemID == toMatch[j]);

                if (contains)
                {
                    item.Light = entry.m_Light;
                    return;
                }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_Addon != null)
                m_Addon.OnComponentUsed(this, from);
        }

        public void OnChop(Mobile from)
        {
            if (m_Addon != null && from.InRange(GetWorldLocation(), 3))
                m_Addon.OnChop(from);
            else
                from.SendLocalizedMessage(500446); // That is too far away.
        }

        public override void OnLocationChange(Point3D old)
        {
            if (m_Addon != null)
                m_Addon.Location = new Point3D(X - m_Offset.X, Y - m_Offset.Y, Z - m_Offset.Z);
        }

        public override void OnMapChange()
        {
            if (m_Addon != null)
                m_Addon.Map = Map;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Addon != null)
                m_Addon.Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write(m_Addon);
            writer.Write(m_Offset);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                case 0:
                    {
                        m_Addon = reader.ReadItem() as BaseAddon;
                        m_Offset = reader.ReadPoint3D();

                        if (m_Addon != null)
                            m_Addon.OnComponentLoaded(this);

                        ApplyLightTo(this);

                        break;
                    }
            }

            if (version < 1 && Weight == 0)
                Weight = -1;
        }

        private class LightEntry
        {
            public readonly LightType m_Light;
            public readonly int[] m_ItemIDs;
            public LightEntry(LightType light, params int[] itemIDs)
            {
                m_Light = light;
                m_ItemIDs = itemIDs;
            }
        }
    }

    public class InstrumentedAddonComponent : AddonComponent
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public int SuccessSound { get; set; }

        [Constructable]
        public InstrumentedAddonComponent(int itemID, int wellSound)
            : base(itemID)
        {
            SuccessSound = wellSound;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
            else if (from.BeginAction(typeof(InstrumentedAddonComponent)))
            {
                Timer.DelayCall(TimeSpan.FromMilliseconds(1000), () =>
                {
                    from.EndAction(typeof(InstrumentedAddonComponent));
                });

                from.PlaySound(SuccessSound);
            }
            else
            {
                from.SendLocalizedMessage(500119); // You must wait to perform another action
            }
        }

        public InstrumentedAddonComponent(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write((int)SuccessSound);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            SuccessSound = reader.ReadInt();
        }
    }
}