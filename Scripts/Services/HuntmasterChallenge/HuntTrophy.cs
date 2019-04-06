using System;
using System.Linq;

using Server;
using Server.Multis;
using Server.Mobiles;
using Server.Engines.HuntsmasterChallenge;

namespace Server.Items
{
	public class HuntTrophy : Item, IAddon
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
        public TextDefinition Species { get { return Info.Species; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public MeasuredBy MeasuredBy { get { return Info.MeasuredBy; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int EastID { get { return Info.EastID; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int SouthID { get { return Info.SouthID; } }

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
        public HuntingTrophyInfo Info { get { return HuntingTrophyInfo.Infos[Index]; } }

        public override int LabelNumber
        {
            get
            {
                if (Species.Number > 0)
                    return Species.Number;

                return 1084024 + ItemID;
            }
        }

        public virtual Item Deed
        { 
            get 
            {
                if (Info.RequiresWall)
                {
                    return new HuntTrophyDeed(m_Owner, Index, m_Measurement, m_DateKilled, m_Location);
                }
                else
                {
                    return new HuntTrophyAddonDeed(m_Owner, Index, m_Measurement, m_DateKilled, m_Location);
                }
            }
        }

        public HuntTrophy(string name, int index, int measurement, string killed, string location)
		{
            Index = index;

            m_Owner = name;
			m_Location = location;
			m_DateKilled = killed;
            m_Measurement = measurement;

			switch(MeasuredBy)
			{
				case MeasuredBy.Weight:
					Weight = measurement;
					break;
				case MeasuredBy.Length:
				case MeasuredBy.Wingspan:
					Weight = 5.0;
					break;
			}

            Movable = false;
		}

        public bool CouldFit(IPoint3D p, Map map)
        {
            if (!map.CanFit(p.X, p.Y, p.Z, this.ItemData.Height))
                return false;

            if (this.ItemID == EastID)
                return BaseAddon.IsWall(p.X, p.Y - 1, p.Z, map); // North wall
            else
                return BaseAddon.IsWall(p.X - 1, p.Y, p.Z, map); // West wall
        }
		
		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

            list.Add(1155708, m_Owner != null ? m_Owner : "Unknown"); // Hunter: ~1_NAME~
            list.Add(1155709, m_DateKilled); // Date of Kill: ~1_DATE~
				
			if(m_Location != null)
                list.Add(1061114, m_Location); // Location: ~1_val~

            list.Add(1155718, Species.ToString());

            if (MeasuredBy == MeasuredBy.Length)
                list.Add(1155711, m_Measurement.ToString()); // Length: ~1_VAL~
            else if (MeasuredBy == MeasuredBy.Wingspan)
                list.Add(1155710, m_Measurement.ToString());	// Wingspan: ~1_VAL~
		}

        void IChopable.OnChop(Mobile user)
        {
            OnDoubleClick(user);
        }

        public override void OnDoubleClick(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && (house.IsCoOwner(from) || (house.Addons.ContainsKey(this) && house.Addons[this] == from)))
            {
                if (from.InRange(GetWorldLocation(), 1))
                {
                    from.AddToBackpack(this.Deed);
                    Delete();
                }
                else
                {
                    from.SendLocalizedMessage(500295); // You are too far away to do that.
                }
            }
            else
            {
                from.SendLocalizedMessage(502092); // You must be in your house to do this.
            }
        }
		
		public HuntTrophy(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)2);

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
                    var td = TextDefinition.Deserialize(reader);
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
        public TextDefinition Species { get { return Info.Species; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public MeasuredBy MeasuredBy { get { return Info.MeasuredBy; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int EastID { get { return Info.EastID; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int SouthID { get { return Info.SouthID; } }

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
        public HuntingTrophyInfo Info { get { return HuntingTrophyInfo.Infos[Index]; } }

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

                        if (Info.RequiresWall)
                        {
                            trophy = new HuntTrophy(m_Owner, Index, m_Measurement, m_DateKilled, m_Location);
                            trophy.ItemID = itemID;
                        }
                        else
                        {
                            trophy = new HuntTrophyAddon(m_Owner, Index, m_Measurement, m_DateKilled, m_Location);
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
            writer.Write((int)2);

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
                    var td = TextDefinition.Deserialize(reader);
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
}