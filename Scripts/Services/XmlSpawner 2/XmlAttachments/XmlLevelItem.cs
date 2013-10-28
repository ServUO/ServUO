using System;
using Server.Items;

namespace Server.Engines.XmlSpawner2
{
    public class XmlLevelItem : XmlAttachment
    {
        private int m_Experience = 0;
        private int m_Level = 0;
        private int m_Points = 0;
        private int m_MaxLevel = 0;
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
            this.Name = "LevelItem";
            this.MaxLevel = maxLevel;

            this.Experience = 0;
            this.Level = 1;
            this.Points = 0;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Experience
        {
            get
            {
                return this.m_Experience;
            }
            set
            {
                this.m_Experience = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Level
        {
            get
            {
                return this.m_Level;
            }
            set
            {
                this.m_Level = value;
                /* ChangeName((Item)AttachedTo); */ this.InvalidateParentProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Points
        {
            get
            {
                return this.m_Points;
            }
            set
            {
                this.m_Points = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxLevel
        {
            get
            {
                return this.m_MaxLevel;
            }
            set
            {
                this.m_MaxLevel = value;
                if (this.m_MaxLevel > LevelItems.MaxLevelsCap)
                    this.m_MaxLevel = LevelItems.MaxLevelsCap;
            }
        }
        public override void OnAttach()
        {
            base.OnAttach();

            if (this.AttachedTo is BaseWeapon || this.AttachedTo is BaseArmor || this.AttachedTo is BaseJewel)
            {
                this.InvalidateParentProperties();
            }
            else
            {
                this.Delete();
            }
        }

        public override void OnDelete()
        {
            base.OnDelete();

            this.InvalidateParentProperties();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write((int)this.m_Experience);
            writer.Write((int)this.m_Level);
            writer.Write((int)this.m_MaxLevel);
            writer.Write((int)this.m_Points);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_Experience = reader.ReadInt();
            this.m_Level = reader.ReadInt();
            this.m_MaxLevel = reader.ReadInt();
            this.m_Points = reader.ReadInt();
        }
    }
}