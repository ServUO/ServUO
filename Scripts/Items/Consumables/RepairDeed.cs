using System;
using Server.Engines.Craft;
using Server.Mobiles;
using Server.Regions;

namespace Server.Items
{
    public class RepairDeed : Item
    {
        private RepairSkillType m_Skill;
        private double m_SkillLevel;
        private Mobile m_Crafter;
        [Constructable]
        public RepairDeed()
            : this(RepairSkillType.Smithing, 100.0, null, true)
        {
        }

        [Constructable]
        public RepairDeed(RepairSkillType skill, double level)
            : this(skill, level, null, true)
        {
        }

        [Constructable]
        public RepairDeed(RepairSkillType skill, double level, bool normalizeLevel)
            : this(skill, level, null, normalizeLevel)
        {
        }

        public RepairDeed(RepairSkillType skill, double level, Mobile crafter)
            : this(skill, level, crafter, true)
        {
        }

        public RepairDeed(RepairSkillType skill, double level, Mobile crafter, bool normalizeLevel)
            : base(0x14F0)
        {
            if (normalizeLevel)
                this.SkillLevel = (int)(level / 10) * 10;
            else
                this.SkillLevel = level;

            this.m_Skill = skill;
            this.m_Crafter = crafter;
            this.Hue = 0x1BC;
            this.LootType = LootType.Regular;
        }

        public RepairDeed(Serial serial)
            : base(serial)
        {
        }

        public enum RepairSkillType
        {
            Smithing,
            Tailoring,
            Tinkering,
            Carpentry,
            Fletching
        }
        public override bool DisplayLootType
        {
            get
            {
                return Core.ML;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public RepairSkillType RepairSkill
        {
            get
            {
                return this.m_Skill;
            }
            set
            {
                this.m_Skill = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public double SkillLevel
        {
            get
            {
                return this.m_SkillLevel;
            }
            set
            {
                this.m_SkillLevel = Math.Max(Math.Min(value, 120.0), 0) ;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter
        {
            get
            {
                return this.m_Crafter;
            }
            set
            {
                this.m_Crafter = value;
                this.InvalidateProperties();
            }
        }
        public static RepairSkillType GetTypeFor(CraftSystem s)
        {
            for (int i = 0; i < RepairSkillInfo.Table.Length; i++)
            {
                if (RepairSkillInfo.Table[i].System == s)
                    return (RepairSkillType)i;
            }

            return RepairSkillType.Smithing;
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1061133, String.Format("{0}\t{1}", GetSkillTitle(this.m_SkillLevel).ToString(), RepairSkillInfo.GetInfo(this.m_Skill).Name)); // A repair service contract from ~1_SKILL_TITLE~ ~2_SKILL_NAME~.
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.m_Crafter != null)
                list.Add(1050043, m_Crafter.TitleName); // crafted by ~1_NAME~
            //On OSI it says it's exceptional.  Intentional difference.
        }

        public override void OnSingleClick(Mobile from)
        {
            if (this.Deleted || !from.CanSee(this))
                return;

            this.LabelTo(from, 1061133, String.Format("{0}\t{1}", GetSkillTitle(this.m_SkillLevel).ToString(), RepairSkillInfo.GetInfo(this.m_Skill).Name)); // A repair service contract from ~1_SKILL_TITLE~ ~2_SKILL_NAME~.

            if (this.m_Crafter != null)
				this.LabelTo(from, 1050043, m_Crafter.TitleName); // crafted by ~1_NAME~
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.Check(from))
                Repair.Do(from, RepairSkillInfo.GetInfo(this.m_Skill).System, this);
        }

        public bool Check(Mobile from)
        {
            if (!this.IsChildOf(from.Backpack))
                from.SendLocalizedMessage(1047012); // The contract must be in your backpack to use it.
            else if (!this.VerifyRegion(from))
                TextDefinition.SendMessageTo(from, RepairSkillInfo.GetInfo(this.m_Skill).NotNearbyMessage);
            else
                return true;

            return false;
        }

        public bool VerifyRegion(Mobile m)
        {
            //TODO: When the entire region system data is in, convert to that instead of a proximity thing.
            if (!m.Region.IsPartOf<TownRegion>())
                return false;

            return Server.Factions.Faction.IsNearType(m, RepairSkillInfo.GetInfo(this.m_Skill).NearbyTypes, 6);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)this.m_Skill);
            writer.Write(this.m_SkillLevel);
            writer.Write(this.m_Crafter);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        this.m_Skill = (RepairSkillType)reader.ReadInt();
                        this.m_SkillLevel = reader.ReadDouble();
                        this.m_Crafter = reader.ReadMobile();

                        break;
                    }
            }
        }

        private static TextDefinition GetSkillTitle(double skillLevel)
        {
            int skill = (int)(skillLevel / 10);

            if (skill >= 11)
                return (1062008 + skill - 11);
            else if (skill >= 5)
                return (1061123 + skill - 5);

            switch( skill )
            {
                case 4:
                    return "a Novice";
                case 3:
                    return "a Neophyte";
                default:
                    return "a Newbie";		//On OSI, it shouldn't go below 50, but, this is for 'custom' support.
            }
        }

        private class RepairSkillInfo
        {
            private static readonly RepairSkillInfo[] m_Table = new RepairSkillInfo[]
            {
                new RepairSkillInfo(DefBlacksmithy.CraftSystem, typeof(Blacksmith), 1047013, 1023015),
                new RepairSkillInfo(DefTailoring.CraftSystem, typeof(Tailor), 1061132, 1022981),
                new RepairSkillInfo(DefTinkering.CraftSystem, typeof(Tinker), 1061166, 1022983),
                new RepairSkillInfo(DefCarpentry.CraftSystem, typeof(Carpenter), 1061135, 1060774),
                new RepairSkillInfo(DefBowFletching.CraftSystem, typeof(Bowyer), 1061134, 1023005)
            };
            private readonly CraftSystem m_System;
            private readonly Type[] m_NearbyTypes;
            private readonly TextDefinition m_NotNearbyMessage;
            private readonly TextDefinition m_Name;
            public RepairSkillInfo(CraftSystem system, Type[] nearbyTypes, TextDefinition notNearbyMessage, TextDefinition name)
            {
                this.m_System = system;
                this.m_NearbyTypes = nearbyTypes;
                this.m_NotNearbyMessage = notNearbyMessage;
                this.m_Name = name;
            }

            public RepairSkillInfo(CraftSystem system, Type nearbyType, TextDefinition notNearbyMessage, TextDefinition name)
                : this(system, new Type[] { nearbyType }, notNearbyMessage, name)
            {
            }

            public static RepairSkillInfo[] Table
            {
                get
                {
                    return m_Table;
                }
            }
            public TextDefinition NotNearbyMessage
            {
                get
                {
                    return this.m_NotNearbyMessage;
                }
            }
            public TextDefinition Name
            {
                get
                {
                    return this.m_Name;
                }
            }
            public CraftSystem System
            {
                get
                {
                    return this.m_System;
                }
            }
            public Type[] NearbyTypes
            {
                get
                {
                    return this.m_NearbyTypes;
                }
            }
            public static RepairSkillInfo GetInfo(RepairSkillType type)
            {
                int v = (int)type;

                if (v < 0 || v >= m_Table.Length)
                    v = 0;

                return m_Table[v];
            }
        }
    }
}