using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using Server.Misc;
using Server.ContextMenus;

namespace Server.Engines.XmlSpawner2
{
    public class XmlLevelItem : XmlAttachment
    {
        private int m_Experience = 0;
        private int m_Level = 0;
        private int m_Points = 0;
        private int m_MaxLevel = 0;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Experience { get { return m_Experience; } set { m_Experience = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Level { get { return m_Level; } set { m_Level = value; /* ChangeName((Item)AttachedTo); */ InvalidateParentProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Points { get { return m_Points; } set { m_Points = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxLevel { get { return m_MaxLevel; } set { m_MaxLevel = value; if (m_MaxLevel > LevelItems.MaxLevelsCap) m_MaxLevel = LevelItems.MaxLevelsCap; } }

        public XmlLevelItem(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlLevelItem()
            : this(200)
        {
        }

        [Attachable]
        public XmlLevelItem(int maxLevel)
        {
            Name = "LevelItem";
            MaxLevel = maxLevel;

            Experience = 0;
            Level = 1;
            Points = 0;
        }

        public override void OnAttach()
        {
            base.OnAttach();

            if (AttachedTo is BaseWeapon || AttachedTo is BaseArmor || AttachedTo is BaseJewel)
            {
                InvalidateParentProperties();
            }
            else
            {
                Delete();
            }
        }

        public override void OnDelete()
        {
            base.OnDelete();

            InvalidateParentProperties();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write((int)m_Experience);
            writer.Write((int)m_Level);
            writer.Write((int)m_MaxLevel);
            writer.Write((int)m_Points);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Experience = reader.ReadInt();
            m_Level = reader.ReadInt();
            m_MaxLevel = reader.ReadInt();
            m_Points = reader.ReadInt();
        }
    }
}
