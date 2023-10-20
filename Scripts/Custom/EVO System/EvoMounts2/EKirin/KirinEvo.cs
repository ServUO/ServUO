using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Xanthos.Interfaces;

namespace Xanthos.Evo
{
	[CorpseName( "a kirin corpse" )]
	public class EvoKirin : BaseEvoMount, IEvoCreature
	{
		public override BaseEvoSpec GetEvoSpec()
		{
			return KirinEvoSpec.Instance;
		}

		public override BaseEvoEgg GetEvoEgg()
		{
			return new KirinEvoEgg();
		}

		public override bool AddPointsOnDamage { get { return true; } }
		public override bool AddPointsOnMelee { get { return false; } }
		public override Type GetEvoDustType() { return typeof( KirinEvoDust ); }

		public override bool HasBreath{ get{ return false; } }

		public EvoKirin( string name ) : base( name, 132, 0x3EAD )
		{
		}

		public EvoKirin( Serial serial ) : base( serial )
		{
		}

		public override WeaponAbility GetWeaponAbility()	
		{
			return WeaponAbility.Dismount;
		} 

		public override bool SubdueBeforeTame{ get{ return false; } } // Must be beaten into submission
		
		public override int GetAngerSound()
		{
			return 0x3C6;
		}

		public override int GetIdleSound()
		{
			return 0x3C7;
		}

		public override int GetAttackSound()
		{
			return 0x3C8;
		}

		public override int GetHurtSound()
		{
			return 0x3C9;
		}

		public override int GetDeathSound()
		{
			return 0x3CA;
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