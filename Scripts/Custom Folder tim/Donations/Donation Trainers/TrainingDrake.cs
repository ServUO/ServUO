using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a training drake corpse" )]
	public class TrainingDrake : BaseCreature
	{
		[Constructable]
		public TrainingDrake () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a training drake";
			Body = Utility.RandomList( 60, 61 );
			BaseSoundID = 362;
            Hue = 2490;
            CantWalk = true;
			SetStr( 1, 2 );
			SetDex( 10 );
			SetInt( 1 );

			SetHits( 999999 );

			SetDamage( 1, 2 );
			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 50 );
			SetResistance( ResistanceType.Fire, 10 );
			SetResistance( ResistanceType.Cold, 10 );
			SetResistance( ResistanceType.Poison, 100 );
			SetResistance( ResistanceType.Energy, 10 );

			SetSkill( SkillName.Tactics, 5.1, 10.0 );
			SetSkill( SkillName.Wrestling, 5.1, 10.0 );
			SetSkill( SkillName.Anatomy, 5.1, 10.0 );

			Fame = 2500;
			Karma = -2500;

			VirtualArmor = 200;

			
		}

		public override int Meat{ get{ return 12; } }
		public override int Hides{ get{ return 8; } } 

		public TrainingDrake( Serial serial ) : base( serial )
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
