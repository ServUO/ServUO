using Server;
using System;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Engines.Despise
{
	public class DespiseBoss : BaseCreature
	{
		private readonly int ArtifactChance = 5;
		
		public virtual BaseCreature SummonWisp { get { return null; } }
        public virtual double WispScalar { get { return 0.33; } }

		private BaseCreature m_Wisp;
		private Timer m_SummonTimer;
		
		[CommandProperty(AccessLevel.GameMaster)]
		public BaseCreature Wisp { get { return m_Wisp; } }
	
		public DespiseBoss(AIType ai, FightMode fightmode) : base(ai, fightmode, 10, 1, .1, .2)
		{
            m_SummonTimer = Timer.DelayCall(TimeSpan.FromSeconds(5), new TimerCallback(SummonWisp_Callback));

            FollowersMax = 100;
		}

        public void SetNonMovable(Item item)
        {
            item.Movable = false;
            AddItem(item);
        }

        public override void Damage(int amount, Mobile from, bool informMount, bool checkDisrupt)
		{
            if (from is DespiseCreature)
                base.Damage(amount, from, informMount, checkDisrupt);
		}
		
		public override void OnKilledBy( Mobile mob )
		{
			if(mob is PlayerMobile)
			{
                int chance = ArtifactChance + (int)Math.Min(10, ((PlayerMobile)mob).Luck / 180);

                if (chance >= Utility.Random(100))
                {
                    Type t = m_Artifacts[Utility.Random(m_Artifacts.Length)];

                    if (t != null)
                    {
                        Item arty = Loot.Construct(t);

                        if (arty != null)
                        {
                            Container pack = mob.Backpack;

                            if (pack == null || !pack.TryDropItem(mob, arty, false))
                            {
                                mob.BankBox.DropItem(arty);
                                mob.SendMessage("An artifact has been placed in your bankbox!");
                            }
                            else
                                mob.SendLocalizedMessage(1153440); // An artifact has been placed in your backpack!
                        }
                    }
                }
			}
		}

        public override void AlterMeleeDamageTo(Mobile to, ref int damage)
        {
            base.AlterMeleeDamageTo(to, ref damage);

            if(m_Wisp != null && !m_Wisp.Deleted && m_Wisp.Alive)
                damage += (int)((double)damage * WispScalar);
        }

        public override void AlterMeleeDamageFrom(Mobile from, ref int damage)
        {
            base.AlterMeleeDamageFrom(from, ref damage);

            if(m_Wisp != null && !m_Wisp.Deleted && m_Wisp.Alive)
                damage -= (int)((double)damage * WispScalar);
        }

        public override void AlterSpellDamageTo(Mobile to, ref int damage)
        {
            base.AlterSpellDamageTo(to, ref damage);

            if (m_Wisp != null && !m_Wisp.Deleted && m_Wisp.Alive)
                damage += (int)((double)damage * WispScalar);
        }

        public override void AlterSpellDamageFrom(Mobile from, ref int damage)
        {
            base.AlterSpellDamageFrom(from, ref damage);

            if (m_Wisp != null && !m_Wisp.Deleted && m_Wisp.Alive)
                damage -= (int)((double)damage * WispScalar);
        }

		public override void OnThink()
		{
			base.OnThink();
			
			if(m_SummonTimer == null && (m_Wisp == null || !m_Wisp.Alive || m_Wisp.Deleted))
			{
				m_SummonTimer = Timer.DelayCall(TimeSpan.FromSeconds(Utility.RandomMinMax(40, 60)), new TimerCallback(SummonWisp_Callback));
			}
		}
		
		public void SummonWisp_Callback()
		{
			m_Wisp = SummonWisp;
            BaseCreature.Summon(m_Wisp, true, this, this.Location, 0, TimeSpan.FromMinutes(90));

            m_SummonTimer = null;
		}

        public override void Delete()
        {
            base.Delete();

            if (m_Wisp != null && m_Wisp.Alive)
                m_Wisp.Kill();
        }
		
		private static Type[] m_Artifacts = new Type[]
		{
			typeof(CompassionsEye),
			typeof(UnicornManeWovenSandals),
			typeof(UnicornManeWovenTalons),
			typeof(DespicableQuiver),
			typeof(UnforgivenVeil),
			typeof(HailstormHuman),
			typeof(HailstormGargoyle),
		};

		public DespiseBoss(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
			writer.Write(m_Wisp);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
			m_Wisp = reader.ReadMobile() as BaseCreature;
		}
	}
	
	public class AdrianTheGloriousLord : DespiseBoss
	{
		[Constructable]
		public AdrianTheGloriousLord() : base(AIType.AI_Mage, FightMode.Closest)
		{
			Name = "Adrian";
			Title = "the Glorious Lord";
			
			Race = Race.Human;
			Body = 0x190;
			Female = false;
			
			Hue = Race.RandomSkinHue();
			HairItemID = 8252;
			HairHue = 153;
			
			SetStr( 900, 1200 );
			SetDex( 500, 600 );
			SetInt( 500, 600 );
			
			SetHits( 60000 );
			SetStam( 415 );
			SetMana( 22000 );

			SetDamage( 18, 28 );
			
			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 40, 60 );
			SetResistance( ResistanceType.Fire, 40, 60 );
			SetResistance( ResistanceType.Cold, 40, 60 );
			SetResistance( ResistanceType.Poison, 40, 60 );
			SetResistance( ResistanceType.Energy, 40, 60 );
			
			SetSkill( SkillName.MagicResist, 120 );
			SetSkill( SkillName.Tactics, 120 );
			SetSkill( SkillName.Wrestling, 120 );
			SetSkill( SkillName.Anatomy, 120 );
			SetSkill( SkillName.Magery, 120 );
			SetSkill( SkillName.EvalInt, 120 );
			SetSkill( SkillName.Mysticism, 120 );
			SetSkill( SkillName.Focus, 160 );
			
			Fame = 22000;
			Karma = 22000;

            Item boots = new ThighBoots();
            boots.Hue = 1;

            Item scimitar = new Item(5046);
            scimitar.Hue = 1818;
            scimitar.Layer = Layer.OneHanded;

			SetNonMovable(boots);
            SetNonMovable(scimitar);
			SetNonMovable(new LongPants(1818));
			SetNonMovable(new FancyShirt(194));
			SetNonMovable(new Doublet(1281));
		}
		
		public override bool InitialInnocent { get { return true; } }
		public override BaseCreature SummonWisp { get { return new EnsorcledWisp(); } }
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.SuperBoss, 3);
		}
	
		public AdrianTheGloriousLord(Serial serial) : base(serial)
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
	
	public class AndrosTheDreadLord : DespiseBoss
	{
		[Constructable]
		public AndrosTheDreadLord() : base(AIType.AI_Mage, FightMode.Closest)
		{
			Name = "Andros";
			Title = "the Dread Lord";
			
			Race = Race.Human;
			Body = 0x190;
			Female = false;
			
			Hue = Race.RandomSkinHue();
			HairItemID = 0;
			
			SetStr( 900, 1200 );
			SetDex( 500, 600 );
			SetInt( 500, 600 );
			
			SetHits( 60000 );
			SetStam( 415 );
			SetMana( 22000 );

			SetDamage( 18, 28 );
			
			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 40, 60 );
			SetResistance( ResistanceType.Fire, 40, 60 );
			SetResistance( ResistanceType.Cold, 40, 60 );
			SetResistance( ResistanceType.Poison, 40, 60 );
			SetResistance( ResistanceType.Energy, 40, 60 );
			
			SetSkill( SkillName.MagicResist, 120 );
			SetSkill( SkillName.Tactics, 120 );
			SetSkill( SkillName.Wrestling, 120 );
			SetSkill( SkillName.Anatomy, 120 );
			SetSkill( SkillName.Magery, 120 );
			SetSkill( SkillName.EvalInt, 120 );
			SetSkill( SkillName.Mysticism, 120 );
			SetSkill( SkillName.Focus, 160 );
			
			Fame = 22000;
			Karma = -22000;

            Item boots = new ThighBoots();
            boots.Hue = 1;

            Item staff = new Item(3721);
            staff.Layer = Layer.TwoHanded;

			SetNonMovable(boots);
			SetNonMovable(new LongPants(1818));
			SetNonMovable(new FancyShirt(2726));
			SetNonMovable(new Doublet(1153));
			SetNonMovable(staff);
		}
		
		public override bool AlwaysMurderer { get { return true; } }
		public override BaseCreature SummonWisp { get { return new CorruptedWisp(); } }
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.SuperBoss, 3);
		}

        public AndrosTheDreadLord(Serial serial)
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
	
	public class EnsorcledWisp : BaseCreature
	{
		public EnsorcledWisp() : base(AIType.AI_Melee, FightMode.None, 10, 1, .2, .4)
		{
			Name = "Ensorcled Wisp";
            Body = 165;
            Hue = 0x901;
            BaseSoundID = 466;

			SetStr( 600, 700 );
			SetDex( 500, 600 );
			SetInt( 500, 600 );

			SetHits( 7000, 8000 );
			
			SetDamage( 12, 19 );
			
			SetDamageType( ResistanceType.Physical, 40 );
			SetDamageType( ResistanceType.Fire, 30 );
			SetDamageType( ResistanceType.Energy, 30 );

			SetResistance( ResistanceType.Physical, 50 );
			SetResistance( ResistanceType.Fire, 60, 70 );
			SetResistance( ResistanceType.Cold, 60, 70 );
			SetResistance( ResistanceType.Poison, 50, 60 );
			SetResistance( ResistanceType.Energy, 60, 70 );
			
			SetSkill( SkillName.MagicResist, 110, 125 );
			SetSkill( SkillName.Tactics, 110, 125 );
			SetSkill( SkillName.Wrestling, 110, 125 );
			SetSkill( SkillName.Anatomy, 110, 125 );
			
			Fame = 8000;
			Karma = 8000;
		}

        public override void OnThink()
        {
            base.OnThink();

            if (ControlTarget != ControlMaster || ControlOrder != OrderType.Follow)
            {
                ControlTarget = ControlMaster;
                ControlOrder = OrderType.Follow;
            }
        }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 3);
		}

        public override bool OnBeforeDeath()
        {
            Summoned = false;
            PackItem(new Gold(Utility.Random(800, 1000)));
            return base.OnBeforeDeath();
        }
		
		public override bool InitialInnocent { get { return true; } }
        //public override bool ForceNotoriety { get { return true; } }

		public EnsorcledWisp(Serial serial) : base(serial)
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
	
	public class CorruptedWisp : BaseCreature
	{
        public CorruptedWisp() : base(AIType.AI_Melee, FightMode.None, 10, 1, .2, .4)
		{
			Name = "Corrupted Wisp";
            Body = 165;
            Hue = 1955;
            BaseSoundID = 466;

			SetStr( 600, 700 );
			SetDex( 500, 600 );
			SetInt( 500, 600 );

			SetHits( 7000, 8000 );
			
			SetDamage( 12, 19 );
			
			SetDamageType( ResistanceType.Physical, 40 );
			SetDamageType( ResistanceType.Fire, 30 );
			SetDamageType( ResistanceType.Energy, 30 );

			SetResistance( ResistanceType.Physical, 50 );
			SetResistance( ResistanceType.Fire, 60, 70 );
			SetResistance( ResistanceType.Cold, 60, 70 );
			SetResistance( ResistanceType.Poison, 50, 60 );
			SetResistance( ResistanceType.Energy, 60, 70 );
			
			SetSkill( SkillName.MagicResist, 110, 125 );
			SetSkill( SkillName.Tactics, 110, 125 );
			SetSkill( SkillName.Wrestling, 110, 125 );
			SetSkill( SkillName.Anatomy, 110, 125 );
			
			Fame = 8000;
			Karma = -8000;
		}

        public override void OnThink()
        {
            base.OnThink();

            if (ControlTarget != ControlMaster || ControlOrder != OrderType.Follow)
            {
                ControlTarget = ControlMaster;
                ControlOrder = OrderType.Follow;
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 3);
        }

        public override bool OnBeforeDeath()
        {
            Summoned = false;
            PackItem(new Gold(Utility.Random(800, 1000)));
            return base.OnBeforeDeath();
        }
		
		public override bool AlwaysMurderer { get { return true; } }
        //public override bool ForceNotoriety { get { return true; } }
		
		public CorruptedWisp(Serial serial) : base(serial)
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
}