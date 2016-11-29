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
            get { return this.m_Altar; }
            set { this.m_Altar = value; }
        }

        public override bool CanBeParagon { get { return false; } }
        public override bool Unprovokable { get { return true; } }
        public virtual double ChangeCombatant { get { return 0.3; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Greater; } }
        public override int TreasureMapLevel { get { return 5; } }

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
    }
}