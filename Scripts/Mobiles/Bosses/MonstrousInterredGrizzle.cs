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

        private readonly DateTime m_NextDrop = DateTime.UtcNow;

        [Constructable]
        public MonstrousInterredGrizzle()
            : base(AIType.AI_Spellweaving, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a monstrous interred grizzle";
            Body = 0x103;			
            BaseSoundID = 589;

            SetStr(1198, 1207);
            SetDex(127, 135);
            SetInt(595, 646);

            SetHits(50000);

            SetDamage(27, 31);

            SetDamageType(ResistanceType.Physical, 60);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Energy, 20);

            SetResistance(ResistanceType.Physical, 48, 52);
            SetResistance(ResistanceType.Fire, 77, 82);
            SetResistance(ResistanceType.Cold, 56, 61);
            SetResistance(ResistanceType.Poison, 32, 40);
            SetResistance(ResistanceType.Energy, 69, 71);

            SetSkill(SkillName.Wrestling, 112.6, 116.9);
            SetSkill(SkillName.Tactics, 118.5, 119.2);
            SetSkill(SkillName.MagicResist, 120);
            SetSkill(SkillName.Anatomy, 111.0, 111.7);
            SetSkill(SkillName.Magery, 100.0);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);
            SetSkill(SkillName.Spellweaving, 100.0);

            Fame = 24000;
            Karma = -24000;

            VirtualArmor = 80;
            PackResources(8);
            PackTalismans(5);

            for (int i = 0; i < Utility.RandomMinMax(1, 6); i++)
            {
                PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }

            SetSpecialAbility(SpecialAbility.HowlOfCacophony);
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

        public override void GenerateLoot()
        {
            AddLoot(LootPack.AosSuperBoss, 8);
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

            if (Utility.RandomDouble() < 0.10)
                c.DropItem(new HumanFeyLeggings());
			
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

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {				
            if (Utility.RandomDouble() < 0.3)
                DropOoze();
			
            base.OnDamage(amount, from, willKill);
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

        public virtual void DropOoze()
        {
            int amount = Utility.RandomMinMax(1, 3);
            bool corrosive = Utility.RandomBool();
			
            for (int i = 0; i < amount; i ++)
            {
                Item ooze = new InfernalOoze(corrosive);				
                Point3D p = new Point3D(Location);
				
                for (int j = 0; j < 5; j ++)
                {
                    p = GetSpawnPosition(2);
                    bool found = false;
				
                    foreach (Item item in Map.GetItemsInRange(p, 0))
                        if (item is InfernalOoze)
                        {
                            found = true;
                            break;
                        }
						
                    if (!found)
                        break;			
                }
				
                ooze.MoveToWorld(p, Map);
            }
			
            if (Combatant is PlayerMobile)
            {
                if (corrosive)
                    ((PlayerMobile)Combatant).SendLocalizedMessage(1072071); // A corrosive gas seeps out of your enemy's skin!
                else
                    ((PlayerMobile)Combatant).SendLocalizedMessage(1072072); // A poisonous gas seeps out of your enemy's skin!
            }
        }
    }

    public class InfernalOoze : Item
    { 
        private bool m_Corrosive;
        private Hashtable m_Table;

        private int m_Damage;

        [Constructable]
        public InfernalOoze()
            : this(false)
        {
        }

        [Constructable]
        public InfernalOoze(bool corrosive, int damage = 40)
            : base(0x122A)
        {
            Movable = false;
            Hue = 0x95;

            m_Damage = damage;
			
            m_Corrosive = corrosive;			
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
                return m_Corrosive;
            }
            set
            {
                m_Corrosive = value;
            }
        }
        public override bool OnMoveOver(Mobile m)
        {
            if (m_Table == null)
                m_Table = new Hashtable();
			
            if ((m is BaseCreature && ((BaseCreature)m).Controlled) || m.Player)
                m_Table[m] = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), new TimerStateCallback(Damage_Callback), m);
			
            return base.OnMoveOver(m);
        }

        public override bool OnMoveOff(Mobile m)
        { 
            if (m_Table == null)
                m_Table = new Hashtable();
				
            if (m_Table[m] is Timer)
            {
                Timer timer = (Timer)m_Table[m];
				
                timer.Stop();
				
                m_Table[m] = null;
            }
			
            return base.OnMoveOff(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.Write((int)0); // version
			
            writer.Write((bool)m_Corrosive);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
			
            m_Corrosive = reader.ReadBool();
        }

        public virtual void Damage(Mobile m)
        { 
            if (!m.Alive)
                StopTimer(m);

            if (m_Corrosive)
            {
                for (int i = 0; i < m.Items.Count; i++)
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
            {
                int dmg = m_Damage;

                if (m is PlayerMobile)
                {
                    PlayerMobile pm = m as PlayerMobile;
                    dmg = (int)BalmOfProtection.HandleDamage(pm, dmg);
                    AOS.Damage(m, dmg, 0, 0, 0, 100, 0);
                }
                else
                    AOS.Damage(m, dmg, 0, 0, 0, 100, 0);
            }
        }

        public virtual void Morph()
        {
            ItemID += 1;
			
            Timer.DelayCall(TimeSpan.FromSeconds(5), new TimerCallback(Decay));
        }

        public virtual void StopTimer(Mobile m)
        {
            if (m_Table[m] is Timer)
            {
                Timer timer = (Timer)m_Table[m];				
                timer.Stop();			
                m_Table[m] = null;	
            }
        }

        public virtual void Decay()
        { 
            if (m_Table == null)
                m_Table = new Hashtable();
				
            foreach (DictionaryEntry entry in m_Table)
                if (entry.Value is Timer)
                    ((Timer)entry.Value).Stop();
			
            m_Table.Clear();
			
            Delete();
        }

        private void Damage_Callback(object state)
        {
            if (state is Mobile)
                Damage((Mobile)state);
        }
    }
}
