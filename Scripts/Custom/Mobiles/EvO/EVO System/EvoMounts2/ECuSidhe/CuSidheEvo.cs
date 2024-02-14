using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Xanthos.Interfaces;

namespace Xanthos.Evo
{
    [CorpseName("a cu sidhe corpse")]
	public class EvoCuSidhe : BaseEvoMount, IEvoCreature
	{
		public override BaseEvoSpec GetEvoSpec()
		{
			return CuSidheEvoSpec.Instance;
		}

		public override BaseEvoEgg GetEvoEgg()
		{
			return new CuSidheEvoEgg();
		}

		public override bool AddPointsOnDamage { get { return true; } }
		public override bool AddPointsOnMelee { get { return true; } }
		public override Type GetEvoDustType() { return typeof( CuSidheEvoDust ); }

		//public override bool HasBreath{ get{ return false; } }

		public EvoCuSidhe( string name ) : base( name, 277, 0x3E91 )
		{
		}

        public EvoCuSidhe(Serial serial) : base(serial)
		{
		}

		public override WeaponAbility GetWeaponAbility()	
		{
			return WeaponAbility.Dismount;
		} 

		public override bool SubdueBeforeTame{ get{ return false; } } // Must be beaten into submission
		
		public override int GetAngerSound()
		{
			return 0xE7;
		}

		public override int GetIdleSound()
		{
			return 0xE6;
		}

		public override int GetAttackSound()
		{
			return 0xE8;
		}

		public override int GetHurtSound()
		{
			return 0xE9;
		}

		public override int GetDeathSound()
		{
			return 0xEA;
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