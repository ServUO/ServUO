using System.Collections; 
using Server.Items; 
using Server.Misc; 
using Server.Network; 
using Server.Multis;
using Server;
using Server.Regions; 

namespace Server.Items 
{
    public class GroupDungeonStone : Item 
	{
        private string m_DungeonName;
        private int m_MaxPlayers;
        private int m_Size;
        private Point3D m_EntrancePoint;
        private Map m_EntranceMap;
        private GroupDungeonRegion m_IRegion;
        private int m_MaxSkills;
        private int m_MinSkills;
        private bool m_AllowPets;


        [CommandProperty(AccessLevel.GameMaster)]
        public string DungeonName
        {
            get { return m_DungeonName; }
            set { m_DungeonName = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Size
        {
            get { return m_Size; }
            set { m_Size = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxPlayers
        {
            get { return m_MaxPlayers; }
            set { m_MaxPlayers = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxSkills
        {
            get { return m_MaxSkills; }
            set { m_MaxSkills = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MinSkills
        {
            get { return m_MinSkills; }
            set { m_MinSkills = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool AllowPets
        {
            get { return m_AllowPets; }
            set { m_AllowPets = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D EntrancePoint
        {
            get { return m_EntrancePoint; }
            set { m_EntrancePoint = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Map EntranceMap
        {
            get { return m_EntranceMap; }
            set { m_EntranceMap = value; }
        }

        public GroupDungeonRegion IRegion
        {
            get { return m_IRegion; }
            set { m_IRegion = value; }
        }

        public GroupDungeonRegion RegisterDungeon(GroupDungeonStone stone)
        {
            if (stone != null)
            {
                //Build Rect2d from the stone, then register the region.
                Point2D center = new Point2D(stone.X, stone.Y);
                Point2D start = new Point2D(center.X - stone.Size, center.Y - stone.Size);
                Point2D end = new Point2D(center.X + stone.Size, center.Y + stone.Size);
                Rectangle2D box = new Rectangle2D(start, end);

                GroupDungeonRegion reg = new GroupDungeonRegion(stone, stone.Map, stone.DungeonName, box);
                reg.DungeonReset();
                reg.Register();

                return reg;
            }
            else return null;
        }

        public void UnRegisterDungeon(GroupDungeonStone stone)
        {
            if (stone.IRegion != null)
                stone.IRegion.Unregister();
        }

        [Constructable] 
		public GroupDungeonStone() : base( 0xED4 ) 
		{ 
			Name = "a dungeon control stone"; 
			Movable = false;
            DungeonName = "A Blank Dungeon";
            Size = 2;
            MaxPlayers = 1;
            Visible = false;
            EntranceMap = this.Map;
            EntrancePoint = this.Location;
            MaxSkills = 66000;
            MinSkills = 0;
            AllowPets = true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
            {
                UnRegisterDungeon(this);
                IRegion = RegisterDungeon(this);
                this.Name = "a dungeon control stone for \"" + this.DungeonName + "\"";
                
                if (IRegion != null)
                    from.SendMessage(34, "Dungeon Region Updated");
                else from.SendMessage(34, "Dungeon Update Error. Contact the Admin.");
            }
        }

		public GroupDungeonStone( Serial serial ) : base( serial ) 
		{ 
		}

        public override void OnDelete()
        {
            UnRegisterDungeon(this);
            base.OnDelete();
        }

		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 

			writer.Write( (int) 3 ); // version 

            writer.Write(m_AllowPets);//3
            writer.Write(m_MaxSkills);
            writer.Write(m_MinSkills);//2
            writer.Write(m_EntranceMap);//1
            writer.Write(m_DungeonName);
            writer.Write(m_Size);
            writer.Write(m_MaxPlayers);
            writer.Write(m_EntrancePoint);//0
            } 

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 

			int version = reader.ReadInt();

            switch (version)
            {
                case 3:
                    {
                        m_AllowPets = reader.ReadBool();
                        goto case 2;
                    }
                case 2:
                    {
                        m_MaxSkills = reader.ReadInt();
                        m_MinSkills = reader.ReadInt();
                        goto case 1;
                    }
                case 1:
                    {
                        m_EntranceMap = reader.ReadMap();
                        goto case 0;
                    }
                case 0:
                    {
                        m_DungeonName = reader.ReadString();
                        m_Size = reader.ReadInt();
                        m_MaxPlayers = reader.ReadInt();
                        m_EntrancePoint = reader.ReadPoint3D();
                        break;
                    }
            }

            UnRegisterDungeon(this);
            RegisterDungeon(this);
		} 
	} 
} 
