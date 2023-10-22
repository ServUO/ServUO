using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Xanthos.Interfaces;

namespace Xanthos.Evo
{
	[CorpseName( "a unicorn corpse" )]
	public class EvoUnicorn : BaseEvoMount, IEvoCreature
	{
		public override BaseEvoSpec GetEvoSpec()
		{
			return UnicornEvoSpec.Instance;
		}

		public override BaseEvoEgg GetEvoEgg()
		{
			return new UnicornEvoEgg();
		}

		public override bool AddPointsOnDamage { get { return true; } }
		public override bool AddPointsOnMelee { get { return true; } }
		public override Type GetEvoDustType() { return typeof( UnicornEvoDust ); }

		//public override bool HasBreath{ get{ return false; } }

		public EvoUnicorn( string name ) : base( name, 122, 0x3EB4 )
		{
		}

		public EvoUnicorn( Serial serial ) : base( serial )
		{
		}

		public override WeaponAbility GetWeaponAbility()	
		{
			return WeaponAbility.Dismount;
		} 

		public override bool SubdueBeforeTame{ get{ return false; } } // Must be beaten into submission
		
		public override int GetAngerSound()
		{
			return 0x4BD;
		}

		public override int GetIdleSound()
		{
			return 0x4BE;
		}

		public override int GetAttackSound()
		{
			return 0x4BF;
		}

		public override int GetHurtSound()
		{
			return 0x4C0;
		}

		public override int GetDeathSound()
		{
			return 0x4C1;
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write( (int)0 );			
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}