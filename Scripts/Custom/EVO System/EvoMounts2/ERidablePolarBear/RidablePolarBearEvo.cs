using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Xanthos.Interfaces;

namespace Xanthos.Evo
{
	[CorpseName( "a ridable polar bear corpse" )]
	public class EvoRidablePolarBear : BaseEvoMount, IEvoCreature
	{
		public override BaseEvoSpec GetEvoSpec()
		{
			return RidablePolarBearEvoSpec.Instance;
		}

		public override BaseEvoEgg GetEvoEgg()
		{
			return new RidablePolarBearEvoEgg();
		}

		public override bool AddPointsOnDamage { get { return true; } }
		public override bool AddPointsOnMelee { get { return false; } }
		public override Type GetEvoDustType() { return typeof( RidablePolarBearEvoDust ); }

		public override bool HasBreath{ get{ return false; } }

		public EvoRidablePolarBear( string name ) : base( name, 213, 0x3EC5 )
		{
		}

        public EvoRidablePolarBear(Serial serial)
            : base(serial)
		{
		}

		public override WeaponAbility GetWeaponAbility()	
		{
			return WeaponAbility.Dismount;
		} 

		public override bool SubdueBeforeTame{ get{ return false; } } // Must be beaten into submission
		
		public override int GetAngerSound()
		{
			return 0x60;
		}

		public override int GetIdleSound()
		{
			return 0x61;
		}

		public override int GetAttackSound()
		{
			return 0x62;
		}

		public override int GetHurtSound()
		{
			return 0x63;
		}

		public override int GetDeathSound()
		{
			return 0x64;
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