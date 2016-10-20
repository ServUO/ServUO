using System;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
    public class BaseExodusPeerless : BaseCreature
    {
        private PeerlessExodusAltar m_Altar;
        private TimeSpan m_RespawnTime;

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan RespawnTime
        {
            get { return m_RespawnTime; }
            set { m_RespawnTime = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public PeerlessExodusAltar Altar
        {
            get
            {
                return this.m_Altar;
            }
            set
            {
                this.m_Altar = value;
            }
        }

        public override bool CanBeParagon { get { return false; } }

        public override bool Unprovokable
        {
            get
            {
                return true;
            }
        }
        public virtual double ChangeCombatant
        {
            get
            {
                return 0.3;
            }
        }
		
        public BaseExodusPeerless(Serial serial)
            : base(serial)
        {
        }		
		
        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            new PeerlessExodusAltar.ExitTimer().Start();                        
        }        

        public BaseExodusPeerless(AIType aiType, FightMode fightMode, int rangePerception, int rangeFight, double activeSpeed, double passiveSpeed)
            : base(aiType, fightMode, rangePerception, rangeFight, activeSpeed, passiveSpeed)
        {       
        }

        public class InternalTimer : Timer
        {
            private BaseExodusPeerless m_bep;
            public InternalTimer(BaseExodusPeerless bep) : base(bep.RespawnTime)
            {
                m_bep = bep;
            }
            protected override void OnTick()
            {
                if (m_bep != null && !m_bep.Deleted)
                {
                    m_bep.Delete();
                    m_bep.DoSpawn();
                    new InternalTimer(m_bep).Start();
                }
            }
        }

        public void DoSpawn()
        {
            ClockworkExodus m = new ClockworkExodus();
            m.Home = new Point3D(854, 642, -40); 
            m.RangeHome = 4;
            m.MoveToWorld(new Point3D(854, 642, -40), Map.Ilshenar);            
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
			
            writer.Write((Item)this.m_Altar);
            writer.Write((TimeSpan)m_RespawnTime);
        }
		
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
            this.m_Altar = reader.ReadItem() as PeerlessExodusAltar;
            this.m_RespawnTime = reader.ReadTimeSpan();
        }        
		
        public virtual void PackResources(int amount)
        {
            for (int i = 0; i < amount; i ++)
                switch ( Utility.Random(6) )
                {
                    case 0:
                        this.PackItem(new Blight());
                        break;
                    case 1:
                        this.PackItem(new Scourge());
                        break;
                    case 2:
                        this.PackItem(new Taint());
                        break;
                    case 3:
                        this.PackItem(new Putrefication());
                        break;
                    case 4:
                        this.PackItem(new Corruption());
                        break;
                    case 5:
                        this.PackItem(new Muculent());
                        break;
                }
        }
		
        public virtual void PackItems(Item item, int amount)
        {
            for (int i = 0; i < amount; i ++)
                this.PackItem(item);
        }
		
        public virtual void PackTalismans(int amount)
        { 
            int count = Utility.Random(amount);
			
            for (int i = 0; i < count; i ++)
                this.PackItem(Loot.RandomTalisman());
        }
		
        public virtual Point3D GetSpawnPosition(int range)
        {
            return GetSpawnPosition(this.Location, this.Map, range);
        }
		
        public static Point3D GetSpawnPosition(Point3D from, Map map, int range)
        {
            if (map == null)
                return from;
				
            for (int i = 0; i < 10; i ++)
            {
                int x = from.X + Utility.Random(range);
                int y = from.Y + Utility.Random(range);
                int z = map.GetAverageZ(x, y);
				
                if (Utility.RandomBool())
                    x *= -1;
					
                if (Utility.RandomBool())
                    y *= -1;
					
                Point3D p = new Point3D(x, y, from.Z);
				
                if (map.CanSpawnMobile(p) && map.LineOfSight(from, p))
                    return p;
				
                p = new Point3D(x, y, z);
					
                if (map.CanSpawnMobile(p) && map.LineOfSight(from, p))
                    return p;
            }
			
            return from;
        }       
    }
}