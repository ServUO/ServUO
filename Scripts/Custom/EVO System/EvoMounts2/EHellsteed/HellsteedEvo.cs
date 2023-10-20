using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Xanthos.Interfaces;

namespace Xanthos.Evo
{
	[CorpseName( "a hellsteed corpse" )]
	public class EvoHellsteed : BaseEvoMount, IEvoCreature
	{
		public override BaseEvoSpec GetEvoSpec()
		{
			return HellsteedEvoSpec.Instance;
		}

		public override BaseEvoEgg GetEvoEgg()
		{
			return new HellsteedEvoEgg();
		}

		public override bool AddPointsOnDamage { get { return true; } }
		public override bool AddPointsOnMelee { get { return false; } }
		public override Type GetEvoDustType() { return typeof( HellsteedEvoDust ); }

		public override bool HasBreath{ get{ return true; } }

		public EvoHellsteed( string name ) : base( name, 793, 0x3EBB )
		{
		}

		public EvoHellsteed( Serial serial ) : base( serial )
		{
		}

		public override WeaponAbility GetWeaponAbility()	
		{
			return WeaponAbility.Dismount;
		} 

		public override bool SubdueBeforeTame{ get{ return false; } } // Must be beaten into submission
		
		public override int GetAngerSound()
		{
			return 0x172;
		}

		public override int GetIdleSound()
		{
			return 0x170;
		}

		public override int GetAttackSound()
		{
			return 0x169;
		}

		public override int GetHurtSound()
		{
			return 0x171;
		}

		public override int GetDeathSound()
		{
			return 0x173;
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