using System;
using Server;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Engines.HuntsmasterChallenge;

namespace Server.Items
{
    public class HuntingDisplayTrophy : Item
    {
        private HuntType m_HuntType;

        public override int LabelNumber
        {
            get
            {
                HuntingTrophyInfo info = HuntingTrophyInfo.GetInfo(m_HuntType);

                if (info != null && info.Species.Number > 0)
                    return info.Species.Number;

                return 1084024 + ItemID;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public HuntType HuntType
        {
            get { return m_HuntType; }
            set
            {
                m_HuntType = value;
                InvalidateType();
            }
        }

        public void InvalidateType()
        {
            HuntingTrophyInfo info = HuntingTrophyInfo.GetInfo(m_HuntType);

            if (info != null)
                ItemID = info.SouthID;

            InvalidateProperties();
        }

        [Constructable]
        public HuntingDisplayTrophy() : this(HuntType.GrizzlyBear)
        {
        }

        [Constructable]
        public HuntingDisplayTrophy(HuntType type)
        {
            HuntType = type;
            Movable = false;

            m_DisplayTrophies.Add(this);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (HuntingSystem.Instance == null)
                return;

            if (!HuntingSystem.Instance.Leaders.ContainsKey(m_HuntType))
                HuntingSystem.Instance.Leaders[m_HuntType] = new List<HuntingKillEntry>();

            List<HuntingKillEntry> entries = HuntingSystem.Instance.Leaders[m_HuntType];
            HuntingKillEntry entry = null;

            if (entries.Count > 0)
                entry = entries[0];

            if (entry != null)
            {
                HuntingTrophyInfo info = HuntingTrophyInfo.GetInfo(m_HuntType);

                if (info != null)
                {
                    list.Add(1155708, entry.Owner != null ? entry.Owner.Name : "Unknown"); // Hunter: ~1_NAME~
                    list.Add(1155709, entry.DateKilled.ToShortDateString()); // Date of Kill: ~1_DATE~

                    if (entry.Location != null)
                        list.Add(1061114, entry.Location); // Location: ~1_val~

                    list.Add(1155718, info.Species.ToString());

                    if (info.MeasuredBy == MeasuredBy.Length)
                        list.Add(1155711, entry.Measurement.ToString()); // Length: ~1_VAL~
                    else if (info.MeasuredBy == MeasuredBy.Wingspan)
                        list.Add(1155710, entry.Measurement.ToString());	// Wingspan: ~1_VAL~
                    else
                        list.Add(1072789, entry.Measurement.ToString()); // Weight: ~1_WEIGHT~
                }
            }

            if (HuntingSystem.Instance != null && HuntingSystem.Instance.Active)
                list.Add("Season Ends: {0}", HuntingSystem.Instance.SeasonEnds.ToShortDateString());
        }

        private static List<HuntingDisplayTrophy> m_DisplayTrophies = new List<HuntingDisplayTrophy>();

        public static void InvalidateDisplayTrophies()
        {
            foreach (HuntingDisplayTrophy trophy in m_DisplayTrophies)
            {
                if(trophy != null && !trophy.Deleted)
                    trophy.InvalidateProperties();
            }
        }

        public override void Delete()
        {
            if (m_DisplayTrophies.Contains(this))
                m_DisplayTrophies.Remove(this);

            base.Delete();
        }

        public HuntingDisplayTrophy(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write((int)m_HuntType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            m_HuntType = (HuntType)reader.ReadInt();

            m_DisplayTrophies.Add(this);
        }
    }
}