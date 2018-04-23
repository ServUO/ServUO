using System;
using Server;
using Server.Items;

namespace Server.Mobiles 
{ 
	[CorpseName( "a chaotic energy corpse" )] 
	public class ChaoticEnergy : BaseCreature 
	{ 		
		[Constructable] 
		public ChaoticEnergy() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 ) 
		{

			Body = 43;
			Hue = 18;
			Name = "Chaotic Energy";
			BaseSoundID = 0x47D;
					
			SetStr( 742, 890 );
			SetDex( 228, 345 );
			SetInt( 801, 850 );
			
			SetHits( 1202, 1215 );
			SetStam( 128, 145 );
			SetMana( 801, 850 );

			SetDamage( 16, 40 );

			SetDamageType( ResistanceType.Physical, 0 );
			SetDamageType( ResistanceType.Cold, 10 );
			SetDamageType( ResistanceType.Energy, 90 );

			SetResistance( ResistanceType.Physical, 20, 35 );
			SetResistance( ResistanceType.Fire, 76, 88 );
			SetResistance( ResistanceType.Cold, 70, 80 );
			SetResistance( ResistanceType.Poison, 61, 70 );
			SetResistance( ResistanceType.Energy, 65, 74 );

			SetSkill( SkillName.Wrestling, 63.1, 79.6 );	
			SetSkill( SkillName.Tactics, 71.2, 79.6 );
			SetSkill( SkillName.MagicResist, 100.4, 108.4 );
			SetSkill( SkillName.Magery, 120.9, 128.5 );
			SetSkill( SkillName.EvalInt, 101.5, 108.7 );
			SetSkill( SkillName.Meditation, 100.2, 109.1 );

			Fame = 18000;
			Karma = -18000;

			VirtualArmor = 85;
		}
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.AosFilthyRich, 2 );
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );	
			
			c.DropItem( new PowerCrystalTwo() );
		}

		public ChaoticEnergy( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			
			int version = reader.ReadInt();
		}
	}
}