using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
	public class WereWolf : BaseCreature
	{
        public override bool ClickTitle { get { return false; } }
        private bool m_TrueForm;

		[Constructable]
        public WereWolf()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
			SpeechHue = Utility.RandomDyedHue();

			Hue = Utility.RandomSkinHue();

			if ( this.Female = Utility.RandomBool() )
			{
				Body = 0x191;
				Name = NameList.RandomName( "female" );
				AddItem( new Skirt( Utility.RandomNeutralHue() ) );
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );
				AddItem( new ShortPants( Utility.RandomNeutralHue() ) );
			}

			SetStr( 386, 500 );
			SetDex( 481, 595 );
			SetInt( 861, 975 );

			SetDamage( 25, 43 );

			SetSkill( SkillName.Fencing, 86.0, 107.5 );
			SetSkill( SkillName.Macing, 95.0, 115.5 );
			SetSkill( SkillName.MagicResist, 95.0, 117.5 );
			SetSkill( SkillName.Swords, 85.0, 97.5 );
			SetSkill( SkillName.Tactics, 75.0, 87.5 );
			SetSkill( SkillName.Wrestling, 105.0, 127.5 );

            Fame = 2500;
            Karma = -2500;

			AddItem( new Boots( Utility.RandomNeutralHue() ) );
			AddItem( new FancyShirt());
			AddItem( new Bandana());

			switch ( Utility.Random( 7 ))
			{
				case 0: AddItem( new Longsword() ); break;
				case 1: AddItem( new Cutlass() ); break;
				case 2: AddItem( new Broadsword() ); break;
				case 3: AddItem( new Axe() ); break;
				case 4: AddItem( new Club() ); break;
				case 5: AddItem( new Dagger() ); break;
				case 6: AddItem( new Spear() ); break;
			}

			Utility.AssignRandomHair( this );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 2 );
            switch (Utility.Random(5)) //Rarity 1031 
            {
                case 0: PackItem(new PawsOfTheWereWolf());
                    break;

            }
		}
                
        private static readonly double[] m_Offsets = new double[]
			{
				Math.Cos( 000.0 / 180.0 * Math.PI ), Math.Sin( 000.0 / 180.0 * Math.PI ),
				Math.Cos( 040.0 / 180.0 * Math.PI ), Math.Sin( 040.0 / 180.0 * Math.PI ),
				Math.Cos( 080.0 / 180.0 * Math.PI ), Math.Sin( 080.0 / 180.0 * Math.PI ),
				Math.Cos( 120.0 / 180.0 * Math.PI ), Math.Sin( 120.0 / 180.0 * Math.PI ),
				Math.Cos( 160.0 / 180.0 * Math.PI ), Math.Sin( 160.0 / 180.0 * Math.PI ),
				Math.Cos( 200.0 / 180.0 * Math.PI ), Math.Sin( 200.0 / 180.0 * Math.PI ),
				Math.Cos( 240.0 / 180.0 * Math.PI ), Math.Sin( 240.0 / 180.0 * Math.PI ),
				Math.Cos( 280.0 / 180.0 * Math.PI ), Math.Sin( 280.0 / 180.0 * Math.PI ),
				Math.Cos( 320.0 / 180.0 * Math.PI ), Math.Sin( 320.0 / 180.0 * Math.PI ),
			};
		public override bool AlwaysMurderer{ get{ return true; } }
        public void Morph()
        {
            if (m_TrueForm)
                return;

            m_TrueForm = true;

            Name = "A Were Wolf";
            Body = 23;
            Hue = 2312;
            BaseSoundID = 0xE5;

            SetStr(586, 700);
            Hits = HitsMax;
            Stam = StamMax;
            Mana = ManaMax;

            SetDamage(50, 67);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 20, 25);
            SetResistance(ResistanceType.Fire, 10, 20);
            SetResistance(ResistanceType.Cold, 5, 10);
            SetResistance(ResistanceType.Poison, 5, 10);
            SetResistance(ResistanceType.Energy, 10, 15);

            SetSkill(SkillName.MagicResist, 57.6, 75.0);
            SetSkill(SkillName.Tactics, 50.1, 70.0);
            SetSkill(SkillName.Wrestling, 60.1, 80.0);

            Fame = 2500;
            Karma = -2500;

            VirtualArmor = 45;
            ProcessDelta();

            Say("AaRrrrooooooooo!"); 
        }
		[CommandProperty( AccessLevel.GameMaster )]
		public override int HitsMax{ get{ return m_TrueForm ? 6000 : 12000; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public override int ManaMax{ get{ return 5000; } }

       	public WereWolf( Serial serial ) : base( serial )
		{
		}

		public override bool OnBeforeDeath()
		{
			if ( m_TrueForm )
			{
				return base.OnBeforeDeath();
			}
			else
			{
				Morph();
				return false;
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );

			writer.Write( m_TrueForm );			
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_TrueForm = reader.ReadBool();

					break;
				}
			}

        }
	}
}