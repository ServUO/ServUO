using System;
using Server.Items;

namespace Server.Mobiles 
{
	[CorpseName( "a knight's corpse" )]
	public class DupresKnight : BaseCreature
	{
		private static string[] m_Names = new string[]
		{
			"Percy",
			"Jim",
			"Bob",
			"Gary",
			"Jack",
			"Troy",
			"Roy"
		};

		private static string[] m_Titles = new string[]
		{
			"The Knight"
		};	
			
		[Constructable]
		public DupresKnight() : base( AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = m_Names[Utility.Random( m_Names.Length )];
			Title = m_Titles[Utility.Random( m_Titles.Length )];
			Body = 0x190;
			Hue = Utility.RandomSkinHue();

			SetStr( 190, 200 );
			SetDex( 50, 75 );
			SetInt( 150, 250  );
			SetHits( 3900, 4100 );
			SetDamage( 22, 28 );
			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 50, 70 );
			SetResistance( ResistanceType.Fire, 50, 70 );
			SetResistance( ResistanceType.Cold, 50, 70 );
			SetResistance( ResistanceType.Poison, 50, 70 );
			SetResistance( ResistanceType.Energy, 50, 70 );

			SetSkill( SkillName.EvalInt, 195.0, 220.0 );
			SetSkill( SkillName.Magery, 195.0, 220.0 );
			SetSkill( SkillName.Meditation, 195.0, 200.0 );
			SetSkill( SkillName.MagicResist, 100.0, 120.0 );
			SetSkill( SkillName.Tactics, 195.0, 220.0 );
			SetSkill( SkillName.Wrestling, 195.0, 220.0 );

			VirtualArmor = 70;

            Item longsword = new Longsword();
            longsword.LootType = LootType.Blessed;
            AddItem(longsword);

            Item ph = new PlateHelm();
            ph.LootType = LootType.Blessed;
            AddItem(ph);

            Item pa = new PlateArms();
            pa.LootType = LootType.Blessed;
            AddItem(pa);

            Item pg = new PlateGorget();
            pg.LootType = LootType.Blessed;
            AddItem(pg);

            Item pgl = new PlateGloves();
            pgl.LootType = LootType.Blessed;
            AddItem(pgl);

            Item pl = new PlateLegs();
            pl.LootType = LootType.Blessed;
            AddItem(pl);

            Item pc = new PlateChest();
            pc.LootType = LootType.Blessed;
            AddItem(pc);

            Item mks = new MetalKiteShield();
            mks.LootType = LootType.Blessed;
            mks.Hue = 0x794;
            AddItem(mks);

            Item bs = new BodySash(0x794); // dark purple
            bs.LootType = LootType.Blessed;
            AddItem(bs);

            PackGold( 400, 600 );            
		}
		
        public override void GenerateLoot()
        {
            if (Utility.RandomDouble() < 0.1)
            {
                switch (Utility.Random(4))
                {
                    case 0:
                        PackItem(new ExodusSummoningRite());
                        break;
                    case 1:
                        PackItem(new ExodusSacrificalDagger());
                        break;
                    case 2:
                        PackItem(new RobeofRite());
                        break;
                    case 3:
                        PackItem(new ExodusSummoningAlter());
                        break;
                }
            }
        }

        public override bool InitialInnocent { get { return true; } }
        public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override int TreasureMapLevel{ get{ return 5; } }

        public DupresKnight(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
