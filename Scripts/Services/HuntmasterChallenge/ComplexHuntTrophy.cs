using System;
using Server;
using Server.Mobiles;
using Server.Engines.HuntsmasterChallenge;
using System.Linq;

namespace Server.Items
{
    [FlipableAddon(Direction.South, Direction.East)]
	public class HuntTrophyAddon : BaseAddon
	{
        private string m_Owner;
        private int m_Measurement;
        private string m_Location;
        private TextDefinition m_Species;
        private string m_DateKilled;
        private MeasuredBy m_MeasuredBy;
        private int m_SouthID;

        [CommandProperty(AccessLevel.GameMaster)]
        public string Owner { get { return m_Owner; } set { m_Owner = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public string KillLocation { get { return m_Location; } set { m_Location = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public TextDefinition Species { get { return m_Species; } set { m_Species = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Measurement { get { return m_Measurement; } set { m_Measurement = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public string DateKilled { get { return m_DateKilled; } set { m_DateKilled = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public MeasuredBy MeasuredBy { get { return m_MeasuredBy; } set { m_MeasuredBy = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SouthID { get { return m_SouthID; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int EastID { get { return m_SouthID + 4; } }

        public override BaseAddonDeed Deed { get { return new HuntTrophyAddonDeed(m_Owner, m_MeasuredBy, m_Measurement, m_SouthID, m_DateKilled, m_Location, m_Species); } }

		public HuntTrophyAddon(string name, MeasuredBy measuredBy, int measurement, int id, string killed, string location, TextDefinition species)
        {
            m_SouthID = id;
            m_Owner = name;
            m_Species = species;
            m_Location = location;
            m_DateKilled = killed;
            m_MeasuredBy = measuredBy;
            m_Measurement = measurement;

            //TODO: CHeck this
            switch (measuredBy)
            {
                case MeasuredBy.Weight:
                    Weight = measurement;
                    break;
                case MeasuredBy.Length:
                case MeasuredBy.Wingspan:
                    Weight = 5.0;
                    break;
            }

            AddComponent(new HuntTrophyComponent(SouthID), 0, 0, 0);
            AddComponent(new HuntTrophyComponent(SouthID + 1), 0, -1, 0);
		}

        public virtual void Flip(Mobile from, Direction direction)
        {
            switch (direction)
            {
                case Direction.West:
                    AddComponent(new HuntTrophyComponent(SouthID), 0, 0, 0);
                    AddComponent(new HuntTrophyComponent(SouthID + 1), 0, -1, 0);
                    break;
                case Direction.North:
                    AddComponent(new HuntTrophyComponent(EastID), 0, 0, 0);
                    AddComponent(new HuntTrophyComponent(EastID + 1), 1, 0, 0);
                    break;
            }
        }

        public class HuntTrophyComponent : AddonComponent
        {
            public override int LabelNumber { get { return 1084024 + ItemID; } }

            public HuntTrophyComponent(int id)
                : base(id)
            {
            }

            public override void GetProperties(ObjectPropertyList list)
            {
                base.GetProperties(list);

                HuntTrophyAddon addon = Addon as HuntTrophyAddon;

                if (addon != null)
                {
                    list.Add(1155708, addon.Owner != null ? addon.Owner : "Unknown"); // Hunter: ~1_NAME~
                    list.Add(1155709, addon.DateKilled); // Date of Kill: ~1_DATE~

                    if (addon.KillLocation != null)
                        list.Add(1061114, addon.KillLocation); // Location: ~1_val~

                    list.Add(1155718, addon.Species.ToString());

                    if (addon.MeasuredBy == MeasuredBy.Length)
                        list.Add(1155711, addon.Measurement.ToString()); // Length: ~1_VAL~
                    else if (addon.MeasuredBy == MeasuredBy.Wingspan)
                        list.Add(1155710, addon.Measurement.ToString());	// Wingspan: ~1_VAL~
                }
            }

            public HuntTrophyComponent(Serial serial)
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
                int v = reader.ReadInt();
            }
        }

        public HuntTrophyAddon(Serial serial)
            : base(serial)
		{
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(m_Owner);
            writer.Write(m_Measurement);
            writer.Write(m_DateKilled);
            writer.Write(m_Location);
            TextDefinition.Serialize(writer, m_Species);
            writer.Write((int)m_MeasuredBy);
            writer.Write(m_SouthID);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            m_Owner = reader.ReadString();
            m_Measurement = reader.ReadInt();
            m_DateKilled = reader.ReadString();
            m_Location = reader.ReadString();
            m_Species = TextDefinition.Deserialize(reader);
            m_MeasuredBy = (MeasuredBy)reader.ReadInt();
            m_SouthID = reader.ReadInt();
        }
	}

    public class HuntTrophyAddonDeed : BaseAddonDeed
    {
        public override int LabelNumber { get { return 1084024 + m_SouthID; } }

        public override BaseAddon Addon { get { return new HuntTrophyAddon(m_Owner, m_MeasuredBy, m_Measurement, m_SouthID, m_DateKilled, m_Location, m_Species); } }

        private string m_Owner;
        private int m_Measurement;
        private string m_Location;
        private TextDefinition m_Species;
        private string m_DateKilled;
        private MeasuredBy m_MeasuredBy;
        private int m_SouthID;

        [CommandProperty(AccessLevel.GameMaster)]
        public string Owner { get { return m_Owner; } set { m_Owner = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public string KillLocation { get { return m_Location; } set { m_Location = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public TextDefinition Species { get { return m_Species; } set { m_Species = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Measurement { get { return m_Measurement; } set { m_Measurement = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public string DateKilled { get { return m_DateKilled; } set { m_DateKilled = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public MeasuredBy MeasuredBy { get { return m_MeasuredBy; } set { m_MeasuredBy = value; } }

        public HuntTrophyAddonDeed(string name, MeasuredBy measuredBy, int measurement, int id, string killed, string location, TextDefinition species)
        {
            m_SouthID = id;
            m_Owner = name;
            m_Species = species;
            m_Location = location;
            m_DateKilled = killed;
            m_MeasuredBy = measuredBy;
            m_Measurement = measurement;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            HuntTrophyAddon addon = Addon as HuntTrophyAddon;

            if (addon != null)
            {
                list.Add(1155708, addon.Owner != null ? addon.Owner : "Unknown"); // Hunter: ~1_NAME~
                list.Add(1155709, addon.DateKilled); // Date of Kill: ~1_DATE~

                if (m_Location != null)
                    list.Add(1061114, addon.KillLocation); // Location: ~1_val~

                list.Add(1155718, addon.Species.ToString());

                if (addon.MeasuredBy == MeasuredBy.Length)
                    list.Add(1155711, addon.Measurement.ToString()); // Length: ~1_VAL~
                else if (addon.MeasuredBy == MeasuredBy.Wingspan)
                    list.Add(1155710, addon.Measurement.ToString());	// Wingspan: ~1_VAL~
                else
                    list.Add(1072225, addon.Measurement.ToString()); // Weight: ~1_WEIGHT~ stones
            }
        }

        public HuntTrophyAddonDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(m_Owner);
            writer.Write(m_Measurement);
            writer.Write(m_DateKilled);
            writer.Write(m_Location);
            TextDefinition.Serialize(writer, m_Species);
            writer.Write((int)m_MeasuredBy);
            writer.Write(m_SouthID);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            m_Owner = reader.ReadString();
            m_Measurement = reader.ReadInt();
            m_DateKilled = reader.ReadString();
            m_Location = reader.ReadString();
            m_Species = TextDefinition.Deserialize(reader);
            m_MeasuredBy = (MeasuredBy)reader.ReadInt();
            m_SouthID = reader.ReadInt();
        }
    }
}