using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a draco lich corpse" )]
	[TypeAlias( "Server.Mobiles.ThrazhathaTheForgottenSlayerOfLegions", "Server.Mobiles.DelGorTheDestroyer", "Server.Mobiles.ElthdrakTheAncientTerror", "Server.Mobiles.AshKrazhanTheAncientDefiler", "Server.Mobiles.DorThregothTheDreadedOne", "Server.Mobiles.LythindracTheAbysmalHarbingerOfDoom", "Server.Mobiles.ArcerborTheAncientAnnihilatorOfEmpires", "Server.Mobiles.BeodracTheSinnisterGuardian" )]
	public class DracoLich : BaseCreature
	{
		private static string[] m_Names = new string[]
			{
				"Thrazhatha the Forgotten Slayer of Legions",
				"Del'Gor the Destroyer",
				"Elthdrak the Ancient Terror",
				"Ash'Krazhan the Ancient Defiler",
				"Dor'Thregoth the Dreaded One",
				"Lythindrac the Abysmal Harbinger of Doom",
				"Arcerbor the Ancient Annihilator of Empires",
				"Beodrac the Sinnister Guardian"
			};

		[Constructable]
		public DracoLich () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = m_Names[Utility.Random( m_Names.Length )];
			Body = 104;
			BaseSoundID = 0x488;
			Hue = Utility.RandomList(1021,1022,1023,1024,1025,1026,1027,1202,1203,1504,1505,1506,1441,1442,1443,2203,744);

			SetStr( 898, 1030 );
			SetDex( 68, 200 );
			SetInt( 888, 1020 );

			SetHits( 758, 899 );

			SetDamage( 50, 55 );

			SetDamageType( ResistanceType.Physical, 65 );
			SetDamageType( ResistanceType.Poison, 25 );
			SetDamageType( ResistanceType.Fire, 10 );

			SetResistance( ResistanceType.Physical, 85, 90 );
			SetResistance( ResistanceType.Fire, 70, 75 );
			SetResistance( ResistanceType.Cold, 80, 90 );
			SetResistance( ResistanceType.Poison, 70, 75 );
			SetResistance( ResistanceType.Energy, 70, 75 );

			SetSkill( SkillName.EvalInt, 80.1, 100.0 );
			SetSkill( SkillName.Magery, 80.1, 100.0 );
			SetSkill( SkillName.MagicResist, 100.3, 130.0 );
			SetSkill( SkillName.Tactics, 97.6, 100.0 );
			SetSkill( SkillName.Wrestling, 97.6, 100.0 );

			Fame = 22500;
			Karma = -22500;

			VirtualArmor = 80;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 4 );
			AddLoot( LootPack.FilthyRich, 3 );
			AddLoot( LootPack.Gems, 5 );
		}

		public override bool ReacquireOnMovement{ get{ return true; } }
		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override int BreathFireDamage{ get{ return 0; } }
		public override int BreathColdDamage{ get{ return 100; } }
		public override int BreathEffectHue{ get{ return 0x480; } }
		public override double BonusPetDamageScalar{ get{ return (Core.SE)? 3.0 : 1.0; } }
		// TODO: Undead summoning?

		public override bool AutoDispel{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override Poison HitPoison{ get{ return Poison.Lethal; } }
		public override bool BleedImmune{ get{ return true; } }
		public override int Meat{ get{ return 19; } } // where's it hiding these? :)
		public override int Hides{ get{ return 20; } }
		public override HideType HideType{ get{ return HideType.Barbed; } }

		public DracoLich( Serial serial ) : base( serial )
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