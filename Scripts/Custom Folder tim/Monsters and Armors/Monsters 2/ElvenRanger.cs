using System; 
using Server;
using Server.Items;

namespace Server.Mobiles 
{ 
	[CorpseName( "an elven ranger corpse" )] 
	public class ElvenRanger : BaseCreature 
	{ 
		[Constructable] 
		public ElvenRanger() : base( AIType.AI_Archer, FightMode.Evil, 10, 1, 0.2, 0.4 )
		{ 
			Name = "an elven ranger";
			Hue = Race.Elf.RandomSkinHue();

			if ( Female = Utility.RandomBool() )
			{
				Body = 606;
				Name = NameList.RandomName( "female" );

			// hair, facial hair			
			HairItemID = Race.Elf.RandomHair( Female );
			HairHue = Race.Elf.RandomHairHue();
			}
			else
			{
				Body = 605;
				Name = NameList.RandomName( "male" );

			// hair, facial hair			
			HairItemID = Race.Elf.RandomHair( Female );
			HairHue = Race.Elf.RandomHairHue();
			}

			AddItem( new ElvenCompositeLongbow() );
			AddItem( new PadsOfTheCuSidhe() );
			AddItem( new RangerLegs() );
			AddItem( new RangerChest() );
			AddItem( new RangerGorget() );
			AddItem( new RangerArms() );
			AddItem( new RangerGloves() );
			AddItem( new GoldRing() );
			AddItem( new GoldBracelet() );
			AddItem( new GoldEarrings() );
			AddItem( new WoodlandBelt() );
			AddItem( new LeatherMempo() );

			SetStr( 106, 120 );
			SetDex( 100, 128 );
			SetInt( 161, 180 );

			SetHits( 96, 100 );

			SetDamage( 8, 10 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 40, 60 );
			SetResistance( ResistanceType.Fire, 35, 45 );
			SetResistance( ResistanceType.Cold, 65, 65 );
			SetResistance( ResistanceType.Poison, 40, 65 );
			SetResistance( ResistanceType.Energy, 25, 45 );

			SetSkill( SkillName.EvalInt, 95.1, 100.0 );
			SetSkill( SkillName.Focus, 95.1, 100.0 );
			SetSkill( SkillName.Meditation, 95.1, 100.0 );
			SetSkill( SkillName.MagicResist, 102.5, 125.0 );
			SetSkill( SkillName.Tactics, 85.0, 100.5 );
			SetSkill( SkillName.Archery, 85.0, 100.0 );
			SetSkill( SkillName.Anatomy, 85.0, 100.0 );

			PackItem( new Arrow( Utility.RandomMinMax( 80, 100 ) ) );
			Fame = 4000;
			Karma = 4000;

			VirtualArmor = 16;

			if ( 0.7 > Utility.RandomDouble() )
				PackItem( new ArcaneGem() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
		}

		public void AddArcane( Item item )
		{
			if ( item is IArcaneEquip )
			{
				IArcaneEquip eq = (IArcaneEquip)item;
				eq.CurArcaneCharges = eq.MaxArcaneCharges = 20;
			}

			item.Hue = ArcaneGem.DefaultArcaneHue;
			item.LootType = LootType.Newbied;

			AddItem( item );
		}

		public override bool ClickTitle{ get{ return false; } }
		public override bool ShowFameTitle{ get{ return false; } }
		public override bool InitialInnocent{ get{ return true; } }

		public override bool IsEnemy( Mobile m )
		{
			if ( m.Player || m is BaseVendor )
				return false;

			if ( m is BaseCreature )
			{
				BaseCreature bc = (BaseCreature)m;

				Mobile master = bc.GetMaster();
				if( master != null )
					return IsEnemy( master );
			}

			return m.Karma < 0;
		}

		public ElvenRanger( Serial serial ) : base( serial ) 
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