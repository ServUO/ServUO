using System;
using Server.Engines.Craft;
using Server.Mobiles;

namespace Server.Items
{
    public enum ClockworkType
    {
        Wolf = 0,
        Scorpion = 1,
        Vollem = 2,
    }

    [Flipable(0x1EA8, 0x1EAC)]
    public class ModifiedClockworkAssembly : Item, ICraftable
    {
        private class GolemInfo
        {
            readonly int m_Slots;
            readonly Type[] m_Types;
            readonly int[] m_Amounts;
            readonly int[] m_MsgIds;

            public int Slots
            {
                get
                {
                    return m_Slots;
                }
            }
            public Type[] Types
            {
                get
                {
                    return m_Types;
                }
            }
            public int[] Amounts
            {
                get
                {
                    return m_Amounts;
                }
            }
            public int[] MsgIds
            {
                get
                {
                    return m_MsgIds;
                }
            }

            public GolemInfo(int slots, Type[] types, int[] amounts, int[] msgids)
            {
                m_Slots = slots;
                m_Types = types;
                m_Amounts = amounts;
                m_MsgIds = msgids;
            }
        }

        private static readonly GolemInfo[] m_Info =
        {
            // Clockwork Leather Wolf
            new GolemInfo(1,
                new Type[] { typeof(Leather), typeof(OilFlask) },
                new int[] { 100, 1 },
                new int[] { 1113058, 1113059 }),
            // ClockWork Scorpion
            new GolemInfo(1,
                new Type[] { typeof(BronzeIngot), typeof(Gears) },
                new int[] { 100, 10 },
                new int[] { 1113060, 1113061 }),
            // Vollem
            new GolemInfo(2,
                new Type[] { typeof(BronzeIngot), typeof(WhiteScales) },
                new int[] { 50, 25 },
                new int[] { 1113060, 1113062 })
        };

        private ClockworkType m_Type;

        [CommandProperty(AccessLevel.GameMaster)]
        public ClockworkType Type
        {
            get
            {
                return m_Type;
            }
            set
            {
                m_Type = value;
                InvalidateProperties();
            }
        }

        public override int LabelNumber
        {
            get
            {
                return (1113031 + (int)m_Type);
            }
        }

        [Constructable]
        public ModifiedClockworkAssembly()
            : this(ClockworkType.Wolf)
        {
        }

        [Constructable]
        public ModifiedClockworkAssembly(ClockworkType type)
            : base(0x1EA8)
        {
            m_Type = type;
            Weight = 5.0;
        }

        public ModifiedClockworkAssembly(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;
            if (null == pm)
                return;

            if (!pm.MechanicalLife)
            {
                from.SendLocalizedMessage(1113034);   // You haven't read the Mechanical Life Manual. Talking to Sutek might help!
            }

            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1071944);   // The clockwork assembly must be in your backpack to construct a golem.
                return;
            }

            double tinkerSkill = from.Skills[SkillName.Tinkering].Value;

            if (tinkerSkill < 60.0)
            {
                from.SendLocalizedMessage(1113054); // You must be a Journeyman or higher Tinker to construct a mechanical pet.
                return;
            }

            if ((int)m_Type > m_Info.Length)
                return;

            GolemInfo ginfo = m_Info[(int)m_Type];

            if ((from.Followers + ginfo.Slots) > from.FollowersMax)
            {
                from.SendLocalizedMessage(1049607); // You have too many followers to control that creature.
                return;
            }

            Container pack = from.Backpack;

            if (pack == null)
                return;

            // check array length to prevent errors
            if (ginfo.Types.Length != ginfo.Amounts.Length)
                return;

            int res = pack.ConsumeTotal(ginfo.Types, ginfo.Amounts);

            if (res >= 0)
            {
                // send message, if valid index
                if (res < ginfo.MsgIds.Length)
                    from.SendLocalizedMessage(ginfo.MsgIds[res]);   // You need _____ to construct a mechanical pet.
                return;
            }

            BaseCreature bc = null;

            switch (m_Type)
            {
                case ClockworkType.Scorpion:
                    bc = new ClockworkScorpion();
                    break;
                case ClockworkType.Wolf:
                    bc = new LeatherWolf();
                    break;
                case ClockworkType.Vollem:
                    bc = new Vollem();
                    break;
            }

            if (null != bc && bc.SetControlMaster(from))
            {
                Delete();

                bc.MoveToWorld(from.Location, from.Map);
                from.PlaySound(0x241);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);

            writer.Write((int)m_Type);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    m_Type = (ClockworkType)reader.ReadInt();
                    break;
            }
        }

        #region ICraftable Members
        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            switch (craftItem.NameNumber)
            {
                default:
                case 1113031:
                    m_Type = ClockworkType.Wolf;
                    break;
                case 1113032:
                    m_Type = ClockworkType.Scorpion;
                    break;
                case 1113033:
                    m_Type = ClockworkType.Vollem;
                    break;
            }

            return quality;
        }
        #endregion
    }
}
