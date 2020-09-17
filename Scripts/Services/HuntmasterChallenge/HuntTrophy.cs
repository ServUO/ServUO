using Server.Engines.HuntsmasterChallenge;
using Server.Multis;
using System;

namespace Server.Items
{
    public class HuntTrophy : Item, IFlipable
    {
        private string m_Owner;
        private int m_Measurement;
        private string m_Location;
        private string m_DateKilled;
        private int m_Index;

        [CommandProperty(AccessLevel.GameMaster)]
        public string Owner { get { return m_Owner; } set { m_Owner = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public string KillLocation { get { return m_Location; } set { m_Location = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Measurement { get { return m_Measurement; } set { m_Measurement = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public string DateKilled { get { return m_DateKilled; } set { m_DateKilled = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public TextDefinition Species => Info.Species;

        [CommandProperty(AccessLevel.GameMaster)]
        public MeasuredBy MeasuredBy => Info.MeasuredBy;

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int EastID => Info.EastID;

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int SouthID => Info.SouthID;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Index
        {
            get
            {
                if (m_Index < 0 || m_Index >= HuntingTrophyInfo.Infos.Count)
                {
                    m_Index = 4;
                }

                return m_Index;
            }
            set
            {
                m_Index = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public HuntingTrophyInfo Info => HuntingTrophyInfo.Infos[Index];

        public override int LabelNumber => Info.TrophyName.Number;

        public HuntTrophy(string name, int index, int measurement, string killed, string location)
        {
            Index = index;
            ItemID = Info.SouthID;

            m_Owner = name;
            m_Location = location;
            m_DateKilled = killed;
            m_Measurement = measurement;

            if (!string.IsNullOrEmpty(Info.TrophyName.String))
            {
                Name = Info.TrophyName.String;
            }
        }

        public void OnFlip(Mobile m)
        {
            if (ItemID == Info.SouthID)
            {
                ItemID = Info.EastID;
            }
            else
            {
                ItemID = Info.SouthID;
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1155708, m_Owner != null ? m_Owner : "Unknown"); // Hunter: ~1_NAME~
            list.Add(1155709, m_DateKilled); // Date of Kill: ~1_DATE~

            if (m_Location != null)
                list.Add(1061114, m_Location); // Location: ~1_val~

            list.Add(1155718, Species.ToString());

            if (MeasuredBy == MeasuredBy.Length)
                list.Add(1155711, m_Measurement.ToString()); // Length: ~1_VAL~
            else if (MeasuredBy == MeasuredBy.Wingspan)
                list.Add(1155710, m_Measurement.ToString());	// Wingspan: ~1_VAL~
            else
                list.Add(1072225, m_Measurement.ToString()); // Weight: ~1_WEIGHT~ stones

        }

        public HuntTrophy(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(2);

            writer.Write(m_Index);
            writer.Write(m_Owner);
            writer.Write(m_Measurement);
            writer.Write(m_DateKilled);
            writer.Write(m_Location);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            switch (v)
            {
                case 2:
                    m_Index = reader.ReadInt();
                    m_Owner = reader.ReadString();
                    m_Measurement = reader.ReadInt();
                    m_DateKilled = reader.ReadString();
                    m_Location = reader.ReadString();
                    break;
                case 1:
                    reader.ReadBool();
                    m_Owner = reader.ReadString();
                    m_Measurement = reader.ReadInt();
                    m_DateKilled = reader.ReadString();
                    m_Location = reader.ReadString();
                    TextDefinition td = TextDefinition.Deserialize(reader);
                    reader.ReadInt();
                    reader.ReadInt();

                    Timer.DelayCall(() =>
                    {
                        Index = HuntingTrophyInfo.CheckInfo(td.Number);
                    });
                    break;
            }
        }
    }

    public class HuntTrophyDeed : Item
    {
        private string m_Owner;
        private int m_Measurement;
        private string m_Location;
        private string m_DateKilled;
        private int m_Index;

        [CommandProperty(AccessLevel.GameMaster)]
        public string Owner { get { return m_Owner; } set { m_Owner = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public string KillLocation { get { return m_Location; } set { m_Location = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Measurement { get { return m_Measurement; } set { m_Measurement = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public string DateKilled { get { return m_DateKilled; } set { m_DateKilled = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public TextDefinition Species => Info.Species;

        [CommandProperty(AccessLevel.GameMaster)]
        public MeasuredBy MeasuredBy => Info.MeasuredBy;

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int EastID => Info.EastID;

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int SouthID => Info.SouthID;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Index
        {
            get
            {
                if (m_Index < 0 || m_Index >= HuntingTrophyInfo.Infos.Count)
                {
                    m_Index = 4;
                }

                return m_Index;
            }
            set
            {
                m_Index = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public HuntingTrophyInfo Info => HuntingTrophyInfo.Infos[Index];

        public override int LabelNumber
        {
            get
            {
                if (Species.Number > 0)
                    return Species.Number;

                return 1084024 + ItemID;
            }
        }

        public HuntTrophyDeed(string from, int index, int measurement, string killed, string location)
            : base(5359)
        {
            Index = index;

            m_Owner = from;
            m_Location = location;
            m_DateKilled = killed;
            m_Measurement = measurement;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                BaseHouse house = BaseHouse.FindHouseAt(from);

                if (house != null && house.IsCoOwner(from))
                {
                    bool northWall = BaseAddon.IsWall(from.X, from.Y - 1, from.Z, from.Map);
                    bool westWall = BaseAddon.IsWall(from.X - 1, from.Y, from.Z, from.Map);

                    if (northWall && westWall)
                    {
                        switch (from.Direction & Direction.Mask)
                        {
                            case Direction.North:
                            case Direction.South: northWall = true; westWall = false; break;

                            case Direction.East:
                            case Direction.West: northWall = false; westWall = true; break;

                            default: from.SendMessage("Turn to face the wall on which to hang this trophy."); return;
                        }
                    }

                    int itemID = 0;

                    if (northWall)
                        itemID = SouthID;
                    else if (westWall)
                        itemID = EastID;
                    else
                        from.SendLocalizedMessage(1042626); // The trophy must be placed next to a wall.

                    if (itemID > 0)
                    {
                        Item trophy;

                        if (Info.Complex)
                        {
                            trophy = new HuntTrophyAddon(m_Owner, Index, m_Measurement, m_DateKilled, m_Location);
                        }
                        else
                        {
                            trophy = new HuntTrophy(m_Owner, Index, m_Measurement, m_DateKilled, m_Location)
                            {
                                ItemID = itemID
                            };
                        }

                        trophy.MoveToWorld(from.Location, from.Map);

                        house.Addons[trophy] = from;
                        Delete();
                    }
                }
                else
                {
                    from.SendLocalizedMessage(502092); // You must be in your house to do this.
                }
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1155708, m_Owner != null ? m_Owner : "Unknown"); // Hunter: ~1_NAME~
            list.Add(1155709, m_DateKilled); // Date of Kill: ~1_DATE~

            if (m_Location != null)
                list.Add(1061114, m_Location); // Location: ~1_val~

            list.Add(1155718, Species.ToString());

            if (MeasuredBy == MeasuredBy.Length)
                list.Add(1155711, Measurement.ToString()); // Length: ~1_VAL~
            else if (MeasuredBy == MeasuredBy.Wingspan)
                list.Add(1155710, Measurement.ToString());	// Wingspan: ~1_VAL~
            else
                list.Add(1072225, Measurement.ToString()); // Weight: ~1_WEIGHT~ stones
        }

        public HuntTrophyDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(2);

            writer.Write(m_Index);
            writer.Write(m_Owner);
            writer.Write(m_Measurement);
            writer.Write(m_DateKilled);
            writer.Write(m_Location);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            switch (v)
            {
                case 2:
                    m_Index = reader.ReadInt();
                    m_Owner = reader.ReadString();
                    m_Measurement = reader.ReadInt();
                    m_DateKilled = reader.ReadString();
                    m_Location = reader.ReadString();
                    break;
                case 1:
                    reader.ReadBool();
                    m_Owner = reader.ReadString();
                    m_Measurement = reader.ReadInt();
                    m_DateKilled = reader.ReadString();
                    m_Location = reader.ReadString();
                    TextDefinition td = TextDefinition.Deserialize(reader);
                    reader.ReadInt();
                    reader.ReadInt();

                    Timer.DelayCall(() =>
                        {
                            Index = HuntingTrophyInfo.CheckInfo(td.Number);
                        });
                    break;
            }

            Timer.DelayCall(TimeSpan.FromSeconds(30), Replace);
        }

        private void Replace()
        {
            HuntTrophy trophy = new HuntTrophy(m_Owner, Index, m_Measurement, m_DateKilled, m_Location);

            if (Parent is Container)
            {
                ((Container)Parent).DropItem(trophy);
            }
            else
            {
                BaseHouse house = BaseHouse.FindHouseAt(this);
                trophy.MoveToWorld(GetWorldLocation(), Map);

                trophy.IsLockedDown = IsLockedDown;
                trophy.IsSecure = IsSecure;
                trophy.Movable = Movable;

                if (house != null && house.LockDowns.ContainsKey(this))
                {
                    house.LockDowns.Remove(this);
                    house.LockDowns.Add(trophy, house.Owner);
                }
                else if (house != null && house.IsSecure(this))
                {
                    house.ReleaseSecure(house.Owner, this);
                    house.AddSecure(house.Owner, trophy);
                }
            }

            Delete();
        }
    }
}
