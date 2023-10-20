using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Xanthos.Interfaces;

namespace Xanthos.Evo
{
	[CorpseName( "a nightmare corpse" )]
	public class EvoNightmare : BaseEvoMount, IEvoCreature
	{
		public override BaseEvoSpec GetEvoSpec()
		{
			return NightmareEvoSpec.Instance;
		}

		public override BaseEvoEgg GetEvoEgg()
		{
			return new NightmareEvoEgg();
		}

		public override bool AddPointsOnDamage { get { return true; } }
		public override bool AddPointsOnMelee { get { return false; } }
		public override Type GetEvoDustType() { return typeof( NightmareEvoDust ); }

		public override bool HasBreath{ get{ return true; } }

		public EvoNightmare( string name ) : base( name, 179, 0x3EB7 )
		{
		}

		public EvoNightmare( Serial serial ) : base( serial )
		{
		}

		public override WeaponAbility GetWeaponAbility()	
		{
			return WeaponAbility.Dismount;
		} 

		public override bool SubdueBeforeTame{ get{ return true; } } // Must be beaten into submission
		
		public override int GetAngerSound()
		{
			return 0xA9;
		}

		public override int GetIdleSound()
		{
			return 0xAA;
		}

		public override int GetAttackSound()
		{
			return 0xAB;
		}

		public override int GetHurtSound()
		{
			return 0xAC;
		}

		public override int GetDeathSound()
		{
			return 0xAD;
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