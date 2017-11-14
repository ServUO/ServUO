using System;
using Server.Items;
using Server.Spells;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class BasePeerless : BaseCreature
    {
        private PeerlessAltar m_Altar;
		
        [CommandProperty(AccessLevel.GameMaster)]
        public PeerlessAltar Altar
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
        public virtual bool DropPrimer { get { return Core.TOL; } }

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
		
        public BasePeerless(Serial serial)
            : base(serial)
        {
        }
		
        public override void OnThink()
        {
            base.OnThink();
			
            if (this.HasFireRing && this.Combatant != null && this.Alive && this.Hits > 0.8 * this.HitsMax && this.m_NextFireRing > Core.TickCount && Utility.RandomDouble() < this.FireRingChance)
                this.FireRing();
				
            if (this.CanSpawnHelpers && this.Combatant != null && this.Alive && this.CanSpawnWave())
                this.SpawnHelpers();
        }
		
        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (DropPrimer)
            {
                SkillMasteryPrimer primer = SkillMasteryPrimer.GetRandom();
                List<DamageStore> rights = GetLootingRights();

                if (rights.Count > 0)
                {
                    Mobile m = rights[Utility.Random(rights.Count)].m_Mobile;

                    m.SendLocalizedMessage(1156209); // You have received a mastery primer!

                    if (m.Backpack == null || !m.Backpack.TryDropItem(m, primer, false))
                        m.BankBox.DropItem(primer);
                }
                else
                {
                    c.DropItem(primer);
                }
            }

            if (GivesMLMinorArtifact && 0.5 > Utility.RandomDouble())
            {
                MondainsLegacy.DropPeerlessMinor(c);
            }

            if (this.m_Altar != null)
                this.m_Altar.OnPeerlessDeath();
        }
		
        public BasePeerless(AIType aiType, FightMode fightMode, int rangePerception, int rangeFight, double activeSpeed, double passiveSpeed)
            : base(aiType, fightMode, rangePerception, rangeFight, activeSpeed, passiveSpeed)
        {
            this.m_NextFireRing = Core.TickCount + 10000;			
            this.m_CurrentWave = this.MaxHelpersWaves;
        }
		
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
			
            writer.Write((Item)this.m_Altar);
        }
		
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
            this.m_Altar = reader.ReadItem() as PeerlessAltar;
        }
		
        #region Helpers		
        public virtual bool CanSpawnHelpers
        {
            get
            {
                return false;
            }
        }
        public virtual int MaxHelpersWaves
        {
            get
            {
                return 0;
            }
        }
        public virtual double SpawnHelpersChance
        {
            get
            {
                return 0.05;
            }
        }
		
        private int m_CurrentWave;
		
        public int CurrentWave
        {
            get
            {
                return this.m_CurrentWave;
            }
            set
            {
                this.m_CurrentWave = value;
            }
        }

        public bool AllHelpersDead
        {
            get
            {
                if (this.m_Altar != null)
                    return this.m_Altar.AllHelpersDead();

                return true;
            }
        }
		
        public virtual bool CanSpawnWave()
        {
            if (this.MaxHelpersWaves > 0 && this.m_CurrentWave > 0)
            {
                double hits = (this.Hits / (double)this.HitsMax);
                double waves = (this.m_CurrentWave / (double)(this.MaxHelpersWaves + 1));
				
                if (hits < waves && Utility.RandomDouble() < this.SpawnHelpersChance)
                {
                    this.m_CurrentWave -= 1;
                    return true;
                }
            }
			
            return false;
        }
		
        public virtual void SpawnHelpers()
        { 
        }
		
        public void SpawnHelper(BaseCreature helper, int range)
        {
            this.SpawnHelper(helper, this.GetSpawnPosition(range));					
        }
		
        public void SpawnHelper(BaseCreature helper, int x, int y, int z)
        {
            this.SpawnHelper(helper, new Point3D(x, y, z));					
        }
		
        public void SpawnHelper(BaseCreature helper, Point3D location)
        {
            if (helper == null)
                return;

            helper.Home = location;
            helper.RangeHome = 4;
		
            if (this.m_Altar != null)
                this.m_Altar.AddHelper(helper);
				
            helper.MoveToWorld(location, this.Map);			
        }

        #endregion
		
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
		
        #region Fire Ring
        private static readonly int[] m_North = new int[]
        {
            -1, -1,
            1, -1,
            -1, 2,
            1, 2
        };
		
        private static readonly int[] m_East = new int[]
        {
            -1, 0,
            2, 0
        };		
		
        public virtual bool HasFireRing
        {
            get
            {
                return false;
            }
        }
        public virtual double FireRingChance
        {
            get
            {
                return 1.0;
            }
        }
		
        private long m_NextFireRing = Core.TickCount;
		
        public virtual void FireRing()
        {
            for (int i = 0; i < m_North.Length; i += 2) 
            {
                Point3D p = this.Location;
				
                p.X += m_North[i];
                p.Y += m_North[i + 1];
				
                IPoint3D po = p as IPoint3D;
				
                SpellHelper.GetSurfaceTop(ref po);
				
                Effects.SendLocationEffect(po, this.Map, 0x3E27, 50);
            }
			
            for (int i = 0; i < m_East.Length; i += 2) 
            {
                Point3D p = this.Location;
				
                p.X += m_East[i];
                p.Y += m_East[i + 1];
				
                IPoint3D po = p as IPoint3D;
				
                SpellHelper.GetSurfaceTop(ref po);
				
                Effects.SendLocationEffect(po, this.Map, 0x3E31, 50);
            }
			
            this.m_NextFireRing = Core.TickCount + 10000;
        }
        #endregion
    }
}
