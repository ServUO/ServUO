using System;
using System.Collections;
using Server.Items;
using Server.Network;
using Server.Spells;

namespace Server.Mobiles
{
    [CorpseName("a monstrous interred grizzle corpse")]
    public class MonstrousInterredGrizzle : BasePeerless
    {
        private static readonly int[] m_Tiles = new int[]
        {
            -2, 0,
            2, 0,
            2, -2,
            2, 2,
            -2, -2,
            -2, 2,
            0, 2,
            1, 0,
            0, -2
        };
        private static Hashtable m_Table;
        private readonly DateTime m_NextDrop = DateTime.UtcNow;
        [Constructable]
        public MonstrousInterredGrizzle()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a monstrous interred grizzle";
            this.Body = 0x103;			
            this.BaseSoundID = 589;

            this.SetStr(1198, 1207);
            this.SetDex(127, 135);
            this.SetInt(595, 646);

            this.SetHits(50000);

            this.SetDamage(27, 31);

            this.SetDamageType(ResistanceType.Physical, 60);
            this.SetDamageType(ResistanceType.Fire, 20);
            this.SetDamageType(ResistanceType.Energy, 20);

            this.SetResistance(ResistanceType.Physical, 48, 52);
            this.SetResistance(ResistanceType.Fire, 77, 82);
            this.SetResistance(ResistanceType.Cold, 56, 61);
            this.SetResistance(ResistanceType.Poison, 32, 40);
            this.SetResistance(ResistanceType.Energy, 69, 71);

            this.SetSkill(SkillName.Wrestling, 112.6, 116.9);
            this.SetSkill(SkillName.Tactics, 118.5, 119.2);
            this.SetSkill(SkillName.MagicResist, 120);
            this.SetSkill(SkillName.Anatomy, 111.0, 111.7);
            this.SetSkill(SkillName.Magery, 100.0);
            this.SetSkill(SkillName.EvalInt, 100);
            this.SetSkill(SkillName.Meditation, 100);

            this.Fame = 24000;
            this.Karma = -24000;

            this.VirtualArmor = 80;
            this.PackResources(8);
            this.PackTalismans(5);
        }

        public MonstrousInterredGrizzle(Serial serial)
            : base(serial)
        {
        }

        public override bool GivesMLMinorArtifact
        {
            get
            {
                return true;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 5;
            }
        }
        public static bool UnderCacophonicAttack(Mobile from)
        {
            if (m_Table == null)
                m_Table = new Hashtable();
			
            return m_Table[from] != null;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.AosSuperBoss, 8);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);		
			
            c.DropItem(new GrizzledBones());
			
            switch ( Utility.Random(4) )
            {
                case 0:
                    c.DropItem(new TombstoneOfTheDamned());
                    break;
                case 1:
                    c.DropItem(new GlobOfMonstreousInterredGrizzle());
                    break;
                case 2:
                    c.DropItem(new MonsterousInterredGrizzleMaggots());
                    break;
                case 3:
                    c.DropItem(new GrizzledSkullCollection());
                    break;
            }
			
            if (Utility.RandomDouble() < 0.6)				
                c.DropItem(new ParrotItem());
				
            if (Utility.RandomDouble() < 0.05)				
                c.DropItem(new GrizzledMareStatuette());
							
            if (Utility.RandomDouble() < 0.025)
                c.DropItem(new CrimsonCincture());
				
            if (Utility.RandomDouble() < 0.05)
            {
                switch ( Utility.Random(5) )
                {
                    case 0:
                        c.DropItem(new GrizzleGauntlets());
                        break;
                    case 1:
                        c.DropItem(new GrizzleGreaves());
                        break;
                    case 2:
                        c.DropItem(new GrizzleHelm());
                        break;
                    case 3:
                        c.DropItem(new GrizzleTunic());
                        break;
                    case 4:
                        c.DropItem(new GrizzleVambraces());
                        break;
                }
            }
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < 0.15)
                this.CacophonicAttack(defender);
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        { 
            if (Utility.RandomDouble() < 0.15)
                this.CacophonicAttack(from);
				
            if (Utility.RandomDouble() < 0.3)
                this.DropOoze();
			
            base.OnDamage(amount, from, willKill);
        }

        public override void OnThink()
        {
            base.OnThink();
			
            if (this.Combatant == null)
                return;	
				
            if (this.Hits > 0.8 * this.HitsMax && Utility.RandomDouble() < 0.0025)
                this.FireRing();
        }

        public override void FireRing()
        {
            for (int i = 0; i < m_Tiles.Length; i += 2) 
            {
                Point3D p = this.Location;
				
                p.X += m_Tiles[i];
                p.Y += m_Tiles[i + 1];
				
                IPoint3D po = p as IPoint3D;
				
                SpellHelper.GetSurfaceTop(ref po);
				
                Effects.SendLocationEffect(po, this.Map, Utility.RandomBool() ? 0x3E31 : 0x3E27, 100);
            }
        }

        public override int GetDeathSound()
        {
            return 0x57F;
        }

        public override int GetAttackSound()
        {
            return 0x580;
        }

        public override int GetIdleSound()
        {
            return 0x581;
        }

        public override int GetAngerSound()
        {
            return 0x582;
        }

        public override int GetHurtSound()
        {
            return 0x583;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
        }

        public virtual void CacophonicAttack(Mobile to)
        {
            if (m_Table == null)
                m_Table = new Hashtable();
		
            if (to.Alive && to.Player && m_Table[to] == null)
            {
                to.Send(SpeedControl.WalkSpeed);
                to.SendLocalizedMessage(1072069); // A cacophonic sound lambastes you, suppressing your ability to move.
                to.PlaySound(0x584);
				
                m_Table[to] = Timer.DelayCall(TimeSpan.FromSeconds(30), new TimerStateCallback(EndCacophonic_Callback), to);
            }
        }

        public virtual void CacophonicEnd(Mobile from)
        {
            if (m_Table == null)
                m_Table = new Hashtable();
				
            m_Table[from] = null;
				
            from.Send(SpeedControl.Disable);
        }

        public virtual void DropOoze()
        {
            int amount = Utility.RandomMinMax(1, 3);
            bool corrosive = Utility.RandomBool();
			
            for (int i = 0; i < amount; i ++)
            {
                Item ooze = new InfernalOoze(corrosive);				
                Point3D p = new Point3D(this.Location);
				
                for (int j = 0; j < 5; j ++)
                {
                    p = this.GetSpawnPosition(2);
                    bool found = false;
				
                    foreach (Item item in this.Map.GetItemsInRange(p, 0))
                        if (item is InfernalOoze)
                        {
                            found = true;
                            break;
                        }
						
                    if (!found)
                        break;			
                }
				
                ooze.MoveToWorld(p, this.Map);
            }
			
            if (this.Combatant != null)
            {
                if (corrosive)
                    this.Combatant.SendLocalizedMessage(1072071); // A corrosive gas seeps out of your enemy's skin!
                else
                    this.Combatant.SendLocalizedMessage(1072072); // A poisonous gas seeps out of your enemy's skin!
            }
        }

        private void EndCacophonic_Callback(object state)
        {
            if (state is Mobile)
                this.CacophonicEnd((Mobile)state);
        }
    }

    public class InfernalOoze : Item
    { 
        private bool m_Corrosive;
        private Hashtable m_Table;
        [Constructable]
        public InfernalOoze()
            : this(false)
        {
        }

        [Constructable]
        public InfernalOoze(bool corrosive)
            : base(0x122A)
        {
            this.Movable = false;
            this.Hue = 0x95;
			
            this.m_Corrosive = corrosive;			
            Timer.DelayCall(TimeSpan.FromSeconds(30), new TimerCallback(Morph));
        }

        public InfernalOoze(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Corrosive
        {
            get
            {
                return this.m_Corrosive;
            }
            set
            {
                this.m_Corrosive = value;
            }
        }
        public override bool OnMoveOver(Mobile m)
        {
            if (this.m_Table == null)
                this.m_Table = new Hashtable();
			
            if ((m is BaseCreature && ((BaseCreature)m).Controlled) || m.Player)
                this.m_Table[m] = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), new TimerStateCallback(Damage_Callback), m);
			
            return base.OnMoveOver(m);
        }

        public override bool OnMoveOff(Mobile m)
        { 
            if (this.m_Table == null)
                this.m_Table = new Hashtable();
				
            if (this.m_Table[m] is Timer)
            {
                Timer timer = (Timer)this.m_Table[m];
				
                timer.Stop();
				
                this.m_Table[m] = null;
            }
			
            return base.OnMoveOff(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.Write((int)0); // version
			
            writer.Write((bool)this.m_Corrosive);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
			
            this.m_Corrosive = reader.ReadBool();
        }

        public virtual void Damage(Mobile m)
        { 
            if (!m.Alive)
                this.StopTimer(m);
			
            if (this.m_Corrosive)
            {
                for (int i = 0; i < m.Items.Count; i ++)
                {
                    IDurability item = m.Items[i] as IDurability;
	
                    if (item != null && Utility.RandomDouble() < 0.25)
                    { 
                        if (item.HitPoints > 10)
                            item.HitPoints -= 10;
                        else
                            item.HitPoints -= 1;
                    }
                }
            }
            else
                AOS.Damage(m, 40, 0, 0, 0, 100, 0);
        }

        public virtual void Morph()
        {
            this.ItemID += 1;
			
            Timer.DelayCall(TimeSpan.FromSeconds(5), new TimerCallback(Decay));
        }

        public virtual void StopTimer(Mobile m)
        {
            if (this.m_Table[m] is Timer)
            {
                Timer timer = (Timer)this.m_Table[m];				
                timer.Stop();			
                this.m_Table[m] = null;	
            }
        }

        public virtual void Decay()
        { 
            if (this.m_Table == null)
                this.m_Table = new Hashtable();
				
            foreach (DictionaryEntry entry in this.m_Table)
                if (entry.Value is Timer)
                    ((Timer)entry.Value).Stop();
			
            this.m_Table.Clear();
			
            this.Delete();
        }

        private void Damage_Callback(object state)
        {
            if (state is Mobile)
                this.Damage((Mobile)state);
        }
    }
}